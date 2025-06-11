using System.Collections.Generic;
using SF.Characters;
using UnityEngine;
namespace SF.Weapons
{
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
        
        [SerializeField] private BoxCollider2D _hitBox;
        private List<Collider2D> _hitResults = new();

        [SerializeField] private int _comboIndex = 0;
        private Vector2 _originalColliderOffset;
        
        
        private void Awake()
        {
            _attackTimer = new Timer(ComboAttacks[0].AttackTimer, OnUseComplete);
            _hitBoxTimer = new Timer(ComboAttacks[0].HitBoxDelay, OnHitBoxDelay);
            _comboTimer = new Timer(ComboAttacks[0].ComboInputDelay, OnComboReset);
            
            
            if(_controller2D != null)
                _controller2D.OnDirectionChanged += OnDirectionChange;

                
            if (_hitBox != null)
            {
                _originalColliderOffset = _hitBox.offset;
            }
        }

        protected override void OnDirectionChange(object sender, Vector2 newDirection)
        {
            // Flip the weapons hitbox when switching direction.
            if (_hitBox != null && newDirection != Vector2.zero)
            {
                _hitBox.offset = _originalColliderOffset * newDirection.x;
            }
        }

        public override void Use()
        {
            if (OnCooldown)
                return;

            // Stop attack while dead attack while dead.
            if (_controller2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
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
        }

        private void SingleAttack()
        {
            if(_character2D != null)
                _character2D.SetAnimationState(
                    ComboAttacks[0].Name, 
                    ComboAttacks[0].AttackAnimationClip.averageDuration
                );
            
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
            _hitBox.Overlap(_hitBoxFilter, _hitResults);
            
            for(int i = 0; i < _hitResults.Count; i++)
            {
                if(_hitResults[i].TryGetComponent(out IDamagable damageable))
                {
                    damageable.TakeDamage(WeaponDamage,_knockBackForce);
                }
            }
        }
        
        private void OnUseComplete()
        {
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
            for (int i = 0; i < ComboAttacks.Count; i++)
            {
                ComboAttacks[i].AttackTimer = ComboAttacks[i].AttackAnimationClip.length;
            }
        }
        #endif
    }
}
