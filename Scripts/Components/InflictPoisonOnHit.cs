using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class InflictPoisonOnHit : OnHit
    {
        public int strength { get; set; }
        public override void Hit(Entity attacker, Entity target, int dmg, string type)
        {
            if (target.GetComponent<Poison>() == null && !target.GetComponent<Stats>().immunities.Contains("Poison"))
            {
                if (World.random.Next(1, 21) + strength > 10 + target.GetComponent<Stats>().strength)
                {
                    target.AddComponent(new Poison(strength * 3, strength));
                    if (attacker.GetComponent<PronounSet>().present) { Log.Add(attacker.GetComponent<Description>().name + " has inflicted " + target.GetComponent<Description>().name + " with a malignant Green*poison"); }
                    else { Log.Add(attacker.GetComponent<Description>().name + " have inflicted " + target.GetComponent<Description>().name + " with a malignant Green*poison"); }
                    target.GetComponent<Harmable>().statusEffects.Add("Green*Poisoned");
                }
            }
        }
        public InflictPoisonOnHit(int _strength) { strength = _strength; attack = true; }
        public InflictPoisonOnHit() { attack = true; }
    }
}
