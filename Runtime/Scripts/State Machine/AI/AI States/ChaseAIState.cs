using SF.Characters.Controllers;
using SF.LevelModule;
using SF.Managers;
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
        protected override void OnInit(RigidbodyController2D rigidbodyController2D)
        {
            base.OnInit(rigidbodyController2D);
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
                _rigidbodyController.SetDirection(_targetDirection);
        }
    }
}
