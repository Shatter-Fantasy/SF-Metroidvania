using UnityEngine.InputSystem;

using SF.AbilityModule;
using SF.InputModule;
using SF.Weapons;

namespace SF.Abilities.CombatModule
{
    public class UseWeaponAbility : AbilityCore, IInputAbility
    {

        [UnityEngine.SerializeField] private WeaponBase _weaponBase;

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if(_weaponBase == null)
                return;
            
            if(!CanStartAbility()) 
                return;

            _weaponBase.Use();
        }
        
        private void OnEnable()
        {
            InputManager.Controls.Player.Enable();
            InputManager.Controls.Player.Attack.performed += OnAttackPerformed;
        }

        private void OnDisable()
        {
            if(InputManager.Instance == null) return;

            InputManager.Controls.Player.Attack.performed -= OnAttackPerformed;
        }
    }
}