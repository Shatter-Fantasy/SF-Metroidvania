using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Weapons
{
    using Characters;
    using DamageModule;
    using PhysicsLowLevel;
    
    public class MeleeWeapon : WeaponBase, IWeapon
    {
        /// <summary>
        /// The delay before the hit box is enabled. This allows for matching damage with the animation visuals.
        /// </summary>
        [SerializeField] protected Timer _hitBoxTimer;
        /// <summary>
        /// The timer to keep track of time between combo attacks to see if a combo should continue if not a lot of time has passed.
        /// </summary>
        [SerializeField] protected Timer _comboTimer;
        
        [SerializeField] private SFShapeComponent _hitBox;
        private readonly List<PhysicsShape> _hitResults = new();

        [SerializeField] private int _comboIndex = 0;
        private Vector2 _originalColliderOffset;
        
        private void Awake()
        {
            _attackTimer = new Timer(ComboAttacks[0].AttackTimer, OnUseComplete);
            _hitBoxTimer = new Timer(ComboAttacks[0].HitBoxDelay, OnHitBoxDelay);
            _comboTimer = new Timer(ComboAttacks[0].ComboInputDelay, OnComboReset);
            
            
            if(_controllerBody2D != null)
                _controllerBody2D.OnDirectionChanged += OnDirectionChange;

                
            if (_hitBox != null)
            {
                _originalColliderOffset = _hitBox.transform.localPosition;
            }
        }

        protected override void OnDirectionChange(object sender, Vector2 newDirection)
        {
            // Flip the weapons hitbox when switching direction.
            if (_hitBox != null && newDirection != Vector2.zero)
            {
                _hitBox.transform.localPosition = _originalColliderOffset * newDirection.x;
            }
        }

        public override void Use()
        {
            if (OnCooldown)
                return;

            // Stop attack while dead attack while dead.
            if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
                return;
            
            // If we have a combo enabled for the current weapon do it.
            if (ComboAttacks.Count > 1)
            {
                ComboAttack();
            }
            else
            {
                SingleAttack();
            }
            _character2D.CharacterState.AttackState = AttackState.Attacking;
        }

        private void SingleAttack()
        {
            if (_character2D != null && !_character2D.UseAnimatorTransitions)
            {
                _character2D.SetAnimationState(
                    ComboAttacks[0].Name,
                    ComboAttacks[0].AttackAnimationClip.length);
            }

            _ = _hitBoxTimer.StartTimerAsync();
            _ = _attackTimer.StartTimerAsync();

            OnCooldown = true;
        }
        private void ComboAttack()
        {
            if(_character2D != null)
                _character2D.SetAnimationState(
                    ComboAttacks[_comboIndex].Name, 
                    ComboAttacks[_comboIndex].AttackAnimationClip.length
                );
            
            _attackTimer = new Timer(ComboAttacks[_comboIndex].AttackTimer, OnUseComplete);
            _hitBoxTimer = new Timer(ComboAttacks[_comboIndex].HitBoxDelay, OnHitBoxDelay);
            
            // Stop the previous combo timer.
            _comboTimer.StopTimer();
            _comboTimer = new Timer(ComboAttacks[_comboIndex].ComboInputDelay, OnComboReset);
            _ = _hitBoxTimer.StartTimerAsync();
            _ = _attackTimer.StartTimerAsync();
            _ = _comboTimer.StartTimerAsync();

            _comboIndex++;
            
            if (_comboIndex >= ComboAttacks.Count)
                _comboIndex = 0;
            OnCooldown = true;
        }

        /// <summary>
        /// Finishes a timed delay before activating the hit box for weapons to do hit box checks and apply damage.
        /// Allows for syncing visual animations with the hit box better to make combat feel more accurate. 
        /// </summary>
        private void OnHitBoxDelay()
        {
            /*
            _hitBox.Overlap(_hitBoxFilter, _hitResults);
            
            for(int i = 0; i < _hitResults.Count; i++)
            {
                if(_hitResults[i].TryGetComponent(out IDamagable damageable))
                {
                    damageable.TakeDamage(WeaponDamage,_knockBackForce);
                }
            }
            */
        }
        
        private void OnUseComplete()
        {
            _controllerBody2D.CharacterState.AttackState = AttackState.NotAttacking;
            _comboTimer.StopTimer();
            _hitBoxTimer.StopTimer();
            _attackTimer.StopTimer();
            UseCompleted?.Invoke();
            OnCooldown = false;
        }

        private void OnComboReset()
        {
            _comboIndex = 0;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Syncs all the attack timers to match the length of the animation clip length for that attack animation.
        /// </summary>
        [ContextMenu("Sync attack and animation timers.")]
        void SetAllAttacksTimerViaAnimation()
        {
            AnimationClip clip;
            
            for (int i = 0; i < ComboAttacks.Count; i++)
            {
                clip = ComboAttacks[i].AttackAnimationClip;
                
                float targetFrameTimer = (ComboAttacks[i].HitBoxAnimationFrame + 1) * (float)Math.Round((1f / clip.frameRate), 3);
                ComboAttacks[i].AttackTimer = clip.length;
                ComboAttacks[i].HitBoxDelay = targetFrameTimer;
            }
        }
        #endif
    }
}
