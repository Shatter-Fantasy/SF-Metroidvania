using System.Collections.Generic;
using UnityEngine;

namespace SF.StateMachine.Decisions
{
    using Core;

    /// <summary>
    /// Transitions are the classes that control the logic for when a state should change to another state.
    /// </summary>
    public class StateDecisionCore : MonoBehaviour
    {
        protected string _decisionName = "Transition";

        [Header("States")]
        [SerializeField] protected StateCore _trueState;
        [SerializeField] protected StateCore _falseState;

        [Space]
		[SerializeField] protected bool _canTransistToSelf = false;
		[SerializeField] private List<StateCore> _blockingStates = new();
		protected virtual void Awake() 
        {
            Init();
        }
        /// <summary>
        /// Anything we need to initialize goes here.
        /// </summary>
        protected virtual void Init()
		{

		}

        protected virtual void Start()
        {
            LateInit();
        }

        protected virtual void LateInit()
        {

        }
        /// <summary>
        /// This is where we check to see if a condition is met to change states.
        /// We return both a bool and state. Reason is if neither truth or false states 
        /// are to be transitioned too we need to tell the statemachine to not change state or will return null state and cause issues.
        /// </summary>
        /// <returns></returns>
        public virtual void CheckDecision(ref DecisionTransition decision, StateCore currentState)
		{

		}

        protected virtual bool CanTransist(StateCore currentState, StateCore newState)
        {
			if (newState == null)
				return false;

			if (_canTransistToSelf == false && currentState == newState)
				return false;
            

			foreach (StateCore state in _blockingStates)
			{
				if (currentState == state)
					return false;
			}

			return true;
		}
    }
}