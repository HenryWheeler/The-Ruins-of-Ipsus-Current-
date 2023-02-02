using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class DungeonGenerator: AGenerator
    {
        private int roomsToGenerate = 20;
        private int minRoomSize = 5;
        private int maxRoomSize = 12;
        public void CreateMap(int _mapWidth, int _mapHeight, int strength)
        {
            mapWidth = _mapWidth; mapHeight = _mapHeight;
            roomsToGenerate = World.seed.Next(15, 25);
            minRoomSize = World.seed.Next(4, 7);
            maxRoomSize = World.seed.Next(12, 15);

            SetAllWalls();

            for (int i = 0; i < roomsToGenerate; i++)
            {
                int xSP = World.seed.Next(0, mapWidth);
                int ySP = World.seed.Next(0, mapHeight);
                int rW = World.seed.Next(minRoomSize, maxRoomSize);
                int rH = World.seed.Next(minRoomSize, maxRoomSize);

                if (!CheckIfHasSpace(xSP, ySP, xSP + rW - 1, ySP + rH - 1)) { i--; continue; }
                CreateRoom(xSP, ySP, rW, rH);
            }
            CreateConnections(3, 0);

            string table = "Dungeon-" + strength.ToString();
            //FillChunk(table, World.seed.Next(4, 10));
            CreateStairs();
        }
        public override void SetAllWalls()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0);
                }
            }
        }
        public void CreateRoom(int _x, int _y, int roomWidth, int roomHeight)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                for (int x = 0; x < roomWidth; x++)
                {
                    int _X = _x + x;
                    int _Y = _y + y;

                    SetTile(_X, _Y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                }
            }
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
                    if (World.tiles[x, y].terrainType == 0)
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
                    if (World.tiles[x, y].terrainType == 0)
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
                if (CMath.CheckBounds(x, y)) { if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1); } 
                else { SetTile(x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1); } }
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
