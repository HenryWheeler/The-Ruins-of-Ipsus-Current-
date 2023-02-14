using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class ElementalAI : AI
    {
        public override void ExecuteAction()
        {
            switch (currentState)
            {
                case State.Curious:
                    {
                        AIActions.TestHuntAction(entity);
                        break;
                    }
                case State.Bored:
                    {
                        AIActions.TestHuntAction(entity);
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
                { new StateMachine(State.Bored, Input.Noise), State.Curious },
                { new StateMachine(State.Angry, Input.Bored), State.Bored },
                { new StateMachine(State.Curious, Input.Bored), State.Bored },
                { new StateMachine(State.Curious, Input.Hurt), State.Angry },
                { new StateMachine(State.Bored, Input.Hurt), State.Angry },
                { new StateMachine(State.Curious, Input.Hatred), State.Angry },
                { new StateMachine(State.Bored, Input.Hatred), State.Angry },
            };
        }
        public ElementalAI(List<string> favoredEntities, List<string> hatedEntities, int baseInterest, int minDistance, int preferredDistance, int maxDistance, int abilityChance, int hate, int fear, int greed)
            : base(favoredEntities, hatedEntities, baseInterest, minDistance, preferredDistance, maxDistance, abilityChance, hate, fear, greed)
        { SetTransitions(); currentState = State.Bored; }
        public ElementalAI()
        {

        }
    }
}
