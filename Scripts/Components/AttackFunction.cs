using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class AttackFunction : Component
    {
        public List<OnHit> onHitComponents = new List<OnHit>();
        public string dmgType { get; set; }
        public int die1 { get; set; }
        public int die2 { get; set; }
        public int damageModifier { get; set; }
        public int toHitModifier { get; set; }
        public AttackFunction(int _die1, int _die2, int _damageModifier, int _toHitModifier, string _dmgType)
        {
            die1 = _die1;
            die2 = _die2;
            damageModifier = _damageModifier;
            toHitModifier = _toHitModifier;
            dmgType = _dmgType;
        }
        public AttackFunction() { }
    }
}
