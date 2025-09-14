using System;
using SF.Managers;
using UnityEngine;

using SF.Physics;

#if SF_Utilities
using SF.Utilities;
#else
using SF.Platformer.Utilities;
#endif

namespace SF.Characters.Controllers
{
    /// <summary>
    /// A physics controller used to add custom physics logic to any object. 
    /// This physics controller adds the ability to invoke events when colliding on per direction basis by
    /// using the <see cref="CollisionController"/> 
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Controller2D : MonoBehaviour, IForceReciever
    {
        /// <summary>
		/// Reference speed if used for passing in a value in horizontal calculatin based on running or not.
		/// </summary>
		[NonSerialized] public float ReferenceSpeed;

        public float DistanceToGround;

        [Header("Physics Properties")]
        public MovementProperties DefaultPhysics = new(new Vector2(5, 5));
        public MovementProperties CurrentPhysics = new(new Vector2(5, 5));
        /// <summary>
        /// The type of PhysicsVolumeType the controller is in. Sets as none if not in one.
        /// </summary>
        public PhysicsVolumeType PhysicsVolumeType;
        
        public CharacterState CharacterState;
        public ContactFilter2D PlatformFilter;
        
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
        public bool IsFrozen;
        #region Components 
        protected BoxCollider2D _boxCollider;
        protected Rigidbody2D _rigidbody2D;
        #endregion

        #region
        [NonSerialized] public Bounds Bounds;
        #endregion
        [Header("Collision Data")]
        public CollisionInfo CollisionInfo = new();
        public CollisionController CollisionController = new(0.05f, 0.02f, 3, 4);

        /// <summary>
        /// The overall velocity to be added this frame.
        /// </summary>
        protected Vector2 _calculatedVelocity;
        /// <summary>
        /// Velocity adding through external physics forces such as gravity and interactable objects.
        /// </summary>
        protected Vector2 _externalVelocity;

        public bool CollisionActivated
        {
            get => CollisionController.CollisionActivated;
            set => CollisionController.CollisionActivated = value;
        }
        
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
            // I think I should add the collider assignment here.
            // Even flying enemies need colliders to hurt the player. 
            _rigidbody2D = _rigidbody2D != null ? _rigidbody2D : GetComponent<Rigidbody2D>();
            _boxCollider = _boxCollider != null ? _boxCollider : GetComponent<BoxCollider2D>();
            CollisionInfo.Collider2D = _boxCollider;
            SetComponentSetting();
            OnInit();
        }

        private void SetComponentSetting()
        {
            if(_boxCollider != null)
                _boxCollider.isTrigger = false;

            if(_rigidbody2D != null)
            {
                //_rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                _rigidbody2D.freezeRotation = true;
            }
        }

