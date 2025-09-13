using SF.Characters.Controllers;
using SF.StateMachine.Core;
using SF.Weapons;
using UnityEngine;

namespace SF.StateMachine
{
    public class SingleAttackState : StateCore
    {

        [SerializeField, SerializeReference] private WeaponBase _weapon;
        
        protected override void OnInit(Controller2D controller2D)
        {
            base.OnInit(controller2D);

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
            _controller.FreezeController();
        }

        protected void OnUseCompleted()
        {
            _controller.UnfreezeController();
            StateBrain.ChangeStateWithCheck(StateBrain.PreviousState);
        }
    }
}
