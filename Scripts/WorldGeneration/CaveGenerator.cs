using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class CaveGenerator: AGenerator
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
                        if (World.seed.Next(0, 100) < randomFill) { if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Gray_Blue", "Black", false, 1); } 
                        else { SetTile(x, y, '`', "Stone Floor", "A simple stone floor.", "Light_Gray_Blue", "Black", false, 1); } }
                    }
                }
            }
            for (int z = 0; z < smooth; z++) { SmoothMap(); }
            CreateSurroundingWalls();
            CreateConnections(3, 2);
            string table = "Cave-" + strength.ToString();
            //FillChunk(table, World.seed.Next(6, 10));
            CreateStairs();
        }
        public override void SetAllWalls()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0); }
                    else { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0); }
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
                        if (walls > wallsNeeded) { if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0); }
                        else { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0); } }
                        else if (walls < wallsNeeded) { if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Gray_Blue", "Black", false, 1); } 
                        else { SetTile(x, y, '`', "Stone Floor", "A simple stone floor.", "Light_Gray_Blue", "Black", false, 1); } }
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
            int t;
            int x = r1x; int y = r1y;
            int delta_x = r2x - r1x; int delta_y = r2y - r1y;
            int abs_delta_x = Math.Abs(delta_x); int abs_delta_y = Math.Abs(delta_y);
            int sign_x = Math.Sign(delta_x); int sign_y = Math.Sign(delta_y);
            bool hasConnected = false;

            if (abs_delta_x > abs_delta_y)
            {
                t = abs_delta_y * 2 - abs_delta_x;
                do
                {
                    if (t >= 0) { y += sign_y; t -= abs_delta_x * 2; }
                    x += sign_x;
                    t += abs_delta_y * 2;
                    if (World.tiles[x, y].terrainType != 0)
                    {
                        SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                        SetTile(x + 1, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                    }
                    if (x == r2x && y == r2y) { hasConnected = true; }
                }
                while (!hasConnected);
            }
            else
            {
                t = abs_delta_x * 2 - abs_delta_y;
                do
                {
                    if (t >= 0) { x += sign_x; t -= abs_delta_y * 2; }
                    y += sign_y;
                    t += abs_delta_x * 2;
                    if (World.tiles[x, y].terrainType != 0)
                    {
                        SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                        SetTile(x, y + 1, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                    }
                    if (x == r2x && y == r2y) { hasConnected = true; }
                }
                while (!hasConnected);
            }
        }
        public override void CreateBezierCurve(int r0x, int r0y, int r2x, int r2y)
        {
            int r1x; int r1y;

            r1x = World.seed.Next(1, 80);
            r1y = World.seed.Next(1, 70);

            for (float t = 0; t < 1; t += .001f)
            {
                int x = (int)((1 - t) * ((1 - t) * r0x + t * r1x) + t * ((1 - t) * r0x + t * r2x));
                int y = (int)((1 - t) * ((1 - t) * r0y + t * r1y) + t * ((1 - t) * r0y + t * r2y));
                if (CMath.CheckBounds(x, y)) { if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Gray_Blue", "Black", false, 1); } 
                else { SetTile(x, y, '`', "Stone Floor", "A simple stone floor.", "Light_Gray_Blue", "Black", false, 1); } }
            }
        }
        public override void CreateStraightPassage(int r1x, int r1y, int r2x, int r2y)
        {
            if (World.seed.Next(0, 1) == 0)
            {
                for (int x = Math.Min(r1x, r2x); x <= Math.Max(r1x, r2x); x++)
                {
                    SetTile(x, r1y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                }
                for (int y = Math.Min(r1y, r2y); y <= Math.Max(r1y, r2y); y++)
                {
                    SetTile(r2x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                }
            }
            else
            {
                for (int y = Math.Min(r1y, r2y); y <= Math.Max(r1y, r2y); y++)
                {
                    SetTile(r1x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                }
                for (int x = Math.Min(r1x, r2x); x <= Math.Max(r1x, r2x); x++)
                {
                    SetTile(x, r2y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                }
            }
        }
    }
}
