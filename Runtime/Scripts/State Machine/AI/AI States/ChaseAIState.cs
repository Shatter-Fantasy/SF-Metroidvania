using SF.Characters.Controllers;
using SF.LevelModule;
using SF.Managers;
using SF.PhysicsLowLevel;
using SF.SpawnModule;
using UnityEngine;

using SF.StateMachine.Core;

namespace SF.StateMachine
{
    public class ChaseAIState : StateCore
    {
        [SerializeField] private bool _chasePlayer;
        /* Note when using something like the distance decision we won't need to have the enemy change direction. when getting to close to the target, because a different state will be switched to most of the time. */
        [SerializeField] private Transform _target;
        private float _targetDirection; 
        protected override void OnInit(ControllerBody2D controllerBody2D)
        {
            base.OnInit(controllerBody2D);
            if (_chasePlayer)
            {
                _target = SpawnSystem.SpawnedPlayerController.transform;
            }
        }

        protected override void OnUpdateState()
        {
            if(_target == null)
                return;

            _targetDirection = Vector3.Cross(transform.position,_target.position).normalized.z;
            
            if(_targetDirection == 1 ||  _targetDirection == -1)
                _controllerBody2D.SetDirection(_targetDirection);
        }
    }
}
