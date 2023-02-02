using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class TargetReticle
    {
        public static Entity targetWeapon { get; set; }
        public static Vector2 position { get; set; } 
        public static bool targeting = false;
        public static bool throwing = false;
        public static bool inInventory { get; set; }
        private static Entity player;
        public TargetReticle(Entity _player) { player = _player; }
        public static void StartTargeting(bool _inInventory, bool _throwing)
        {
            inInventory = _inInventory;
            if (inInventory) 
            {
                InventoryManager.CloseInventory(); 
            }

            targeting = true; 
            player.GetComponent<TurnFunction>().turnActive = false;
            if (_throwing) 
            { 
                throwing = true;
            }
            else 
            {
                throwing = false;
            }

            Vector2 vector3 = player.GetComponent<Vector2>();
            position = new Vector2(vector3.x, vector3.y);
            Move(0, 0);
        }
        public static void StopTargeting()
        {
            targeting = false;
            if (!inInventory) 
            {
                player.GetComponent<TurnFunction>().turnActive = true;
                Log.DisplayLog();
            }
            else 
            {
                InventoryManager.OpenInventory(); 
            }

            Renderer.MoveCamera(player.GetComponent<Vector2>());
            Renderer.DrawMapToScreen();
        }
        public static void Throw(Entity player)
        {
            if (throwing)
            {
                ThrowWeapon(targetWeapon);
            }
            else
            {
                Vector2 vector2 = ReturnCoords(true);
                if (vector2 != null)
                {
                    foreach (OnUse component in targetWeapon.GetComponent<Usable>().onUseComponents)
                    {
                        if (component != null)
                        {
                            component.Use(player, vector2);
                        }
                    }
                }
            }
        }
        public static void Move(int _x, int _y)
        {
            if (CMath.CheckBounds(position.x + _x, position.y + _y))
            {
                World.ClearSFX();
                position.x += _x; position.y += _y;

                if (throwing)
                {
                    bool check = CreateLine(player.GetComponent<Stats>().strength + 10, true);
                    if (targetWeapon.GetComponent<Throwable>() != null)
                    {
                        List<OnThrow> properties = targetWeapon.GetComponent<Throwable>().onThrowComponents;

                        if (properties.Count != 0)
                        {
                            Vector2 vector2 = player.GetComponent<Vector2>();

                            foreach (OnThrow property in properties)
                            {
                                foreach (Vector2 coordinate in RangeModels.FindModel(vector2, new Vector2(position.x, position.y), property.strength, player.GetComponent<Stats>().strength + 10, true, property.rangeModel))
                                {
                                    if (CMath.Distance(position.x, position.y, vector2.x, vector2.y) < player.GetComponent<Stats>().strength + 10 && World.tiles[position.x, position.y].terrainType != 0 && check)
                                    {
                                        if (World.sfx[coordinate.x, coordinate.y] != null && World.sfx[coordinate.x, coordinate.y].fColor == "Gray")
                                        {
                                            if (coordinate.x == position.x && coordinate.y == position.y)
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", 'X');
                                            }
                                            else
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", '*');
                                            }
                                        }
                                        else
                                        {
                                            if (coordinate.x == position.x && coordinate.y == position.y)
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Yellow", "Black", 'X');
                                            }
                                            else
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Yellow", "Black", '*');
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (coordinate.x == position.x && coordinate.y == position.y)
                                        {
                                            World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", 'X');
                                        }
                                        else
                                        {
                                            World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", '*');
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    int maxRange = 1000;

                    List<OnUse> properties = new List<OnUse>();

                    if (targetWeapon.GetComponent<Usable>() != null)
                    {
                        properties = targetWeapon.GetComponent<Usable>().onUseComponents;

                        if (properties.Count != 0)
                        {
                            foreach (OnUse property in properties)
                            {
                                if (property.range < maxRange)
                                {
                                    maxRange = property.range;
                                }
                            }
                        }
                    }


                    CreateLine(maxRange, true);

                    if (properties.Count != 0)
                    {
                        Vector2 vector2 = player.GetComponent<Vector2>();
                        foreach (OnUse property in properties)
                        {
                            List<Vector2> coordinates = RangeModels.FindModel(vector2, new Vector2(position.x, position.y), property.strength, property.range, true, property.rangeModel);
                            if (coordinates != null && coordinates.Count != 0)
                            {
                                foreach (Vector2 coordinate in coordinates)
                                {
                                    if (World.tiles[coordinate.x, coordinate.y].terrainType != 0)
                                    {
                                        if (World.sfx[coordinate.x, coordinate.y] != null && World.sfx[coordinate.x, coordinate.y].fColor == "Gray")
                                        {
                                            if (coordinate.x == position.x && coordinate.y == position.y)
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", 'X');
                                            }
                                            else
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", '*');
                                            }
                                        }
                                        else
                                        {
                                            if (coordinate.x == position.x && coordinate.y == position.y)
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Yellow", "Black", 'X');
                                            }
                                            else
                                            {
                                                World.sfx[coordinate.x, coordinate.y] = new("Yellow", "Black", '*');
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (coordinate.x == position.x && coordinate.y == position.y)
                                        {
                                            World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", 'X');
                                        }
                                        else
                                        {
                                            World.sfx[coordinate.x, coordinate.y] = new("Gray", "Black", '*');
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            CMath.DisplayToConsole(Log.console, $"Use/Throw Item Yellow*[T/U/Enter]", 0, 2, 1, 6, false);
            CMath.DisplayToConsole(Log.console, $"Move Reticle Yellow*[Arrow Yellow*Keys]", 0, 2, 1, 9, false);
            CMath.DisplayToConsole(Log.console, $"Cancel Target Yellow*[S/Escape]", 0, 2, 1, 12, false);

            Renderer.MoveCamera(new Vector2(position.x, position.y));
            Renderer.DrawMapToScreen();
        }
        public static bool CreateLine(int range, bool visual)
        {
            bool check = true;
            Vector2 vector3 = player.GetComponent<Vector2>();
            int t;
            int x = vector3.x; int y = vector3.y;
            int delta_x = position.x - vector3.x; int delta_y = position.y - vector3.y;
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
                    if (x == position.x && y == position.y)
                    {
                        hasConnected = true;
                        if (CMath.Distance(vector3.x, vector3.y, x, y) < range)
                        {
                            if (CMath.PathBlocked(vector3, new Vector2(x, y), range) && World.tiles[x, y].terrainType != 0)
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Yellow", "Black", 'X');
                                }
                                CMath.DisplayToConsole(Log.console, "", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                            }
                            else
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Gray", "Black", 'X');
                                }
                                CMath.DisplayToConsole(Log.console, "Your target is blocked.", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                                check = false;
                            }
                        }
                        else
                        {
                            if (visual)
                            {
                                World.sfx[x, y] = new("Gray", "Black", 'X');
                            }
                            CMath.DisplayToConsole(Log.console, "Your target is out of range.", 1, 1);
                            Renderer.CreateConsoleBorder(Program.logConsole, true);
                            check = false;
                        }
                    }
                    else
                    {
                        if (CMath.Distance(vector3.x, vector3.y, x, y) < range)
                        {
                            if (CMath.PathBlocked(vector3, new Vector2(x, y), range) && World.tiles[x, y].terrainType != 0)
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Yellow", "Black", '.');
                                }
                                CMath.DisplayToConsole(Log.console, "", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                            }
                            else
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Gray", "Black", '.');
                                }
                                CMath.DisplayToConsole(Log.console, "Your target is blocked.", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                                check = false;
                            }
                        }
                        else
                        {
                            if (visual)
                            {
                                World.sfx[x, y] = new("Gray", "Black", '.');
                            }
                            CMath.DisplayToConsole(Log.console, "Your target is out of range.", 1, 1);
                            Renderer.CreateConsoleBorder(Program.logConsole, true);
                            check = false;
                        }
                    }
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
                    if (x == position.x && y == position.y)
                    {
                        hasConnected = true;
                        if (CMath.Distance(vector3.x, vector3.y, x, y) < range)
                        {
                            if (CMath.PathBlocked(vector3, new Vector2(x, y), range) && World.tiles[x, y].terrainType != 0)
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Yellow", "Black", 'X');
                                }
                                CMath.DisplayToConsole(Log.console, "", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                            }
                            else
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Gray", "Black", 'X');
                                }
                                CMath.DisplayToConsole(Log.console, "Your target is blocked.", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                                check = false;
                            }
                        }
                        else
                        {
                            if (visual)
                            {
                                World.sfx[x, y] = new("Gray", "Black", 'X');
                            }
                            CMath.DisplayToConsole(Log.console, "Your target is out of range.", 1, 1);
                            Renderer.CreateConsoleBorder(Program.logConsole, true);
                            check = false;
                        }
                    }
                    else
                    {
                        if (CMath.Distance(vector3.x, vector3.y, x, y) < range)
                        {
                            if (CMath.PathBlocked(vector3, new Vector2(x, y), range) && World.tiles[x, y].terrainType != 0)
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Yellow", "Black", '.');
                                }
                                CMath.DisplayToConsole(Log.console, "", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                            }
                            else
                            {
                                if (visual)
                                {
                                    World.sfx[x, y] = new("Gray", "Black", '.');
                                }
                                CMath.DisplayToConsole(Log.console, "Your target is blocked.", 1, 1);
                                Renderer.CreateConsoleBorder(Program.logConsole, true);
                                check = false;
                            }
                        }
                        else
                        {
                            if (visual)
                            {
                                World.sfx[x, y] = new("Gray", "Black", '.');
                            }
                            CMath.DisplayToConsole(Log.console, "Your target is out of range.", 1, 1);
                            Renderer.CreateConsoleBorder(Program.logConsole, true);
                            check = false;
                        }
                    }
                }
                while (!hasConnected);
            }
            return check;
        }
        public static Vector2 ReturnCoords(bool magic)
        {
            int range;
            if (magic)
            {
                range = 1000;

                List<OnUse> properties = targetWeapon.GetComponent<Usable>().onUseComponents;

                if (properties.Count != 0)
                {
                    foreach (OnUse property in properties)
                    {
                        if (property.range < range)
                        {
                            range = property.range;
                        }
                    }
                }
            }
            else
            {
                range = player.GetComponent<Stats>().strength + 10;
            }
            Vector2 refVector3 = player.GetComponent<Vector2>();
            if (CMath.Distance(refVector3.x, refVector3.y, position.x, position.y) < range)
            {
                if (CMath.PathBlocked(player.GetComponent<Vector2>(), new Vector2(position.x, position.y), range) && World.tiles[position.x, position.y].terrainType != 0)
                {
                    World.ClearSFX();
                    StopTargeting();
                    return new Vector2(position.x, position.y);
                }
                else { CMath.DisplayToConsole(Log.console, "Your target is blocked.", 1, 1); Renderer.CreateConsoleBorder(Program.logConsole, true); return null; }
            }
            else { CMath.DisplayToConsole(Log.console, "Your target is out of range.", 1, 1); Renderer.CreateConsoleBorder(Program.logConsole, true); return null; }
        }
        public static void ThrowWeapon(Entity weaponUsed)
        {
            Vector2 vector2 = ReturnCoords(false);
            if (vector2 != null)
            {
                AttackManager.ThrowWeapon(player, vector2, weaponUsed);
            }
        }
    }
}
