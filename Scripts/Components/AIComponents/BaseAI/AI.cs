using System;
using System.Collections.Generic;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public abstract class AI : Component
    {
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
        public int abilityChance { get; set; }
        //The chance for a creature to use an ability each round
        public void Process()
        {
            try
            {
                if (transitions.Count == 0)
                {
                    throw new Exception("Entity Transition Count == 0");
                }

                Observe();
                StateMachine stateMachine = new StateMachine(currentState, currentInput);
                if (transitions.ContainsKey(stateMachine))
                {
                    currentState = transitions[stateMachine];
                    currentInput = Input.None;
                }
                ExecuteAction();
            }
            catch (Exception e)
            {
                Log.Add($"{entity.GetComponent<Description>().name} gives error: {e.Message}");
                entity.GetComponent<TurnFunction>().EndTurn();
            }
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
                //Hight level of observation
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
        public void OnHit(Entity attacker)
        {
            if (attacker.GetComponent<Faction>() != null && !hatedEntities.Contains(attacker.GetComponent<Faction>().faction))
            {
                hatedEntities.Add(attacker.GetComponent<Faction>().faction);
            }
            target = attacker;
            interest = 100;
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
                            int referenceInterest = ReturnFeelings(traversable.actorLayer);
                            if (interest < referenceInterest)
                            {
                                //Check for entities relative hatred or fear, etc, of the target
                                //For now default to 25 interest
                                target = traversable.actorLayer;
                                greatestInput = Input.Hatred;
                                currentInterest = referenceInterest;
                                if (!highLevel)
                                {
                                    interest = currentInterest;
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
            if (greatestInput != Input.None)
            {
                interest = currentInterest;
            }
            return greatestInput;
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
        public int ReturnFeelings(Entity entity)
        {
            try
            {
                string faction = entity.GetComponent<Faction>().faction;
                Stats stats = entity.GetComponent<Stats>();
                Stats AIStats = this.entity.GetComponent<Stats>();
                if (!favoredEntities.Contains(faction) && hatedEntities.Contains(faction))
                {
                    if (AIStats.acuity < 0)
                    {
                        return baseInterest - stats.hp;
                    }
                    else if (AIStats.acuity < 3)
                    {
                        int baseInterest = this.baseInterest;
                        baseInterest -= stats.hp + (stats.strength * 5) + (stats.acuity * 5);
                        baseInterest -= AIStats.hp + (AIStats.strength * 5) + (AIStats.acuity * 5);
                        return baseInterest;
                    }
                    else
                    {

                        int baseInterest = this.baseInterest;
                        baseInterest -= stats.hp + (stats.strength * 3) + (stats.acuity * 3) + (stats.ac * 3);
                        baseInterest -= AIStats.hp + (AIStats.strength * 3) + (AIStats.acuity * 3) + (AIStats.ac * 3);
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
                        return baseInterest;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Add($"{ex.Message}");
                return 0;
            }
        }
        public AI() { }
    }
}
