using SF.Characters.Controllers;
using SF.Characters;
using SF.CommandModule;
using SF.DataManagement;
using SF.Events;
using UnityEngine;

namespace SF.SpawnModule
{
    public class CharacterHealth : Health
    {
        [SerializeField] protected Timer _invicibilityTimer;
        /// <summary>
        /// This is the boolean to see if the character is currently not damageable during the invicibility frames.
        /// </summary>
        [SerializeField] protected bool _activeInvicibility;
        
        [Header("Animation Setting")]
        [Tooltip("If you want to force an animation state when this object is damaged than set this string to the name of the animation state.")]
        public const string HitAnimationName = "Damaged";
        public readonly int HitAnimationHash = Animator.StringToHash(HitAnimationName);

        public const string DeathAnimationName = "Death";
        public readonly int DeathAnimationHash = Animator.StringToHash(DeathAnimationName);

        public float HitAnimationDuration = 0.3f;
        
        public SpriteBlinkCommand DamageBlink;
        
        protected RigidbodyController2D _rigidbodyController;
        protected CharacterRenderer2D _character2D;

        protected virtual void Awake()
        {
            _rigidbodyController = GetComponent<RigidbodyController2D>();
            _character2D = GetComponent<CharacterRenderer2D>();
            _invicibilityTimer = new Timer(_invicibilityTimer.Duration,OnInvicibilityTimerCompleted);
        }

        protected override void Kill()
        {

            if(_rigidbodyController != null)
                _rigidbodyController.CharacterState.CharacterStatus = CharacterStatus.Dead;

            if(_character2D != null && !string.IsNullOrEmpty(DeathAnimationName))
                _character2D.SetAnimationState(DeathAnimationName,0.01f);
            
            DamageBlink.StopInteruptBlinking();
            
            base.Kill();
        }

        public override void Respawn()
        {
            if(_rigidbodyController != null)
            {
                _rigidbodyController.Reset();
                _rigidbodyController.CharacterState.CharacterStatus = CharacterStatus.Alive;
            }

            base.Respawn();
        }

        public override void TakeDamage(int damage, Vector2 knockback = new Vector2())
        {
           
            if (_rigidbodyController?.CharacterState.CharacterStatus == CharacterStatus.Dead || _activeInvicibility)
                return;
            
            if(_character2D != null && !string.IsNullOrEmpty(HitAnimationName))
                _character2D.SetAnimationState(HitAnimationName, HitAnimationDuration);

            base.TakeDamage(damage);
            _ = DamageBlink.Use();
            
            _rigidbodyController?.SetDirectionalForce(knockback);
            
            _activeInvicibility = true;
            _ = _invicibilityTimer.StartTimerAsync();

        }

        protected void OnInvicibilityTimerCompleted()
        {
            _activeInvicibility = false;
        }
    }
}
