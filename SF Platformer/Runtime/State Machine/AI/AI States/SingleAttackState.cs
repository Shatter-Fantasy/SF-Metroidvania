using SF.Characters;
using SF.Characters.Controllers;
using SF.StateMachine.Core;
using SF.StateMachine.Decisions;
using SF.Weapons;
using UnityEngine;

namespace SF.StateMachine
{
    public class SingleAttackState : StateCore
    {
        private CharacterRenderer2D _renderer2D;
        [SerializeField, SerializeReference] private WeaponBase _weapon;
        
        protected override void OnInit(Controller2D controller2D)
        {
            base.OnInit(controller2D);
            _renderer2D = StateBrain.ControlledGameObject.GetComponent<CharacterRenderer2D>();

            if (_weapon == null)
                _weapon = GetComponent<WeaponBase>();

            if (_weapon != null)
                _weapon.UseCompleted += OnUseCompleted;
        }

        protected override void OnUpdateState()
        {
            if (_weapon == null)
                return;
            
            _controller.FreezeController();
            _weapon.Use();
        }

        protected void OnUseCompleted()
        {
            _controller.UnfreezeController();
            StateBrain.ChangeStateWithCheck(StateBrain.PreviousState);
        }
    }
}
