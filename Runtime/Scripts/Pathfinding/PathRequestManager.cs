using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace SF.Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        public static PathRequestManager _instance;

        public Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;

        private bool _isProcessingAPath;
        [HideInInspector] public PathAStar PathFinding;

        private void Awake()
        {
            _instance = this;
            PathFinding = GetComponent<PathAStar>();
        }

        public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<NativeList<float2>, bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
            _instance._pathRequestQueue.Enqueue(newRequest);
            _instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if(!_isProcessingAPath && _pathRequestQueue.Count > 0)
            {
                _currentPathRequest = _pathRequestQueue.Dequeue();
                _isProcessingAPath = true;
                PathFinding.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
            }
        }
        
        public void FinishedProcessingPath(NativeList<float2> path, bool succes)
        {
            _currentPathRequest.Callback(path, succes);
            _isProcessingAPath = false;
            TryProcessNext();
        }

        public struct PathRequest
        {
            public Vector2 PathStart;
            public Vector2 PathEnd;
            public Action<NativeList<float2>, bool> Callback;
            
            public PathRequest(Vector2 pathStart, Vector2 pathEnd, Action<NativeList<float2>, bool> callback)
            {
                PathStart = pathStart;
                PathEnd = pathEnd;
                Callback = callback;
            }
        }
    }
}
