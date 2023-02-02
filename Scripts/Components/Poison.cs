using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Poison : OnTurn
    {
        public int timeLeft { get; set; }
        public int strength { get; set; }
        public override void Turn()
        {
            timeLeft--; if (timeLeft == 0)
            {
                entity.collectionsToRemove.Add(this); entity.GetComponent<Harmable>().statusEffects.Remove("Green*Poisoned");
                if (entity.GetComponent<PlayerComponent>() != null)
                { Log.AddToStoredLog("The Green*poison ailing " + entity.GetComponent<PronounSet>().subjective + " has subsided."); }
            }
            else
            {
                int dmg = World.random.Next(strength - 2, strength + 2);
                entity.GetComponent<Harmable>().LowerHealth(dmg, "Poison");
                if (entity.GetComponent<Stats>() != null && entity.GetComponent<PlayerComponent>() != null)
                { Log.AddToStoredLog("The Green*poison drains " + dmg + " points of " + entity.GetComponent<PronounSet>().possesive + " health away."); }
            }
        }
        public Poison(int _timeLeft, int _strength) { timeLeft = _timeLeft; strength = _strength; start = true; }
        public Poison() { start = true; }
    }
}
