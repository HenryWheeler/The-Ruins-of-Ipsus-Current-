using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Harmable : Component
    {
        public List<OnHit> onHitComponents = new List<OnHit>();
        public List<string> statusEffects = new List<string>();
        public void Hit(int dmg, string type, string weaponName, Entity attacker)
        {
            TriggerOnHit(attacker, entity, dmg, type);

            if (CMath.ReturnAI(entity) != null)
            {
                CMath.ReturnAI(entity).OnHit(attacker);
            }

            Stats stats = entity.GetComponent<Stats>();
            stats.hp -= dmg;

            if (entity.GetComponent<PlayerComponent>() != null)
            {
                StatManager.UpdateStats(entity);
            }

            if (stats.hp <= 0)
            {
                Death(attacker.GetComponent<Description>().name);
            }
            else
            {
                PronounSet pronounSet = attacker.GetComponent<PronounSet>();
                if (entity == attacker)
                {
                    Log.Add($"{attacker.GetComponent<Description>().name} hit {pronounSet.reflexive} for {dmg} damage with {pronounSet.possesive} {weaponName}.");
                }
                else if (pronounSet != null && pronounSet.present)
                {
                    Log.Add($"{attacker.GetComponent<Description>().name} hit {entity.GetComponent<Description>().name} for {dmg} damage with {pronounSet.possesive} {weaponName}.");
                }
                else if (pronounSet != null)
                {
                    Log.Add($"{attacker.GetComponent<Description>().name} hit you for {dmg} damage with {pronounSet.possesive} {weaponName}.");
                }
                else
                {
                    Log.Add($"{attacker.GetComponent<Description>().name} hit you for {dmg} damage with the {weaponName}.");
                }

                Entity hitParticle = new Entity(new List<Component>
                        {
                            new Vector2(0, 0),
                            new Draw("Red", "Black", (char)3),
                            new ParticleComponent(2, 1, "None", 1, new Draw[] { new Draw("Red", "Black", (char)3) })
                        });
                Vector2 vector2 = entity.GetComponent<Vector2>();
                Renderer.AddParticle(vector2.x, vector2.y, hitParticle);
            }
        }
        public void LowerHealth(int dmg, string cause)
        {
            Stats stats = entity.GetComponent<Stats>();
            stats.hp -= dmg;
            if (entity.GetComponent<PlayerComponent>() != null)
            {
                StatManager.UpdateStats(entity);
            }
            if (stats.hp <= 0)
            {
                Death(cause);
            }
        }
        public void TriggerOnHit(Entity attacker, Entity target, int dmg, string dmgType)
        {
            foreach (OnHit component in onHitComponents)
            {
                if (component != null)
                {
                    component.Hit(attacker, target, dmg, dmgType);
                }
            }
        }
        public void Death(string causeOfDeath)
        {
            if (entity.GetComponent<PlayerComponent>() == null)
            {
                Entity hitParticle = new Entity(new List<Component>
                        {
                            new Vector2(0, 0),
                            new Draw("Red", "Black", 'X'),
                            new ParticleComponent(2, 1, "None", 1, new Draw[] { new Draw("Red", "Black", 'X') })
                        });
                Vector2 vector2 = entity.GetComponent<Vector2>();
                Renderer.AddParticle(vector2.x, vector2.y, hitParticle);
                Log.Add($"{entity.GetComponent<Description>().name} has died.");

                entity.RemoveComponent(CMath.ReturnAI(entity));
                TurnManager.RemoveActor(entity.GetComponent<TurnFunction>());
                EntityManager.ClearAIOfEntity(entity);
                entity.GetComponent<TurnFunction>().turnActive = false;
                World.tiles[vector2.x, vector2.y].actorLayer = null;
                EntityManager.RemoveEntity(entity);

                entity.ClearEntity();
            }
            else
            {
                Menu.EndGame(causeOfDeath);
            }
        }
        public Harmable() { }
    }
}
