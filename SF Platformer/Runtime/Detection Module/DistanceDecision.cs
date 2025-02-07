using SF.StateMachine.Core;

using UnityEngine;

namespace SF.StateMachine.Decisions
{
    public class DistanceDecision : StateDecisionCore
    {
        [SerializeField] private float _distance = 3.5f;
        [SerializeField] private float calculatedDistance = 3.5f;
        [SerializeField] private Transform _target;

        public override void CheckDecision(ref DecisionTransition decision, StateCore currentState)
        {
            if(_target == null || 
                (_trueState == null && _falseState == null))
            {
                decision.CanTransist = false;
                return;
            }

            calculatedDistance = Vector3.Distance(transform.position, _target.position);

            // If the target is within the distance
            if(_trueState != null && calculatedDistance < _distance)
            {

                decision.CanTransist = true;
                decision.StateGoingTo = _trueState;
                return;
            }
            else if(_falseState != null && calculatedDistance > _distance)
            {

                decision.CanTransist = true;
                decision.StateGoingTo = _falseState;
                return;
            }

            decision.CanTransist = false;
        }
    }
}
