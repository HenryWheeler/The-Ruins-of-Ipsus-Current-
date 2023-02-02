using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public abstract class OnTurn : Component
    {
        public bool start { get; set; }
        public abstract void Turn();
        public OnTurn ReturnBase() { return this; }
        public OnTurn() { }
    }
}
