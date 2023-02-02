using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    class DragonAI : AI
    {
        public override void ExecuteAction()
        {
            switch (currentState)
            {
                case State.Curious:
                    {
                        entity.GetComponent<TurnFunction>().EndTurn();
                        break;
                    }
                case State.Bored:
                    {
                        entity.GetComponent<TurnFunction>().EndTurn();
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
                { new StateMachine(State.Bored, Input.Noise), State.Curious },
                { new StateMachine(State.Angry, Input.Bored), State.Bored },
                { new StateMachine(State.Curious, Input.Bored), State.Bored },
                { new StateMachine(State.Curious, Input.Hurt), State.Angry },
                { new StateMachine(State.Bored, Input.Hurt), State.Angry },
                { new StateMachine(State.Curious, Input.Hatred), State.Angry },
                { new StateMachine(State.Bored, Input.Hatred), State.Angry },
            };
        }
        public DragonAI(List<string> favored, List<string> hated, int _baseInterest, int _minRange, int _preferredRange, int _maxRange, int _abilityChance)
        {
            favoredEntities = favored;
            hatedEntities = hated;
            currentState = State.Angry;
            interest = _baseInterest;
            baseInterest = _baseInterest;
            minDistance = _minRange;
            preferredDistance = _preferredRange;
            abilityChance = _abilityChance;
            maxDistance = _maxRange;
            SetTransitions();
        }
        public DragonAI()
        {

        }
    }
}
