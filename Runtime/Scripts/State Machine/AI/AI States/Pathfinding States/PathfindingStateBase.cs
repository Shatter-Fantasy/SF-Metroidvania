using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

namespace SF.StateMachine
{
    using SF.Pathfinding;
    using SF.SpawnModule;
    
    [BurstCompile]
    public class PathfindingStateBase : StateCore
    {
        /// <summary>
        /// Does the <see cref="_currentTarget"/> get automatically set to the player during Awake.
        /// </summary>
        [SerializeField] protected bool _shouldChasePlayer;
        [SerializeField] protected float _speed = 5;
        [SerializeField] protected Transform _target;
        protected TransformHandle _currentTarget;
        protected TransformHandle _controlledTransform;
        
        /// <summary>
        /// The target position the follower is moving to. This can be the next way point or a player if they are within range.
        /// </summary>
        protected float3 _currentTargetPos;
        protected float3 _currentWayPoint;
        protected float3 _controlledPosition;
        
        protected int _targetIndex = 0;
        protected Awaitable _followPathAwaitable;
        
        /// <summary>
        /// The grid with the path we are following.
        /// </summary>
        [Header("Pathfinding")]
        protected GridBase _grid;
        //protected Vector2[] _path;
        protected NativeList<float2> _path;
        protected float _nodeRadius = 0.5f;
       
        protected bool _isFollowingTarget;
        
        protected override void OnAwake()
        {
            if(_shouldChasePlayer)
                SpawnSystem.InitialPlayerSpawnHandler += OnPlayerSpawnedInitially;
        }

        protected void OnPlayerSpawnedInitially(GameObject playerController)
        {
            _currentTarget = playerController.transformHandle;
            _currentTargetPos = _currentTarget.position;
        }
        

        protected override void OnStart()
        {
            _controlledTransform = StateBrain.ControlledGameObject.transformHandle;
            StartPath();
        }
        
        protected override void OnUpdateState()
        {
            _controlledPosition = _controlledTransform.position;
            FollowPathAsync();
        }

        protected override void OnStateExit()
        {
            _isFollowingTarget = false;
        }

        protected virtual void StartPath()
        {
            try
            {
                _targetIndex = 0;
                _currentTargetPos = _currentTarget.position;

                _isFollowingTarget = true;
                
                if (PathRequestManager._instance?.PathFinding?.GridPath != null)
                {
                    _grid = PathRequestManager._instance.PathFinding.GridPath;
                    _nodeRadius = _grid.NodeRadius;
                }
            }
            catch (Exception e)
            {
                Debug.LogAssertion($"Pathfinding ran into the following exception: {e}",gameObject);
            }
        }
        
        protected async void FollowPathAsync()
        {
            if (math.distance(_currentTargetPos, _controlledPosition) < _nodeRadius)
            {
                _targetIndex++;
                
                // Reached the end of the path. If following a player target this means we reached them.
                if (!_path.IsCreated && _targetIndex >= _path.Length)
                    return;
            }
            else
            {
                _path = await PathRequestManager._instance.PathFinding
                                                .FindPathAwaitable(_controlledPosition.xy, _currentTargetPos.xy);
            }
            
            // Set the current waypoint as the current node position based on the current target index in the path array.
            if (_path.IsCreated)
            {
                if (_targetIndex < _path.Length)
                {
                    var wayPoint = _path[_targetIndex];
                    _currentWayPoint = new float3(wayPoint.x, wayPoint.y, 0);
                }
            }
            
            _controlledTransform.position= Vector3.MoveTowards(_controlledTransform.position, _currentTargetPos, _speed * Time.deltaTime);
        }
    }
}
