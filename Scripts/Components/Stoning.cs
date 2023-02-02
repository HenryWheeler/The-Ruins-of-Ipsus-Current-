using System;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Stoning : OnTurn
    {
        public int timeLeft = 10;
        public override void Turn()
        {
            timeLeft--;
            switch (timeLeft)
            {
                case 6:
                    {
                        if (entity.GetComponent<PronounSet>().present) { Log.AddToStoredLog(entity.GetComponent<Description>().name + "'s bones become stiffer"); }
                        else { Log.AddToStoredLog(entity.GetComponent<PronounSet>().possesive + " bones become stiffer"); }
                        break;
                    }
                case 3:
                    {
                        if (entity.GetComponent<PronounSet>().present) { Log.AddToStoredLog(entity.GetComponent<Description>().name + "'s skin turns Gray*gray"); }
                        else { Log.AddToStoredLog(entity.GetComponent<PronounSet>().possesive + " skin turns Gray*gray"); }
                        break;
                    }
                case 0:
                    {
                        if (entity.GetComponent<PronounSet>().present) { Log.AddToStoredLog(entity.GetComponent<Description>().name + "'s body has becomes completely stone"); }
                        else { Log.AddToStoredLog(entity.GetComponent<PronounSet>().possesive + " body has become completely stone"); }

                        Vector2 vector2 = entity.GetComponent<Vector2>();

                        Entity statue = new Entity();
                        statue.AddComponent(vector2);
                        statue.AddComponent(new Draw("Gray", "Black", entity.GetComponent<Draw>().character));
                        if (entity.GetComponent<PronounSet>().present) { statue.AddComponent(new Description("Statue of a " + entity.GetComponent<Description>().name, "A highly realistic statue of a " + entity.GetComponent<Description>().name)); }
                        else { statue.AddComponent(new Description("Statue of " + entity.GetComponent<Description>().name, "A highly realistic statue of " + entity.GetComponent<Description>().name)); }
                        statue.AddComponent(new ID(2500));
                        World.tiles[vector2.x, vector2.y].obstacleLayer = statue;
                        entity.GetComponent<Harmable>().Death("Stoning");
                        break;
                    }
            }
        }
        public Stoning() { start = true; }
    }
}
