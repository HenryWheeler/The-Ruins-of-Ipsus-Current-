using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class FieldGenerator: AGenerator
    {
        public void CreateMap(int _mapWidth, int _mapHeight, int strength)
        {
            mapWidth = _mapWidth; mapHeight = _mapHeight;
            SetAllWalls();

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (CMath.CheckBounds(x, y))
                    {
                        int probability = World.seed.Next(0, 100);
                        if (probability > 80) { SetTile(x, y, '"', "Grass", "Soft Green Grass.", "Dark_Green", "Black", false, 1); }
                        else if (probability > 60) { SetTile(x, y, '`', "Grass", "Soft Green Grass.", "Light_Green", "Black", false, 1); }
                        else if (probability == 1) { CreateTreePatch(x, y); }
                        else { SetTile(x, y, '.', "Bare Ground", "The bare dirt ground.", "Brown", "Black", false, 1); }

                        probability = World.seed.Next(0, 3000);
                        if (probability == 1500) { CreatePond(x, y); }
                        else if (probability == 2000) { CreateBezierCurve(x, 1, 1, y); }
                        else if (probability < 500 && probability > 495) { SetTile(x, y, '*', "Rock", "A solid hunk of granite.", "Light_Gray", "Dark_Gray", false, 0); }

                        if (World.tiles[x, y] == null) { SetTile(x, y, '.', "Bare Ground", "The bare dirt ground.", "Brown", "Black", false, 1); }
                    }
                }
            }
            CreateSurroundingWalls();
            string table = "Field-" + strength.ToString();
            CreateStairs();
        }
        public override void SetAllWalls()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    int probability = World.seed.Next(0, 100);
                    if (probability > 80) { SetTile(x, y, '"', "Grass", "Soft Green Grass.", "Dark_Green", "Black", false, 1); }
                    else if (probability > 60) { SetTile(x, y, '`', "Grass", "Soft Green Grass.", "Light_Green", "Black", false, 1); }
                    else { SetTile(x, y, '.', "Bare Ground", "The bare dirt ground.", "Brown", "Black", false, 1); }
                }
            }
        }
        public void CreateTreePatch(int _x, int _y)
        {
            int size = World.seed.Next(1, 4);

            for (int x = _x - size; x < _x + size; x++)
            {
                for (int y = _y - size; y < _y + size; y++)
                {
                    if (CMath.CheckBounds(x, y))
                    {
                        if (World.seed.Next(1, 100) > 50) { SetTile(x, y, (char)20, "Oak Tree", "A solid sturdy oak.", "Dark_Green", "Black", true, 0); }
                    }
                }
            }
        }
        public void CreatePond(int _x, int _y)
        {
            int size = World.seed.Next(3, 12);
            for (int x = _x - size; x < _x + size; x++)
            {
                for (int y = _y - size; y < _y + size; y++)
                {
                    if (CMath.CheckBounds(x, y) && World.seed.Next(0, 100) > 50)
                    {
                        if (World.seed.Next(0, 100) < 50) { SetTile(x, y, (char)247, "Water", "A murky pool.", "Light_Blue", "Dark_Blue", false, 2); }
                        else { SetTile(x, y, (char)247, "Water", "A murky pool.", "Light_Blue", "Blue", false, 2); }
                    }
                }
            }
            for (int i = 0; i < size / 2; i++)
            {
                for (int x = _x - size; x < _x + size; x++)
                {
                    for (int y = _y - size; y < _y + size; y++)
                    {
                        if (x > 1 && x < mapWidth - 2 && y > 1 && y < mapHeight - 2)
                        {
                            if (CMath.CheckBounds(x, y))
                            {
                                int water = WaterCount(x, y);

                                if (water > 4)
                                {
                                    if (World.seed.Next(0, 100) < 50) { SetTile(x, y, (char)247, "Water", "A murky pool.", "Light_Blue", "Dark_Blue", false, 2); }
                                    else { SetTile(x, y, (char)247, "Water", "A murky pool.", "Light_Blue", "Blue", false, 2); }
                                }
                                else if (water < 4)
                                {
                                    int probability = World.seed.Next(0, 100);
                                    if (probability > 80) { SetTile(x, y, '"', "Grass", "Soft Green Grass.", "Dark_Green", "Black", false, 1); }
                                    else if (probability > 60) { SetTile(x, y, '`', "Grass", "Soft Green Grass.", "Light_Green", "Black", false, 1); }
                                    else { SetTile(x, y, '.', "Bare Ground", "The bare dirt ground.", "Brown", "Black", false, 1); }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static int WaterCount(int sX, int sY)
        {
            int walls = 0;

            for (int x = sX - 1; x <= sX + 1; x++)
            {
                for (int y = sY - 1; y <= sY + 1; y++)
                {
                    if (x != sX || y != sY) { if (CMath.CheckBounds(x, y) && World.tiles[x, y] != null && World.tiles[x, y].terrainType == 2) { walls++; } }
                }
            }

            return walls;
        }
        public override void CreateDiagonalPassage(int r1x, int r1y, int r2x, int r2y) { }
        public override void CreateBezierCurve(int r0x, int r0y, int r2x, int r2y)
        {
            int r1x; int r1y;

            r1x = World.seed.Next(1, 80);
            r1y = World.seed.Next(1, 70);

            for (float t = 0; t < 1; t += .001f)
            {
                int x = (int)((1 - t) * ((1 - t) * r0x + t * r1x) + t * ((1 - t) * r0x + t * r2x));
                int y = (int)((1 - t) * ((1 - t) * r0y + t * r1y) + t * ((1 - t) * r0y + t * r2y));
                if (CMath.CheckBounds(x, y))
                {
                    if (World.seed.Next(0, 100) < 50) { SetTile(x, y, (char)247, "Water", "A murky pool.", "Light_Blue", "Dark_Blue", false, 2); }
                    else { SetTile(x, y, (char)247, "Water", "A murky pool.", "Light_Blue", "Blue", false, 2); }
                }
            }
        }
        public override void CreateStraightPassage(int r1x, int r1y, int r2x, int r2y) { }
    }
}
