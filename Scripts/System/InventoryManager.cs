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
        private static TitleConsole console;
        public static bool inventoryOpen = false;
        public static bool equipmentOpen = false;
        private static Entity player;
        public static int selection = 0;
        public static int currentPage = 0;
        private static int maxItemPerPage = 6;
        private static string spacer = " + + ";
        public static List<List<Entity>> inventoryDisplay = new List<List<Entity>>();
        public InventoryManager(TitleConsole _console, Entity _player) { console = _console; player = _player; }
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
            console.Clear();
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
            console.Clear();
            inventoryOpen = true;
            player.GetComponent<TurnFunction>().turnActive = false;
            selection = 0; currentPage = 0;
            Program.playerConsole.Clear();

            if (player.GetComponent<Inventory>().inventory.Count == 0)
            {
                console.Print(2, 2, "Inventory is Empty.", Color.White);
                //Action.InventoryAction(player);
            }
            else
            {
                Refresh();
                DisplayItem();
                DisplayInventory();
                //Action.InventoryAction(player, RLKey.Unknown);
                //CMath.DisplayToConsole(Log.console, "Welcome to your inventory!", 1, 1);
            }
        }
        public static void CloseInventory()
        {
            inventoryDisplay.Clear();
            inventoryOpen = false; player.GetComponent<TurnFunction>().turnActive = true;
            //Action.PlayerAction(player);
            StatManager.UpdateStats(player);
            Log.DisplayLog();
        }
        public static void Refresh()
        {
            inventoryDisplay.Clear();
            int itemCount = 0;
            int pageCount = 0;
            inventoryDisplay.Add(new List<Entity>());
            foreach (Entity item in player.GetComponent<Inventory>().inventory)
            {
                if (itemCount == maxItemPerPage - 1) { itemCount = 0; pageCount++; inventoryDisplay.Add(new List<Entity>()); }
                inventoryDisplay[pageCount].Add(item);
                itemCount++;
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
            console.Clear();
            int x = 0;
            string output = "";
            foreach (Entity item in inventoryDisplay[currentPage])
            {
                string addOn = "";
                if (item.GetComponent<Equippable>() != null && item.GetComponent<Equippable>().equipped)
                {
                    addOn = " - Equipped";
                }
                if (selection == x)
                {
                    output += "X " + item.GetComponent<Description>().name + addOn + " + ";
                }
                else
                {
                    output += item.GetComponent<Description>().name + addOn + " + ";
                }
                x++;
            }
            CMath.DisplayToConsole(console, output, 2, 0, 0, 2);
            console.Print(12, 13, " Page:" + (currentPage + 1) + "/" + inventoryDisplay.Count + " ", Color.White);
            Renderer.CreateConsoleBorder(console, true);
        }
        public static void MoveSelection(int move)
        {
            if (player.GetComponent<Inventory>().inventory.Count != 0)
            {
                if (selection + move > -1 && selection + move < inventoryDisplay[currentPage].Count)
                {
                    selection += move;
                }
                else if (selection + move <= -1)
                {
                    MovePage(-1);
                    //selection = inventoryDisplay[currentPage].Count - 1;
                }
                else if (selection + move >= inventoryDisplay[currentPage].Count)
                {
                    MovePage(1);
                    //selection = 0; 
                }
                DisplayItem();
                DisplayInventory();
            }
        }
        public static void MovePage(int move)
        {
            if (player.GetComponent<Inventory>().inventory.Count != 0)
            {
                if (inventoryDisplay.Count > 1)
                {
                    if (currentPage + move > -1 && currentPage + move < inventoryDisplay.Count)
                    {
                        currentPage += move; selection = 0;
                    }
                    else if (currentPage + move <= -1)
                    {
                        currentPage = inventoryDisplay.Count - 1; selection = 0;
                    }
                    else if (currentPage + move >= inventoryDisplay.Count)
                    {
                        currentPage = 0; selection = 0;
                    }
                }
                DisplayItem();
                DisplayInventory();
            }
        }
        public static void DisplayItem()
        {
            string addition = "";
            if (inventoryDisplay[currentPage][selection].GetComponent<Equippable>() != null)
            {
                string[] slot = inventoryDisplay[currentPage][selection].GetComponent<Equippable>().slot.Split();
                if (slot.Count() == 1)
                {
                    addition += $"{spacer}Yellow*Can be equipped/unequipped in Yellow*{slot[0]} with Yellow*[E].";
                }
                else
                {
                    addition += $"{spacer}Yellow*Can be equipped/unequipped in Yellow*{slot[0]} Yellow*{slot[1]} with Yellow*[E].";
                }

                if (inventoryDisplay[currentPage][selection].GetComponent<AttackFunction>() != null)
                {
                    AttackFunction function = inventoryDisplay[currentPage][selection].GetComponent<AttackFunction>();
                    addition += $"{spacer} Does Yellow*{function.die1}d{function.die2} damage with a damage modifier of Yellow*{function.damageModifier} and a bonus to hit of Yellow*{function.toHitModifier}.";
                }
            }
            else
            {
                addition += $"{spacer}Yellow*Cannot be equipped.";
            }
            if (inventoryDisplay[currentPage][selection].GetComponent<Usable>() != null)
            {
                addition += $"{spacer}Yellow*Can be used with Yellow*[U].";
            }
            else
            {
                addition += $"{spacer}Yellow*Cannot be used.";
            }
            addition += $"{spacer}Yellow*Can be thrown with Yellow*[T].";

            Description description = inventoryDisplay[currentPage][selection].GetComponent<Description>();
            CMath.DisplayToConsole(Program.playerConsole, description.description + addition, 1, 1);
            string[] nameParts = description.name.Split(' ');
            string name = "";
            foreach (string part in nameParts)
            {
                string[] temp = part.Split('*');
                if (temp.Length == 1)
                {
                    name += temp[0] + " ";
                }
                else
                {
                    name += temp[1] + " ";
                }
            }
            int start = 17 - (int)Math.Ceiling((double)name.Length / 2);

            Program.playerConsole.Print(start, 0, " ", Color.White);

            start++;

            foreach (string part in nameParts)
            {
                string[] temp = part.Split('*');
                if (temp.Length == 1)
                {
                    Program.playerConsole.Print(start, 0, temp[0] + " ", Color.White, Color.Black);
                    start += temp[0].Length + 1;
                }
                else
                {
                    Program.playerConsole.Print(start, 0, temp[1] + " ", ColorFinder.ColorPicker(temp[0]), Color.Black);
                    start += temp[1].Length + 1;
                }
            }

            CMath.DisplayToConsole(Program.playerConsole, $"Select Item Yellow*[Arrow Yellow*Keys] {spacer} Close Inventory Yellow*[I/Escape]", 0, 2, 1, 29, false);
            CMath.DisplayToConsole(Program.playerConsole, $"Close Inventory Yellow*[I/Escape]", 0, 2, 1, 32, false);

            Renderer.CreateConsoleBorder(Program.playerConsole, false);

            Program.playerConsole.Print(2, 0, $" Stats {(char)196} Equipment ", Color.Gray);
            Program.playerConsole.Print(21, 0, $"{(char)196} Inventory ", Color.White);
        }
    }
}
