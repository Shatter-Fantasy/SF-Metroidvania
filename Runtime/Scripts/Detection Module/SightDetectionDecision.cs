using SF.StateMachine.Core;

using UnityEngine;

namespace SF.StateMachine.Decisions
{
    public enum SightShapeType
    {
        Box, Line, Arc
    }
    public class SightDetectionDecision : StateDecisionCore
    {
        [SerializeField] private float _sightDistance = 4;
        [SerializeField] private LayerMask _detectionLayer;


        public override void CheckDecision(ref DecisionTransition decision, StateCore currentState)
        {
           if(Physics2D.Raycast(transform.position, Vector2.right, _sightDistance, _detectionLayer))
           {
                decision.CanTransist = true;
                decision.StateGoingTo = _trueState;
           }
           else
            {
                decision.CanTransist = true;
                decision.StateGoingTo = _falseState;
            }
        }
    }
}
