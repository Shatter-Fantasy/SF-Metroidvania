using SF.Weapons;

using UnityEngine;

namespace SF.AbilityModule.CombatModule
{
    public class NPCBasicAttackAbility : MonoBehaviour
    {
        [SerializeField] private WeaponBase _weaponBase;

        public void Attack()
        {
            if(_weaponBase == null)
                return;

            _weaponBase.Use();
        }
    }
}
