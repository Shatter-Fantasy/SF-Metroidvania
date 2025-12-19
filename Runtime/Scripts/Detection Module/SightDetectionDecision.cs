using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.StateMachine.Decisions
{
    using PhysicsLowLevel;
    using Core;
    
    public enum SightShapeType
    {
        Box, Line, Arc
    }
    public class SightDetectionDecision : StateDecisionCore
    {
        [SerializeField] private float _sightDistance = 4;
        [SerializeField] private PhysicsQuery.CastRayInput _rayInput;
        [SerializeField] private PhysicsQuery.QueryFilter _queryFilter;
        
        private ControllerBody2D _controllerBody2D;
        
        protected override void Init()
        {
            if (TryGetComponent(out StateMachineBrain brain)
                && brain.ControlledGameObject.TryGetComponent(out _controllerBody2D))
            {
                // This is empty on purpose. The second TryGetComponent assigns the _controller2D value for this decision.
                return;
            }
        }

        public override void CheckDecision(ref DecisionTransition decision,StateCore currentState)
        {
            ref PhysicsShape   shape = ref _controllerBody2D.ShapeComponent._shape;
            PhysicsWorld world = _controllerBody2D.ShapeComponent._shape.world;
            if (!shape.isValid)
                return;

            // shape.transform = PhysicsTransform type not Transform type.
            _rayInput.origin      = shape.transform.position;
            _rayInput.translation = _controllerBody2D.Direction * _sightDistance;
            
            using var castResults = world.CastRay(_rayInput,_queryFilter);

            if (castResults.Length > 0)
            {
                decision.CanTransist  = true;
                decision.StateGoingTo = _trueState;
            }
            else
            {
                decision.CanTransist  = true;
                decision.StateGoingTo = _falseState;
            }
        }
    }
}
