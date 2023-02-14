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
            depth = 0;
            difficulty = 0;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    AGenerator.SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0);
                }
            }
        }
        public World(int width, int height, int _depth, int _random, int _difficulty)
        {
            mapWidth = width;
            mapHeight = height;
            tiles = new Traversable[width, height];
            sfx = new Draw[width, height];
            depth = _depth;
            difficulty = _difficulty;
            random = new Random();

            //for (int x = 0; x < mapWidth; x++)
            //{
            //    for (int y = 0; y < mapHeight; y++)
            //    {
            //        AGenerator.SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "Light_Gray_Blue", "Black", true, 0);
            //    }
            //}

            LoadSavedFloor(_random);
        }
        public static void LoadSeedState(bool random = true, int _seed = 0)
        {
            int _random;
            if (random) { _random = new Random().Next(1, 99999999); }
            else { _random = _seed; }
            seedInt = _random;
            seed = new Random(_random);
        }
        public static void LoadSavedFloor(int seed)
        {
            LoadSeedState(false, seed);
            FloorSwitchCase();
        }
        public static void GenerateNewFloor(bool up)
        {
            EntityManager.ClearAllIndexes();
            LoadSeedState();
            EntityManager.AddEntity(Program.player, false);
            TurnManager.AddActor(Program.player.GetComponent<TurnFunction>());
            if (up) { depth++; } else { depth--; }
            difficulty++;

            FloorSwitchCase();
            EntityVerificationCheck();

            List<Entity> tiles = new List<Entity>();
            foreach (Traversable tile in World.tiles) { if (tile != null && tile.entity.GetComponent<Traversable>().terrainType != 0) { tiles.Add(tile.entity); } }
            Vector2 vector2 = tiles[seed.Next(0, tiles.Count - 1)].GetComponent<Vector2>();
            World.tiles[vector2.x, vector2.y].actorLayer = Program.player;
            Program.player.GetComponent<Vector2>().x = vector2.x;
            Program.player.GetComponent<Vector2>().y = vector2.y;
            Renderer.MoveCamera(vector2);
            ShadowcastFOV.Compute(vector2, Program.player.GetComponent<Stats>().sight);
            Renderer.DrawMapToScreen();
        }
        public static void FloorSwitchCase()
        {
            switch (depth)
            {
                case 0: { new VerdantCaveGenerator().CreateMap(mapWidth, mapHeight, 1); break; }
                case 1: { new DungeonGenerator().CreateMap(mapWidth, mapHeight, 1); break; }
                case 2: { new VerdantCaveGenerator().CreateMap(mapWidth, mapHeight, 1); break; }
            }
        }
        public static void EntityVerificationCheck()
        {
            foreach (Traversable traversable in tiles)
            {
                if (traversable != null)
                {
                    Vector2 start = traversable.entity.GetComponent<Vector2>();
                    if (traversable.actorLayer != null) 
                    {
                        Vector2 test = traversable.actorLayer.GetComponent<Vector2>();
                        if (tiles[start.x, start.y] != tiles[test.x, test.y])
                        {
                            traversable.actorLayer = null;
                        }
                    }
                    if (traversable.itemLayer != null)
                    {
                        Vector2 test = traversable.itemLayer.GetComponent<Vector2>();
                        if (tiles[start.x, start.y] != tiles[test.x, test.y])
                        {
                            traversable.itemLayer = null;
                        }
                    }
                    if (traversable.obstacleLayer != null)
                    {
                        Vector2 test = traversable.obstacleLayer.GetComponent<Vector2>();
                        if (tiles[start.x, start.y] != tiles[test.x, test.y])
                        {
                            traversable.obstacleLayer = null;
                        }
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
        public static int difficulty { get; set; }
        public static string floorType { get; set; }
        public static bool developerMode { get; set; } = true;
    }
}
