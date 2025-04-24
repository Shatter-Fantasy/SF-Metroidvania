using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace SF.Weapons
{
    public class MeleeWeapon : WeaponBase, IWeapon
    {
        [SerializeField] private BoxCollider2D _hitBox;
        private List<Collider2D> _hitResults = new();

        private int _comboIndex = 0;
        private bool _onCooldown = false;

        private void Awake()
        {
            _attackTimer = new Timer(ComboAttacks[0].AttackTimer, OnUseComplete);
            _controller2D.OnDirectionChanged += OnDirectionChange;
        }

        private void OnDirectionChange(object sender, Vector2 newDirection)
        {
            if (_hitBox != null && newDirection != Vector2.zero)
                transform.localScale = new Vector3(newDirection.x,1,1);
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

            _hitBox.Overlap(_hitBoxFilter, _hitResults);

            for(int i = 0; i < _hitResults.Count; i++)
            {
                if(_hitResults[i].TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(WeaponDamage,_knockBackForce);
                }
            }

            // TODO: We need to do a check for combo timers.
            //_attackDelayTimer = ComboAttacks[_comboIndex].AttackTimer;
            _attackTimer.StartTimerAsync();
            _onCooldown = true;
        }


        private void OnUseComplete()
        {
            UseCompleted?.Invoke();
            _onCooldown = false;
        }
    }
}
