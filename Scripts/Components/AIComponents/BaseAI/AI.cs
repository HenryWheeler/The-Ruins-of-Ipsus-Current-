using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public abstract class AI : Component
    {
        public AI(List<string> favoredEntities, List<string> hatedEntities, int baseInterest, int minDistance, int preferredDistance, int maxDistance, int abilityChance, int hate, int fear, int greed)
        {
            this.favoredEntities = favoredEntities;
            this.hatedEntities = hatedEntities;
            this.baseInterest = baseInterest;
            this.interest = baseInterest;
            this.minDistance = minDistance;
            this.preferredDistance = preferredDistance;
            this.maxDistance = maxDistance;
            this.abilityChance = abilityChance;
            this.hate = hate;
            this.fear = fear;
            this.greed = greed;
        }
        //[Serializable]
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum State
        {
            Asleep,
            Angry,
            Fearful,
            Curious,
            Awake,
            Bored,
            Patrolling,

        }
        //[Serializable]
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum Input
        {
            Noise,
            Hunger,
            Hurt,
            Call,
            Tired,
            Hatred,
            Bored,
            Fear,
            None
        }
        public class StateMachine
        {
            readonly State currentState;
            readonly Input currentInput;
            public StateMachine(State _state, Input _input)
            {
                currentState = _state;
                currentInput = _input;
            }
            public override int GetHashCode()
            {
                return 17 + 31 * currentState.GetHashCode() + 31 * currentInput.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                StateMachine other = obj as StateMachine;
                return other != null && currentState == other.currentState && currentInput == other.currentInput;
            }
        }
        public Dictionary<StateMachine, State> transitions = new Dictionary<StateMachine, State>();
        public State currentState = State.Asleep;
        public Input currentInput = Input.None;
        public List<string> favoredEntities = new List<string>();
        public List<string> hatedEntities = new List<string>();
        public Entity target { get; set; }
        public int interest { get; set; }
        public int baseInterest { get; set; }
        public int maxDistance { get; set; }
        public int minDistance { get; set; }
        public int preferredDistance { get; set; }
        /// <summary>
        /// The level of hate the AI has. 
        /// A higher level will lead to the AI being far more aggressive and less likely to flee.
        /// AI with a hate of zero will never fight.
        /// </summary>
        public int hate { get; set; }
        /// <summary>
        /// The level of fear the AI has. 
        /// A higher level will lead to the AI being far more likely to flee if it feels an enemy is more powerful than it. 
        /// AI with a fear of zero will never run.
        /// </summary>
        public int fear { get; set; }
        /// <summary>
        /// The level of greed the AI has. 
        /// A higher level will lead to the AI being more likely to attempt to grab items within its environment. 
        /// AI with a greed of zero will never attempt to grab items.
        /// </summary>
        public int greed { get; set; }
        /// <summary>
        ///The chance for the AI to use an ability each round
        /// </summary>
        public int abilityChance { get; set; }
        public void Process()
        {
            bool execution = false;
            try
            {
                if (entity.components == null)
                {
                    RemoveEntityEntirely();
                    throw new Exception("Entity component list equals null: Removing Entity to prevent further issues.");
                }
                else if (transitions.Count == 0)
                {
                    //RemoveEntityEntirely();
                    throw new Exception("Entity transition count equals zero: Removing Entity to prevent further issues.");
                }

                Observe();
                StateMachine stateMachine = new StateMachine(currentState, currentInput);
                if (transitions.ContainsKey(stateMachine))
                {
                    currentState = transitions[stateMachine];
                    currentInput = Input.None;

                    StateParticleCreation();
                }

                execution = true;
                ExecuteAction();
            }
            catch (Exception e)
            {
                if (execution)
                {
                    Log.Add($"{entity.GetComponent<Description>().name} gives error: {e.Message}");
                }
                else
                {
                    Log.Add($"{entity.GetComponent<Description>().name} gives error: {e.Message}");
                }
                entity.GetComponent<TurnFunction>().EndTurn();
            }
        }
        public void StateParticleCreation()
        {
            switch (currentState) 
            {
                case State.Angry: 
                    {
                        Vector2 vector2 = entity.GetComponent<Vector2>();
                        Entity hitParticle = new Entity(new List<Component>
                        {
                            new Vector2(0, 0),
                            new Draw("Red", "Black", (char)19),
                            new ParticleComponent(30, "Attached", 1, new Draw[] { new Draw("Red", "Black", (char)19) }, vector2)
                        });
                        Renderer.AddParticle(vector2.x, vector2.y, hitParticle);
                        break;
                    }
                case State.Fearful:
                    {
                        Vector2 vector2 = entity.GetComponent<Vector2>();
                        Entity hitParticle = new Entity(new List<Component>
                        {
                            new Vector2(0, 0),
                            new Draw("Purple", "Black", (char)19),
                            new ParticleComponent(30, "Attached", 1, new Draw[] { new Draw("Purple", "Black", (char)19) }, vector2)
                        });
                        Renderer.AddParticle(vector2.x, vector2.y, hitParticle);
                        break;
                    }
            }
        }
        public void RemoveEntityEntirely()
        {
            TurnManager.RemoveActor(entity.GetComponent<TurnFunction>());
            entity.GetComponent<TurnFunction>().turnActive = false;
            Vector2 position = entity.GetComponent<Vector2>();
            World.tiles[position.x, position.y].actorLayer = null;
            entity.ClearEntity();
        }
        public abstract void ExecuteAction();
        public abstract void SetTransitions();
        public void Observe()
        {
            //Stats stats = entity.GetComponent<Stats>();
            if (currentState == State.Asleep)
            {
                //Low level of observation
                //Check for noise
                //Create relevant input
            }
            else if (currentState == State.Curious)
            {
                //High level of observation
                //Check for noise
                CallObservation(true);
            }
            else
            {
                //Medium level of observation
                //Check for noise
                CallObservation(false);
            }
        }
        public void ConsiderEquipment(string slot)
        {
            List<Entity> items = entity.GetComponent<Inventory>().inventory;
            Entity potentialItem;
            int relativeValue;

            switch (slot)
            {
                case "Armor":
                    {
                        Entity armor = entity.GetComponent<Inventory>().ReturnSlot("Armor").item;
                        potentialItem = armor;

                        if (armor != null && armor.GetComponent<Equippable>().unequipable) 
                        {
                            return;
                        }

                        foreach (Entity item in items)
                        {
                            if (item != armor && item.GetComponent<Equippable>() != null && item.GetComponent<Stats>() != null) 
                            {
                                potentialItem ??= item;
                                if (armor != null)
                                {
                                    Stats original = potentialItem.GetComponent<Stats>();
                                    Stats compare = armor.GetComponent<Stats>();
                                    relativeValue = 0;

                                    if (compare.ac > original.ac) { relativeValue++; }
                                    else if (compare.ac < original.ac) { relativeValue--; }

                                    if (relativeValue > 0)
                                    {
                                        potentialItem = item;
                                    }
                                }
                                else
                                {
                                    potentialItem = item;
                                }
                            }
                        }

                        if (potentialItem != armor)
                        {
                            InventoryManager.EquipItem(entity, potentialItem);
                        }

                        break;
                    }
                case "Weapon":
                    {
                        Entity weapon = entity.GetComponent<Inventory>().ReturnSlot("Weapon").item;
                        potentialItem = weapon;

                        if (weapon != null && weapon.GetComponent<Equippable>().unequipable)
                        {
                            return;
                        }

                        foreach (Entity item in items)
                        {
                            if (item != weapon && item.GetComponent<Equippable>() != null && item.GetComponent<AttackFunction>() != null)
                            {
                                potentialItem ??= item;
                                if (weapon != null)
                                {
                                    AttackFunction original = potentialItem.GetComponent<AttackFunction>();
                                    AttackFunction compare = weapon.GetComponent<AttackFunction>();
                                    relativeValue = 0;

                                    if (compare.die1 > original.die1) { relativeValue += 2; }
                                    else if (compare.die1 < original.die1) { relativeValue -= 2; }

                                    if (compare.die2 > original.die2) { relativeValue++; }
                                    else if (compare.die2 < original.die2) { relativeValue--; }

                                    if (compare.damageModifier > original.damageModifier) { relativeValue++; }
                                    else if (compare.damageModifier < original.damageModifier) { relativeValue--; }

                                    if (compare.toHitModifier > original.toHitModifier) { relativeValue++; }
                                    else if (compare.toHitModifier < original.toHitModifier) { relativeValue--; }

                                    if (relativeValue > 0)
                                    {
                                        potentialItem = item;
                                    }
                                }
                                else
                                {
                                    potentialItem = item;
                                }
                            }
                        }

                        if (potentialItem != weapon)
                        {
                            InventoryManager.EquipItem(entity, potentialItem);
                        }

                        break;
                    }
                case "Off Hand":
                    {
                        Entity offHand = entity.GetComponent<Inventory>().ReturnSlot("Off Hand").item;
                        potentialItem = offHand;

                        foreach (Entity item in items)
                        {
                            if (item != offHand && item.GetComponent<Equippable>() != null)
                            {
                                potentialItem ??= item;

                                AttackFunction original = potentialItem.GetComponent<AttackFunction>();
                                AttackFunction compare = offHand.GetComponent<AttackFunction>();
                                relativeValue = 0;


                                if (relativeValue >= 0)
                                {
                                    potentialItem = item;
                                }
                            }
                        }

                        if (potentialItem != offHand)
                        {
                            InventoryManager.EquipItem(entity, potentialItem);
                        }

                        break;
                    }
                case "Magic Item":
                    {
                        Entity magicItem = entity.GetComponent<Inventory>().ReturnSlot("Magic Item").item;
                        potentialItem = magicItem;

                        foreach (Entity item in items)
                        {
                            if (item != magicItem && item.GetComponent<Equippable>() != null)
                            {
                                potentialItem ??= item;

                                AttackFunction original = potentialItem.GetComponent<AttackFunction>();
                                AttackFunction compare = magicItem.GetComponent<AttackFunction>();
                                relativeValue = 0;


                                if (relativeValue >= 0)
                                {
                                    potentialItem = item;
                                }
                            }
                        }

                        if (potentialItem != magicItem)
                        {
                            InventoryManager.EquipItem(entity, potentialItem);
                        }

                        break;
                    }
            }
        }
        public void OnHit(Entity attacker)
        {
            if (attacker.GetComponent<Faction>() != null)
            {
                if (!hatedEntities.Contains(attacker.GetComponent<Faction>().faction))
                {
                    hatedEntities.Add(attacker.GetComponent<Faction>().faction);
                }
                target = attacker;
            }
            interest = baseInterest;
            currentInput = Input.Hurt;
        }
        public void CallObservation(bool highLevel)
        {
            Input input = Input.None;
            Vector2 startPos = entity.GetComponent<Vector2>();
            int rangeLimit = entity.GetComponent<Stats>().sight;

            for (uint octant = 0; octant < 8; octant++)
            {
                input = ComputeSight(octant, startPos.x, startPos.y, rangeLimit, 1, new Slope(1, 1), new Slope(0, 1), 1, highLevel);
                if (input != Input.None && !highLevel)
                {
                    break;
                }
            }
            if (input != Input.None)
            {
                currentInput = input;
            }
            else
            {
                if (interest <= 0)
                {
                    currentInput = Input.Bored;
                    target = null;
                }
            }
        }
        public Input ComputeSight(uint octant, int oX, int oY, int rangeLimit, int x, Slope top, Slope bottom, int currentInterest, bool highLevel)
        {
            Input greatestInput = Input.None;
            for (; (uint)x <= (uint)rangeLimit; x++)
            {
                int topY = top.X == 1 ? x : ((x * 2 + 1) * top.Y + top.X - 1) / (top.X * 2);
                int bottomY = bottom.Y == 0 ? 0 : ((x * 2 - 1) * bottom.Y + bottom.X) / (bottom.X * 2);

                int wasOpaque = -1;
                for (int y = topY; y >= bottomY; y--)
                {
                    int tx = oX, ty = oY;
                    switch (octant)
                    {
                        case 0: tx += x; ty -= y; break;
                        case 1: tx += y; ty -= x; break;
                        case 2: tx -= y; ty -= x; break;
                        case 3: tx -= x; ty -= y; break;
                        case 4: tx -= x; ty += y; break;
                        case 5: tx -= y; ty += x; break;
                        case 6: tx += y; ty += x; break;
                        case 7: tx += x; ty += y; break;
                    }

                    bool inRange = rangeLimit < 0 || CMath.Distance(oX, oY, tx, ty) <= rangeLimit;
                    if (inRange && (y != topY || top.Y * x >= top.X * y) && (y != bottomY || bottom.Y * x <= bottom.X * y))
                    {
                        Traversable traversable = World.tiles[tx, ty];
                        if (CMath.CheckBounds(tx, ty) && traversable.actorLayer != null && traversable.actorLayer != entity)
                        {
                            //Check for entities relative hatred or fear, etc, of the target
                            InterestInputReturn refInput = ReturnFeelings(traversable.actorLayer);
                            if (refInput.input != Input.None && refInput.interest > interest && refInput.interest > 0)
                            {
                                greatestInput = refInput.input;
                                currentInterest = refInput.interest;
                                if (!highLevel)
                                {
                                    interest = currentInterest;
                                    target = traversable.actorLayer;
                                    return greatestInput;
                                }
                            }
                        }
                    }

                    bool isOpaque = !inRange || BlocksLight(new Vector2(tx, ty));
                    if (x != rangeLimit)
                    {
                        if (isOpaque)
                        {
                            if (wasOpaque == 0)
                            {
                                Slope newBottom = new Slope(y * 2 + 1, x * 2 - 1);
                                if (!inRange || y == bottomY) { bottom = newBottom; break; }
                                else { greatestInput = ComputeSight(octant, oX, oY, rangeLimit, x + 1, top, newBottom, currentInterest, highLevel); }
                            }
                            wasOpaque = 1;
                        }
                        else
                        {
                            if (wasOpaque > 0) top = new Slope(y * 2 + 1, x * 2 + 1);
                            wasOpaque = 0;
                        }
                    }
                }
                if (wasOpaque != 0) break;
            }
            if (currentInterest > interest && greatestInput != Input.None)
            {
                interest = currentInterest;
                return greatestInput;
            }
            else
            {
                return Input.None;
            }
        }
        public bool BlocksLight(Vector2 vector2)
        {
            if (CMath.CheckBounds(vector2.x, vector2.y))
            {
                Traversable traversable = World.tiles[vector2.x, vector2.y];
                if (traversable.entity.GetComponent<Visibility>().opaque)
                {
                    return true;
                }
                else if (traversable.obstacleLayer != null && traversable.obstacleLayer.GetComponent<Visibility>().opaque)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        public InterestInputReturn ReturnFeelings(Entity entity)
        {
            if (entity.GetComponent<ID>().entityType == "Actor")
            {
                string faction = entity.GetComponent<Faction>().faction;
                Stats stats = entity.GetComponent<Stats>();
                Stats AIStats = this.entity.GetComponent<Stats>();
                if (!favoredEntities.Contains(faction) && hatedEntities.Contains(faction))
                {
                    int baseInterest = this.baseInterest;
                    if (AIStats.acuity <= -1)
                    {
                        baseInterest -= stats.hp;
                        baseInterest += (int)(AIStats.hp * 3f) + hate;
                    }
                    else if (AIStats.acuity < 3)
                    {
                        baseInterest -= stats.hp + ((stats.strength + 1) * 5) + ((stats.acuity + 1) * 5);
                        baseInterest += (int)((AIStats.hp + ((AIStats.strength + 1) * 5) + ((AIStats.acuity + 1) * 5)) * 1.5f) + hate;
                    }
                    else
                    {
                        baseInterest -= stats.hp + ((stats.strength + 1) * 3) + ((stats.acuity + 1) * 3) + ((stats.ac + 1) * 3);
                        baseInterest += AIStats.hp + ((AIStats.strength + 1) * 3) + ((AIStats.acuity + 1) * 3) + ((AIStats.ac + 1) * 3) + hate;
                        foreach (string status in entity.GetComponent<Harmable>().statusEffects)
                        {
                            if (hatedEntities.Contains(status))
                            {
                                baseInterest += 25;
                            }
                            else
                            {
                                baseInterest += 5;
                            }
                        }
                    }

                    if (baseInterest - fear < 0 && fear > 0)
                    {
                        baseInterest = Math.Abs(baseInterest) + fear;
                        return new InterestInputReturn(Input.Fear, baseInterest);
                    }
                    else if (baseInterest > 1 && hate > 0)
                    {
                        return new InterestInputReturn(Input.Hatred, baseInterest);
                    }

                    return new InterestInputReturn(Input.None, 0);
                }
                else
                {
                    return new InterestInputReturn(Input.None, 0);
                }
            }
            else if (entity.GetComponent<ID>().entityType == "Item")
            {
                return new InterestInputReturn(Input.None, 0);
            }
            else
            {
                return new InterestInputReturn(Input.None, 0);
            }
        }
        public AI() { }
    }
    public class InterestInputReturn
    {
        public AI.Input input { get; set; }
        public int interest { get; set; }
        public InterestInputReturn(AI.Input input, int interest) 
        {
            this.input = input;
            this.interest = interest;
        }
    }
}
