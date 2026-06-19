using SF.SpawnModule;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

namespace SF.StateMachine
{
    [BurstCompile]
    public class PatrolPointsPathfindingState : PathfindingStateBase
    {
        [SerializeField] private Transform[] _patrolPoints;
        private TransformHandle[] _patrolPointHandles;
        
        private int _patrolPointIndex = 0;
        private bool _isBackTracking;

        private static readonly ProfilerMarker FollowPathMarker = new ProfilerMarker("Patrol Point Follow Path");
        
        protected override void OnAwake()
        {
            if(_shouldChasePlayer)
                SpawnSystem.InitialPlayerSpawnHandler += OnPlayerSpawnedInitially;

            if (_patrolPoints.Length < 2)
                return;

            _patrolPointHandles = new TransformHandle[_patrolPoints.Length];
            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                _patrolPointHandles[i] = _patrolPoints[i].transformHandle;
            }
            
            if (_patrolPointHandles.Length > 0)
                _currentTarget = _patrolPointHandles[0];
        }
        
        protected override void OnUpdateState()
        {
            FollowPathMarker.Begin();
            base.OnUpdateState();
            FollowPathMarker.End();
            
            if (!WithinStoppingDistance(_controlledPosition, _currentTargetPos, _nodeRadius)) 
                return;
            
            if (!_isBackTracking && _patrolPointIndex + 1 >= _patrolPointHandles.Length)
            {
                _isBackTracking = true;
                _patrolPointIndex--;
            }
            else if (_isBackTracking && _patrolPointIndex <= 0)
            {
                _isBackTracking = false;
                _patrolPointIndex++;
            }
            else if (_isBackTracking)
                _patrolPointIndex--;
            else
                _patrolPointIndex++;

            _currentTarget = _patrolPointHandles[_patrolPointIndex];
            _currentTargetPos = _currentTarget.position;
        }

        [BurstCompile]
        protected static bool WithinStoppingDistance(
            in float3 currentPosition,
            in float3 currentPatrolPoint,
            in float stoppingDistance)
        {
            return math.distance(currentPosition, currentPatrolPoint) < stoppingDistance;
        }
    }
}
