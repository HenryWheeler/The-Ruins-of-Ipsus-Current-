using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Ruins_of_Ipsus.Scripts.Components.AIComponents;
using The_Ruins_of_Ipsus.Scripts.JsonDataManagement;

namespace The_Ruins_of_Ipsus
{
    public class TestingGenerator: AGenerator
    {
        private int wallsNeeded = 4;
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
                        SetTile(x, y, '.', "Stone Floor", "A simple stone floor.","Brown", "Black", false, 1);
                    }
                }
            }

            //for (int i = 0; i < roomsToGenerate; i++)
            //{
            //    int xSP = World.seed.Next(0, mapWidth);
            //    int ySP = World.seed.Next(0, mapHeight);
            //    int rW = World.seed.Next(minRoomSize, maxRoomSize);
            //    int rH = World.seed.Next(minRoomSize, maxRoomSize);
            //    if (!CheckIfHasSpace(xSP, ySP, xSP + rW - 1, ySP + rH - 1)) { i--; continue; }
            //    CreateRoom(xSP, ySP, rW, rH);
            //}

            //CreateConnections(1, 0);
            CreateSurroundingWalls();
            CreateStairs();
            CreatePatrolLocations();

            int current = 0;
            foreach (Traversable tile in World.tiles)
            {
                if (tile.terrainType == 1)
                {
                    /*
                    if (World.seed.Next(0, 10000) > 119995)
                    {
                        TurnFunction function = new TurnFunction();
                        EntityManager.CreateEntity(tile.entity.GetComponent<Vector2>(), new Entity(new List<Component>()
                            {
                                new ID("Actor"),
                                tile.entity.GetComponent<Vector2>(),
                                new Draw("Red", "Black", (char)(current + 48)),
                                new Description("Test Entity", "Testing."),
                                PronounReferences.pronounSets["Nueter"],
                                new Stats(5, 10, .8f, 5, 0, 0),
                                function,
                                //new GuardAI(new List<string>() { "Beast" }, new List<string>(), 50),
                                new PatrolFunction(),
                                new Movement(new List<int> { 1, 2 }),
                                new Inventory(),
                                new Faction($"Beast"),
                        }), false, false);
                        TurnManager.AddActor(function);
                        current++;
                        if (current == 9)
                        {
                            current = 0;
                        }
                    } else if (World.seed.Next(0, 15000) > 114980)
                    {
                        TurnFunction function = new TurnFunction();
                        EntityManager.CreateEntity(tile.entity.GetComponent<Vector2>(), new Entity(new List<Component>()
                            {
                                new ID("Actor"),
                                tile.entity.GetComponent<Vector2>(),
                                new Draw("Red", "Black", 'D'),
                                new Description("Red*Red Red*Dragon", "A terrifying sight, the hulking form of one of the ancient tyrant drakes stands before you. Smoke billowing out of every nostril the ancient Red*Red Red*Dragon is a fearsome foe indeed."),
                                PronounReferences.pronounSets["Nueter"],
                                new Stats(10, 18, .8f, 50, 4, 4),
                                function,
                                new DragonAI(new List<string>() { "Beast" }, new List<string>() { "Player" }, 70, 0, 0, 8, 10),
                                new PatrolFunction(),
                                new Movement(new List<int> { 1, 2 }),
                                new BreathWeaponOnUse(3, "Fire", 12, false),
                                new Usable(),
                                new Harmable(),
                                new Inventory(),
                                new Faction($"Beast"),
                        }), false, false);
                        TurnManager.AddActor(function);
                    } else if (World.seed.Next(0, 15000) > 114980)
                    {
                        TurnFunction function = new TurnFunction();
                        EntityManager.CreateEntity(tile.entity.GetComponent<Vector2>(), new Entity(new List<Component>()
                            {
                                new ID("Actor"),
                                tile.entity.GetComponent<Vector2>(),
                                new Draw("Green", "Black", 'T'),
                                new Description("The Whispering Green*Pine", "A giant unnatural Green*pine Green*tree. It almost sounds as if it is talking to you."),
                                PronounReferences.pronounSets["Nueter"],
                                new Stats(5, 14, .25f, 50, 3, 2, null, new List<string>() { "Fire" }),
                                function,
                                new PlantAI(new List<string>() { "Plant" }, new List<string>() { "Player", "Beast" }, 100, 0, 0, 1, 100),
                                new PatrolFunction(),
                                new Movement(new List<int> { 1 }),
                                new Harmable(),
                                new Inventory(),
                                new Faction($"Plant"),
                        }), false, false);
                        TurnManager.AddActor(function);
                    }
                    else if (World.seed.Next(0, 15000) > 14995)
                    {
                        TurnFunction function = new TurnFunction();
                        EntityManager.CreateEntity(tile.entity.GetComponent<Vector2>(), new Entity(new List<Component>()
                            {
                                new ID("Actor"),
                                tile.entity.GetComponent<Vector2>(),
                                new Draw("Lime", "Black", 's'),
                                new Description("Splitting Lime*Slime", "An oozing pulsating mass ready to burst into parts at any time."),
                                PronounReferences.pronounSets["Nueter"],
                                new Stats(4, 8, .75f, 30, 2, -5, null, new List<string>() { "Fire" }),
                                function,
                                new OozelingAI(new List<string>() { "Oozeling" }, new List<string>() { "Player" }, 50, 0, 0, 1, 10),
                                new Splitting(1, 1),
                                new Movement(new List<int> { 1 }),
                                new Harmable(),
                                new Inventory(),
                                new Faction($"Oozeling"),
                        }), false, false);
                        TurnManager.AddActor(function);
                    }
                    */
                }
            }
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
            List<Vector2> doorSpots = new List<Vector2>();
            if (World.seed.Next(0, 1) == 0)
            {
                for (int x = Math.Min(r1x, r2x); x <= Math.Max(r1x, r2x); x++)
                {
                    SetTile(x, r1y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                    if (World.tiles[x, r1y + 1].terrainType == 0 && World.tiles[x, r1y - 1].terrainType == 0)
                    {
                        doorSpots.Add(new Vector2(x, r1y));
                    }
                }
                for (int y = Math.Min(r1y, r2y); y <= Math.Max(r1y, r2y); y++)
                {
                    SetTile(r2x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                    if (World.tiles[r2x + 1, y].terrainType == 0 && World.tiles[r2x - 1, y].terrainType == 0)
                    {
                        doorSpots.Add(new Vector2(r2x, y));
                    }
                }
            }
            else
            {
                for (int y = Math.Min(r1y, r2y); y <= Math.Max(r1y, r2y); y++)
                {
                    SetTile(r1x, y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                    if (World.tiles[r1x + 1, y].terrainType == 0 && World.tiles[r1x - 1, y].terrainType == 0)
                    {
                        doorSpots.Add(new Vector2(r1x, y));
                    }
                }
                for (int x = Math.Min(r1x, r2x); x <= Math.Max(r1x, r2x); x++)
                {
                    SetTile(x, r2y, '.', "Stone Floor", "A simple stone floor.", "Brown", "Black", false, 1);
                    if (World.tiles[x, r2y + 1].terrainType == 0 && World.tiles[x, r2y - 1].terrainType == 0)
                    {
                        doorSpots.Add(new Vector2(x, r2y));
                    }
                }
            }
            if (doorSpots.Count != 0)
            {
                EntityManager.CreateEntity(doorSpots[0], JsonDataManager.ReturnEntity("Door"), false, false);
                EntityManager.CreateEntity(doorSpots[doorSpots.Count - 1], JsonDataManager.ReturnEntity("Door"), false, false);
            }
        }
    }
}
