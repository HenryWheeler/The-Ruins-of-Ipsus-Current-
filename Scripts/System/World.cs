using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class World
    {
        public World(int width, int height)
        {
            random = new Random();
            mapWidth = width;
            mapHeight = height;
            tiles = new Traversable[width, height];
            sfx = new Draw[width, height];
            depth = -1;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    AGenerator.SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0);
                }
            }
        }
        public World(int width, int height, int _depth, int _random)
        {
            mapWidth = width;
            mapHeight = height;
            tiles = new Traversable[width, height];
            sfx = new Draw[width, height];
            depth = _depth;
            random = new Random();

            //for (int x = 0; x < mapWidth; x++)
            //{
            //    for (int y = 0; y < mapHeight; y++)
            //    {
            //        AGenerator.SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0);
            //    }
            //}

            LoadSavedFloor(_random, testing);
        }
        public static void LoadSeedState(bool random = true, int _seed = 0)
        {
            int _random;
            if (random) { _random = new Random().Next(1, 99999999); }
            else { _random = _seed; }
            seedInt = _random;
            seed = new Random(_random);
        }
        public static void LoadSavedFloor(int seed, bool testing)
        {
            LoadSeedState(false, seed);
            FloorSwitchCase(testing);
        }
        public static void GenerateNewFloor(bool up)
        {
            EntityManager.ClearAllIndexes();
            LoadSeedState();
            EntityManager.AddEntity(Program.player, false);
            TurnManager.AddActor(Program.player.GetComponent<TurnFunction>());
            if (up) { depth++; } else { depth--; }

            FloorSwitchCase(testing);

            List<Entity> tiles = new List<Entity>();
            foreach (Traversable tile in World.tiles) { if (tile != null && tile.entity.GetComponent<Traversable>().terrainType != 0) { tiles.Add(tile.entity); } }
            Vector2 vector2 = tiles[seed.Next(0, tiles.Count - 1)].GetComponent<Vector2>();
            World.tiles[vector2.x, vector2.y].actorLayer = Program.player;
            Program.player.GetComponent<Vector2>().x = vector2.x;
            Program.player.GetComponent<Vector2>().y = vector2.y;
            Renderer.MoveCamera(vector2);
            ShadowcastFOV.Compute(vector2, Program.player.GetComponent<Stats>().sight);

            //PopulateFloor(testing);
        }
        public static void FloorSwitchCase(bool testing)
        {
            if (testing)
            {
                new TestingGenerator().CreateMap(mapWidth, mapHeight, 1);
            }
            else
            {
                switch (depth)
                {
                    case -1: { new CaveGenerator().CreateMap(mapWidth, mapHeight, 1); break; }
                    case 0: { new VerdantCaveGenerator().CreateMap(mapWidth, mapHeight, 1); break; }
                    case 1: { new DungeonGenerator().CreateMap(mapWidth, mapHeight, 1); break; }
                }
            }
        }
        public static void PopulateFloor(bool testing)
        {
            if (testing)
            {

            }
            else
            {
                int goal = seed.Next(5, 10);

                for (int i = 0; i < goal; i++)
                {
                    switch (depth)
                    {
                        case -3: { EntityManager.CreateEntity(new Vector2(0, 0), SpawnTableManager.RetrieveRandomEntity("Shore-1", true), true, true); break; }
                        case -2: { EntityManager.CreateEntity(new Vector2(0, 0), SpawnTableManager.RetrieveRandomEntity("Field-1", true), true, true); break; }
                        case -1: { EntityManager.CreateEntity(new Vector2(0, 0), SpawnTableManager.RetrieveRandomEntity("Cave-1", true), true, true); break; }
                        case 0: { EntityManager.CreateEntity(new Vector2(0, 0), SpawnTableManager.RetrieveRandomEntity("Dungeon-1", true), true, true); break; }
                        case 1: { EntityManager.CreateEntity(new Vector2(0, 0), SpawnTableManager.RetrieveRandomEntity("Shore-1", true), true, true); break; }
                    }
                }
            }
        }
        public static void ClearSFX()
        {
            sfx = new Draw[mapWidth, mapHeight];
        }
        public static Random seed { get; set; }
        public static int seedInt { get; set; }
        public static Random random { get; set; }
        public static Traversable[,] tiles;
        public static Draw[,] sfx;
        public static int mapWidth { get; set; }
        public static int mapHeight { get; set; }
        public static int depth { get; set; }
        public static bool testing = false;
    }
}
