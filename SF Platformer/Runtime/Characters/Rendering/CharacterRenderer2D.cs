#if DEBUG
using Unity.Profiling;
#endif

using UnityEngine;

using SF.Characters.Controllers;


namespace SF.Characters
{
	[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class CharacterRenderer2D : MonoBehaviour
    {
#if DEBUG
		private static ProfilerMarker s_AnimationUpdateMarker = new(ProfilerCategory.Animation, "SF.Animation.Update" );
#endif
		public enum CharacterTypes { Player, Ally, Enemy, NPC}
		public CharacterTypes CharacterType = CharacterTypes.Player;
		public CharacterState CharacterState => _controller.CharacterState;

		public bool CanTurnAround = true;
		public bool StartedFacingRight = true;
		#region Common Components
		private SpriteRenderer _spriteRend;
		private Animator _animator;
		private Controller2D _controller;
		#endregion

		private int AnimationHash => Animator.StringToHash(CharacterState.MovementState.ToString());
		private int _forcedStateHash = 0;
		private float _animationFadeTime = 0;
		private bool _isForcedCrossFading = false;
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
			OnInit();
		}
		
		protected virtual void OnInit()
		{
            _controller.OnDirectionChanged += OnDirectionChanged;
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
#if DEBUG
            s_AnimationUpdateMarker.Begin();
#endif
			if(_isForcedCrossFading)
			{
				_animationFadeTime -= Time.deltaTime;
				if(_animationFadeTime < 0)
					_isForcedCrossFading = false;
                return;
			}

			if(_forcedStateHash != 0 && _animator.HasState(0, _forcedStateHash))
			{
				_isForcedCrossFading = true;
                _animator.CrossFade(_forcedStateHash, _animationFadeTime,0);
				_forcedStateHash = 0;
			}
			else if(_animator.HasState(0, AnimationHash) 
					&& _controller.CharacterState.CharacterStatus != CharacterStatus.Dead)
			{
				_animator.CrossFade(AnimationHash, 0,0);
			}
#if DEBUG
            s_AnimationUpdateMarker.End();
#endif
        }

        // The 0.3f is the default fade time for Unity's crossfade api.
        public void SetAnimationState(string stateName, float animationFadeTime = 0.3f)
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
			if(direction.x == 0)
				return;

			SpriteFlip(direction);
		}
	}
}