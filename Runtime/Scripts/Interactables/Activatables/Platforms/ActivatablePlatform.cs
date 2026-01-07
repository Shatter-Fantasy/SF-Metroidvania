using SF.HandleModule;

using UnityEngine;

namespace SF
{
    [RequireComponent(typeof(PositionHandle))]
    
    public class ActivatablePlatform : ActivatableWrapper, IActivatable
    {
        [SerializeField] private float _speed = 5;

        private Vector3 _targetPosition;

        private PositionHandle _positioner;

        private void Awake()
        {
            _positioner = GetComponent<PositionHandle>();
        }

        private void Start()
        {

            // _targetPosition = _endingPoint.position;
            _targetPosition = _positioner.EndPosition;
        }

        private void Update()
        {
            if(!Activated)
                return;

            if(_positioner == null)
                return;

            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);

            if( _targetPosition == _positioner.EndPosition
                && (transform.position - _targetPosition).sqrMagnitude <= Mathf.Epsilon )
            {
                _targetPosition = _positioner.StartPosition;
            }
            else if(_targetPosition == _positioner.StartPosition
                && (transform.position - _targetPosition).sqrMagnitude <= Mathf.Epsilon)
            {
                _targetPosition = _positioner.EndPosition;
            }

        }
    }
}