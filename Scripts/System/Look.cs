using SadRogue.Primitives;
using SadConsole;
using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    public class Look
    {
        public static Entity player;
        public static Vector2 position { get; set; }
        public static bool looking = false;
        public Look(Entity _player) { player = _player; }
        public static void StartLooking(Vector2 coordinate)
        {
            position = new Vector2(coordinate.x, coordinate.y);
            looking = true;
            player.GetComponent<TurnFunction>().turnActive = false;
            Program.playerConsole.Clear();
            Program.lookConsole.Clear();
            Program.rootConsole.Children.MoveToTop(Program.lookConsole);
            Move(0, 0);
        }
        public static void StopLooking()
        {
            player.GetComponent<TurnFunction>().turnActive = true;
            Renderer.MoveCamera(player.GetComponent<Vector2>());
            looking = false;

            Program.playerConsole.Clear();
            Program.lookConsole.Clear();
            Program.rootConsole.Children.MoveToTop(Program.playerConsole);
            Program.rootConsole.Children.MoveToTop(Program.mapConsole);
            StatManager.UpdateStats(player);
            Log.DisplayLog();
            World.ClearSFX();
            Renderer.DrawMapToScreen();
        }
        public static void Move(int _x, int _y)
        {
            if (CMath.CheckBounds(position.x + _x, position.y + _y))
            {
                //Change position of reticle and clear all SFX/LookConsole.
                World.ClearSFX();
                Program.lookConsole.Clear();
                position.x += _x; position.y += _y;

                //Display description of object player is looking at, if object cannot be seen produce a message.
                Traversable traversable = World.tiles[position.x, position.y];
                Description description = null;
                string health = "";

                if (!World.tiles[position.x, position.y].entity.GetComponent<Visibility>().visible)
                {
                    Program.lookConsole.Fill(Color.AntiqueWhite, Color.Black, 177);
                    Program.lookConsole.Print(0, Program.lookConsole.Height / 2 - 3, " You cannot look at ".Align(HorizontalAlignment.Center, Program.lookConsole.Width, (char)177), Color.AntiqueWhite);
                    Program.lookConsole.Print(0, Program.lookConsole.Height / 2 - 1, " what you cannot see. ".Align(HorizontalAlignment.Center, Program.lookConsole.Width, (char)177), Color.AntiqueWhite);
                    Program.lookConsole.DrawBox(new Rectangle(1, 1, Program.lookConsole.Width - 2, Program.lookConsole.Height - 2),
                        ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));

                }
                else if (traversable.actorLayer != null)
                {
                    description = traversable.actorLayer.GetComponent<Description>();
                }
                else if (traversable.itemLayer != null)
                {
                    description = traversable.itemLayer.GetComponent<Description>();
                }
                else if (traversable.obstacleLayer != null)
                {
                    description = traversable.obstacleLayer.GetComponent<Description>();
                }
                else
                {
                    description = World.tiles[position.x, position.y].entity.GetComponent<Description>();
                }
                if (description != null)
                {

                    if (description.entity != null && description.entity.GetComponent<PronounSet>() != null)
                    {
                        if (description.entity.GetComponent<PronounSet>().present)
                        {
                            health += $"{description.name} is: + ";
                        }
                        else
                        {
                            health += $"{description.name} are: + ";
                        }

                        if (CMath.ReturnAI(description.entity) != null)
                        {
                            health += $"{CMath.ReturnAI(description.entity).currentState}, ";

                            if (description.entity.GetComponent<Harmable>().statusEffects.Count == 0)
                            {
                                health += "and ";
                            }
                        }

                        Stats stats = description.entity.GetComponent<Stats>();

                        if (stats.hp == stats.hpCap) { health += "Green*Uninjured"; }
                        else if (stats.hp <= stats.hpCap && stats.hp >= stats.hpCap / 2) { health += "Yellow*Hurt"; }
                        else { health += "Red*Badly Red*Hurt"; }

                        if (description.entity.GetComponent<Harmable>().statusEffects.Count == 0)
                        {
                            health += ".";
                        }
                        else
                        {
                            health += ", + ";
                        }

                        for (int i = 0; i < description.entity.GetComponent<Harmable>().statusEffects.Count; i++)
                        {
                            if (i == description.entity.GetComponent<Harmable>().statusEffects.Count - 1)
                            {
                                health += $"and {description.entity.GetComponent<Harmable>().statusEffects[i]}. + ";
                            }
                            else
                            {
                                health += $"{description.entity.GetComponent<Harmable>().statusEffects[i]}, ";
                            }
                        }
                    }


                    //Create boxes to surround look menu, and display information.
                    Program.lookConsole.DrawBox(new Rectangle(1, 1, Program.lookConsole.Width - 2, 5),
                        ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));

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
                    int start = (Program.lookConsole.Width / 2) - (int)Math.Ceiling((double)name.Length / 2);

                    start++;

                    foreach (string part in nameParts)
                    {
                        string[] temp = part.Split('*');
                        if (temp.Length == 1)
                        {
                            Program.lookConsole.Print(start, 3, temp[0] + " ", Color.White);
                            start += temp[0].Length + 1;
                        }
                        else
                        {
                            Program.lookConsole.Print(start, 3, temp[1] + " ", ColorFinder.ColorPicker(temp[0]), Color.Black);
                            start += temp[1].Length + 1;
                        }
                    }
                    if (health == "")
                    {
                        Program.lookConsole.DrawBox(new Rectangle(1, 6, Program.lookConsole.Width - 2, Program.lookConsole.Height - 7),
                            ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));
                        CMath.DisplayToConsole(Program.lookConsole, $"{description.description}", 2, 1, 0, 8, false);
                    }
                    else if (health != "")
                    {
                        int difference = CMath.DisplayToConsole(Program.lookConsole, $"{health}", 2, 1, 0, 8, false);

                        Program.lookConsole.DrawBox(new Rectangle(1, 6, Program.lookConsole.Width - 2, difference),
                            ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));

                        int difference2 = CMath.DisplayToConsole(Program.lookConsole, $"{description.description}", 2, 1, 0, 8 + difference, false);

                        Program.lookConsole.DrawBox(new Rectangle(1, 6 + difference, Program.lookConsole.Width - 2, Program.lookConsole.Height - (7 + difference)),
                            ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black)));
                    }

                    World.sfx[position.x, position.y] = new Draw("Yellow", "Black", 'X');
                }
                else
                {
                    World.sfx[position.x, position.y] = new Draw("Gray", "Black", 'X');
                }
            }

            Renderer.CreateConsoleBorder(Program.lookConsole);
            Renderer.MoveCamera(new Vector2(position.x, position.y));
            Renderer.DrawMapToScreen();
            Program.rootConsole.IsDirty = true;
        }
    }
}
