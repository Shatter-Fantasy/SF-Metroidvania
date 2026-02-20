using UnityEngine;

namespace SF.StateMachine
{
    using StateMachine.Core;
    using SpawnModule;
    public class ChaseAIState : StateCore
    {
        [SerializeField] private bool _chasePlayer;
        /* Note when using something like the distance decision we won't need to have the enemy change direction. when getting to close to the target, because a different state will be switched to most of the time. */
        [SerializeField] private Transform _target;
        private float _targetDirection; 
        
        /// <summary>
        /// Set the initial target in ONStart because the SpawnSystem should have already finished up spawning the player for the first time by now.
        /// </summary>
        protected override void OnStart()
        {
            _target = SpawnSystem.SpawnedPlayerController?.transform;
        }
        protected override void OnUpdateState()
        {
            if(!_chasePlayer 
               || _target == null ) 
                return;

            _targetDirection = Vector3.Cross(transform.position,_target.position).normalized.z;
            
            if(_targetDirection is 1 or -1)
                _controllerBody2D.SetDirection(_targetDirection);
        }
    }
}
