using System;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class InventoryManager
    {
        public static bool inventoryOpen = false;
        public static bool equipmentOpen = false;
        private static Entity player;
        public static int selection = 0;
        private static int maxItemPerSide = 8;
        private static string spacer = " + + ";
        public static List<Entity> inventoryDisplay = new List<Entity>();
        public InventoryManager(Entity _player)
        { 
            player = _player; 
        }
        public static void GetItem(Entity entity)
        {
            Vector2 vector2 = entity.GetComponent<Vector2>();
            Traversable traversable = World.tiles[vector2.x, vector2.y];
            if (traversable.itemLayer != null)
            {
                Entity itemRef = traversable.itemLayer;
                if (entity.GetComponent<PlayerComponent>() != null)
                {
                    Log.Add($"You picked up the {traversable.itemLayer.GetComponent<Description>().name}.");

                    Log.OutputParticleLog(itemRef.GetComponent<Description>().name, itemRef.GetComponent<Draw>().fColor, vector2);

                }
                AddToInventory(entity, traversable.itemLayer); traversable.itemLayer = null;
                entity.GetComponent<TurnFunction>().EndTurn();
                EntityManager.UpdateMap(itemRef);
            }
            else
            {
                if (entity.GetComponent<PlayerComponent>() != null)
                {
                    Log.Add("There is nothing to pick up.");
                }
            }
        }
        public static void DropItem(Entity entity, Entity item)
        {
            RemoveFromInventory(entity, item);
            PlaceItem(entity.GetComponent<Vector2>(), item);
            if (item.GetComponent<Equippable>() != null && item.GetComponent<Equippable>().equipped)
            {
                UnequipItem(entity, item);
            }
            if (entity.GetComponent<PlayerComponent>() != null)
            {
                CloseInventory();
                Log.Add($"You dropped the {item.GetComponent<Description>().name}.");
            }
            entity.GetComponent<TurnFunction>().EndTurn();
        }
        public static void AddToInventory(Entity actor, Entity item)
        {
            if (actor != null && item != null && actor.GetComponent<Inventory>().inventory != null)
            {
                actor.GetComponent<Inventory>().inventory.Add(item);

                if (CMath.ReturnAI(actor) != null && item.GetComponent<Equippable>() != null)
                {
                    CMath.ReturnAI(actor).ConsiderEquipment(item.GetComponent<Equippable>().slot);
                }
            }
        }
        public static void RemoveFromInventory(Entity actor, Entity item)
        {
            if (actor != null && item != null)
            {
                actor.GetComponent<Inventory>().inventory.Remove(item);
            }
        }
        public static void OpenEquipment()
        {
            Program.inventoryConsole.Clear();
            equipmentOpen = true;
            player.GetComponent<TurnFunction>().turnActive = false;
            Program.playerConsole.Clear();

            string display = "";

            foreach (EquipmentSlot slot in player.GetComponent<Inventory>().equipment)
            {
                if (slot != null)
                {
                    if (slot.item != null)
                    {
                        display += $"{slot.name}: {slot.item.GetComponent<Description>().name}{spacer}";
                    }
                    else
                    {
                        display += $"{slot.name}: Empty{spacer}";
                    }
                }
            }

            CMath.DisplayToConsole(Program.playerConsole, display, 0, 2, 1);

            CMath.DisplayToConsole(Program.playerConsole, $"Close Equipment Yellow*[E/Escape]", 0, 2, 1, 32, false);

            Renderer.CreateConsoleBorder(Program.playerConsole, false);
            Program.playerConsole.Print(2, 0, " Stats ", Color.Gray);
            Program.playerConsole.Print(9, 0, $"{(char)196} Equipment {(char)196}", Color.Black);
            Program.playerConsole.Print(22, 0, " Inventory ", Color.Gray);
        }
        public static void CloseEquipment()
        {
            equipmentOpen = false; player.GetComponent<TurnFunction>().turnActive = true;
            //Action.PlayerAction(player);
            StatManager.UpdateStats(player);
            Log.DisplayLog();
        }
        public static void OpenInventory()
        {
            Program.playerConsole.Clear();
            Program.rootConsole.Children.MoveToTop(Program.inventoryConsole);
            inventoryOpen = true;
            player.GetComponent<TurnFunction>().turnActive = false;
            selection = 0;

            if (player.GetComponent<Inventory>().inventory.Count == 0)
            {
                Program.inventoryConsole.Print(2, 2, "Inventory is Empty.", Color.White);
                Renderer.CreateConsoleBorder(Program.inventoryConsole);
                //Action.InventoryAction(player);
            }
            else
            {
                Refresh();
                DisplayInventory();
                //Action.InventoryAction(player, RLKey.Unknown);
                //CMath.DisplayToConsole(Log.console, "Welcome to your inventory!", 1, 1);
            }
        }
        public static void CloseInventory()
        {
            Program.playerConsole.Clear();
            inventoryDisplay.Clear();
            Program.inventoryConsole.Clear();
            inventoryOpen = false; 
            player.GetComponent<TurnFunction>().turnActive = true;
            Program.rootConsole.Children.MoveToTop(Program.playerConsole);
            Program.rootConsole.Children.MoveToTop(Program.logConsole);
            StatManager.UpdateStats(player);
            Log.DisplayLog();
        }
        public static void Refresh()
        {
            inventoryDisplay.Clear();
            foreach (Entity item in player.GetComponent<Inventory>().inventory)
            {
                inventoryDisplay.Add(item);
            }
            DisplayInventory();
        }
        public static void EquipItem(Entity entity, Entity item)
        {
            if (item.GetComponent<Equippable>() != null)
            {
                if (entity.GetComponent<Inventory>().ReturnSlot(item.GetComponent<Equippable>().slot).item != null)
                {
                    if (entity.GetComponent<Inventory>().ReturnSlot(item.GetComponent<Equippable>().slot).item.GetComponent<Equippable>().unequipable)
                    {
                        UnequipItem(entity, entity.GetComponent<Inventory>().ReturnSlot(item.GetComponent<Equippable>().slot).item);
                        item.GetComponent<Equippable>().Equip(entity);
                        if (entity.GetComponent<PlayerComponent>() != null)
                        {
                            Log.Add($"You equip the {item.GetComponent<Description>().name}.");
                            CloseInventory();
                        }
                        entity.GetComponent<TurnFunction>().EndTurn();
                    }
                    else if (entity.GetComponent<PlayerComponent>() != null)
                    {
                        Log.Add($"You cannot equip the {item.GetComponent<Description>().name} because the {entity.GetComponent<Inventory>().ReturnSlot(item.GetComponent<Equippable>().slot).item.GetComponent<Description>().name} cannot be unequipped.");
                    }
                }
                else
                {
                    item.GetComponent<Equippable>().Equip(entity);
                    if (entity.GetComponent<PlayerComponent>() != null)
                    {
                        Log.Add($"You equip the {item.GetComponent<Description>().name}.");
                        CloseInventory();
                    }
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
            else if (entity.GetComponent<PlayerComponent>() != null)
            {
                Log.Add($"You cannot equip the {item.GetComponent<Description>().name} .");
            }
        }
        public static void UnequipItem(Entity entity, Entity item, bool display = false)
        {
            if (item.GetComponent<Equippable>().unequipable)
            {
                item.GetComponent<Equippable>().Unequip(entity);
                if (entity.GetComponent<PlayerComponent>() != null && display)
                {
                    CloseInventory();
                    Log.Add($"You unequip the {item.GetComponent<Description>().name}.");
                    entity.GetComponent<TurnFunction>().EndTurn();
                }
            }
            else if (entity.GetComponent<PlayerComponent>() != null)
            {
                Log.Add($"You cannot unequip the {item.GetComponent<Description>().name}.");
            }
        }
        public static void UseItem(Entity entity, Entity item)
        {
            entity.GetComponent<Inventory>().inventory.Remove(item);
            if (item.GetComponent<Equippable>() != null && item.GetComponent<Equippable>().equipped)
            {
                item.GetComponent<Equippable>().Unequip(entity);
            }
            if (entity.GetComponent<PlayerComponent>() != null) { CloseInventory(); }
            item.GetComponent<Usable>().Use(entity, null);
            if (!TargetReticle.targeting)
            {
                entity.GetComponent<TurnFunction>().EndTurn();
            }
        }
        public static void PlaceItem(Vector2 targetCoordinate, Entity item)
        {
            Vector2 placement = CMath.ReturnNearestValidCoordinate("Item", targetCoordinate);
            item.GetComponent<Vector2>().x = placement.x;
            item.GetComponent<Vector2>().y = placement.y;
            World.tiles[placement.x, placement.y].itemLayer = item;
            EntityManager.UpdateMap(item);

        }
        public static void DisplayInventory()
        {
            Program.inventoryConsole.Clear();

            //Create list of items with selection indicator.
            int y = 2;
            int index = -1;

            for (int i = selection - maxItemPerSide; i <= selection + maxItemPerSide; i++)
            {
                if (inventoryDisplay.Count <= (maxItemPerSide * 2) + 1) { index++; }
                else if (i < 0) { index = inventoryDisplay.Count + i; }
                else if (i >= inventoryDisplay.Count) { index = i - inventoryDisplay.Count; }
                else { index = i; }

                //If the index is equal to the selection create a pattern like this >{INCLUDE ITEM NAME HERE}<.
                if (index >= 0 && index < inventoryDisplay.Count)
                {
                    if (selection == index)
                    {
                        Program.inventoryConsole.SetCellAppearance(2, y, new ColoredGlyph(Color.Yellow, Color.Black, '>'));
                        Program.inventoryConsole.SetCellAppearance(Program.inventoryConsole.Width - 3, y, new ColoredGlyph(Color.Yellow, Color.Black, '<'));
                    }
                    if (inventoryDisplay[index].GetComponent<Equippable>() != null && inventoryDisplay[index].GetComponent<Equippable>().equipped)
                    {
                        CMath.DisplayToConsole(Program.inventoryConsole, $"{inventoryDisplay[index].GetComponent<Description>().name}- Equipped", 3, 0, 0, y, false);
                    }
                    else { CMath.DisplayToConsole(Program.inventoryConsole, $"{inventoryDisplay[index].GetComponent<Description>().name}", 3, 0, 0, y, false); }
                }
                else
                {
                    Program.inventoryConsole.DrawLine(new Point(2, y), new Point(Program.inventoryConsole.Width - 3, y), '-', Color.White);
                }
                y++;
            }

            //Draw box around item display
            Program.inventoryConsole.DrawBox(new Rectangle(1, 1, Program.inventoryConsole.Width - 2, (maxItemPerSide * 2) + 3),
                ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));

            Program.inventoryConsole.Print(2, (maxItemPerSide * 2) + 3, 
                $" {selection + 1}/{inventoryDisplay.Count} ".Align(HorizontalAlignment.Center, Program.inventoryConsole.Width - 5, 
                    (char)196), Color.Yellow, Color.Black);

            //Display selected item.
            string addition = "";
            if (inventoryDisplay[selection].GetComponent<Equippable>() != null)
            {
                string[] slot = inventoryDisplay[selection].GetComponent<Equippable>().slot.Split();
                if (slot.Count() == 1)
                {
                    addition += $"{spacer}Yellow*Can be equipped/unequipped in Yellow*{slot[0]} with Yellow*[E].";
                }
                else
                {
                    addition += $"{spacer}Yellow*Can be equipped/unequipped in Yellow*{slot[0]} Yellow*{slot[1]} with Yellow*[E].";
                }

                if (inventoryDisplay[selection].GetComponent<AttackFunction>() != null)
                {
                    AttackFunction function = inventoryDisplay[selection].GetComponent<AttackFunction>();
                    addition += $"{spacer} Does Yellow*{function.die1}d{function.die2} damage with a damage modifier of Yellow*{function.damageModifier} and a bonus to hit of Yellow*{function.toHitModifier}.";
                }
            }
            else
            {
                addition += $"{spacer}Yellow*Cannot be equipped.";
            }
            if (inventoryDisplay[selection].GetComponent<Usable>() != null)
            {
                addition += $"{spacer}Yellow*Can be used with Yellow*[U].";
            }
            else
            {
                addition += $"{spacer}Yellow*Cannot be used.";
            }
            addition += $"{spacer}Yellow*Can be thrown with Yellow*[T].";

            Description description = inventoryDisplay[selection].GetComponent<Description>();
            int length = CMath.DisplayToConsole(Program.inventoryConsole, description.description + addition, 2, 1, 0, (maxItemPerSide * 2) + 6, false);

            //Create box surrounding item information.

            Program.inventoryConsole.DrawBox(new Rectangle(1, (maxItemPerSide * 2) + 4, Program.inventoryConsole.Width - 2, 50 - ((maxItemPerSide * 2) + 5)),
                ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));

            //Create outline for console with console name.
            Renderer.CreateConsoleBorder(Program.inventoryConsole, true);
        }
        public static void MoveSelection(bool up)
        {
            if (player.GetComponent<Inventory>().inventory.Count != 0)
            {
                int move; 
                if (up) { move = -1; }
                else { move = 1; }
                if (selection + move > -1 && selection + move < inventoryDisplay.Count)
                {
                    selection += move;
                }
                else if (selection + move <= -1)
                {
                    selection = inventoryDisplay.Count - 1;
                }
                else if (selection + move >= inventoryDisplay.Count)
                {
                    selection = 0; 
                }
                DisplayInventory();
            }
        }
    }
}
