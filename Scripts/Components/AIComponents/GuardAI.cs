using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class GuardAI: AI
    {
        public override void ExecuteAction()
        {
            switch (currentState)
            {
                case State.Fearful:
                    {
                        if (interest <= 0) 
                        {
                            interest = baseInterest;
                            currentInput = Input.Bored;
                            Log.Add("Korbold calms down.");
                            entity.GetComponent<TurnFunction>().EndTurn();
                        }
                        else
                        {
                            Log.Add("Korbold scared and running away! Interest:" + interest);
                            AIActions.MoveAwayFromTarget(entity, target);
                        }
                        break;
                    }
                case State.Asleep:
                    {
                        AIActions.TestSleep(entity);
                        break;
                    }
                case State.Patrolling:
                    {
                        AIActions.TestPatrol(entity);
                        break;
                    }
                case State.Angry:
                    {
                        AIActions.EngageEnemy(entity);
                        break;
                    }
            }
        }
        public override void SetTransitions()
        {
            transitions = new Dictionary<StateMachine, State>
            {
                { new StateMachine(State.Asleep, Input.Noise), State.Patrolling },
                { new StateMachine(State.Asleep, Input.Fear), State.Fearful },
                { new StateMachine(State.Asleep, Input.Hurt), State.Angry },
                { new StateMachine(State.Angry, Input.Tired), State.Asleep },
                { new StateMachine(State.Angry, Input.Fear), State.Fearful },
                { new StateMachine(State.Angry, Input.Bored), State.Patrolling },
                { new StateMachine(State.Patrolling, Input.Hatred), State.Angry },
                { new StateMachine(State.Patrolling, Input.Hurt), State.Angry },
                { new StateMachine(State.Patrolling, Input.Tired), State.Asleep },
                { new StateMachine(State.Patrolling, Input.Fear), State.Fearful },
                { new StateMachine(State.Fearful, Input.Hatred), State.Angry },
                { new StateMachine(State.Fearful, Input.Hurt), State.Angry },
                { new StateMachine(State.Fearful, Input.Bored), State.Patrolling },
                { new StateMachine(State.Fearful, Input.Tired), State.Asleep },
            };
            currentInput = Input.Bored;
            currentState = State.Patrolling;
        }
        public GuardAI(List<string> favoredEntities, List<string> hatedEntities, int baseInterest, int minDistance, int preferredDistance, int maxDistance, int abilityChance, int hate, int fear, int greed)
            : base(favoredEntities, hatedEntities, baseInterest, minDistance, preferredDistance, maxDistance, abilityChance, hate, fear, greed)
        { SetTransitions(); }
        public GuardAI()
        {

        }
    }
}
