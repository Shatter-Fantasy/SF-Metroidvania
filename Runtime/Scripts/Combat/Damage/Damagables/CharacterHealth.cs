using System;
using UnityEngine;

namespace SF.SpawnModule
{
    using Characters;
    using CommandModule;
    using PhysicsLowLevel;
    
    /*  TODO: Merge this with the CombatantHealth.cs
        and remove this script from the package.    */
    
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
        
        protected ControllerBody2D _controllerBody2D;
        protected CharacterRenderer2D _character2D;

        public Action CharacterDiedHandler;
        
        protected virtual void Awake()
        {
            _controllerBody2D = GetComponent<ControllerBody2D>();
            _character2D = GetComponent<CharacterRenderer2D>();
            _invicibilityTimer = new Timer(_invicibilityTimer.Duration,OnInvicibilityTimerCompleted);
        }

        protected override void Kill()
        {

            if(_controllerBody2D != null)
                _controllerBody2D.CharacterState.CharacterStatus = CharacterStatus.Dead;

            if(_character2D != null && !string.IsNullOrEmpty(DeathAnimationName))
                _character2D.SetAnimationState(DeathAnimationName,0.01f);
            
            DamageBlink.StopInteruptBlinking();
            
            base.Kill();
            CharacterDiedHandler?.Invoke();
        }

        public override void Respawn()
        {
            if(_controllerBody2D != null)
            {
                // _controllerBody2D.Reset(); Might want to continue using a reset for respawning. Not sure yet.
                _controllerBody2D.CharacterState.CharacterStatus = CharacterStatus.Alive;
            }

            base.Respawn();
        }

        public override void TakeDamage(int damage, Vector2 knockback = new Vector2())
        {
           
            if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead || _activeInvicibility)
                return;
            
            if(_character2D != null && !string.IsNullOrEmpty(HitAnimationName))
                _character2D.SetAnimationState(HitAnimationName, HitAnimationDuration);

            base.TakeDamage(damage);
            _ = DamageBlink.Use();
            
            _controllerBody2D?.SetDirectionalForce(knockback);
            
            _activeInvicibility = true;
            _ = _invicibilityTimer.StartTimerAsync();

        }

        protected void OnInvicibilityTimerCompleted()
        {
            _activeInvicibility = false;
        }
    }
}
