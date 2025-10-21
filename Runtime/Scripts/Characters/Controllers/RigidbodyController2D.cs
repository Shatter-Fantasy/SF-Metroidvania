using System;
using SF.Managers;
using UnityEngine;

using SF.Physics;

namespace SF.Characters.Controllers
{
    /// <summary>
    /// A physics controller used to add custom physics logic to any object with a rigidbody.
    /// This physics controller adds the ability to invoke events when colliding on per direction basis by
    /// using the <see cref="CollisionController"/> 
    /// </summary>
    public class RigidbodyController2D : PhysicController2D, IForceReciever
    {
        
        public CharacterState CharacterState;
        
        #region Components 
        protected BoxCollider2D _boxCollider;
        protected Rigidbody2D _rigidbody2D;
        #endregion

        #region Collision 
        [NonSerialized] public Bounds Bounds;
        public CollisionInfoBase CollisionInfo = new CollisionInfo();
        #endregion
        
        public bool CollisionActivated
        {
            get => CollisionInfo.CollisionActivated;
            set => CollisionInfo.CollisionActivated = value;
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
            
            if(CollisionInfo is CollisionInfo collisionInfo)
                collisionInfo.Initialize(_boxCollider);
            else
            {
                CollisionInfo.Initialize();
            }
            
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
        
        protected override void OnAwake()
        {
            _rigidbody2D.gravityScale = 0;
        }
        private void Start()
        {
            // Need to check why this is in twice. Gravity Scale is being set also in OnAwake.
            _rigidbody2D.gravityScale = 0;

            CharacterState.StatusEffectChanged += OnStatusEffectChanged;
            DefaultPhysics.GroundSpeed = Mathf.Clamp(DefaultPhysics.GroundSpeed, 0, DefaultPhysics.GroundMaxSpeed);
            
            CurrentPhysics = DefaultPhysics;
            ReferenceSpeed = CurrentPhysics.GroundSpeed;

            OnStart();
        }
        protected override void FixedUpdate()
        { 
            Bounds = _boxCollider.bounds;
            
            if (_direction.x != 0)
                _directionLastFrame.x = _direction.x;
 
            // If the player is not in control of the Input or Actions for this frame in game logic return.
            if (GameManager.Instance.ControlState != GameControlState.Player)
                return;
            
            OnPreFixedUpdate();

            // Set all bools for what sides there was a collision on last frame.
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
        #endregion
        #region Movement Calculations
        protected override void Move()
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
        protected override void CalculateHorizontal()
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
        protected override void CalculateVertical() { }
        
        public override void FreezeController()
        {
            _calculatedVelocity.x = 0;
            _externalVelocity.x = 0;
            _rigidbody2D.linearVelocity = Vector2.zero;
            IsFrozen = true;
        }
        protected override void CalculateMovementState()
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

            /* This un-child characters from attached platforms like
             * moving, climbable, and so forth on death to prevent being linked to them if dying while on one. */
            transform.parent = null;
            IsFrozen = false;
            _calculatedVelocity = Vector3.zero;
            _rigidbody2D.linearVelocity = Vector3.zero;
            _externalVelocity = Vector3.zero;
        }

        
        /// TODO: Remove this now that CollisionInfo has all collision details.
        /// <summary>
        /// This is called from external classes to get the current colliders bounds value when a box collider has not been set and had bounds cached yet. Useful for editor debugging.
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

            PostOnEnable();
        }

        protected virtual void PostOnEnable()
        {
            
        }
        
        protected void OnDisable()
        {
            CollisionInfo.OnGroundedHandler -= OnGrounded;
            CollisionInfo.OnCeilingCollidedHandler -= OnCeilingCollided;

            PostOnDiable();
        }
        
        protected virtual void PostOnDiable()
        {
            
        }
    }
}
