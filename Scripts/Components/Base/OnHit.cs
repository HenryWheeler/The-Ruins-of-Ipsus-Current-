using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public abstract class OnHit : Component
    {
        public bool attack { get; set; }
        public abstract void Hit(Entity attacker, Entity target, int dmg, string type);
        public OnHit ReturnBase() { return this; }
        public OnHit() { }
    }
}
