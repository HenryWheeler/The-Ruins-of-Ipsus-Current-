using System.Collections.Concurrent;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    public class DijkstraMaps
    {
        public DijkstraMaps(int width, int height)
        {
            gameMapWidth = width;
            gameMapHeight = height;

            baseIntArray = new int[gameMapWidth, gameMapWidth];

            for (int x = 0; x < gameMapWidth; x++)
            {
                for (int y = 0; y < gameMapHeight; y++)
                {
                    baseIntArray[x, y] = 1000;
                }
            }
        }
        private static int gameMapWidth { get; set; }
        private static int gameMapHeight { get; set; }
        private static int[,] baseIntArray { get; set; }
        public static Dictionary<string, int[,]> maps = new Dictionary<string, int[,]>();
        public static void CreateMap(Entity coordinate, string name)
        {
            List<Entity> entity = new List<Entity>
            {
                coordinate
            }; CreateMap(entity, name, 500);
        }
        public static void CreateMap(Vector2 coordinate, string name)
        {
            List<Entity> entity = new List<Entity>
            {
                new Entity(new List<Component>() { coordinate })
            }; CreateMap(entity, name, 500);
        }
        public static void CreateMap(List<Entity> coordinates, string name, int strength = 25)
        {
            ConcurrentQueue<Vector2> checkList = new ConcurrentQueue<Vector2>();
            HashSet<Vector2> tempList = new HashSet<Vector2>();
            int[,] intArray = (int[,])baseIntArray.Clone();
            for (int o = 0; o < coordinates.Count; o++)
            {
                Vector2 vector3 = coordinates[o].GetComponent<Vector2>();
                intArray[vector3.x, vector3.y] = 0;
                checkList.Enqueue(vector3);
                tempList.Add(vector3);
            }

            //var watch1 = Stopwatch.StartNew();
            for (int o = 0; o < strength; o++)
            {
                for (int i = 0; i < checkList.Count; i++)
                {
                    checkList.TryDequeue(out Vector2 vector2);
                    CheckNeighbors(intArray, tempList, checkList, vector2.x, vector2.y);
                }
                //Log.Add($"{checkList.Count} queued for {name}");
                tempList.Clear();
            }
            //Log.Add($"{name}: {watch1.ElapsedTicks} ticks && {watch1.ElapsedMilliseconds} miliseconds");
            //StatManager.Average(watch1.ElapsedTicks);

            AddMap(intArray, name);
        }
        private static void CheckNeighbors(int[,] intArray, HashSet<Vector2> tempList, ConcurrentQueue<Vector2> checkList, int x, int y)
        {
            int current = intArray[x, y];
            for (int y2 = y - 1; y2 <= y + 1; y2++)
            {
                if (CheckBoundsAndWalls(x, y2) && intArray[x, y2] > current)
                {
                    Vector2 vector21 = new Vector2(x, y2);
                    if (!tempList.Contains(vector21))
                    {
                        intArray[x, y2] = current + 1;
                        tempList.Add(vector21);
                        checkList.Enqueue(vector21);
                    }
                }
                else { continue; }
            }
            for (int x2 = x - 1; x2 <= x + 1; x2++)
            {
                if (CheckBoundsAndWalls(x2, y) && intArray[x2, y] > current)
                {
                    Vector2 vector21 = new Vector2(x2, y);
                    if (!tempList.Contains(vector21))
                    {
                        intArray[x2, y] = current + 1;
                        tempList.Add(vector21);
                        checkList.Enqueue(vector21);
                    }
                }
                else { continue; }
            }
        }
        private static bool CheckBoundsAndWalls(int x, int y)
        {
            return x >= 0 && x <= Program.gameMapWidth && y >= 0 && y <= Program.gameMapHeight && World.tiles[x, y].terrainType != 0;
        }
        private static void AddMap(int[,] map, string name)
        {
            if (maps.ContainsKey(name))
            {
                maps[name] = map;
            }
            else
            {
                maps.Add(name, map);
            }
        }
        public static void DiscardAll()
        {
            maps.Clear();
        }
        public static Vector2 PathFromMap(Entity entity, string mapName)
        {
            Vector2 start = entity.GetComponent<Vector2>();
            int[,] map;
            if (maps.ContainsKey(mapName))
            {
                map = (int[,])maps[mapName].Clone();
            }
            else { return null; }

            Vector2 target = start;
            float v = map[start.x, start.y];

            for (int y = start.y - 1; y <= start.y + 1; y++)
            {
                for (int x = start.x - 1; x <= start.x + 1; x++)
                {
                    if (CheckBoundsAndWalls(x, y))
                    {
                        if (map[x, y] == 0)
                        {
                            target = new Vector2(x, y);
                            return target;
                        }
                        else
                        {
                            if (entity.GetComponent<Movement>().moveTypes.Contains(World.tiles[x, y].terrainType) && map[x, y] < v)
                            {
                                target = new Vector2(x, y);
                                v = map[x, y];
                            }
                            else { continue; }
                        }
                    }
                }
            }
            return target;
        }
        public static Vector2 PathFromMap(Vector2 start, string mapName)
        {
            int[,] map;
            if (maps.ContainsKey(mapName))
            {
                map = (int[,])maps[mapName].Clone();
            }
            else { return null; }

            Vector2 target = new Vector2(start.x, start.y);
            float v = map[start.x, start.y];

            for (int y = start.y - 1; y <= start.y + 1; y++)
            {
                for (int x = start.x - 1; x <= start.x + 1; x++)
                {
                    if (CheckBoundsAndWalls(x, y))
                    {
                        if (map[x, y] == 0)
                        {
                            target = new Vector2(x, y);
                            return target;
                        }
                        else
                        {
                            if (map[x, y] < v)
                            {
                                target = new Vector2(x, y);
                                v = map[x, y];
                            }
                            else { continue; }
                        }
                    }
                }
            }
            return target;
        }
    }
}
