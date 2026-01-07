using UnityEngine;

using SF.Characters;
using SF.Managers;
using SF.PhysicsLowLevel;

namespace SF.AbilityModule
{
	/// <summary>
	/// Abilities contain the data for what actions can do and how they do them.
	/// </summary>
    public abstract class AbilityCore : MonoBehaviour, IAbility
    {
		[Header("Blocking States")]
		public MovementState BlockingMovementStates;
		public CharacterStatus BlockingCharacterStatus = CharacterStatus.Dead;

		protected bool _isInitialized;		

		protected ControllerBody2D _controller2d;
		protected bool _isPerformingAbility;

		public void Initialize(PhysicController2D physicController2D)
		{
			if (_isInitialized)
				return;
			
			if (physicController2D is ControllerBody2D controllerBody2D)
				_controller2d = controllerBody2D;
			
			OnInitialize();
			_isInitialized = true;
		}
		
		
		/// <summary>
		/// Overload this to do initialization for abilities.
		/// By this point the Controller2D class is safe to reference and use for event registering.
		/// </summary>
		protected virtual void OnInitialize()
		{
			
		}
		public void PreUpdate() 
		{ 
			OnPreUpdate();
		}
		protected virtual void OnPreUpdate()
		{

		}
		public void UpdateAbility() 
		{
			OnUpdate();
		}
		protected virtual void OnUpdate()
		{

		}
		public void PostUpdate() 
		{
			OnPostUpdate();
		}
		protected virtual void OnPostUpdate()
		{

		}

		protected virtual void OnAbilityInterruption()
		{
			
		}

		/// <summary>
		/// Is there any state for the controller, character or ability that blocks the start of the ability.
		/// </summary>
		/// <returns></returns>
		protected bool CanStartAbility()
		{
			
			if (!_isInitialized
			    || !enabled
			    || _controller2d == null
			    || GameManager.Instance.ControlState != GameControlState.Player)
			{
				return false;
			}

			// If we are in a blocking movement state or blocking movement status don't start ability.
            if((_controller2d.CharacterState.CurrentMovementState & BlockingMovementStates) > 0
                || (_controller2d.CharacterState.CharacterStatus == CharacterStatus.Dead))
					return false;

            bool doAbility = CheckAbilityRequirements();
            // Ability is being interrupted for some reason.
            if (_isPerformingAbility && !doAbility)
            {
	            OnAbilityInterruption();
	            _isPerformingAbility = false;
            }

            return doAbility;
		}

		/// <summary>
		///		Override this to create custom ability checking to make sure the ability can actually be used.
		/// </summary>
		/// <returns></returns>
		protected virtual bool CheckAbilityRequirements()
		{
			return true;
		}
	}
}
