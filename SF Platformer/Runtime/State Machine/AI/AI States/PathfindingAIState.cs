using SF.Characters.Controllers;
using SF.Managers;
using SF.Pathfinding;
using SF.StateMachine.Decisions;
using UnityEngine;

namespace SF.StateMachine.Core
{
    public class PathfindingAIState : StateCore
    {
	    [SerializeField] private float _speed = 5;
        [SerializeField] private bool _chasePlayer;
        [SerializeField] private Transform _target;
        private Transform _controlledTransform;
        
        /// <summary>
        /// The grid with the path we are following.
        /// </summary>
        private GridBase _grid;
        private Vector2[] _path;
        private float _nodeRadius = 0.5f;
        
        /// <summary>
        /// The target position the follower is moving to. This can be the next way point or a player if they are within range.
        /// </summary>
        private Vector2 _currentTargetPos;
	    private Vector2 _currentWayPoint;
	    private int _targetIndex = 0;
        private Awaitable _followPathAwaitable;
        private bool _followingTarget;
        
        protected override void OnInit()
        {
            base.OnInit();
            if (_chasePlayer)
            {
                _target = GameManager.Instance.PlayerController.transform;
                _currentTargetPos = _target.position;
            }

            if (TryGetComponent(out DistanceDecision distance))
            {
	            distance.Distance = _nodeRadius;
	            if (_target != null)
		            distance.Target = _target;
            }
            
            _controlledTransform = StateBrain.ControlledGameObject.transform;
        }

        protected override void OnStart()
        {
            StartPath();
        }

        private async void StartPath()
        {
	        _targetIndex = 0;
	        _currentTargetPos = _target.position;
			
            _path = await PathRequetManager._instance.PathFinding.FindPathAwaitable(_controlledTransform.position, _target.position);
            
            _followingTarget = true;
            
            //_followPathAwaitable = FollowPathAsync();
            
            if(PathRequetManager._instance?.PathFinding?.GridPath != null)
            {
                _grid = PathRequetManager._instance.PathFinding.GridPath;
                _nodeRadius = _grid.NodeRadius;
            }
            
            //await _followPathAwaitable;
        }

        protected override void OnUpdateState()
        {
	        FollowPath();
        }

        private async void FollowPath()
        {
	        if (!_followingTarget)
		        return;
		        
	        if (Vector2.Distance(_currentTargetPos, _target.position) > _nodeRadius)
	        {
		        _path = await PathRequetManager._instance.PathFinding.FindPathAwaitable(_controlledTransform.position,
			        _target.position);

		        // If when updating the path we realized the player moved into the same node 
		        // as the path follower just return and stop looping.
		        if (_path == null || _path.Length < 1)
		        {
			        _currentWayPoint = _target.position;
		        }
		        else
			        _currentWayPoint = _path[0];

		        _targetIndex = 0;
		        _currentTargetPos = _target.position;
	        }
	        else if (Vector2.Distance(_controlledTransform.position, _currentWayPoint) < _nodeRadius)
	        {
		        _targetIndex++;
		        
		        // Reached the end of the path. If following a player target this means we reached them.
		        if (_targetIndex >= _path?.Length)
		        {
			        _path = null;
			        return;
		        }
	        }

	        // Set the current waypoint as the current node position based on the current target index in the path array.
	        if (_path != null && _targetIndex < _path.Length)
		        _currentWayPoint = _path[_targetIndex];
	        
	        _controlledTransform.position= Vector2.MoveTowards(_controlledTransform.position, _currentTargetPos, _speed * Time.deltaTime);
        }
        
        private async Awaitable FollowPathAsync()
        {
        	if(_path.Length < 1)
        	{
        		Debug.Log("There is no path yet.");
        		return;
        	}
	        
	        while (_followingTarget)
	        {
		        if (Vector2.Distance(_currentTargetPos, _target.position) > _nodeRadius)
		        {
			        _path = await PathRequetManager._instance.PathFinding.FindPathAwaitable(_controlledTransform.position,
				        _target.position);

			        // If when updating the path we realized the player moved into the same node 
			        // as the path follower just return and stop looping.
			        if (_path == null || _path.Length < 1)
			        {
				        _currentWayPoint = _target.position;
			        }
			        else
				        _currentWayPoint = _path[0];

			        _targetIndex = 0;
			        _currentTargetPos = _target.position;
		        }
		        else if (Vector2.Distance(_controlledTransform.position, _currentWayPoint) < _nodeRadius)
		        {
			        _targetIndex++;
			        
			        // Reached the end of the path. If following a player target this means we reached them.
			        if (_targetIndex >= _path?.Length)
			        {
				        _path = null;
				        return;
			        }
		        }

		        // Set the current waypoint as the current node position based on the current target index in the path array.
		        if (_path != null && _targetIndex < _path.Length)
			        _currentWayPoint = _path[_targetIndex];
		        
		        _controlledTransform.position= Vector2.MoveTowards(_controlledTransform.position, _currentTargetPos, _speed * Time.deltaTime);
		        await Awaitable.EndOfFrameAsync();
	        }
        }

        protected override void OnStateExit()
        {
	        _followingTarget = false;
        }
    }
}
