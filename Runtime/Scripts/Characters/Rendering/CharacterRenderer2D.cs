using UnityEngine;

namespace SF.Characters
{
	using Managers;
	using PhysicsLowLevel;
	using Weapons;
	
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
		protected SpriteRenderer _spriteRend;
		public Animator Animator;
		/// <summary>
		/// The runtime animator for <see cref="Animator"/>.
		/// This is used to update animation clips at runtime for forced states.
		/// </summary>
		protected RuntimeAnimatorController _runtimeAnimator;
		protected ControllerBody2D _controllerBody2D;
		#endregion
		
		protected int MovementAnimationHash => Animator.StringToHash(CharacterState?.CurrentMovementState.ToString());
		[SerializeField] protected int _forcedStateHash = 0;
		[SerializeField] protected int _lastAnimationHash;
		
		protected static readonly int XSpeedAnimationHash = Animator.StringToHash("XSpeed");
		protected static readonly int IsGroundedAnimationHash = Animator.StringToHash("IsGrounded");
		protected static readonly int IsJumpingAnimationHash = Animator.StringToHash("IsJumping");
		protected static readonly int IsFallingAnimationHash = Animator.StringToHash("IsFalling");
		protected static readonly int DeathAnimationHash = Animator.StringToHash(nameof(CharacterStatus.Dead));
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
		protected void Init()
		{
			AnimatorParameters = Animator.parameters;
			OnInit();
		}
		
		protected virtual void OnInit()
		{
			if (_controllerBody2D == null)
				return;
			
			_controllerBody2D.OnDirectionChanged += OnDirectionChanged;
			// TODO: Make the attacking change the state the animation state to attacking. 
			_controllerBody2D.CharacterState.AttackStateChangedHandler += OnAttackStateChanged;
		}

		protected void OnAttackStateChanged(AttackState attackState)
		{
			//Plays the Attack Substate 
			Animator.Play(_forcedStateHash,0);
		}

		protected void LateUpdate()
		{
			if (UseAnimatorTransitions)
				UpdateAnimatorParameters();
		}

		protected virtual void UpdateAnimatorParameters()
		{
			if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
			{
				Animator.Play(DeathAnimationHash,0);
				return;
			}
			
			if (_controllerBody2D?.CharacterState.AttackState != AttackState.NotAttacking)
				return;
			
			if (_controllerBody2D is null)
				return;
			
			/* All Controller2D have the next set of parameters*/
			Animator.SetFloat(XSpeedAnimationHash, Mathf.Abs(_controllerBody2D.Direction.x));

			// Grounded States
			Animator.SetBool(IsGroundedAnimationHash, _controllerBody2D.CollisionInfo.IsGrounded);
			
			// Jump/Air States
			Animator.SetBool(IsJumpingAnimationHash, _controllerBody2D.IsJumping);
			Animator.SetBool(IsFallingAnimationHash, _controllerBody2D.IsFalling);
		}
        
        // The 0.3f is the default fade time for Unity's crossfade api.
        public void SetAnimationState(string stateName, float animationFadeTime = 0.01f)
        {
			_forcedStateHash = Animator.StringToHash(stateName);
        }
		protected void SpriteFlip(Vector2 direction)
		{
			if(!CanTurnAround || _spriteRend == null)
				return;

            _spriteRend.flipX = StartedFacingRight
				? (!(direction.x > 0))
				: (!(direction.x < 0));
        }

		protected void OnDirectionChanged(object sender, Vector2 direction)
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