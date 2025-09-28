using UnityEngine;
using SF.Physics;

namespace SF.Characters.Controllers
{
	/// <summary>
	/// A physics controller for grounded characters that help implement gravity, slope mechanics, collision for platforms,
	/// and updates the <see cref="MovementState"/> while using the <see cref="CollisionController"/> for custom collision callbacks. 
	/// </summary>
	[RequireComponent(typeof(CharacterRenderer2D))]
	public class GroundedController2D : Controller2D
	{
		#region Booleans
		public bool IsGrounded
		{
			get => CollisionInfo.IsGrounded;
			set => CollisionInfo.IsGrounded = value;
		}
		[Header("Movement Booleans")]
		public bool IsRunning;
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
		#endregion
		
		#region Slope Settings

		[Header("Slope Settings")]
		[SerializeField] private bool _useSlopes;
		//[SerializeField] private float _slopeUpperLimit = 65;
		//[SerializeField] private float _slopeLowerLimit = 15;
		[SerializeField] protected Vector2 _slopeNormal;
		[SerializeField] private float _standingOnSlopeAngle;
		[SerializeField] private float _slopeMultiplier = 0.9f;
        [SerializeField] private bool _onSlope;
        private Vector2 _slopeSideDirection;
        #endregion

		protected CharacterRenderer2D _character;
		#region Components 
		protected Vector2 _originalColliderSize;
		protected Vector2 _modifiedColliderSize;
		protected Vector2 _previousColliderSize;
		#endregion
		protected override void OnAwake()
		{
			_character = GetComponent<CharacterRenderer2D>();
			_boxCollider = GetComponent<BoxCollider2D>();
			Bounds = _boxCollider.bounds;
			_originalColliderSize = _boxCollider.size;
		}
		
		/* Slope Checks.
		public virtual void SlopeChecks()
		{
			if(!_useSlopes)
				return;
			
			_slopeNormal = CollisionInfo.BelowHit.normal;
			_standingOnSlopeAngle = Vector2.Angle(_slopeNormal, Vector2.up);
			_onSlope = _slopeUpperLimit > _standingOnSlopeAngle 
			               && _standingOnSlopeAngle > _slopeLowerLimit;
		}
		
		*/
		
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

			if(!IsGrounded && !IsClimbing && !_onSlope)
			{
                // This is related to the formula of: square root of ( -2 (gravity * height) )
                // https://en.wikipedia.org/wiki/Equations_for_a_falling_body#Example
                _calculatedVelocity.y -= Mathf.Sqrt( -2 * -CurrentPhysics.GravityScale * Time.deltaTime); 

				_calculatedVelocity.y = Mathf.Clamp(_calculatedVelocity.y,
					-CurrentPhysics.TerminalVelocity,
					CurrentPhysics.MaxUpForce);
			}
		}

		protected override void Move()
		{
			
			if(_onSlope)
			{
				_calculatedVelocity *= _slopeMultiplier;
				// TODO: Make the ability to walk up slopes.
				//_calculatedVelocity = Vector3.ProjectOnPlane(_calculatedVelocity, _slopeNormal);
			}

			if (IsFrozen && IsGrounded)
				_calculatedVelocity.x = 0;
			
			base.Move();
        }
		
		/// <summary>
		/// Calculates the current movement state that the player is currently in.
		/// </summary>
		/// <remarks>
		/// This needs to be moved into the Controller2D parent class.
		/// </remarks>
		protected override void CalculateMovementState()
		{
			if(CharacterState.CharacterStatus == CharacterStatus.Dead)
				return;

			// TODO: There are some places that set the values outside of this function. Find a way to make it where this function is the only needed one. Example IsJump in the Jumping Ability.

			if(IsClimbing)
			{
				CharacterState.CurrentMovementState = _calculatedVelocity.y != 0 
					? MovementState.Climbing 
					: MovementState.ClimbingIdle;
			}

			// If our velocity is negative we are either falling/gliding.
			if(_calculatedVelocity.y < 0 && !IsClimbing && !_onSlope)
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

			if(IsGrounded)
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
		
		public virtual void ResizeCollider(Vector2 newSize)
		{
			// Need to keep track of the previous size if the collider was already resized once before, but wasn't reset to the default collider size.
			_previousColliderSize = _boxCollider.size;
			_modifiedColliderSize = newSize;
            _boxCollider.size = newSize;
		}

        public void ResetColliderSize()
		{
			_boxCollider.size = _originalColliderSize;

			// TODO: Do checks if colliding on the sides or ceiling to make sure the default collider size doesn't click through them.
			// TODO: Make sure the new Collider size doesn't clip us into the ground.
		}

        public override void UpdatePhysicsProperties(MovementProperties movementProperties,
             PhysicsVolumeType volumeType = PhysicsVolumeType.None)
        {
			base.UpdatePhysicsProperties(movementProperties, volumeType);

           ReferenceSpeed =  IsRunning
                    ? CurrentPhysics.GroundRunningSpeed
                    : CurrentPhysics.GroundSpeed;
        }

        public override void ResetPhysics(MovementProperties movementProperties)
        {
			base.ResetPhysics(movementProperties);
            ReferenceSpeed = IsRunning
                    ? CurrentPhysics.GroundRunningSpeed
                    : CurrentPhysics.GroundSpeed;
        }

        protected override void OnGrounded()
        {
	        base.OnGrounded();
	        IsFalling = false;
        }

        protected override void OnCeilingCollided()
        {
	        base.OnCeilingCollided();
	        IsJumping = false;
	        IsFalling = true;
        }
    }
}