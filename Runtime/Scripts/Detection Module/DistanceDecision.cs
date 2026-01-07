using SF.SpawnModule;
using SF.StateMachine.Core;

using UnityEngine;
using UnityEngine.Serialization;

namespace SF.StateMachine.Decisions
{
    public class DistanceDecision : StateDecisionCore
    {
        [FormerlySerializedAs("_distance")]
        public float Distance = 3.5f;
        [FormerlySerializedAs("calculatedDistance")]
        [SerializeField] private float _calculatedDistance = 3.5f;
        [FormerlySerializedAs("_target")] public Transform Target;
        
        [SerializeField] private bool _chasePlayer;

        protected override void LateInit()
        {
            if (_chasePlayer)
            {
                Target = SpawnSystem.SpawnedPlayerController?.transform;
            }
        }
        
        public override void CheckDecision(ref DecisionTransition decision, StateCore currentState)
        {
            if(Target == null || 
                (_trueState == null && _falseState == null))
            {
                decision.CanTransist = false;
                return;
            }

            _calculatedDistance = Vector3.Distance(transform.position, Target.position);

            // If the target is within the distance
            if(_trueState != null && _calculatedDistance < Distance)
            {
                decision.CanTransist = true;
                decision.StateGoingTo = _trueState;
                return;
            }
            else if(_falseState != null && _calculatedDistance > Distance)
            {
                Debug.Log($"Going the state of: {_falseState}");
                decision.CanTransist = true;
                decision.StateGoingTo = _falseState;
                return;
            }

            decision.CanTransist = false;
        }
    }
}
