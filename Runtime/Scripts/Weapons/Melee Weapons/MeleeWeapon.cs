using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Weapons
{
    using Characters;
    using CombatModule;
    using DamageModule;
    using PhysicsLowLevel;
    
    public class MeleeWeapon : WeaponBase, IWeapon
    {
        public AttackDefinition AttackDefinition;
        
        /// <summary>
        /// The delay before the hit box is enabled. This allows for matching damage with the animation visuals.
        /// </summary>
        [Header("Timer Settings")]
        [SerializeField] protected Timer _hitBoxTimer;
        
        
        [Header("Hit Box")]
        [SerializeField] protected SFShapeComponent _hitBox;
        private readonly List<PhysicsShape> _hitResults = new();
        
        protected Vector2 _originalColliderOffset;
        private Vector2 _facingDirection;
        
        protected virtual void Awake()
        {
            
        }

        protected override void OnDirectionChange(object sender, Vector2 newDirection)
        {
            _facingDirection = newDirection;
        }

        public override void Use()
        {
            if (_hitBox == null || !_hitBox.Shape.isValid)
                return;
            
            if (OnCooldown)
                return;
            
            // Stop attack while dead attack while dead.
            if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
                return;

            _hitBox.Body.SetAndWriteTransform(new PhysicsTransform(transform.position,PhysicsRotate.identity));
            
            DoAttack();
            _character2D.CharacterState.AttackState = AttackState.Attacking;
        }

        protected virtual void DoAttack()
        {
            if (_character2D != null && !_character2D.UseAnimatorTransitions)
            {
                _character2D.SetAnimationState(
                    AttackDefinition.Name,
                    AttackDefinition.AttackAnimationClip.length);
            }

            _ = _hitBoxTimer.StartTimerAsync();
            _ = _attackTimer.StartTimerAsync();

            OnCooldown = true;
        }

        /// <summary>
        /// Finishes a timed delay before activating the hit box for weapons to do hit box checks and apply damage.
        /// Allows for syncing visual animations with the hit box better to make combat feel more accurate. 
        /// </summary>
        protected virtual void OnHitBoxDelay()
        {
            var world  = _hitBox.World;
            using var result = world.OverlapShape(_hitBox.Shape, _filter);

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].shape.callbackTarget is SFShapeComponent shapeComponent 
                    && shapeComponent.TryGetComponent(out IDamagable damageable))
                {
                    damageable.TakeDamage(WeaponDamage,_knockBackForce);
                }
            }
        }
        
        protected virtual void OnUseComplete()
        {
            _controllerBody2D.CharacterState.AttackState = AttackState.NotAttacking;
            _hitBoxTimer.StopTimer();
            _attackTimer.StopTimer();
            UseCompleted?.Invoke();
            OnCooldown = false;
        }

       
        
        #if UNITY_EDITOR
        /// <summary>
        /// Syncs all the attack timers to match the length of the animation clip length for that attack animation.
        /// </summary>
        [ContextMenu("Sync attack and animation timers.")]
        protected virtual void SetAllAttacksTimerViaAnimation()
        {
            var clip = AttackDefinition.AttackAnimationClip;
            
            float targetFrameTimer = (AttackDefinition.HitBoxAnimationFrame + 1) * (float)Math.Round((1f / clip.frameRate), 3);
            AttackDefinition.AttackTimer = clip.length;
            AttackDefinition.HitBoxDelay = targetFrameTimer;
        }
        #endif
    }
}
