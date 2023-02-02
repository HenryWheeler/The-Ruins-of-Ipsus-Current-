using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class PatrolFunction: Component
    {
        public Vector2 lastPosition { get; set; }
        public int patrolRoute { get; set; }
        public int lastPastInformation = 20;
        public PatrolFunction() 
        {
            lastPosition = new Vector2(0, 0);
        }
    }
}
