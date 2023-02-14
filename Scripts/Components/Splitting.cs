using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Splitting: OnHit
    {
        public int splitNumber { get; set; }
        public int dmgThreshhold { get; set; }
        public override void Hit(Entity attacker, Entity target, int dmg, string type)
        {
            if (entity.GetComponent<Stats>() == null) { return; }
            if (!entity.GetComponent<Stats>().immunities.Contains(type) && !entity.GetComponent<Stats>().weaknesses.Contains(type) && dmg <= dmgThreshhold)
            {
                for (int i = 0; i < splitNumber; i++)
                {
                    Vector2 location = CMath.ReturnNearestValidCoordinate("Actor", entity.GetComponent<Vector2>());
                    //TurnManager.RemoveActor(entity.GetComponent<TurnFunction>());
                    //entity.RemoveComponent(entity.GetComponent<TurnFunction>());
                    List<Component> components = new List<Component>();

                    foreach (Component component in entity.components)
                    {
                        Component component1 = component;
                        components.Add(component1);
                    }

                    components.Add(new TurnFunction());
                    Entity newEntity = new Entity(components);


                    entity.AddComponent(new TurnFunction());
                    EntityManager.CreateEntity(location, newEntity, false);
                    EntityManager.CreateEntity(location, entity, false);
                    TurnManager.AddActor(entity.GetComponent<TurnFunction>());
                }
            }
        }
        public Splitting(int splitNumber, int dmgThreshhold)
        {
            this.splitNumber = splitNumber;
            this.dmgThreshhold = dmgThreshhold;
            attack = false;
        }
        public Splitting() { attack = false; }
    }
}
