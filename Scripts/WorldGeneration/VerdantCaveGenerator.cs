using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class VerdantCaveGenerator : AGenerator
    {
        private int wallsNeeded = 4;
        private int randomFill;
        private int smooth = 5;
        public void CreateMap(int _mapWidth, int _mapHeight, int strength)
        {
            mapWidth = _mapWidth; mapHeight = _mapHeight;
            randomFill = World.seed.Next(48, 52);
            SetAllWalls();

            for (int x = 3; x < mapWidth - 3; x++)
            {
                for (int y = 3; y < mapHeight - 3; y++)
                {
                    if (CMath.CheckBounds(x, y))
                    {
                        if (World.seed.Next(0, 100) < randomFill) 
                        {
                            if (World.seed.Next(0, 101) > 50)
                            {
                                SetTile(x, y, '.', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                            }
                            else
                            {
                                SetTile(x, y, ',', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                            }
                        }
                    }
                }
            }
            for (int z = 0; z < smooth; z++) { SmoothMap(); }
            CreateSurroundingWalls();
            CreateConnections(1, 0);
            CreateFoliage();
            CreateStairs();
        }
        public void CreateFoliage()
        {
            for (int x = 3; x < mapWidth - 3; x++)
            {
                for (int y = 3; y < mapHeight - 3; y++)
                {
                    if (CMath.CheckBounds(x, y))
                    {
                        if (World.seed.Next(0, 100) > 90 && World.tiles[x, y].terrainType == 1)
                        {
                            if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '.', "Yellow*Dead Leaves", "Some Yellow*Dead Leaves.", "Yellow", "Black", false, 1); }
                            else { SetTile(x, y, ',', "Yellow*Dead Leaves", "Some Yellow*Dead Leaves.", "Yellow", "Black", false, 1); }
                        }
                        if (World.seed.Next(0, 100) > 80 && World.tiles[x, y].terrainType == 1)
                        {
                            if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '.', "Red*Dead Leaves", "Some Red*Dead Leaves.", "Red", "Black", false, 1); }
                            else { SetTile(x, y, ',', "Red*Dead Leaves", "Some Red*Dead Leaves.", "Red", "Black", false, 1); }
                        }
                        else if (World.seed.Next(0, 90) > 60 && World.tiles[x, y].terrainType == 1)
                        {
                            Entity foliage = new Entity(new List<Component>()
                            {
                                new Vector2(x, y),
                                new Draw("Green", "Black", (char)231),
                                new Description("Green*Foliage", "Some thick Green*green Green*foliage."),
                                new Visibility(true, false, false),
                            });
                            World.tiles[x, y].obstacleLayer = foliage;
                        }
                    }
                }
            }
        }
        public override void SetAllWalls()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Dark_Brown", "Black", true, 0); }
                    else { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Dark_Brown", "Black", true, 0); }
                }
            }
        }
        public void SmoothMap()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (CMath.CheckBounds(x, y))
                    {
                        int walls = WallCount(x, y);
                        if (walls > wallsNeeded) { if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Dark_Brown", "Black", true, 0); }
                        else { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Dark_Brown", "Black", true, 0); } }
                        else if (walls < wallsNeeded)
                        {
                            if (World.seed.Next(0, 101) > 50)
                            {
                                SetTile(x, y, '.', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                            }
                            else
                            {
                                SetTile(x, y, ',', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                            }
                        }
                    }
                }
            }
        }
        public static int WallCount(int sX, int sY)
        {
            int walls = 0;

            for (int x = sX - 1; x <= sX + 1; x++)
            {
                for (int y = sY - 1; y <= sY + 1; y++)
                {
                   if (x != sX || y != sY) { if (CMath.CheckBounds(x, y) && World.tiles[x, y].terrainType == 0) { walls++; } }
                }
            }

            return walls;
        }
        public override void CreateDiagonalPassage(int r1x, int r1y, int r2x, int r2y)
        {

        }
        public override void CreateBezierCurve(int r0x, int r0y, int r2x, int r2y)
        {

        }
        public override void CreateStraightPassage(int r1x, int r1y, int r2x, int r2y)
        {
            if (World.seed.Next(0, 1) == 0)
            {
                for (int x = Math.Min(r1x, r2x); x <= Math.Max(r1x, r2x); x++)
                {
                    if (World.seed.Next(0, 101) > 50)
                    {
                        SetTile(x, r1y, '.', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                    else
                    {
                        SetTile(x, r1y, ',', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                }
                for (int y = Math.Min(r1y, r2y); y <= Math.Max(r1y, r2y); y++)
                {
                    if (World.seed.Next(0, 101) > 50)
                    {
                        SetTile(r2x, y, '.', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                    else
                    {
                        SetTile(r2x, y, ',', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                }
            }
            else
            {
                for (int y = Math.Min(r1y, r2y); y <= Math.Max(r1y, r2y); y++)
                {
                    if (World.seed.Next(0, 101) > 50)
                    {
                        SetTile(r1x, y, '.', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                    else
                    {
                        SetTile(r1x, y, ',', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                }
                for (int x = Math.Min(r1x, r2x); x <= Math.Max(r1x, r2x); x++)
                {
                    if (World.seed.Next(0, 101) > 50)
                    {
                        SetTile(x, r2y, '.', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                    else
                    {
                        SetTile(x, r2y, ',', "Dirt floor", "The damp dirt ground.", "Brown", "Black", false, 1);
                    }
                }
            }
        }
    }
}
