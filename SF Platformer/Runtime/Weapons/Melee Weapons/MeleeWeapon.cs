using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace SF.Weapons
{
    public class MeleeWeapon : WeaponBase, IWeapon
    {
        [SerializeField] private Collider2D _hitBox;
        private List<Collider2D> _hitResults = new();

        private int _comboIndex = 0;
        private bool _onCooldown = false;
        [SerializeField] private float _attackDelayTimer = 0;

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

            _attackDelayTimer = ComboAttacks[_comboIndex].AttackTimer;
            _onCooldown = true;
        }

        private void Update()
        {
            if(_attackDelayTimer < 0.05)
                _onCooldown = false;
            else
            {
                _attackDelayTimer -= Time.deltaTime;
            }
        }
    }
}
