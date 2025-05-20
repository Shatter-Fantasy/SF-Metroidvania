using System;
using UnityEngine;

namespace SF.Pathfinding
{
    [Serializable]
    public class PathNodeBase : IHeapItem<PathNodeBase>
    {
        public float GCost; // Distance to the starting node.
        public float HCost; // Distance to the ending node.
        public float FCost => GCost + HCost; // G + H. Good way to memorize what this is: F stands for final cost.

        [NonSerialized] public PathNodeBase ParentNodeOnPath;

        /// <summary>
        /// The node position on the current grid this node is on.
        /// </summary>
        public Vector2 GridPosition;
        public Vector2 WorldPosition;
        private bool _isTraversable;

        private int _heapIndex;
        public int HeapIndex
        {
            get => _heapIndex;
            set => _heapIndex = value;
        }

        public virtual bool IsTraversable
        {
            get { return _isTraversable; }
            set { _isTraversable = value; }
        }

        public PathNodeBase(bool isTraversable, Vector2 worldPosition,Vector2 gridPosition)
        {
            IsTraversable = isTraversable;
            WorldPosition = worldPosition;
            GridPosition = gridPosition;
        }

        public int CompareTo(PathNodeBase nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            
            if(compare == 0)
                compare = HCost.CompareTo(nodeToCompare.HCost);

            return -compare;
        }
    }
}
