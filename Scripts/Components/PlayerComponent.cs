using System;
using SadConsole.Components;
using SadConsole;
using SadConsole.Input;
using The_Ruins_of_Ipsus.Scripts.JsonDataManagement;

namespace The_Ruins_of_Ipsus
{
    class PlayerComponent: Component
    {
        public PlayerComponent() { }
    }
    class KeyboardComponent : KeyboardConsoleComponent
    {
        public override void ProcessKeyboard(IScreenObject host, Keyboard info, out bool handled)
        {
            if (Program.gameActive)
            {
                if (Program.player.GetComponent<TurnFunction>().turnActive)
                {
                    if (info.IsKeyPressed(Keys.Up)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(0, -1))); }
                    else if (info.IsKeyPressed(Keys.Down)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(0, 1))); }
                    else if (info.IsKeyPressed(Keys.Left)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(-1, 0))); }
                    else if (info.IsKeyPressed(Keys.Right)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(1, 0))); }
                    else if (info.IsKeyPressed(Keys.NumPad8)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(0, -1))); }
                    else if (info.IsKeyPressed(Keys.NumPad9)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(1, -1))); }
                    else if (info.IsKeyPressed(Keys.NumPad6)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(1, 0))); }
                    else if (info.IsKeyPressed(Keys.NumPad3)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(1, 1))); }
                    else if (info.IsKeyPressed(Keys.NumPad2)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(0, 1))); }
                    else if (info.IsKeyPressed(Keys.NumPad1)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(-1, 1))); }
                    else if (info.IsKeyPressed(Keys.NumPad4)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(-1, 0))); }
                    else if (info.IsKeyPressed(Keys.NumPad7)) { Program.player.GetComponent<Movement>().Move(new Vector2(Program.player.GetComponent<Vector2>(), new Vector2(-1, -1))); }
                    else if (info.IsKeyPressed(Keys.OemPlus))
                    {
                        Vector2 vector2 = Program.player.GetComponent<Vector2>();
                        if (World.tiles[vector2.x, vector2.y].entity.GetComponent<Draw>().character == '>') { World.GenerateNewFloor(true); }
                    }
                    else if (info.IsKeyPressed(Keys.OemMinus))
                    {
                        Vector2 vector2 = Program.player.GetComponent<Vector2>();
                        if (World.tiles[vector2.x, vector2.y].entity.GetComponent<Draw>().character == '<') { World.GenerateNewFloor(false); }
                    }
                    else if (info.IsKeyPressed(Keys.OemPeriod)) { Program.player.GetComponent<TurnFunction>().EndTurn(); }
                    else if (info.IsKeyPressed(Keys.L)) { Look.StartLooking(Program.player.GetComponent<Vector2>()); }
                    else if (info.IsKeyPressed(Keys.I)) { InventoryManager.OpenInventory(); }
                    else if (info.IsKeyPressed(Keys.E)) { InventoryManager.OpenEquipment(); }
                    else if (info.IsKeyPressed(Keys.G)) { InventoryManager.GetItem(Program.player); Log.DisplayLog(); }
                    else if (info.IsKeyPressed(Keys.J)) { SaveDataManager.CreateSave(); Program.ExitProgram(); }
                    else if (info.IsKeyPressed(Keys.V))
                    {
                        foreach (Traversable tile in World.tiles)
                        {
                            if (tile != null)
                            {
                                Vector2 vector2 = tile.entity.GetComponent<Vector2>();
                                ShadowcastFOV.SetVisible(vector2, true, 1000, vector2.x, vector2.y, true);
                            }
                        }
                        Renderer.DrawMapToScreen();
                    }
                }
                else if (InventoryManager.inventoryOpen)
                {
                    if (info.IsKeyPressed(Keys.I)) { InventoryManager.CloseInventory(); }
                    else if (info.IsKeyPressed(Keys.Escape)) { InventoryManager.CloseInventory(); }
                    else if (info.IsKeyPressed(Keys.Up)) { InventoryManager.MoveSelection(true); }
                    else if (info.IsKeyPressed(Keys.Down)) { InventoryManager.MoveSelection(false); }
                    else if (info.IsKeyPressed(Keys.NumPad8)) { InventoryManager.MoveSelection(true); }
                    else if (info.IsKeyPressed(Keys.NumPad2)) { InventoryManager.MoveSelection(false); }
                    else if (info.IsKeyPressed(Keys.Left)) { InventoryManager.MoveSelection(true); }
                    else if (info.IsKeyPressed(Keys.Right)) { InventoryManager.MoveSelection(false); }
                    else if (info.IsKeyPressed(Keys.NumPad4)) { InventoryManager.MoveSelection(true); }
                    else if (info.IsKeyPressed(Keys.NumPad6)) { InventoryManager.MoveSelection(false); }
                    else if (info.IsKeyPressed(Keys.D))
                    {
                        if (Program.player.GetComponent<Inventory>().inventory.Count != 0)
                        {
                            InventoryManager.DropItem(Program.player, InventoryManager.inventoryDisplay[InventoryManager.selection]);
                        }
                    }
                    else if (info.IsKeyPressed(Keys.E))
                    {
                        if (Program.player.GetComponent<Inventory>().inventory.Count != 0)
                        {
                            int second = InventoryManager.selection;
                            if (InventoryManager.inventoryDisplay[second].GetComponent<Equippable>() != null)
                            {
                                if (InventoryManager.inventoryDisplay[second].GetComponent<Equippable>().equipped)
                                {
                                    InventoryManager.UnequipItem(Program.player, InventoryManager.inventoryDisplay[second], true);
                                }
                                else
                                {
                                    InventoryManager.EquipItem(Program.player, InventoryManager.inventoryDisplay[second]);
                                }
                            }
                            //else { CMath.DisplayToConsole(Program.logConsole), "You cannot equip the " + InventoryManager.inventoryDisplay[first][second].GetComponent<Description>().name + ".", 1, 1); }
                        }
                    }
                    else if (info.IsKeyPressed(Keys.U))
                    {
                        if (Program.player.GetComponent<Inventory>().inventory.Count != 0)
                        {
                            int second = InventoryManager.selection;
                            if (InventoryManager.inventoryDisplay[second].GetComponent<Usable>() != null)
                            {
                                TargetReticle.targetWeapon = InventoryManager.inventoryDisplay[second];
                                InventoryManager.UseItem(Program.player, InventoryManager.inventoryDisplay[second]);
                            }
                            //else { CMath.DisplayToConsole(Program.logConsole, "You cannot use the " + InventoryManager.inventoryDisplay[first][second].GetComponent<Description>().name + ".", 1, 1); }
                        }
                    }
                    else if (info.IsKeyPressed(Keys.T))
                    {
                        if (Program.player.GetComponent<Inventory>().inventory.Count != 0)
                        {
                            int second = InventoryManager.selection;
                            TargetReticle.targetWeapon = InventoryManager.inventoryDisplay[second];
                            TargetReticle.StartTargeting(true, true);
                        }
                    }
                }
                else if (InventoryManager.equipmentOpen)
                {
                    if (info.IsKeyPressed(Keys.Escape))
                    {
                        InventoryManager.CloseEquipment();
                    }
                    else if (info.IsKeyPressed(Keys.E))
                    {
                        InventoryManager.CloseEquipment();
                    }
                }
                else if (TargetReticle.targeting)
                {
                    if (info.IsKeyPressed(Keys.Up)) { TargetReticle.Move(0, -1); }
                    else if (info.IsKeyPressed(Keys.Down)) { TargetReticle.Move(0, 1); }
                    else if (info.IsKeyPressed(Keys.Left)) { TargetReticle.Move(-1, 0); }
                    else if (info.IsKeyPressed(Keys.Right)) { TargetReticle.Move(1, 0); }
                    else if (info.IsKeyPressed(Keys.NumPad8)) { TargetReticle.Move(0, -1); }
                    else if (info.IsKeyPressed(Keys.NumPad9)) { TargetReticle.Move(1, -1); }
                    else if (info.IsKeyPressed(Keys.NumPad6)) { TargetReticle.Move(1, 0); }
                    else if (info.IsKeyPressed(Keys.NumPad3)) { TargetReticle.Move(1, 1); }
                    else if (info.IsKeyPressed(Keys.NumPad2)) { TargetReticle.Move(0, 1); }
                    else if (info.IsKeyPressed(Keys.NumPad1)) { TargetReticle.Move(-1, 1); }
                    else if (info.IsKeyPressed(Keys.NumPad4)) { TargetReticle.Move(-1, 0); }
                    else if (info.IsKeyPressed(Keys.NumPad7)) { TargetReticle.Move(-1, -1); }
                    else if (info.IsKeyPressed(Keys.Escape)) { TargetReticle.StopTargeting(); }
                    else if (info.IsKeyPressed(Keys.S)) { TargetReticle.StopTargeting(); }
                    else if (info.IsKeyPressed(Keys.U)) { TargetReticle.Throw(Program.player); }
                    else if (info.IsKeyPressed(Keys.T)) { TargetReticle.Throw(Program.player); }
                    else if (info.IsKeyPressed(Keys.Enter)) { TargetReticle.Throw(Program.player); }
                }
                else if (Look.looking)
                {
                    if (info.IsKeyPressed(Keys.Up)) { Look.Move(0, -1); }
                    else if (info.IsKeyPressed(Keys.Down)) { Look.Move(0, 1); }
                    else if (info.IsKeyPressed(Keys.Left)) { Look.Move(-1, 0); }
                    else if (info.IsKeyPressed(Keys.Right)) { Look.Move(1, 0); }
                    else if (info.IsKeyPressed(Keys.NumPad8)) { Look.Move(0, -1); }
                    else if (info.IsKeyPressed(Keys.NumPad9)) { Look.Move(1, -1); }
                    else if (info.IsKeyPressed(Keys.NumPad6)) { Look.Move(1, 0); }
                    else if (info.IsKeyPressed(Keys.NumPad3)) { Look.Move(1, 1); }
                    else if (info.IsKeyPressed(Keys.NumPad2)) { Look.Move(0, 1); }
                    else if (info.IsKeyPressed(Keys.NumPad1)) { Look.Move(-1, 1); }
                    else if (info.IsKeyPressed(Keys.NumPad4)) { Look.Move(-1, 0); }
                    else if (info.IsKeyPressed(Keys.NumPad7)) { Look.Move(-1, -1); }
                    else if (info.IsKeyPressed(Keys.Escape)) { Look.StopLooking(); }
                    else if (info.IsKeyPressed(Keys.L)) { Look.StopLooking(); }
                }
            }
            else
            {
                if (info.IsKeyPressed(Keys.N)) { Program.NewGame(); }
                else if (info.IsKeyPressed(Keys.L) && SaveDataManager.savePresent) { SaveDataManager.LoadSave(); }
                else if (info.IsKeyPressed(Keys.Q)) { Program.ExitProgram(); }
            }
            handled = true;
        }
    }
}
