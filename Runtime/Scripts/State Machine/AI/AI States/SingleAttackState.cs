using SF.Characters.Controllers;
using SF.StateMachine.Core;
using SF.Weapons;
using UnityEngine;

namespace SF.StateMachine
{
    public class SingleAttackState : StateCore
    {

        [SerializeField, SerializeReference] private WeaponBase _weapon;
        
        protected override void OnInit(RigidbodyController2D rigidbodyController2D)
        {
            base.OnInit(rigidbodyController2D);

            if (_weapon == null)
                _weapon = GetComponent<WeaponBase>();

            if (_weapon != null)
                _weapon.UseCompleted += OnUseCompleted;
        }

        protected override void OnStateEnter()
        {
            if (_weapon == null)
                return;
            
            _weapon.Use();
            _rigidbodyController.FreezeController();
        }

        protected void OnUseCompleted()
        {
            _rigidbodyController.UnfreezeController();
            StateBrain.ChangeStateWithCheck(StateBrain.PreviousState);
        }
    }
}
