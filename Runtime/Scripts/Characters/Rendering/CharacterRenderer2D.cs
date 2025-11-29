using UnityEngine;

using SF.Managers;
using SF.PhysicsLowLevel;

namespace SF.Characters
{
	/// <summary>
	/// Controls the character rendering for Sprites. This includes automatic animator set up and
	/// also includes systems tinting the sprite for vfx.
	/// </summary>
	[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class CharacterRenderer2D : MonoBehaviour
    {
	    public bool UseAnimatorTransitions;
		public CharacterTypes CharacterType = CharacterTypes.Player;
		public CharacterState CharacterState => _controllerBody2D?.CharacterState;

		public bool CanTurnAround = true;
		public bool StartedFacingRight = true;
		#region Common Components
		private SpriteRenderer _spriteRend;
		public Animator Animator;
		/// <summary>
		/// The runtime animator for <see cref="Animator"/>.
		/// This is used to update animation clips at runtime for forced states.
		/// </summary>
		private RuntimeAnimatorController _runtimeAnimator;
		private ControllerBody2D _controllerBody2D;
		#endregion
		
		private int MovementAnimationHash => Animator.StringToHash(CharacterState?.CurrentMovementState.ToString());
		[SerializeField] private int _forcedStateHash = 0;
		[SerializeField] private float _animationFadeTime = 0;
		[SerializeField] private int _lastAnimationHash;
		[SerializeField] private string _lastAnimationName;
		
		private static readonly int DeathAnimationHash = Animator.StringToHash(nameof(CharacterStatus.Dead));
		private static readonly int AttackingStateHash = Animator.StringToHash( nameof(MovementState.Attacking));
		//[SerializeField] private bool _hasForcedState;

		public AnimatorControllerParameter[] AnimatorParameters;
		#region Lifecycle Functions  
		private void Awake()
		{
			Animator = GetComponent<Animator>();
			_spriteRend = GetComponent<SpriteRenderer>();
			_controllerBody2D = GetComponent<ControllerBody2D>();
			Init();
		}
		#endregion
		private void Init()
		{
			AnimatorParameters = Animator.parameters;
			OnInit();
		}
		
		protected virtual void OnInit()
		{
			if (_controllerBody2D)
				_controllerBody2D.OnDirectionChanged += OnDirectionChanged;

			// TODO: Make the attacking change the state the animation state to attacking. 
			//_controllerBody2D.CharacterState.OnMovementStateChanged += UpdateAnimator;
		}

		private void LateUpdate()
		{
			if (UseAnimatorTransitions)
				UpdateAnimatorParameters();
			else
				SetAnimations();
		}

		private void UpdateAnimatorParameters()
		{
			
			if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
			{
				Animator.Play(DeathAnimationHash,0);
				return;
			}
			
			
			if (_controllerBody2D?.CharacterState.CurrentMovementState == MovementState.Attacking)
			{
				Animator.Play(AttackingStateHash,0);
				return;
			}
			
			if (_controllerBody2D is null)
				return;
			
			/* All Controller2D have the next set of parameters*/
			
			if (_controllerBody2D?.CharacterState.CurrentMovementState == MovementState.Attacking)
				Animator.SetTrigger("Attacking");
			
			Animator.SetFloat("XSpeed", Mathf.Abs(_controllerBody2D.Direction.x));


			// Grounded States
			Animator.SetBool("IsGrounded", _controllerBody2D.CollisionInfo.IsGrounded);
			Animator.SetBool("IsCrouching", _controllerBody2D.IsCrouching);
			
			// Jump/Air States
			Animator.SetBool("IsJumping", _controllerBody2D.IsJumping);
			Animator.SetBool("IsFalling", _controllerBody2D.IsFalling);
			Animator.SetBool("IsGliding", _controllerBody2D.IsGliding);
		}
		
        /// <summary>
		/// The Movement State is calculated in the Physics Controller.
        /// <see cref="GroundedController2D.CalculateMovementState"/>
        /// </summary>
        private void SetAnimations()
        {
	        if(Animator == null 
	           || Animator.runtimeAnimatorController == null)
		        return;
	        
	        if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
	        {
		        Animator.Play(DeathAnimationHash,0);
		        return;
	        }
	       
	        MovementCrossfade();
        }

        /// <summary>
        /// Plays a crossfade to an animation state that is being forced on the controller preventing other non-forced state animations from playing. 
        /// </summary>
        private void ForcedCrossFade()
        {
	        // If the forced animation finished clear the forced animation state.
	        if(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
	        {
		        _forcedStateHash = 0;
		        //_hasForcedState = false;
		        _lastAnimationHash = _forcedStateHash;
		        return;
	        }
	        
	        // Make sure we are not resetting to the first frame of a forced animation state that is already being played.
	        if (_lastAnimationHash != _forcedStateHash &&  Animator.HasState(0, _forcedStateHash))
	        {
		        Animator.CrossFadeInFixedTime(_forcedStateHash, 0,0);
		        _lastAnimationHash = _forcedStateHash;
	        }
        }

        private void MovementCrossfade()
        {
	        // Don't reset the animation for lopping movement animations.
	        if (!Animator.HasState(0, MovementAnimationHash)
	            || _lastAnimationHash == MovementAnimationHash
	           )
		        return;
	        
	        Animator.CrossFadeInFixedTime(MovementAnimationHash, 0,0);
	        _lastAnimationHash = MovementAnimationHash;
        }
        
        // The 0.3f is the default fade time for Unity's crossfade api.
        public void SetAnimationState(string stateName, float animationFadeTime = 0.01f)
        {
			_forcedStateHash = Animator.StringToHash(stateName);
            _animationFadeTime = animationFadeTime;
            //_hasForcedState = true;
        }
		private void SpriteFlip(Vector2 direction)
		{
			if(!CanTurnAround || _spriteRend == null)
				return;

            _spriteRend.flipX = StartedFacingRight
				? (!(direction.x > 0))
				: (!(direction.x < 0));
        }

		private void OnDirectionChanged(object sender, Vector2 direction)
		{
			if(direction.x == 0 || GameManager.Instance.ControlState != GameControlState.Player)
				return;

			SpriteFlip(direction);
		}
	}
	
	public static class AnimationUtilities
	{
		public static bool HasParameter(this Animator anim, string paramName)
		{
			foreach (AnimatorControllerParameter param in anim.parameters)
			{
				if (param.name == paramName) return true;
			}
			return false;
		}
	}
}