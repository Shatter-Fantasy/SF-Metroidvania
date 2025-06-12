#if DEBUG
using Unity.Profiling;
#endif

using UnityEngine;

using SF.Characters.Controllers;
using SF.Managers;


namespace SF.Characters
{
	/// <summary>
	/// Controls the character rendering for Sprites. This includes automatic animator set up and
	/// also includes systems tinting the sprite for vfx.
	/// </summary>
	[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class CharacterRenderer2D : MonoBehaviour
    {
#if DEBUG
		private static ProfilerMarker s_AnimationUpdateMarker = new(ProfilerCategory.Animation, "SF.Animation.Update" );
#endif
		public CharacterTypes CharacterType = CharacterTypes.Player;
		public CharacterState CharacterState => _controller?.CharacterState;

		public bool CanTurnAround = true;
		public bool StartedFacingRight = true;
		#region Common Components
		private SpriteRenderer _spriteRend;
		private Animator _animator;
		private Controller2D _controller;
		#endregion

		private int AnimationHash => Animator.StringToHash(CharacterState?.CurrentMovementState.ToString());
		[SerializeField] private int _forcedStateHash = 0;
		private float _animationFadeTime = 0;
		[SerializeField] private bool _isForcedCrossFading = false;
		[SerializeField] private int _lastAnimationHash;
		[SerializeField] private string _lastAnimationName;
		[SerializeField] private int _deathAnimationHash;
		#region Lifecycle Functions  
		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_spriteRend = GetComponent<SpriteRenderer>();
			_controller = GetComponent<Controller2D>();
			Init();
		}
		#endregion
		private void Init()
		{
			_deathAnimationHash = Animator.StringToHash(CharacterStatus.Dead.ToString());
			OnInit();
		}
		
		protected virtual void OnInit()
		{
			if(_controller) // Can happen when attached to an NPC not moving and just idle.
				_controller.OnDirectionChanged += OnDirectionChanged;

			// TODO: Make the attacking change the state the animation state to attacking. 
			//_controller.CharacterState.OnMovementStateChanged += UpdateAnimator;
		}

		private void LateUpdate()
		{
			UpdateAnimator(); 
		}
		private void UpdateAnimator()
		{
			SetAnimations();
		}
        /// <summary>
		/// The Movement State is calculated in the Physics Controller.
        /// <see cref="GroundedController2D.CalculateMovementState"/>
        /// </summary>
        private void SetAnimations()
        {
	        
	        if(_animator == null 
	           || _animator.runtimeAnimatorController == null)
		        return;

	        if (_controller?.CharacterState.CharacterStatus == CharacterStatus.Dead)
	        {
		        _animator.Play(_deathAnimationHash,0);
		        return;
	        }

			
			if(_isForcedCrossFading)
			{
				_animationFadeTime -= Time.deltaTime;
				if(_animationFadeTime < 0)
					_isForcedCrossFading = false;
				return;
			}

			if(_forcedStateHash != 0)
			{
				if (_animator.HasState(0, _forcedStateHash))
				{
					if(_animationFadeTime > 0)
						_isForcedCrossFading = true;
					
					_animator.CrossFadeInFixedTime(_forcedStateHash, _animationFadeTime,0);
					_lastAnimationHash = _forcedStateHash;
					_forcedStateHash = 0;
				}
				else
				{
					// If no state was found or set up in the animator just ignore the forced state.
					_isForcedCrossFading = false;
					_forcedStateHash = 0;
				}
			}
			else if(_animator.HasState(0, AnimationHash)
					&& _lastAnimationHash != AnimationHash
					)
			{
				_animator.CrossFadeInFixedTime(AnimationHash, 0,0);
				_lastAnimationHash = AnimationHash;
			}
        }

        // The 0.3f is the default fade time for Unity's crossfade api.
        public void SetAnimationState(string stateName, float animationFadeTime = 0.01f)
		{
			_forcedStateHash = Animator.StringToHash(stateName);
            _animationFadeTime = animationFadeTime;
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
}