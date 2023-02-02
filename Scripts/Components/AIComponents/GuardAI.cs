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
                        AIActions.TestHuntAction(entity);
                        break;
                    }
            }
        }
        public override void SetTransitions()
        {
            transitions = new Dictionary<StateMachine, State>
            {
                { new StateMachine(State.Asleep, Input.Noise), State.Patrolling },
                { new StateMachine(State.Asleep, Input.Hurt), State.Angry },
                { new StateMachine(State.Angry, Input.Tired), State.Asleep },
                { new StateMachine(State.Patrolling, Input.Hatred), State.Angry },
                { new StateMachine(State.Patrolling, Input.Hurt), State.Angry },
                { new StateMachine(State.Angry, Input.Bored), State.Patrolling },
                { new StateMachine(State.Patrolling, Input.Tired), State.Asleep },
                { new StateMachine(State.Bored, Input.Hatred), State.Angry }
            };
        }
        public GuardAI(List<string> favored, List<string> hated, int _baseInterest)
        {
            favoredEntities = favored;
            hatedEntities = hated;
            currentState = State.Patrolling;
            interest = _baseInterest;
            baseInterest = _baseInterest;
            SetTransitions();
        }
        public GuardAI()
        {

        }
    }
}
