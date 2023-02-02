using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Commerce : Component
    {
        public int value { get; set; }
        public Commerce(int _value)
        {
            value = _value;
        }
        public Commerce() { }
    }
}
