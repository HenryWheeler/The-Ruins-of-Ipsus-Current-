using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Component
    {
        public Entity entity { get; set; }
        public Component() { }
    }
}
