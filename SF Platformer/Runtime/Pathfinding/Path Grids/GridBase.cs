using System;
using System.Collections.Generic;

using SF.Pathfinding;
using UnityEngine;

namespace SF
{
    public class GridBase : MonoBehaviour
    {
        public bool DebugDrawGrid = false;
        public LayerMask UnwalkableMask;
        public Vector2 GridWorldSize;
        public float NodeRadius = 0.5f;

        private PathNodeBase[,] _grid;
        private float _nodeDiameter;
        int _gridSizeX, _gridSizeY;

        public int MaxSize => _gridSizeX * _gridSizeY;

        private void Awake()
        {
            _nodeDiameter = NodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);

            GenerateGrid();
        }


        private void GenerateGrid()
        {
            _grid = new PathNodeBase[_gridSizeX, _gridSizeY];
            Vector2 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.up * GridWorldSize.y / 2;

            for(int x = 0; x < _gridSizeX; x++)
            {
                for(int y = 0; y < _gridSizeY; y++)
                {
                    // Figure out the world point of the node.
                    Vector2 worldPoint = worldBottomLeft +
                        Vector2.right * (x * _nodeDiameter + NodeRadius) +
                        Vector2.up * (y * _nodeDiameter + NodeRadius);

                    bool traversable = !(Physics2D.OverlapCircle(worldPoint, NodeRadius, UnwalkableMask));
                    
                    _grid[x, y] = new PathNodeBase(traversable,worldPoint, new Vector2(x,y));
                }
            }
        }

        public List<PathNodeBase> GetNeighbours(PathNodeBase pathNode)
        {
            List<PathNodeBase> neighberNodes = new();

            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    // Don't add the center node as a neighbor. It is the current node.
                    if(x == 0 && y == 0)
                        continue;

                    // Make sure we don't leave the grid if we are on the outside of it.
                    Vector2 checkPosition =  pathNode.GridPosition + new Vector2(x,y);

                    if(checkPosition.x >= 0 // Not outside the minimum x of the grid.
                        && checkPosition.x < _gridSizeX  // Not outside the maximum x of the grid.
                        && checkPosition.y >= 0 // Not outside the minimum Y of the grid.
                        && checkPosition.y < _gridSizeY) // Not outside the maximum Y of the grid.
                           neighberNodes.Add(_grid[(int)checkPosition.x, (int)checkPosition.y]);
                }
            }

            return neighberNodes;
        }

        /// <summary>
        /// Returns the PathNode from the passed in world position on this grid if it exists.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public PathNodeBase NodeFromWorldPoint(Vector2 worldPosition)
        {
            
            if(_grid == null)
            {
                _nodeDiameter = NodeRadius * 2;
                _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
                _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);

                GenerateGrid();
            }

            
            float percentX = (worldPosition.x - transform.position.x + GridWorldSize.x/2) / GridWorldSize.x;
            float percentY = (worldPosition.y - transform.position.y + GridWorldSize.y/2) / GridWorldSize.y;
            
            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

            // Make sure we don't leave the grid if the target moves out of it.
            x = Math.Min(Mathf.RoundToInt(GridWorldSize.x), x);
            y = Math.Min(Mathf.RoundToInt(GridWorldSize.y), y);
            
            //Debug.Log($"Node World Position: {worldPosition}.  X Calculated Position: {x}. Y Calculated Position: {y}");
            
            return _grid[x, y];
        }

        private void OnDrawGizmos()
        {
            if(!DebugDrawGrid)
                return;

            Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, 0));

            if(_grid != null)
            {
                foreach(var node in _grid)
                {
                    Gizmos.color = (node.IsTraversable) ? Color.white : Color.red;

                    Gizmos.DrawWireCube(node.WorldPosition,Vector3.one * (_nodeDiameter - 0.1f));
                }
            }
        }
    }
}
