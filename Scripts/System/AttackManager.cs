using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    public class AttackManager
    {
        public static void MeleeAllStrike(Entity attacker, Entity target)
        {
            if (attacker.GetComponent<Inventory>().ReturnSlot("Weapon").item == null)
            {
                Attack(attacker, target, new Entity(new List<Component>()
                {
                    new AttackFunction(1, 1, 0, 0, "Bludgeoning"),
                    new Description("Fists", "Fists")
                }));
            }
            else
            {
                Attack(attacker, target, attacker.GetComponent<Inventory>().ReturnSlot("Weapon").item);
            }
        }
        public static void ThrowWeapon(Entity attacker, Vector2 target, Entity weapon)
        {
            try
            {
                Vector2 vector2 = attacker.GetComponent<Vector2>();
                List<Entity> particles = new List<Entity>();
                int time = CMath.Distance(attacker.GetComponent<Vector2>(), target);

                Renderer.StartAnimation(new List<Entity>() {
                    new Entity(new List<Component>
                        {
                            new Vector2(target.x, target.y),
                            new Draw("Yellow", "Black", 'X'),
                            new ParticleComponent(time, "None", 0, new Draw[] { new Draw("Yellow", "Black", 'X'), new Draw("Black", "Black", 'X') }),
                        }),
                    new Entity(new List<Component>
                        {
                            new Vector2(vector2.x, vector2.y),
                            weapon.GetComponent<Draw>(),
                            new ParticleComponent(time, "Target", 0, new Draw[] { weapon.GetComponent<Draw>() }, target),
                        })
                });

                if (weapon.GetComponent<Equippable>() != null && weapon.GetComponent<Equippable>().equipped)
                {
                    InventoryManager.UnequipItem(attacker, weapon);
                }

                PronounSet pronounSet = attacker.GetComponent<PronounSet>();

                if (weapon.GetComponent<Throwable>() != null)
                {
                    weapon.GetComponent<Throwable>().Throw(attacker, target);
                    if (!weapon.GetComponent<Throwable>().consumable)
                    {
                        InventoryManager.PlaceItem(target, weapon);
                    }
                    if (pronounSet.present)
                    {
                        Log.Add($"{attacker.GetComponent<Description>().name} has thrown {pronounSet.possesive} {weapon.GetComponent<Description>().name}! {weapon.GetComponent<Throwable>().throwMessage}");
                    }
                    else
                    {
                        Log.Add($"{attacker.GetComponent<Description>().name} have thrown {pronounSet.possesive} {weapon.GetComponent<Description>().name}! {weapon.GetComponent<Throwable>().throwMessage}");
                    }
                }
                else
                {
                    InventoryManager.PlaceItem(target, weapon);
                    if (pronounSet.present)
                    {
                        Log.Add($"{attacker.GetComponent<Description>().name} has thrown {pronounSet.possesive} {weapon.GetComponent<Description>().name}!");
                    }
                    else
                    {
                        Log.Add($"{attacker.GetComponent<Description>().name} have thrown {pronounSet.possesive} {weapon.GetComponent<Description>().name}!");
                    }
                }
                if (World.tiles[target.x, target.y].actorLayer != null)
                {
                    Attack(attacker, World.tiles[target.x, target.y].actorLayer, weapon, false);
                }

                InventoryManager.RemoveFromInventory(attacker, weapon);

                attacker.GetComponent<TurnFunction>().EndTurn();
            }
            catch (ArgumentNullException e)
            {
                Log.Add($"Cannot throw because {e.Message} is null");
                attacker.GetComponent<TurnFunction>().EndTurn();
            }
        }
        public static void Attack(Entity attacker, Entity target, Entity weapon, bool endTurn = true)
        {
            if (attacker != null && target != null && weapon != null && weapon.GetComponent<AttackFunction>() != null)
            {
                AttackFunction attackFunction = weapon.GetComponent<AttackFunction>();

                if (World.random.Next(0, 20) + attackFunction.toHitModifier + attacker.GetComponent<Stats>().strength >= target.GetComponent<Stats>().ac)
                {
                    if (target.GetComponent<Stats>().immunities.Contains(attackFunction.dmgType))
                    {
                        if (attacker.GetComponent<PlayerComponent>() != null)
                        {
                            Log.Add($"Your {weapon.GetComponent<Description>().name} deals the {target.GetComponent<Description>().name} no harm. The {target.GetComponent<Description>().name} is immune to {attackFunction.dmgType}.");
                        }
                    }
                    else
                    {
                        int dmg = 0;
                        for (int d = 0; d < attackFunction.die1; d++)
                        {
                            dmg += World.random.Next(1, attackFunction.die2);
                        }
                        dmg += attackFunction.damageModifier;

                        if (target.GetComponent<Stats>().weaknesses.Contains(attackFunction.dmgType))
                        {
                            if (attacker.GetComponent<PlayerComponent>() != null)
                            {
                                Log.Add($"Your {weapon.GetComponent<Description>().name} deals the {target.GetComponent<Description>().name} major harm! The {target.GetComponent<Description>().name} is weak to {attackFunction.dmgType}!");
                            }
                            target.GetComponent<Harmable>().Hit(dmg * 2, attackFunction.dmgType, weapon.GetComponent<Description>().name, attacker);
                            TriggerOnHit(attackFunction, attacker, target, dmg * 2, attackFunction.dmgType);
                        }
                        else
                        {
                            target.GetComponent<Harmable>().Hit(dmg, attackFunction.dmgType, weapon.GetComponent<Description>().name, attacker);
                            TriggerOnHit(attackFunction, attacker, target, dmg, attackFunction.dmgType);
                        }
                    }
                }
                else
                {
                    Log.Add($"{attacker.GetComponent<Description>().name} missed with {attacker.GetComponent<PronounSet>().possesive} {weapon.GetComponent<Description>().name}.");
                }

                if (endTurn)
                {
                    attacker.GetComponent<TurnFunction>().EndTurn();
                }
            }
            else
            {
                attacker.GetComponent<TurnFunction>().EndTurn();
            }
        }
        public static void Attack(Entity attacker, Entity target, AttackFunction attackFunction, string attackName)
        {
            try
            {
                if (attacker != null && target != null && attackFunction != null)
                {
                    if (target.GetComponent<Stats>().immunities.Contains(attackFunction.dmgType))
                    {
                        if (attacker.GetComponent<PlayerComponent>() != null)
                        {
                            Log.Add($"Your {attackName} deals the {target.GetComponent<Description>().name} no harm. The {target.GetComponent<Description>().name} is immune to {attackFunction.dmgType}.");
                        }
                    }
                    else
                    {
                        int dmg = 0;
                        for (int d = 0; d < attackFunction.die1; d++)
                        {
                            dmg += World.random.Next(1, attackFunction.die2);
                        }
                        dmg += attackFunction.damageModifier;

                        if (target.GetComponent<Stats>().weaknesses.Contains(attackFunction.dmgType))
                        {
                            if (attacker.GetComponent<PlayerComponent>() != null)
                            {
                                Log.Add($"Your {attackName} deals the {target.GetComponent<Description>().name} major harm! The {target.GetComponent<Description>().name} is weak to {attackFunction.dmgType}!");
                            }
                            target.GetComponent<Harmable>().Hit(dmg * 2, attackFunction.dmgType, attackName, attacker);
                            TriggerOnHit(attackFunction, attacker, target, dmg * 2, attackFunction.dmgType);
                        }
                        else
                        {
                            target.GetComponent<Harmable>().Hit(dmg, attackFunction.dmgType, attackName, attacker);
                            TriggerOnHit(attackFunction, attacker, target, dmg, attackFunction.dmgType);
                        }
                    }
                }
                else
                {
                    if (attacker == null)
                    {
                        throw new ArgumentNullException(paramName: nameof(attacker), message: "Attacker cannot be null");
                    }
                    else if (target == null)
                    {
                        throw new ArgumentNullException(paramName: nameof(target), message: "Target cannot be null");
                    }
                    else if (attackFunction == null)
                    {
                        throw new ArgumentNullException(paramName: nameof(attackFunction), message: "AttackFunction cannot be null");
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Add($"Cannot attack because {e.Message} is null");
                attacker.GetComponent<TurnFunction>().EndTurn();
            }
        }
        public static void TriggerOnHit(AttackFunction attackFunction, Entity attacker, Entity target, int dmg, string dmgType)
        {
            foreach (OnHit component in attackFunction.onHitComponents)
            {
                component.Hit(attacker, target, dmg, dmgType);
            }
        }
    }
}