        protected virtual void OnInit()
        {

        }
        protected virtual void OnAwake()
        {
            _rigidbody2D.gravityScale = 0;
        }
        private void Start()
        {
            // Need to check why this is in twice. Gravity Scale is being set also in OnAwake.
            _rigidbody2D.gravityScale = 0;

            CharacterState.StatusEffectChanged += OnStatusEffectChanged;
            DefaultPhysics.GroundSpeed = Mathf.Clamp(DefaultPhysics.GroundSpeed, 0, DefaultPhysics.GroundMaxSpeed);

            PlatformFilter.useLayerMask = true;

            CurrentPhysics = DefaultPhysics;
            ReferenceSpeed = CurrentPhysics.GroundSpeed;

            OnStart();
        }
        protected virtual void OnStart()
        {
        }
        protected virtual void FixedUpdate()
        { 
            Bounds = _boxCollider.bounds;
            
            if (_direction.x != 0)
                _directionLastFrame.x = _direction.x;
 
            // If the player is not in control of the Input or Actions for this frame in game logic return.
            if (GameManager.Instance.ControlState != GameControlState.Player)
                return;
            
            OnPreFixedUpdate();

            // Set the bools for what sides there was a collision on last frame.
            CollisionInfo.WasCollidingRight = CollisionInfo.IsCollidingRight;
            CollisionInfo.WasCollidingLeft = CollisionInfo.IsCollidingLeft;
            CollisionInfo.WasCollidingAbove = CollisionInfo.IsCollidingAbove;
            CollisionInfo.WasCollidingBelow = CollisionInfo.IsGrounded;

            CollisionInfo.CheckCollisions();
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
        protected virtual void Move()
        {
            if(CharacterState.CharacterStatus == CharacterStatus.Dead)
                _calculatedVelocity = Vector2.zero;

            if(_externalVelocity != Vector2.zero)
            {
                _calculatedVelocity = _externalVelocity;
                _externalVelocity = Vector2.zero;
            }
            
            _rigidbody2D.linearVelocity = _calculatedVelocity;
        }
        protected virtual void CalculateHorizontal()
        {
            if(Direction.x != 0)
            {
                // We only have to do a single clamp because than Direction.x takes care of it being negative or not when being multiplied.
                ReferenceSpeed = Mathf.Clamp(ReferenceSpeed, 0, CurrentPhysics.GroundMaxSpeed);

                // TODO: When turning around erase previously directional velocity.
                // If it is kept the player could slide in the previous direction for a second before running the new direction on smaller ground acceleration values.
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, ReferenceSpeed * Direction.x, CurrentPhysics.GroundAcceleration);

                // Moving right
                if(Direction.x > 0 && CollisionInfo.IsCollidingRight)
                    _calculatedVelocity.x = 0;
                // Moving left
                else if(Direction.x < 0 && CollisionInfo.IsCollidingLeft)
                {

                    _calculatedVelocity.x = 0;
                }

            }
            else
            {
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, 0, CurrentPhysics.GroundDeacceleration);
            }
        }
        protected virtual void CalculateVertical() { }

        public void FreezeController()
        {
            _calculatedVelocity.x = 0;
            _externalVelocity.x = 0;
            _rigidbody2D.linearVelocity = Vector2.zero;
            IsFrozen = true;
        }
        public void UnfreezeController()
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
        public virtual void SetDirectionalForce(Vector2 force)
        {
            _externalVelocity = force * -_directionLastFrame.x;
        }
        public virtual void AddVelocity(Vector2 velocity)
        {
            _externalVelocity += velocity;
        }
        public virtual void AddHorizontalVelocity(float horizontalVelocity)
        {
            _externalVelocity.x += horizontalVelocity;
        }
        public virtual void AddVerticalVelocity(float verticalVelocity)
        {
            _calculatedVelocity.y += verticalVelocity;
        }
        public virtual void SetVelocity(Vector2 velocity)
        {
            _calculatedVelocity = velocity;
        }
        public virtual void SetHorizontalVelocity(float horizontalVelocity)
        {
            _calculatedVelocity.x = horizontalVelocity;
        }
        public virtual void SetVerticalVelocity(float verticalVelocity)
        {
            // Need to compare this to _rigidbody2D.velocityY to see which one feels better. 
            _calculatedVelocity.y = verticalVelocity;
        }

        protected virtual void CalculateMovementState()
        {

        }
        #endregion
        
        public void ChangeDirection()
        {
            Direction *= -1;
        }

        public void SetDirection(float newDirection)
        {
            Direction = new Vector2(newDirection, 0);
        }

        public virtual void UpdatePhysicsProperties(MovementProperties movementProperties, 
            PhysicsVolumeType volumeType = PhysicsVolumeType.None)
        {
            CurrentPhysics.GroundSpeed = movementProperties.GroundSpeed;
            CurrentPhysics.GroundRunningSpeed = movementProperties.GroundRunningSpeed;
            CurrentPhysics.GroundAcceleration = movementProperties.GroundAcceleration;
            CurrentPhysics.GroundMaxSpeed = movementProperties.GroundMaxSpeed;

            CurrentPhysics.GravityScale = movementProperties.GravityScale;
            CurrentPhysics.TerminalVelocity = movementProperties.TerminalVelocity;
            CurrentPhysics.MaxUpForce = movementProperties.MaxUpForce;

            PhysicsVolumeType = volumeType;

            ReferenceSpeed = CurrentPhysics.GroundSpeed;
        }

        public virtual void ResetPhysics(MovementProperties movementProperties)
        {
            CurrentPhysics = DefaultPhysics;
            PhysicsVolumeType = PhysicsVolumeType.None;

            ReferenceSpeed = CurrentPhysics.GroundSpeed;
        }

        protected void OnStatusEffectChanged(StatusEffect statusEffect)
        {
            if(statusEffect == StatusEffect.Berserk)
                GetComponent<SpriteRenderer>().color = Color.red;
        }

        public virtual void Reset()
        {
            if(_rigidbody2D == null)
                _rigidbody2D = GetComponent<Rigidbody2D>();

            /* This unchilds characters from attached platforms like
             * moving, climbable, and so forth on death to prevent being linked to them if dying while on one. */
            transform.parent = null;
            IsFrozen = false;
            _calculatedVelocity = Vector3.zero;
            _rigidbody2D.linearVelocity = Vector3.zero;
            _externalVelocity = Vector3.zero;
        }

        
        /// TODO: Remove this now that CollisionInfo has all collision details.
        /// <summary>
        /// This is called from external classes to get the current colliders bounds value when a box collider has not been set and had it bounds cached yet. Useful for editor debugging.
        /// </summary>
        /// <returns></returns>
        public Bounds GetColliderBounds()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            Bounds = _boxCollider.bounds;
            if(_boxCollider != null)
                return Bounds;
            else 
                return new Bounds();
        }

        #region Collision Events
        protected virtual void OnCeilingCollided()
        {
            /* If colliding above reset the vertical velocity if it is above zero, but not if we already started to fall downward away from the ceiling.
             * This prevents that hanging feeling when touching a ceiling.
             * If anything adds velocity for y while still touching the ceiling we might have an issue.
             * Something to keep in mind.*/
            if(_calculatedVelocity.y > 0)
                _calculatedVelocity.y = 0;
        }
        protected virtual void OnGrounded()
        {
            _calculatedVelocity.y = 0;
        }
        #endregion
        
        protected void OnEnable()
        {
            CollisionInfo.OnGroundedHandler += OnGrounded;
            CollisionInfo.OnCeilingCollidedHandler += OnCeilingCollided;
        }
        
        protected void OnDisable()
        {
            CollisionInfo.OnGroundedHandler -= OnGrounded;
            CollisionInfo.OnCeilingCollidedHandler -= OnCeilingCollided;
        }
    }
}
