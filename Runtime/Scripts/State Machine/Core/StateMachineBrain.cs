using System.Collections.Generic;
using System.Linq;

using SF.Characters.Controllers;

using UnityEngine;

namespace SF.StateMachine.Core
{
	
	/// <summary>
	/// This is for controlling non-player controlled characters states and actions.
	/// 
	/// <see cref="SF.StateMachine.Core.StateCore"/> are for non-player characters
	/// and <see cref="SF.AbilityModule.AbilityCore"/> are for player controlled characters.
	/// </summary>
	public class StateMachineBrain : MonoBehaviour
    {
	    [field: SerializeField] public StateCore DefaultState { get; protected set; }
        [field: SerializeField] public StateCore CurrentState { get; protected set; }
        [field: SerializeField] public StateCore PreviousState { get;protected set; }
		[Tooltip("This is the game object that the newState machine brain is controlling.")]
		
        public GameObject ControlledGameObject;
        protected List<StateCore> _states = new();

        protected RigidbodyController2D _rigidbodyController2D;
		private void Awake()
		{
            _states.Clear();
            _states = GetComponentsInChildren<StateCore>().ToList();

            if(ControlledGameObject != null)
                _rigidbodyController2D = ControlledGameObject.GetComponent<RigidbodyController2D>();
            else
                _rigidbodyController2D = GetComponent<RigidbodyController2D>();

            if(!_states.Any()) return;
            
            foreach(StateCore state in _states)
            {
                state.StateBrain = this;
                state.Init(_rigidbodyController2D);
            }
		}

		private void Start()
		{
			InitStateBrain();
		}
		private void Update()
		{
            UpdateState();
		}
        /// <summary>
        /// We run the newState logic of the current active newState if the current newState is not null.
        /// </summary>
        private void UpdateState()
		{
            if (CurrentState == null)
                return;

            CurrentState.UpdateState();
		}

		public void InitStateBrain()
		{
			if (DefaultState != null)
				CurrentState = DefaultState;
			        
			if(CurrentState == null)
				CurrentState = _states.First();

			// Don't do first Enter State in awake or you might call it before the _states init.
			if(CurrentState != null)
				CurrentState.EnterState();
		}
		
        /// <summary>
        /// Changes the current newState and runs the enter and
        /// exit functions for the appropriate states without doing transition 
        /// or decision newState checks. Due note this is called normally after 
        /// newState checks, but can be used outside of them to directly bypass them.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(StateCore state)
		{
            if (state == null)
                return;

            if(CurrentState != null)
			{
                PreviousState = CurrentState;
                CurrentState.ExitState();
			}

            CurrentState = state;
            CurrentState.EnterState();
		}
		/// <summary>
		/// Changes the current newState and runs the enter and 
        /// exit functions for the appropriate states after checking
        /// if the newState trying to be changed to can be entered.
		/// </summary>
		/// <param name="newState"></param>
		public void ChangeStateWithCheck(StateCore newState)
        {
			if(newState == null)
				return;

            if(newState == CurrentState && !CurrentState.CanStateTransistToSelf)
                return;

            if(newState.CheckEnterableState(CurrentState))
                ChangeState(newState);
		}
    }
}