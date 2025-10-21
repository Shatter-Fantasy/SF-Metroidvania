using System.Collections.Generic;
using System.Linq;
using SF.Characters;
using SF.Characters.Controllers;
using SF.StateMachine.Decisions;

using UnityEngine;

namespace SF.StateMachine.Core
{
	public class DecisionTransition
	{
		public bool CanTransist;
		public StateCore StateGoingTo;
	}

	/// <summary>
	/// <seealso cref="StateMachineBrain"/>
	/// </summary>
	public abstract class StateCore : MonoBehaviour
    {
		public string StateName { get; protected set; } = "Idle";

		/// <summary>
		/// This is to allow setting states to enter themselves on loop.
		/// </summary>
		public bool CanStateTransistToSelf = false;

		[Tooltip("You can add states in this list so certain states can't enter this state during a transition check.")]
		public List<StateCore> NonEnterableStates = new();

		public List<StateDecisionCore> Decisions = new();
		[HideInInspector] public StateMachineBrain StateBrain;

		protected DecisionTransition _decision = new();
		protected bool _initialized = false;

        protected RigidbodyController2D _rigidbodyController;

        private void Awake()
		{
			Init();
			OnAwake();
		}
		protected virtual void OnAwake()
		{
		}

        private void Start()
		{
			OnStart();
		}
        
        protected virtual void OnStart()
		{

		}
		/// <summary>
		/// This is run the very first time this object is enabled and when the StateMachineBrain inits it.
		/// This won't run everytime this state is interacted with.
		/// </summary>
		public void Init(RigidbodyController2D rigidbodyController2D = null)
		{
			if(!_initialized)
				_initialized = true;

			if(rigidbodyController2D == null)
				OnInit();
			else
			{
				_rigidbodyController = rigidbodyController2D;
                OnInit(rigidbodyController2D);
			}
		}

		/// <summary>
		/// This function can be overridden to give sub class of State Core custom Init functionality.
		/// </summary>
		protected virtual void OnInit()
		{

		}

        protected virtual void OnInit(RigidbodyController2D rigidbodyController2D)
        {

        }

        /// <summary>
        /// This is where the state machine calls the start of the states update.
        /// </summary>
        public void UpdateState()
		{
			
			CheckTransitions();

			
			if (_rigidbodyController == null && _rigidbodyController?.CharacterState.CharacterStatus == CharacterStatus.Dead)
			{
				return;
			}
			OnUpdateState();
		}

		/// <summary>
		/// We do per frame update logic here. This is controlled by the state machine brain on the object.
		/// </summary>
		protected virtual void OnUpdateState(){}

        /// <summary>
        /// This function is called from the <see cref="StateMachineBrain"/> to set up calling the OnEnterState function for methods.
        /// </summary>
        public void EnterState()
		{
			OnStateEnter();
		}
		/// <summary>
		/// When we enter the state we do prep stuf here
		/// if we need to prep something every state entrance
		/// </summary>
		protected virtual void OnStateEnter()
		{

		}
		/// <summary>
		/// This function is called from the <see cref="StateMachineBrain"/> to set up calling the OnExitState function for methods.
		/// </summary>
		public void ExitState()
		{
			OnStateExit();
		}
		/// <summary>
		/// When we leave the state use this function to do clean up actions.
		/// </summary>
		protected virtual void OnStateExit()
		{

		}
		public virtual bool CheckEnterableState(StateCore currentState)
		{
			if (_rigidbodyController == null
			    || _rigidbodyController.CharacterState.CharacterStatus == CharacterStatus.Dead)
			{
				return false;
			}
			
			if(!NonEnterableStates.Any())
				return true;
			// If there is a blocking state check if the state we are in
			// blocks the new state trying to be entered.
			return NonEnterableStates.Any(stateToCheck =>
			{
				return stateToCheck != null;
			});
		}
		protected virtual void CheckTransitions()
		{
			Decisions.ForEach(decision => 
			{
				decision.CheckDecision(ref _decision, StateBrain.CurrentState);
				if(_decision.CanTransist)
				{
					StateBrain.ChangeStateWithCheck(_decision.StateGoingTo);
				}
			});
		}
		/// <summary>
		/// When the state is enabled we can do set up work here.
		/// </summary>
		protected virtual void OnEnable(){}

		/// <summary>
		/// When the state is disabled we can do clean up work here.
		/// </summary>
		protected virtual void OnDisable(){ }
	}
}
