using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Vector2 : Component
    {
        public int x { get; set; }
        public int y { get; set; }
        public Vector2(int _x, int _y) 
        { 
            x = _x; 
            y = _y; 
        }
        public Vector2(Vector2 _1, Vector2 _2)
        {
            x = _1.x + _2.x;
            y = _1.y + _2.y;
        }
        public override int GetHashCode()
        {
            return 17 + 31 * x.GetHashCode() + 31 * y.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Vector2 other = obj as Vector2;
            return other != null && x == other.x && y == other.y;
        }
        public Vector2() { }
    }
}
