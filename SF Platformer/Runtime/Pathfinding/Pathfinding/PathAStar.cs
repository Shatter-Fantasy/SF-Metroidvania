using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SF.Pathfinding
{

    /* A* Pathfinding pseudo code
     * 
     * public List<PathNodeBase> OpenNodes; // The nodes to be evaluated
     * public List<PathNodeBase> ClosedNodes; // The nodes already been evaluated
     * 
     * public PathNodeBase CurrentNode; // The current node being evaluated.
     * public PathNodeBase GoalNode;
     * 
     * PathFinding Code
     * 
     * add the starting node to the OpenNodes list.
     * 
     *      loop
     *      {
     *          CurrentNode = node in open node with the lowest F_cost.
     *          remove the CurrentNode from the OpenNode list
     *          Add CurrentNode to the ClosedNode list.
     *          
     *          if(CurrentNode == GoalNode)
     *              return;
     *              
     *          foreach NeighborNode of the CurrentNode
     *          {
     *              if(NeighborNode is not traversable or NeighborNode is in the ClosedNode list)
     *              skip to the next NeighborNode for checks.
     *              
     *              if new path to NeighboreNode is shorter OR is not in OpenNodes list
     *                  set FCost of NeighborNode
     *                  set parent of NeighberNode to CurrentNode
     
     *                  if(NeigberNode is not in OpenNodes)
     *                      Add NeighberNode to OpenNodes
     *          }
     *          
     *       }
     */
    [RequireComponent(typeof(PathRequetManager))]
    public class PathAStar : MonoBehaviour
    {
        private PathRequetManager _pathRequestManager;
        [field: SerializeField] public GridBase GridPath { get; private set; }

        private PathNodeBase _startNode;
        private PathNodeBase _goalNode;

        private void Awake()
        {
            _pathRequestManager = GetComponent<PathRequetManager>();
            if(GridPath == null)
                GridPath = GetComponent<GridBase>();
        }

        public void StartFindPath(Vector2 startPos, Vector2 targetPos)
        {
            FindPath(startPos,targetPos);
        }

        private async void FindPath(Vector2 startPotion, Vector2 goalPosition)
        {
            if(GridPath == null)
                return;

            Vector2[] wayPoints = new Vector2[0];
            bool pathSuccess = false;

            _startNode = GridPath.NodeFromWorldPoint(startPotion);
            _goalNode = GridPath.NodeFromWorldPoint(goalPosition);

            // If either nodes are not traveserable don't bother finding a path.
            if(!_startNode.IsTraversable || !_goalNode.IsTraversable)
                return;

            Heap<PathNodeBase> _openNodes = new Heap<PathNodeBase>(GridPath.MaxSize);
            HashSet<PathNodeBase> _closedNodes = new();

            _openNodes.Add(_startNode);

            while(_openNodes.Count > 0)
            {
                PathNodeBase currentNode = _openNodes.RemoveFirst();

                _closedNodes.Add(currentNode);

                // We found our goal node.
                if(currentNode == _goalNode)
                {
                    pathSuccess = true;
                    break;
                }


                foreach(PathNodeBase neighbourNode in GridPath.GetNeighbours(currentNode))
                {
                    if(!neighbourNode.IsTraversable || _closedNodes.Contains(neighbourNode))
                        continue;

                    float newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode);

                    if(newMovementCostToNeighbour < neighbourNode.GCost || !_openNodes.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourNode, _goalNode);
                        neighbourNode.ParentNodeOnPath = currentNode;

                        if(!_openNodes.Contains(neighbourNode))
                            _openNodes.Add(neighbourNode);
                        else
                            _openNodes.UpdateItem(neighbourNode);
                    }
                }
            }
            await Awaitable.EndOfFrameAsync();
            
            if(pathSuccess)
                wayPoints = RetracePath(_startNode, _goalNode);

            _pathRequestManager.FinishedProcessingPath(wayPoints,pathSuccess);
        }

        public async Awaitable<Vector2[]> FindPathAwaitable(Vector2 startPotion, Vector2 goalPosition)
        {
            if(GridPath == null)
                return null;

            Vector2[] wayPoints = new Vector2[0];
            bool pathSuccess = false;

            _startNode = GridPath.NodeFromWorldPoint(startPotion);
            _goalNode = GridPath.NodeFromWorldPoint(goalPosition);

            // If either nodes are not traveserable don't bother finding a path.
            if(!_startNode.IsTraversable || !_goalNode.IsTraversable)
                return null;

            Heap<PathNodeBase> _openNodes = new Heap<PathNodeBase>(GridPath.MaxSize);
            HashSet<PathNodeBase> _closedNodes = new();

            _openNodes.Add(_startNode);

            while(_openNodes.Count > 0)
            {
                PathNodeBase currentNode = _openNodes.RemoveFirst();

                _closedNodes.Add(currentNode);

                // We found our goal node.
                if(currentNode == _goalNode)
                {
                    pathSuccess = true;
                    break;
                }


                foreach(PathNodeBase neighbourNode in GridPath.GetNeighbours(currentNode))
                {
                    if(!neighbourNode.IsTraversable || _closedNodes.Contains(neighbourNode))
                        continue;

                    float newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode);

                    if(newMovementCostToNeighbour < neighbourNode.GCost || !_openNodes.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourNode, _goalNode);
                        neighbourNode.ParentNodeOnPath = currentNode;

                        if(!_openNodes.Contains(neighbourNode))
                            _openNodes.Add(neighbourNode);
                        else
                            _openNodes.UpdateItem(neighbourNode);
                    }
                }
            }
            await Awaitable.EndOfFrameAsync();
            if(pathSuccess)
                wayPoints = RetracePath(_startNode, _goalNode);

            return wayPoints;
        }

        private Vector2[] RetracePath(PathNodeBase startNode, PathNodeBase goalNode)
        {
            List<PathNodeBase> pathNodes = new();

            PathNodeBase currentNode = goalNode;

            while (currentNode != startNode)
            {
                pathNodes.Add(currentNode);
                currentNode = currentNode.ParentNodeOnPath;
            }
            Vector2[] waypoints = SimplifyPath(pathNodes);
            System.Array.Reverse(waypoints);
            return waypoints;
        }

        private Vector2[] SimplifyPath(List<PathNodeBase> path)
        {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 directionOld = Vector2.zero;

            for(int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(
                    path[i-1].GridPosition.x - path[i].GridPosition.x, 
                    path[i-1].GridPosition.y - path[i].GridPosition.y
                    );

                if(directionNew != directionOld)
                    waypoints.Add(path[i-1].WorldPosition);

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        private float GetDistance(PathNodeBase nodeA, PathNodeBase nodeB)
        {
            float distanceX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
            float distanceY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);

            // Handling Diagonals for longer horizontal distances
            if(distanceX > distanceY)
                return 1.4f * distanceY + (distanceX - distanceY);
            else // Handling Diagonals for longer vertical distances
                return 1.4f *distanceX + (distanceY - distanceX);
        }
    }
}
