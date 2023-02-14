using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    public abstract class AGenerator
    {
        public List<Vector2> viableTiles = new List<Vector2>();
        public int mapWidth;
        public int mapHeight;
        public int startX;
        public int startY;
        public abstract void CreateDiagonalPassage(int r1x, int r1y, int r2x, int r2y);
        public abstract void CreateBezierCurve(int r0x, int r0y, int r2x, int r2y);
        public abstract void CreateStraightPassage(int r1x, int r1y, int r2x, int r2y);
        public abstract void SetAllWalls();
        public void PopulateFloor()
        {
            int actorsToSpawn = World.seed.Next(8, 15) + World.difficulty;
            int itemsToSpawn = World.seed.Next(6, 10);
            int obstaclesToSpawn = World.seed.Next(10, 20);

            for (int i = 0; i < actorsToSpawn; i++)
            {
                Vector2 targetLocation = null;

                do
                {
                    Vector2 testLocation = new Vector2(World.seed.Next(1, World.mapWidth), World.seed.Next(1, World.mapHeight));

                    if (viableTiles.Contains(testLocation) && World.tiles[testLocation.x, testLocation.y].actorLayer == null)
                    {
                        targetLocation = testLocation;
                    }

                } while (targetLocation == null);

                EntityManager.CreateEntityFromFile(targetLocation,
                    SpawnTableManager.RetrieveRandomEntity($"{World.floorType}_{World.difficulty}", 1, true), true);
            }
            for (int i = 0; i < itemsToSpawn; i++)
            {
                Vector2 targetLocation = null;

                do
                {
                    Vector2 testLocation = new Vector2(World.seed.Next(1, World.mapWidth), World.seed.Next(1, World.mapHeight));

                    if (viableTiles.Contains(testLocation) && World.tiles[testLocation.x, testLocation.y].itemLayer == null)
                    {
                        targetLocation = testLocation;
                    }

                } while (targetLocation == null);

                EntityManager.CreateEntityFromFile(targetLocation,
                    SpawnTableManager.RetrieveRandomEntity($"Item_Table", World.difficulty, true), true);
            }
            for (int i = 0; i < obstaclesToSpawn; i++)
            {
                Vector2 targetLocation = null;

                do
                {
                    Vector2 testLocation = new Vector2(World.seed.Next(1, World.mapWidth), World.seed.Next(1, World.mapHeight));

                    if (viableTiles.Contains(testLocation) && World.tiles[testLocation.x, testLocation.y].obstacleLayer == null)
                    {
                        targetLocation = testLocation;
                    }

                } while (targetLocation == null);

                EntityManager.CreateEntityFromFile(targetLocation,
                    SpawnTableManager.RetrieveRandomEntity($"Obstacle_Table", World.difficulty, true), true);
            }
        }
        public void CreatePatrolLocations()
        {
            List<Entity> tiles = new List<Entity>();
            foreach (Traversable tile in World.tiles)
            {
                if (tile != null && tile.terrainType == 1)
                {
                    tiles.Add(tile.entity);
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Entity tile = tiles[World.seed.Next(0, tiles.Count - 1)];
                tiles.Remove(tile);
                DijkstraMaps.CreateMap(tile, $"Patrol{i}");
            }
        }
        public void CreateSurroundingWalls()
        {
            int h = mapHeight - 1;
            int w = mapWidth - 1;
            for (int y = mapHeight; y >= 0; y--)
            {
                for (int x = mapWidth; x >= 0; x--)
                {
                    if (CMath.CheckBounds(x, y))
                    {
                        if (y == h && x == 1) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0); }
                        else if (y == h && x == w) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0); }
                        else if (y == 1 && x == 1) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0); }
                        else if (x == w && y == 1) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0); }
                        else if (y == 1 || y == h) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0); }
                        else if (x == 1 || x == w) { SetTile(x, y, '#', "Stone Wall", "A cold stone wall.", "White", "Gray", true, 0); }
                    }
                }
            }
        }
        public void CreateConnections(int loopCount, int type)
        {
            bool doneConnecting = false;
            int maxVisitations = 100;
            for (int v = 0; v < loopCount; v++)
            {
                int currentVisitation = 0;
                while (!doneConnecting)
                {
                    currentVisitation++;

                    if (currentVisitation >= maxVisitations)
                    {
                        break;
                    }

                    Vector2 firstCoordinate = null;
                    Vector2 lastCoordinate = null;
                    List<Entity> cavernTiles = new List<Entity>();

                    foreach (Traversable tile in World.tiles)
                    {
                        if (tile != null && tile.terrainType != 0)
                        {
                            firstCoordinate = tile.entity.GetComponent<Vector2>();
                            break;
                        }
                    }

                    DijkstraMaps.CreateMap(World.tiles[firstCoordinate.x, firstCoordinate.y].entity, "CurrentRoom");
                    int[,] map = DijkstraMaps.maps["CurrentRoom"];
                    foreach (Traversable tile in World.tiles)
                    {
                        if (tile != null)
                        {
                            Vector2 vector2 = tile.entity.GetComponent<Vector2>();
                            if (map[vector2.x, vector2.y] < 1000)
                            {
                                cavernTiles.Add(tile.entity);
                            }
                        }
                    }
                    foreach (Traversable tile in World.tiles)
                    {
                        if (tile != null && tile.terrainType != 0 && !cavernTiles.Contains(tile.entity))
                        {
                            Vector2 coordinate = tile.entity.GetComponent<Vector2>();
                            if (lastCoordinate == null)
                            {
                                lastCoordinate = tile.entity.GetComponent<Vector2>();
                            }
                            else
                            {
                                for (int y = coordinate.y - 1; y <= coordinate.y + 1; y++)
                                {
                                    for (int x = coordinate.x - 1; x <= coordinate.x + 1; x++)
                                    {
                                        if (CMath.CheckBounds(x, y))
                                        {
                                            if (World.tiles[x, y].terrainType != 0)
                                            {
                                                if (CMath.Distance(new Vector2(x, y), firstCoordinate) < CMath.Distance(lastCoordinate, firstCoordinate))
                                                {
                                                    lastCoordinate = new Vector2(x, y);
                                                }
                                            }
                                            else { continue; }
                                        }
                                        else continue;
                                    }
                                }
                            }
                        }
                    }
                    foreach (Entity tile in cavernTiles)
                    {
                        if (tile != null && tile.GetComponent<Traversable>().terrainType != 0)
                        {
                            Vector2 coordinate = tile.GetComponent<Vector2>();
                            if (lastCoordinate != null)
                            {
                                for (int y = coordinate.y - 1; y <= coordinate.y + 1; y++)
                                {
                                    for (int x = coordinate.x - 1; x <= coordinate.x + 1; x++)
                                    {
                                        if (CMath.CheckBounds(x, y))
                                        {
                                            if (World.tiles[x, y].terrainType != 0)
                                            {
                                                if (CMath.Distance(new Vector2(x, y), lastCoordinate) < CMath.Distance(lastCoordinate, firstCoordinate))
                                                {
                                                    firstCoordinate = new Vector2(x, y);
                                                }
                                            }
                                            else { continue; }
                                        }
                                        else continue;
                                    }
                                }
                            }
                        }
                    }
                    if (lastCoordinate == null) { doneConnecting = true; }
                    else
                    {
                        switch (type)
                        {
                            case 0: CreateStraightPassage(firstCoordinate.x, firstCoordinate.y, lastCoordinate.x, lastCoordinate.y); break;
                            case 1: CreateDiagonalPassage(firstCoordinate.x, firstCoordinate.y, lastCoordinate.x, lastCoordinate.y); break;
                            case 2: CreateBezierCurve(firstCoordinate.x, firstCoordinate.y, lastCoordinate.x, lastCoordinate.y); break;
                        }
                    }
                }
            }
        }
        public void CreateStairs()
        {
            List<Entity> tiles = new List<Entity>();
            foreach (Traversable tile in World.tiles) { if (tile != null && tile.terrainType == 1 && tile.obstacleLayer == null) { tiles.Add(tile.entity); } }
            Entity upStair = tiles[World.seed.Next(0, tiles.Count - 1)];
            tiles.Remove(upStair);
            Entity downStair = tiles[World.seed.Next(0, tiles.Count - 1)];
            Vector2 vector2 = upStair.GetComponent<Vector2>();
            SetTile(vector2.x, vector2.y, '<', "Stairs Up", "A winding staircase upward.", "White", "Black", false, 1);
            vector2 = downStair.GetComponent<Vector2>();
            SetTile(vector2.x, vector2.y, '>', "Stairs Down", "A winding staircase downward.", "White", "Black", false, 1);
        }
        public void CreateDoors()
        {
            foreach (Traversable tile in World.tiles)
            {
                if (tile.terrainType != 0)
                {

                }
            }
        }
        public bool CheckIfHasSpace(int sX, int sY, int eX, int eY)
        {
            for (int y = sY - 1; y <= eY + 1; y++)
            {
                for (int x = sX - 1; x <= eX + 1; x++)
                {
                    if (x < 2 || y <= 2 || x >= mapWidth - 1 || y >= mapHeight - 1) return false;
                    if (World.tiles[x, y].terrainType != 0) return false;
                }
            }
            return true;
        }
        public static void SetTile(int x, int y, char character, string name, string description, string fColor, string bColor, bool opaque, int moveType)
        {
            Traversable traversable = new Traversable(moveType);
            new Entity(new List<Component>() {
                new Vector2(x, y),
                new Draw(fColor, bColor, character),
                new Description(name, description),
                new Visibility(opaque, false, false),
                traversable});
            World.tiles[x, y] = traversable;
        }
    }
}
