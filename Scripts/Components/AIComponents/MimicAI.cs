using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class MimicAI : AI
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
                        AIActions.TestMimicWait(entity);
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
                { new StateMachine(State.Asleep, Input.Noise), State.Awake },
                { new StateMachine(State.Asleep, Input.Hurt), State.Angry },
                { new StateMachine(State.Angry, Input.Tired), State.Asleep },
                { new StateMachine(State.Awake, Input.Hatred), State.Angry },
                { new StateMachine(State.Awake, Input.Hurt), State.Angry },
                { new StateMachine(State.Angry, Input.Bored), State.Awake },
                { new StateMachine(State.Awake, Input.Tired), State.Asleep },
            };
        }
        public MimicAI(List<string> favoredEntities, List<string> hatedEntities, int baseInterest, int minDistance, int preferredDistance, int maxDistance, int abilityChance, int hate, int fear, int greed)
            : base(favoredEntities, hatedEntities, baseInterest, minDistance, preferredDistance, maxDistance, abilityChance, hate, fear, greed)
        { SetTransitions(); currentState = State.Awake; }
        public MimicAI()
        {

        }
    }
}
