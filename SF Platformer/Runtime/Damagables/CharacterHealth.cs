using SF.Characters.Controllers;
using SF.Characters;
using SF.DataManagement;
using SF.Events;
using UnityEngine;

namespace SF.SpawnModule
{
    public class CharacterHealth : Health
    {
        [Header("Animation Setting")]

        [Tooltip("If you want to force an animation state when this object is damaged than set this string to the name of the animation state.")]
        public const string HitAnimationName = "Damaged";
        public readonly int HitAnimationHash = Animator.StringToHash(HitAnimationName);

        public const string DeathAnimationName = "Death";
        public readonly int DeathAnimationHash = Animator.StringToHash(DeathAnimationName);

        public float HitAnimationDuration = 0.3f;

        protected Controller2D _controller;
        protected CharacterRenderer2D _character2D;

        protected virtual void Awake()
        {
            _controller = GetComponent<Controller2D>();
            _character2D = GetComponent<CharacterRenderer2D>();
        }

        protected override void Kill()
        {

            if(_controller != null)
                _controller.CharacterState.CharacterStatus = CharacterStatus.Dead;

            if(_character2D != null && !string.IsNullOrEmpty(DeathAnimationName))
                _character2D.SetAnimationState(DeathAnimationName);

            base.Kill();
        }

        public override void Respawn()
        {
            if(_controller != null)
            {
                _controller.Reset();
                _controller.CharacterState.CharacterStatus = CharacterStatus.Alive;
            }

            base.Respawn();
        }

        public override void TakeDamage(int damage, Vector2 knockback = new Vector2())
        {
            if(_character2D != null && !string.IsNullOrEmpty(HitAnimationName))
                _character2D.SetAnimationState(HitAnimationName, HitAnimationDuration);

            _controller.SetExternalVelocity(knockback);
            base.TakeDamage(damage);
        }
    }
}
