using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SadConsole.Entities;
using SadConsole.Host;
using System.ComponentModel;

namespace The_Ruins_of_Ipsus
{
    public class Renderer
    {
        public static int mapWidth;
        public static int mapHeight;
        public static TitleConsole mapConsole;
        public static bool inventoryOpen = false;
        public static List<ParticleComponent> particles = new List<ParticleComponent>();
        public static List<Entity> clearList = new List<Entity>();
        public static int current = 1;
        public static bool running = false;
        public static int offSetX { get; set; }
        public static int offSetY { get; set; }
        public static int minX { get; set; }
        public static int maxX { get; set; }
        public static int minY { get; set; }
        public static int maxY { get; set; }
        public Renderer(TitleConsole _mapConsole, int _mapWidth, int _mapHeight)
        {
            //rootConsole = _rootConsole;
            mapConsole = _mapConsole;
            mapWidth = _mapWidth;
            mapHeight = _mapHeight;
        }
        public static void AddParticle(int x, int y, Entity particle)
        {
            if (particles.Count > 0)
            {
                //Log.Add("Started Thread");
                //Thread thread = new Thread(() => RenderParticles());
                //thread.Start();
            }
            particle.GetComponent<Vector2>().x = x;
            particle.GetComponent<Vector2>().y = y;
            ParticleComponent particleComponent = particle.GetComponent<ParticleComponent>();
            Program.rootConsole.particles.Add(particleComponent);
        }
        public static void StartAnimation(List<Entity> particlesRef)
        {
            foreach (Entity particle in particlesRef)
            {
                if (particle != null)
                {
                    Vector2 vector2 = particle.GetComponent<Vector2>();
                    ParticleComponent particleComponent = particle.GetComponent<ParticleComponent>();
                    Program.rootConsole.particles.Add(particleComponent);
                }
            }
            //RenderParticles();
        }
        public static void RenderParticles()
        {
            while (particles.Count > 0)
            {
                if (particles.Count > 0)
                {
                    for (int i = 0; i < particles.Count; i++)
                    {
                        ParticleComponent particle = particles[i];
                        if (particle != null)
                        {

                        }
                    }
                    DrawMapToScreen();
                }

                //Thread.Sleep(TimeSpan.FromMilliseconds(16.66f));

                if (current == 10)
                {
                    current = 1;
                }
                else
                {
                    current++;
                }
            }
        }
        public static void RenderMenu()
        {
            /*
            if (Menu.openingScreen)
            {
                rootConsole.Print((rootConsole.Width / 2) - 46, (rootConsole.Height / 3) - 14, " ______  _             ____         __                   ___   __                          ", RLColor.White);
                rootConsole.Print((rootConsole.Width / 2) - 46, (rootConsole.Height / 3) - 13, "|__/ __||/|__   ____  |__  | __ __ |__| ___  ____   ___ |/__| |/ | ____  ____  __ __  ____ ", RLColor.White);
                rootConsole.Print((rootConsole.Width / 2) - 46, (rootConsole.Height / 3) - 12, "  |/ |  |/|  | |/__ | |/  _||/ |  ||/ ||/  ||/ __| |/_ ||/ _| |/ ||/__ ||/ __||/ |  ||/ __|", RLColor.White);
                rootConsole.Print((rootConsole.Width / 2) - 46, (rootConsole.Height / 3) - 11, "  |/ |  |/ _  ||/___| |/| | |/ |  ||/ ||/| ||___ | ||_|||/|   |/ ||/ __||___ ||/ |  ||___ |", RLColor.White);
                rootConsole.Print((rootConsole.Width / 2) - 46, (rootConsole.Height / 3) - 10, "  |__|  |_| |_||____| |_|_| |_____||__||_|_||____| |___||_|   |__||_|   |____||_____||____|", RLColor.White);
                rootConsole.Print((rootConsole.Width / 2) - 7, (rootConsole.Height / 2) - 6, "New Game: [N]", RLColor.White);
                if (SaveDataManager.savePresent) { rootConsole.Print((rootConsole.Width / 2) - 10, (rootConsole.Height / 2) - 3, "Load Save Game: [L]", RLColor.White); }
                else { rootConsole.Print((rootConsole.Width / 2) - 10, (rootConsole.Height / 2) - 3, "Load Save Game: [L]", RLColor.Gray); }
                rootConsole.Print((rootConsole.Width / 2) - 5, rootConsole.Height / 2, "Quit: [Q]", RLColor.White);
            }
            else
            {
                for (int x = 0; x < 100; x++)
                {
                    for (int y = 48; y > 40; y--)
                    {
                        //rootConsole.Set(x, y, ColorFinder.ColorPicker("Dark_Brown"), ColorFinder.ColorPicker("Black"), (char)177);
                    }
                }
                for (int x = 0; x < 100; x++)
                {
                    for (int y = 40; y > 32; y--)
                    {
                        //rootConsole.Set(x, y, ColorFinder.ColorPicker("Dark_Brown"), ColorFinder.ColorPicker("Dark_Gray"), (char)177);
                    }
                }
                for (int x = 0; x < 100; x++)
                {
                    //rootConsole.Set(x, 32, ColorFinder.ColorPicker("Dark_Brown"), ColorFinder.ColorPicker("Dark_Gray"), ',');
                }
                for (int x = 0; x < 100; x++)
                {
                    //rootConsole.Set(x, 31, ColorFinder.ColorPicker("Green"), ColorFinder.ColorPicker("Dark_Green"), 'x');
                }
                for (int x = 20; x < 80; x++)
                {
                    for (int y = 30; y > 5; y--)
                    {
                        //rootConsole.Set(x, y, ColorFinder.ColorPicker("Gray"), ColorFinder.ColorPicker("Dark_Gray"), (char)177);
                    }
                }

                string[] nameParts = Menu.causeOfDeath.Split(' ');
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

                //Program.rootConsole.Print(50 - (int)Math.Ceiling((double)name.Length/ 2 + 1), (rootConsole.Height / 3) - 2, " " + name.Trim() + ". ", ColorFinder.ColorPicker("Light_Gray"), RLColor.Black);

                rootConsole.Print((rootConsole.Width / 2) - 13, (rootConsole.Height / 2) + 16, "New Game: [N] - Quit: [Q]", RLColor.Brown, RLColor.Black);
            }
            //CreateConsoleBorder(rootConsole);
            */
        }
        public static void MoveCamera(Vector2 vector3)
        {
            minX = vector3.x - mapWidth / 2;
            maxX = minX + mapWidth;
            minY = vector3.y - mapHeight / 2;
            maxY = minY + mapHeight;

            offSetX = (minX + maxX) / 2;
            offSetY = (minY + maxY) / 2;
        }
        public static void DrawMapToScreen()
        {
            int y = 0;
            for (int ty = minY; ty < maxY; ty++)
            {
                int x = 0;
                for (int tx = minX; tx < maxX; tx++)
                {
                    if (CMath.CheckBounds(tx, ty))
                    {
                        Entity tile = World.tiles[tx, ty].entity;
                        Visibility visibility = tile.GetComponent<Visibility>();
                        Traversable traversable = tile.GetComponent<Traversable>();

                        if (World.sfx[tx, ty] != null) { World.sfx[tx, ty].DrawToScreen(Program.mapConsole, x, y); }
                        else if (!visibility.visible && !visibility.explored) { mapConsole.SetCellAppearance(x, y, new ColoredGlyph(Color.Black, Color.Black, '?')); }
                        else if (!visibility.visible && visibility.explored)
                        {
                            if (traversable.obstacleLayer != null)
                            {
                                Draw draw = traversable.obstacleLayer.GetComponent<Draw>();
                                mapConsole.SetCellAppearance(x, y, new ColoredGlyph(new Color(ColorFinder.ColorPicker(draw.fColor), .5f), Color.Black, draw.character));
                            }
                            else
                            {
                                Draw draw = tile.GetComponent<Draw>();
                                mapConsole.SetCellAppearance(x, y, new ColoredGlyph(new Color(ColorFinder.ColorPicker(draw.fColor), .5f), Color.Black, draw.character));
                            }
                        }
                        else if (traversable.actorLayer != null) { traversable.actorLayer.GetComponent<Draw>().DrawToScreen(mapConsole, x, y); }
                        else if (traversable.itemLayer != null) { traversable.itemLayer.GetComponent<Draw>().DrawToScreen(mapConsole, x, y); }
                        else if (traversable.obstacleLayer != null) { traversable.obstacleLayer.GetComponent<Draw>().DrawToScreen(mapConsole, x, y); }
                        else { tile.GetComponent<Draw>().DrawToScreen(mapConsole, x, y); }
                    }
                    else 
                    {
                        mapConsole.SetCellAppearance(x, y, new ColoredGlyph(Color.Black, Color.Black, '?')); 
                    }
                    x++;
                }
                y++;
            }

            CreateConsoleBorder(mapConsole);

            mapConsole.IsDirty = true;
        }
        public static void CreateConsoleBorder(TitleConsole console, bool includeTitle = true)
        {
            console.DrawBox(new Rectangle(0, 0, console.Width, console.Height), 
                ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.AntiqueWhite, Color.Black)));
            if (includeTitle)
            {
                console.Print(0, 0, console.title.Align(HorizontalAlignment.Center, console.Width, (char)196), Color.Black, Color.AntiqueWhite);
            }
            console.IsDirty = true;
        }
    }
}
