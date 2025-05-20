using UnityEngine;

namespace SF.CommandModule
{
    [System.Serializable]
    public class TransformCommand : CommandNode, ICommand
    {
        public Vector3 MovementAmount = new Vector3(5,5,0);
        public float Duration = 3;
        public float DurationRemaining;
        public MovementFollowType FollowType;
        public Transform _transform;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        public TransformCommand() { }
        public TransformCommand(Transform transform )
        {
            _transform = transform;
        }

        public async override Awaitable Use()
        {
            await StartMove();
        }

        public async Awaitable StartMove()
        {
            DurationRemaining = Duration;

            _startPosition = _transform.position;
            _endPosition = _transform.position + MovementAmount;

            await MoveLinear();
        }
        public async Awaitable MoveLinear()
        {
            float percentCompleted = 0;
            float timeElapsed = 0;

            while(percentCompleted < 1)
            {
                _transform.position = 
                    Vector3.Lerp(_startPosition, _endPosition, percentCompleted);
                
                percentCompleted = timeElapsed / Duration;
                timeElapsed += Time.deltaTime;
                DurationRemaining -= Time.deltaTime;
                
                await Awaitable.EndOfFrameAsync();
            }
          
        }
    }
}
