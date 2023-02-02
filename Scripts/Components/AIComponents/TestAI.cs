using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class TestAI: AI
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
                case State.Awake:
                    {
                        AIActions.TestAwake(entity);
                        break;
                    }
                case State.Angry:
                    {
                        AIActions.TestHuntAction(entity);
                        break;
                    }
                case State.Bored:
                    {
                        entity.GetComponent<TurnFunction>().EndTurn();
                        break;
                    }
            }
        }
        public override void SetTransitions()
        {
            transitions = new Dictionary<StateMachine, State>
            {
                { new StateMachine(State.Asleep, Input.Noise), State.Awake },
                { new StateMachine(State.Asleep, Input.Hurt), State.Angry },
                { new StateMachine(State.Angry, Input.Tired), State.Asleep },
                { new StateMachine(State.Awake, Input.Hatred), State.Angry },
                { new StateMachine(State.Awake, Input.Bored), State.Bored },
                { new StateMachine(State.Angry, Input.Bored), State.Bored },
                { new StateMachine(State.Bored, Input.Tired), State.Asleep },
                { new StateMachine(State.Bored, Input.Hatred), State.Angry }
            };
        }
        public TestAI(List<string> favored, List<string> hated, int _baseInterest)
        {
            favoredEntities = favored;
            hatedEntities = hated;
            currentState = State.Asleep;
            interest = _baseInterest;
            baseInterest = _baseInterest;
            SetTransitions();
        }
        public TestAI()
        {

        }
    }
}
