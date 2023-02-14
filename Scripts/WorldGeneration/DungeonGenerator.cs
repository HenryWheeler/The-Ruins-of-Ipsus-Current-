using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Ruins_of_Ipsus.Scripts.JsonDataManagement;
using static SadConsole.Readers.Playscii;

namespace The_Ruins_of_Ipsus
{
    public class DungeonGenerator: AGenerator
    {
        private int roomsToGenerate = 20;
        private int minRoomSize = 5;
        private int maxRoomSize = 12;
        private int debrisCount { get; set; }
        public void CreateMap(int _mapWidth, int _mapHeight, int strength)
        {
            viableTiles.Clear();
            mapWidth = _mapWidth; mapHeight = _mapHeight;
            roomsToGenerate = World.seed.Next(15, 25);
            minRoomSize = World.seed.Next(4, 7);
            maxRoomSize = World.seed.Next(12, 15);
            //debrisCount = World.seed.Next(25, 37);
            World.floorType = "Dungeon";
            World.difficulty = 1;
            debrisCount = 5;

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

            foreach (Traversable coordinate in World.tiles)
            {
                if (coordinate != null && coordinate.terrainType == 1)
                {
                    viableTiles.Add(coordinate.entity.GetComponent<Vector2>());
                }
            }

            CreateConnections(1, 0);
            CreatePatrolLocations();
            CreateDetritus();
            CreateStairs();

            /*
            string name = "Dungeon_1";
            Dictionary<int, string> table = new Dictionary<int, string>
            {
                { 1, "Korbold" }
            };
            SpawnTableManager.CreateNewTable(name, table);
            */

            PopulateFloor();
        }
        public void CreateDetritus()
        {
            int debrisLeft = debrisCount;

            do
            {
                int x = World.seed.Next(0, mapWidth);
                int y = World.seed.Next(0, mapHeight);

                if (World.tiles[x, y].terrainType == 1 && World.tiles[x, y].obstacleLayer == null)
                {
                    string debris = "Debris {Var" + World.seed.Next(1, 4) + "}";
                    EntityManager.CreateEntityFromFile(new Vector2(x, y), debris);

                    /*
                    TurnFunction function = new TurnFunction();
                    Entity entity = new Entity(new List<Component>()
                            {
                                new ID("Actor"),
                                new Vector2(x, y),
                                new Draw("Gray", "Black", 'p'),
                                new Description("Gray*Cave Swine", "This Brown*brown motley pig is accustomed to life deep underground. It is nearly blind and highly territorial."),
                                PronounReferences.pronounSets["Nueter"],
                                new Stats(2, 12, .75f, 8, 0, 1, 0),
                                function,
                                new BeastAI(new List<string>() { "Beast" }, new List<string>() { "Player", "Good Humanoid", "Evil Humanoid", "Plant", "Horror" }, 50, 0, 1, 1, 10),
                                new Movement(new List<int> { 1 }),
                                new Inventory(),
                                new Harmable(),
                                new Faction($"Beast"),
                        });
                    EntityManager.CreateEntity(new Vector2(x, y), entity, false, false);
                    TurnManager.AddActor(function);
                    CMath.ReturnAI(entity).transitions.Clear();

                    Entity weapon = new Entity(new List<Component>()
                    {
                        new ID("Item"),
                        new Vector2(x, y),
                        new Draw("White", "Black", ')'),
                        new Description("Knashing Teeth", "Sharp Teeth."),
                        new AttackFunction(1, 6, 0, 0, "Piercing"),
                        new Equippable("Weapon", false),
                    });
                    InventoryManager.EquipItem(entity, weapon);

                    JsonDataManager.SaveEntity(entity, "Cave Swine");

                    /*
                    JsonDataManager.SaveEntity(weapon, "Rusty Shortsword");

                    Entity weapon2 = new Entity(new List<Component>()
                    {
                        new ID("Item"),
                        new Vector2(x, y),
                        new Draw("Brown", "Black", ')'),
                        new Description("Brown*Rusty Longsword", "A Brown*rusted longsword, its keen edge has been worn away leaving a jagged Brown*rust bitten edge behind."),
                        new AttackFunction(1, 8, -1, 0, "Slashing"),
                        new Equippable("Weapon", true),
                    });

                    JsonDataManager.SaveEntity(weapon2, "Rusty longsword");

                    Entity weapon3 = new Entity(new List<Component>()
                    {
                        new ID("Item"),
                        new Vector2(x, y),
                        new Draw("Brown", "Black", ')'),
                        new Description("Brown*Rusty Mace", "A Brown*rusted mace, its hefty head has rusted away leaving only a stump of metal left."),
                        new AttackFunction(2, 2, -1, 0, "Bludgeoning"),
                        new Equippable("Weapon", true),
                    });

                    JsonDataManager.SaveEntity(weapon3, "Rusty Mace");
                    */

                    debrisLeft--;
                }

            } while (debrisLeft > 0);
        }
        public override void SetAllWalls()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (World.seed.Next(0, 100) < 50) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Steel_Blue", "Black", true, 0); }
                    else { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Steel_Blue", "Black", true, 0); }
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
