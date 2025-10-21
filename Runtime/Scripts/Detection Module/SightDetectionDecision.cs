using SF.Characters.Controllers;
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
        [SerializeField] private ContactFilter2D _detectionFilter;

        private readonly RaycastHit2D[] _filteredHits = new RaycastHit2D[1];
        private RigidbodyController2D _rigidbodyController2D;
        
        protected override void Init()
        {
            if (TryGetComponent(out StateMachineBrain brain)
                && brain.ControlledGameObject.TryGetComponent(out _rigidbodyController2D))
            {
                // This is empty on purpose. The second TryGetComponent assigns the _controller2D value for this decision.
                return;
            }
        }

        public override void CheckDecision(ref DecisionTransition decision, StateCore currentState)
        {
            if(Physics2D.Raycast(transform.position, _rigidbodyController2D.Direction, _detectionFilter,_filteredHits,_sightDistance) > 0)
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
