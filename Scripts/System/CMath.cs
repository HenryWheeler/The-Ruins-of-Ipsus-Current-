using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using SadConsole;

namespace The_Ruins_of_Ipsus
{
    public class CMath
    {
        public static double Distance(int oX, int oY, int eX, int eY) { return Math.Sqrt(Math.Pow(eX - oX, 2) + Math.Pow(eY - oY, 2)); }
        public static int Distance(Vector2 one, Vector2 two)
        {
            Vector2 origin = one; Vector2 destination = two;
            int returnValue = (int)Math.Sqrt(Math.Pow(destination.x - origin.x, 2) + Math.Pow(destination.y - origin.y, 2));
            if (returnValue == 0) { return 1; }
            else { return returnValue; }
        }
        public static bool PathBlocked(Vector2 coordinate, Vector2 coordinate2, int range)
        {
            bool passing = true;
            int oX = coordinate.x; int oY = coordinate.y;
            int eX = coordinate2.x; int eY = coordinate2.y;
            int t;
            int x = oX; int y = oY;
            int delta_x = eX - oX; int delta_y = eY - oY;
            int abs_delta_x = Math.Abs(delta_x); int abs_delta_y = Math.Abs(delta_y);
            int sign_x = Math.Sign(delta_x); int sign_y = Math.Sign(delta_y);

            if (oX == eX && oY == eY) return true;
            if (Math.Abs(delta_x) > range || Math.Abs(delta_y) > range) return false;

            if (abs_delta_x > abs_delta_y)
            {
                t = abs_delta_y * 2 - abs_delta_x;
                Traversable traversable;
                do
                {
                    if (t >= 0) { y += sign_y; t -= abs_delta_x * 2; }
                    x += sign_x;
                    t += abs_delta_y * 2;
                    if (x == eX && y == eY) { return true; }
                    traversable = World.tiles[x, y];

                    if (!TargetReticle.throwing && !traversable.entity.GetComponent<Visibility>().explored && traversable.terrainType != 0)
                    {
                        passing = false;
                    }
                    else if (traversable.terrainType != 0 && traversable.obstacleLayer != null && traversable.obstacleLayer.GetComponent<Visibility>() != null && traversable.obstacleLayer.GetComponent<Visibility>().opaque)
                    {
                        passing = false;
                    }
                    else if (traversable.entity.GetComponent<Visibility>().opaque)
                    {
                        passing = false;
                    }
                }
                while (passing);
                return false;
            }
            else
            {
                t = abs_delta_x * 2 - abs_delta_y;
                Traversable traversable;
                do
                {
                    if (t >= 0) { x += sign_x; t -= abs_delta_y * 2; }
                    y += sign_y;
                    t += abs_delta_x * 2;
                    if (x == eX && y == eY) { return true; }
                    traversable = World.tiles[x, y];

                    if (!TargetReticle.throwing && !traversable.entity.GetComponent<Visibility>().explored && traversable.terrainType != 0)
                    {
                        passing = false;
                    }
                    else if (traversable.terrainType != 0 && traversable.obstacleLayer != null && traversable.obstacleLayer.GetComponent<Visibility>() != null && traversable.obstacleLayer.GetComponent<Visibility>().opaque)
                    {
                        passing = false;
                    }
                    else if (traversable.entity.GetComponent<Visibility>().opaque)
                    {
                        passing = false;
                    }
                }
                while (passing);
                return false;
            }
        }
        public static Vector2 ReturnNearestValidCoordinate(string type, Vector2 startingCoordinate)
        {
            Vector2 finalLanding = new Vector2(startingCoordinate.x, startingCoordinate.y);
            int moveCount = 0;

            switch (type)
            {
                case "Obstacle":
                    {
                        if (World.tiles[startingCoordinate.x, startingCoordinate.y].obstacleLayer == null)
                        {
                            return startingCoordinate;
                        }
                        do
                        {
                            Vector2 start = finalLanding;
                            for (int y = start.y - 1; y <= start.y + 1; y++)
                            {
                                for (int x = start.x - 1; x <= start.x + 1; x++)
                                {
                                    Traversable traversable = World.tiles[x, y];
                                    if (CheckBounds(x, y) && traversable.terrainType != 0 && traversable.obstacleLayer == null)
                                    {
                                        return new Vector2(x, y);
                                    }
                                    else
                                    {
                                        finalLanding = new Vector2(x, y);
                                        continue;
                                    }
                                }
                            }
                            moveCount++;
                            if (moveCount >= 25)
                            {
                                break;
                            }
                        } while (World.tiles[finalLanding.x, finalLanding.y].obstacleLayer != null);
                        return startingCoordinate;
                    }
                case "Item":
                    {
                        if (World.tiles[startingCoordinate.x, startingCoordinate.y].itemLayer == null)
                        {
                            return startingCoordinate;
                        }
                        do
                        {
                            Vector2 start = finalLanding;
                            for (int y = start.y - 1; y <= start.y + 1; y++)
                            {
                                for (int x = start.x - 1; x <= start.x + 1; x++)
                                {
                                    Traversable traversable = World.tiles[x, y];
                                    if (CheckBounds(x, y) && traversable.terrainType != 0 && traversable.itemLayer == null)
                                    {
                                        return new Vector2(x, y);
                                    }
                                    else
                                    {
                                        finalLanding = new Vector2(x, y);
                                        continue;
                                    }
                                }
                            }
                            moveCount++;
                            if (moveCount >= 25)
                            {
                                break;
                            }
                        } while (World.tiles[finalLanding.x, finalLanding.y].itemLayer != null);
                        return startingCoordinate;
                    }
                case "Actor":
                    {
                        if (World.tiles[startingCoordinate.x, startingCoordinate.y].actorLayer == null)
                        {
                            return startingCoordinate;
                        }
                        do
                        {
                            Vector2 start = finalLanding;
                            for (int y = start.y - 1; y <= start.y + 1; y++)
                            {
                                for (int x = start.x - 1; x <= start.x + 1; x++)
                                {
                                    Traversable traversable = World.tiles[x, y];
                                    if (CheckBounds(x, y) && traversable.terrainType != 0 && traversable.actorLayer == null)
                                    {
                                        return new Vector2(x, y);
                                    }
                                    else
                                    {
                                        finalLanding = new Vector2(x, y);
                                        continue;
                                    }
                                }
                            }
                            moveCount++;
                            if (moveCount >= 25)
                            {
                                break;
                            }
                        } while (World.tiles[finalLanding.x, finalLanding.y].actorLayer != null);
                        return startingCoordinate;
                    }
            }
            return startingCoordinate;
        }
        public static AI ReturnAI(Entity entityRef)
        {
            foreach (Component component in entityRef.components)
            {
                if (component.GetType().BaseType.Equals(typeof(AI))) { return (AI)component; }
            }
            return null;
        }
        public static int DisplayToConsole(TitleConsole console, string logOut, int a, int b, int m = 0, int y = 2, bool clear = true)
        {
            if (clear)
            {
                console.Clear();
            }
            string[] outPut = logOut.Split(' ');
            int c = a;
            foreach (string text in outPut)
            {
                string[] split = text.Split('*');
                if (split.Count() == 1)
                {
                    if (split[0].Contains("+")) { y += 2 + m; c = a; }
                    else
                    {
                        if (c + split[0].Length > console.Width - 4) { y += 2 + m; c = a; }
                        console.Print(c + b, y, split[0], Color.White);
                        c += split[0].Length + 1;
                    }
                }
                else
                {
                    if (split[1].Contains("+")) { y += 2 + m; c = a; }
                    else
                    {
                        if (c + split[0].Length > console.Width - 4) { y += 2 + m; c = a; }
                        console.Print(c + b, y, split[1], ColorFinder.ColorPicker(split[0]));
                        c += split[1].Length + 1;
                    }
                }
            }

            return y;
        }
        public static bool CheckBounds(int x, int y)
        {
            if (x <= 0 || x >= Program.gameMapWidth || y <= 0 || y >= Program.gameMapHeight) return false;
            else return true;
        }
    }
}
