using System.Collections.Generic;

using UnityEngine;
namespace SF.Weapons
{
    public class MeleeWeapon : WeaponBase, IWeapon
    {
        /// <summary>
        /// The delay before the hit box is enabled. This allows for matching damage with the animation visuals.
        /// </summary>
        [SerializeField] protected Timer _hitBoxTimer;
        
        [SerializeField] private BoxCollider2D _hitBox;
        private List<Collider2D> _hitResults = new();

        //private int _comboIndex = 0;
        private bool _onCooldown = false;
        private Vector2 _originalColliderOffset;
        
        
        private void Awake()
        {
            _attackTimer = new Timer(ComboAttacks[0].AttackTimer, OnUseComplete);
            _hitBoxTimer = new Timer(ComboAttacks[0].HitBoxDelay, OnHitBoxDelay);
            _controller2D.OnDirectionChanged += OnDirectionChange;

            if (_hitBox != null)
            {
                _originalColliderOffset = _hitBox.offset;
            }
        }

        private void OnDirectionChange(object sender, Vector2 newDirection)
        {
            // Flip the weapons hitbox when switching direction.
            if (_hitBox != null && newDirection != Vector2.zero)
            {
                _hitBox.offset = _originalColliderOffset * newDirection.x;
            }
        }

        public override void Use()
        {
            if(_onCooldown)
                return;
            
            if(_character2D != null)
                _character2D.SetAnimationState(
                    ComboAttacks[0].Name, 
                    ComboAttacks[0].AttackAnimationClip.averageDuration
                );
            
            _ = _hitBoxTimer.StartTimerAsync();
            _ = _attackTimer.StartTimerAsync();
            
            
            // TODO: We need to do a check for combo timers.
            //_attackDelayTimer = ComboAttacks[_comboIndex].AttackTimer;
            

            _onCooldown = true;
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
            _onCooldown = false;
        }
    }
}
