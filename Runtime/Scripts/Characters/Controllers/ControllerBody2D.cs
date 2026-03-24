using UnityEngine;

namespace SF.U2D.Physics
{
    using Characters;
    
    /// <summary>
    /// The base class for high performance physics controller that uses Unity's Low Level Physics2D system to allow for
    /// highly customizable physics that can even destroy, edit, and manipulate physics shapes in real time.
    /// </summary>
    /// <remarks>
    /// This can be used for Character Controllers, real time editable terrain (think Team 17 Worms games), and fast pace physics based 2D games that need high performance. 
    /// </remarks>
    [DisallowMultipleComponent]
    public class ControllerBody2D : PhysicController2D
    {
        public CharacterState CharacterState;
        
        /// <summary>
        /// The <see cref="SFShapeComponent"/> that is defining the physics shape and physics body of the controller. 
        /// </summary>
        public SFShapeComponent ShapeComponent;
        
        /* These are separate from the MovementState enum because some of the following can be true at the same time.
         * Example you could be falling while gliding or falling into a water pool.
         */
        [Header("Movement Booleans")]
        public bool IsRunning;
        /// <summary>
        /// Tracks if the character in a pool of water or other liquid.
        /// </summary>
        public bool IsSwimming;
        public bool IsJumping;
        public bool IsFalling;
        public bool IsGliding;
        public bool IsCrouching;
        
        public bool IsClimbing
        {
            get { return _isClimbing; }
            set
            {
                // Don't do any other checks if we are setting is climbing to the exact same value.
                if (_isClimbing == value)
                    return;
				
                _isClimbing = value; 

                if(!_isClimbing)
                {
                    CurrentPhysics.GravityScale = DefaultPhysics.GravityScale;
                    _character.CanTurnAround = true;
                }
                else
                {
                    CurrentPhysics.GravityScale = 0;
                    _character.CanTurnAround = false;
                }
            }
        }
        [SerializeField] private bool _isClimbing;

        public BodyCollisionInfo CollisionInfo = new BodyCollisionInfo();
        
        protected CharacterRenderer2D _character;
        
        protected override void Move()
        {
            if (ShapeComponent == null 
                || !ShapeComponent.Shape.isValid)
                return;
            
            /* Slope Calculations
            if(_onSlope)
            {
                _calculatedVelocity *= _slopeMultiplier;
                // TODO: Make the ability to walk up slopes.
                // TODO: Use the new PhysicsShape math for testing the projection on the plane instead.
                //_calculatedVelocity = Vector3.ProjectOnPlane(_calculatedVelocity, _slopeNormal);
            } */

            if (IsFrozen && CollisionInfo.IsGrounded)
                _calculatedVelocity.x = 0;
            
            if(CharacterState.CharacterStatus == CharacterStatus.Dead)
                _calculatedVelocity = Vector2.zero;

            if(_externalVelocity != Vector2.zero)
            {
                _calculatedVelocity = _externalVelocity;
                _externalVelocity = Vector2.zero;
            }
            
            ShapeComponent.Body.linearVelocity = _calculatedVelocity;
        }

        protected override void CalculateHorizontal()
        {
            if(IsClimbing)
            {
                _calculatedVelocity.x = 0;
                return;
            }

            if(Direction.x != 0)
            {
                // We only have to do a single clamp because than Direction.x takes care of it being negative or not when being multiplied.
                ReferenceSpeed = Mathf.Clamp(ReferenceSpeed, 0, CurrentPhysics.GroundMaxSpeed);

                // TODO: When turning around erase previously directional velocity.
                // If it is kept the player could slide in the previous direction for a second before running the new direction on smaller ground acceleration values.
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, ReferenceSpeed * Direction.x, CurrentPhysics.GroundAcceleration);
            }
            else
            {
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, 0, CurrentPhysics.GroundDeacceleration);
            }
			
            // If we are moving left and not hitting a slope, but an obstacle, stop moving left.
            if (CollisionInfo.IsCollidingLeft && Direction.x < 0 && CollisionInfo.IsCollidingLeft)
            {
                _calculatedVelocity.x = 0;
            }
			
