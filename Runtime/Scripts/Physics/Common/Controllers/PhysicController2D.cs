using System;
using UnityEngine;

namespace SF.Physics
{
    /// <summary>
    /// Base class for custom Physics Controllers in a 2D simulation.
    /// Inherit from this to create a custom velocity and physics interaction solution for any type of game object.
    /// </summary>
    /// <remarks>
    /// The base class doesn't care or understand the difference between a <see cref="Rigidbody2D"/> or a <see cref="UnityEngine.LowLevelPhysics2D.PhysicsBody"/>.
    /// They inheriting classes define which one to use.
    /// </remarks>
    public abstract class PhysicController2D : MonoBehaviour, IForceReciever
    {
        /// <summary>
        /// Reference speed if used for passing in a value in horizontal calculating based on running or not.
        /// </summary>
        [NonSerialized] public float ReferenceSpeed;

        [Header("Physics Properties")]
        public MovementProperties DefaultPhysics = new(new Vector2(5, 5));
        public MovementProperties CurrentPhysics = new(new Vector2(5, 5));
        /// <summary>
        /// The type of PhysicsVolumeType the controller is in. Sets as none if not in one.
        /// </summary>
        public PhysicsVolumeType PhysicsVolumeType;

        #region Velocity Direction
        /// <summary>
        /// The property for which direction the Controller is having external input tell it to move.
        /// Either sent via an input system for player controlled objects or a state machine for AI controlled objects.
        /// </summary>
        public Vector2 Direction
        {
            get { return _direction; }
            set
            {
                if (_previousDirection != value)
                    _previousDirection = _direction;
                
                value.x = Mathf.RoundToInt(value.x);
                _direction = value;
                OnDirectionChanged?.Invoke(this, _direction);
            }
        }
        
        [SerializeField] protected Vector2 _direction;
        [SerializeField] protected Vector2 _directionLastFrame;
        /// <summary>
        /// Used to keep track of the direction to restore after unfreezing the Controller2D.
        /// </summary>
        protected Vector2 _previousDirection;
        
        public EventHandler<Vector2> OnDirectionChanged;
      
        #endregion 
        
        #region Velocity Values
        /// <summary>
        /// The overall velocity to be added this frame.
        /// </summary>
        protected Vector2 _calculatedVelocity;
        /// <summary>
        /// Velocity adding through external physics forces such as gravity and interactable objects.
        /// </summary>
        protected Vector2 _externalVelocity;
        
        public Vector2 CurrentVelocity => _calculatedVelocity;
        #endregion

        public bool IsFrozen;

        #region Lifecycle Methods
        private void Awake()
        {
            Init();
            OnAwake();
        }
        /// <summary>
        /// This runs before OnAwake code to make sure things needing Initialized are
        /// ready before it is called and needed. This can be called externally if
        /// the Controller ever needs reset. Think spawning a character.
        /// </summary>
        public void Init()
        {
            OnInit();
        }
        

        protected virtual void OnInit()
        {

        }
        protected virtual void OnAwake()
        {
            
        }
        private void Start()
        {
            DefaultPhysics.GroundSpeed = Mathf.Clamp(DefaultPhysics.GroundSpeed, 0, DefaultPhysics.GroundMaxSpeed);
            
            CurrentPhysics = DefaultPhysics;
            ReferenceSpeed = CurrentPhysics.GroundSpeed;

            OnStart();
        }
        protected virtual void OnStart()
        {
        }
        protected virtual void FixedUpdate()
        { 
            if (_direction.x != 0)
                _directionLastFrame.x = _direction.x;
            
            OnPreFixedUpdate();

            /*
            // Set all bools for what sides there was a collision on last frame.
            CollisionInfo.WasCollidingRight = CollisionInfo.IsCollidingRight;
            CollisionInfo.WasCollidingLeft = CollisionInfo.IsCollidingLeft;
            CollisionInfo.WasCollidingAbove = CollisionInfo.IsCollidingAbove;
            CollisionInfo.WasCollidingBelow = CollisionInfo.IsGrounded;
          
            CollisionInfo.CheckCollisions();
            */
            
            CalculateHorizontal();
            CalculateVertical();
            
            
            Move();
        }
        private void LateUpdate()
        {
            CalculateMovementState();
            OnLateUpdate();
        }
        protected virtual void OnLateUpdate()
        {

        }
        protected virtual void OnPreFixedUpdate()
        {
        }
        #endregion
        
        #region Movement Calculations

        protected abstract void Move();

        protected abstract void CalculateHorizontal();
        protected abstract void CalculateVertical();
        
        public virtual void FreezeController()
        {
            _calculatedVelocity.x = 0;
            _externalVelocity.x = 0;
            IsFrozen = true;
        }
        public virtual void UnfreezeController()
        {
            IsFrozen = false;
        }
        public void SetExternalVelocity(Vector2 force)
        {
            _externalVelocity = force;
        }
        /// <summary>
        /// This function applies velocity compared to the direction the user is facing.
        /// </summary>
        public void SetDirectionalForce(Vector2 force)
        {
            _externalVelocity = force * -_directionLastFrame.x;
        }
        public void AddVelocity(Vector2 velocity)
        {
            _externalVelocity += velocity;
        }
        public void AddHorizontalVelocity(float horizontalVelocity)
        {
            _externalVelocity.x += horizontalVelocity;
        }
        public void AddVerticalVelocity(float verticalVelocity)
        {
            _calculatedVelocity.y += verticalVelocity;
        }
        public void SetVelocity(Vector2 velocity)
        {
            _calculatedVelocity = velocity;
        }
        public void SetHorizontalVelocity(float horizontalVelocity)
        {
            _calculatedVelocity.x = horizontalVelocity;
        }
        public void SetVerticalVelocity(float verticalVelocity)
        {
            // Need to compare this to _rigidbody2D.velocityY to see which one feels better. 
            _calculatedVelocity.y = verticalVelocity;
        }
        #endregion

        protected abstract void CalculateMovementState();
    }
}
