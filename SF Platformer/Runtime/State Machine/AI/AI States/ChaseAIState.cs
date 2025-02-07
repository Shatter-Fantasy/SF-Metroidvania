using UnityEngine;

using SF.StateMachine.Core;

namespace SF.StateMachine
{
    public class ChaseAIState : StateCore
    {
        /* Note when using something like the distance decision we won't need to have the enemy change direction. when getting to close to the target, because a different state will be switched to most of the time. */
        [SerializeField] private Transform _target;

        protected override void OnUpdateState()
        {
            if(_target == null)
                return;

            var targetDirection = Vector3.Cross(transform.position, _target.position).normalized.z;

            if(targetDirection == 1 ||  targetDirection == -1)
                _controller.SetDirection(targetDirection);
        }
    }
}
