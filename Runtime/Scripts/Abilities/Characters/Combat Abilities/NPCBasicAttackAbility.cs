using UnityEngine;

namespace SF.AbilityModule
{
    using Weapons;
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