            // If we are moving Right and not hitting a slope, but an obstacle, stop moving Right.
            if (CollisionInfo.IsCollidingRight && Direction.x > 0 && CollisionInfo.IsCollidingRight)
            {
                _calculatedVelocity.x = 0;
            }
        }

        protected override void CalculateVertical()
        {
            
            if(IsClimbing)
            {
                _calculatedVelocity.y = Direction.y * CurrentPhysics.ClimbSpeed.y;
            }
            
            //if(!CollisionInfo.IsGrounded  && !IsClimbing && !_onSlope)
            if(!CollisionInfo.IsGrounded && !IsClimbing)
            {
                // This is related to the formula of: square root of ( -2 (gravity * height) )
                // https://en.wikipedia.org/wiki/Equations_for_a_falling_body#Example
                _calculatedVelocity.y -= Mathf.Sqrt( -2 * -CurrentPhysics.GravityScale * Time.deltaTime); 

                _calculatedVelocity.y = Mathf.Clamp(_calculatedVelocity.y,
                    -CurrentPhysics.TerminalVelocity,
                    CurrentPhysics.MaxUpForce);
            }
        }

        protected override void CalculateMovementState()
        {
            if(CharacterState.CharacterStatus == CharacterStatus.Dead)
                return;

            /* TODO: There are some places that set the values outside of this function.
             * Find a way to make it where this function is the only needed one. Example IsJump in the Jumping Ability.
             */

            if(IsClimbing)
            {
                CharacterState.CurrentMovementState = _calculatedVelocity.y != 0 
                    ? MovementState.Climbing 
                    : MovementState.ClimbingIdle;
            }

            // If our velocity is negative we are either falling/gliding.
            //if(_calculatedVelocity.y < 0 && !IsClimbing && !_onSlope)
            if(_calculatedVelocity.y < 0 && !IsClimbing)
            {
                if(IsGliding)
                    CharacterState.CurrentMovementState = MovementState.Gliding;
                else
                {
                    // Need to remove the crouch check when I get the collider calculation more accurate on resizing.
                    if(!IsCrouching)
                        CharacterState.CurrentMovementState = MovementState.Falling;
                }
                IsFalling = true;
                IsJumping = false;
            }

            if(IsJumping)
                CharacterState.CurrentMovementState = MovementState.Jumping;

            if(CollisionInfo.IsGrounded)
            {
                IsFalling = false;

                if(IsCrouching)
                {
                    CharacterState.CurrentMovementState = MovementState.Crouching;
                    return;
                }
				
                CharacterState.CurrentMovementState = (Direction.x == 0) ? MovementState.Idle : MovementState.Walking;
            }
        }


        protected override void OnAwake()
        {
            CollisionInfo = new BodyCollisionInfo
            {
                ControllerBody2D = this
            };

            if (!TryGetComponent(out ShapeComponent))
            {
                ShapeComponent = gameObject.AddComponent<SFCapsuleShape>();
            }
        }
        
        protected override void FixedUpdate()
        {
            if (_direction.x != 0)
                _directionLastFrame.x = _direction.x;
            
            if (!ShapeComponent.Shape.isValid)
                return;

            OnPreFixedUpdate();
            
            // We now use the PhysicsEvent.PostSimulate callback for checking collisions post movement
            // We should make sure this is not skipping first frame of collision though.
            CollisionInfo.CheckCollisions();
            
            CalculateHorizontal();
            CalculateVertical();
            
            Move();
        }
        
        public void ResizePhysicsShape(Vector2 newSize)
        {
            /* Implement for Alpha 9
             * Needed to finish the updated crouch and climb ability implementation.
             * This just needs to update the ShapeComponent.PhysicsShape size
             * */
        }
        public void ResetPhysicsShapeSize()
        {
            /* Implement for Alpha 9
             * Needed to finish the updated crouch and climb ability implementation.
             * This just needs to update the ShapeComponent.PhysicsShape size
             * */
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
            // TODO: Implement the movement properties being passed in from states like gliding/exiting physics volume.
            CurrentPhysics = DefaultPhysics;
            PhysicsVolumeType = PhysicsVolumeType.None;

            ReferenceSpeed = CurrentPhysics.GroundSpeed;
        }
    }
}
