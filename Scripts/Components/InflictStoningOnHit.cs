using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class InflictStoningOnHit : OnHit
    {
        public int strength { get; set; }
        public override void Hit(Entity attacker, Entity target, int dmg, string type)
        {
            if (target.GetComponent<Stoning>() == null && !target.GetComponent<Stats>().immunities.Contains("Stoning"))
            {
                if (World.random.Next(1, 21) + strength > 10 + target.GetComponent<Stats>().strength)
                {
                    target.AddComponent(new Stoning());
                    if (target.GetComponent<PronounSet>().present) { Log.AddToStoredLog(target.GetComponent<Description>().name + " has begun to turn to Gray*stone"); }
                    else { Log.AddToStoredLog(target.GetComponent<Description>().name + " have begun to turn to Gray*stone"); }
                    target.GetComponent<Harmable>().statusEffects.Add("Gray*Stoned");
                }
            }
        }
        public InflictStoningOnHit(int _strength) { strength = _strength; attack = true; }
        public InflictStoningOnHit() { attack = true; }
    }
}
