using UnityEngine.InputSystem;

using SF.InputModule;
using SF.Weapons;

namespace SF.AbilityModule.CombatModule
{
    public class UseWeaponAbility : AbilityCore, IInputAbility
    {

        [UnityEngine.SerializeField] private WeaponBase _weaponBase;
        
        protected override void OnInitialize()
        {
            if (_controller2d == null || _weaponBase == null)
                return;

            _weaponBase.UseCompleted += OnUseCompleted;
            _weaponBase.Initialize(_controller2d);
        }

        /// <summary>
        /// This plays after the attack finishes. Use this to wait for the attack animation to finish.
        /// </summary>
        protected virtual void OnUseCompleted() { }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if(_weaponBase == null || !CanStartAbility())
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