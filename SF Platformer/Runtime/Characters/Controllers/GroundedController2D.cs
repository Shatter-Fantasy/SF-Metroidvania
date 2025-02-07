#if UNITY_EDITOR
// The two below namespaces are only used in the OnDrawGizmos
// at the time of typing this comment.
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endif

using UnityEngine;
using SF.Physics;
using System;

#if SF_Utilities
using SF.Utilities;
#else
using SF.Platformer.Utilities;
#endif

namespace SF.Characters.Controllers
{
	[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(CharacterRenderer2D))]
	public class GroundedController2D : Controller2D
	{
		[Header("Platform Settings")]
		public ContactFilter2D OneWayPlatformFilter;
		[SerializeField] protected LayerMask MovingPlatformLayer;
		[field: SerializeField] public GameObject StandingOnObject { get; protected set; }
		

        [Header("Booleans")]
		public bool IsGrounded = false;
		protected bool _wasGroundedLastFrame = false;
		public bool IsRunning = false;
		public bool IsSwimming = false;
		public bool IsJumping = false;
		public bool IsFalling = false;
		public bool IsGliding = false;
		public bool IsCrouching = false;


		public bool IsClimbing
		{
			get { return _isClimbing; }
			set 
			{ 
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
		[SerializeField] private bool _isClimbing = false;
		
		// [Header("Slope Settings")]
		private bool _useSlopes = false;
        private float SlopeLimit = 55;
        private float SlopeSlipLimit = 35;
		protected Vector2 _slopeNormal;
        private float StandingOnSlopeAngle;
        private bool OnSlope = false;
        private Vector2 _slopeSideDirection;

		protected int OneWayFilterBitMask => PlatformFilter.layerMask & OneWayPlatformFilter.layerMask;
		public Action OnGrounded;

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

		#region Collision Calculations
		protected override void ColisionChecks()
		{
			CollisionInfo.CollisionHits.Clear();
            _wasGroundedLastFrame = IsGrounded;
			GroundChecks();
			CeilingChecks();
			SideCollisionChecks();
			ClimbableSurfaceChecks();
			CheckOnCollisionActions();
		}
		protected override void GroundChecks()
		{
			Bounds = _boxCollider.bounds;

			CollisionInfo.DistanceToGround = Physics2D.Raycast(
				Bounds.BottomCenter(),
				Vector2.down,
				10,PlatformFilter.layerMask).distance;

			// This will eventually also show colliding with other things than platforms.
            CollisionInfo.BelowHit = DebugBoxCast(
                        Bounds.BottomCenter(),
                        new Vector2(Bounds.size.x, .02f),
                        0,
                        Vector2.down,
                        CollisionController.VerticalRayDistance,
                        PlatformFilter.layerMask
                    );

			// If we did collide with something below.
			if(CollisionInfo.BelowHit)
			{
				CollisionInfo.CollisionHits.Add(CollisionInfo.BelowHit);

                // If we are standing on something keep track of it. This can be useful for things like moving platforms.
                StandingOnObject = CollisionInfo.BelowHit.collider.gameObject;

				// Only set the transform if we already are not a child of another gameobject.
				// If we don't do this than we will cnstantly be restuck to the moving platforms transform.
				if(transform.parent == null && LayerMask.LayerToName(CollisionInfo.BelowHit.collider.gameObject.layer) == "MovingPlatforms")
                    transform.SetParent(CollisionInfo.BelowHit.collider.gameObject.transform);

                CollisionInfo.IsCollidingBelow = true;
                IsGrounded = true;
				LowerToGround();
            }
			else // If we are not colliding with anything below.
			{
				StandingOnObject = null;

                // If we are attacthed to another object and was standing on something last frame
                // unattach the character from the object.
                if(transform.parent != null)
					transform.SetParent(null);

                CollisionInfo.IsCollidingBelow = false;
                IsGrounded = false;
            }

            if(IsJumping)
			{
				IsGrounded = false;
				// We return to prevent and grounded collision resetting velocity for y on the
				// frame we are leaving the ground.
				return;
			}

			// At this point we are not jumping or falling
            if(IsGrounded)
				_calculatedVelocity.y = 0;

			// If not grounded last frame, but grounded this frame call OnGrounded
			if(!_wasGroundedLastFrame && IsGrounded)
			{
				OnGrounded?.Invoke();
			}
        }
		protected override void SideCollisionChecks()
		{
			//TODO: Clean the below up and fuse the IsColliding value setting with the variable hit2D checks.

            // Right Side
            CollisionInfo.RightHit = Physics2D.BoxCast(Bounds.MiddleRight(), new Vector2(.02f,Bounds.size.y - CollisionController.RayOffset), 0, Vector2.right, CollisionController.HoriztonalRayDistance, PlatformFilter.layerMask);

			CollisionInfo.IsCollidingRight = CollisionInfo.RightHit;

            // Left Side
            CollisionInfo.LeftHit = Physics2D.BoxCast(Bounds.MiddleLeft(), new Vector2(.02f, Bounds.size.y - CollisionController.RayOffset), 0, Vector2.left, CollisionController.HoriztonalRayDistance, PlatformFilter.layerMask);

            CollisionInfo.IsCollidingLeft = CollisionInfo.LeftHit;
		}

		protected virtual void ClimbableSurfaceChecks()
		{
			foreach(RaycastHit2D hit2D in CollisionInfo.CollisionHits)
			{
				if(!hit2D)
					CollisionInfo.ClimbableSurfaceHit = new RaycastHit2D();
				else if(hit2D.collider.TryGetComponent(out ClimbableSurface climbableSurface))
				{
					CollisionInfo.ClimbableSurfaceHit = hit2D;
                    // We return and break out of the loop so any other collision hits don't go through the
                    // initial if statement making the CollisionInfo.ClimbableSurfaceHit = a new RaycastHit2D()
                    return;
				}
            }
        }

        #endregion

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
		protected override void CalculateVertical()
		{
			if(IsClimbing)
			{
				_calculatedVelocity.y = Direction.y * CurrentPhysics.ClimbSpeed.y;
			}

			if(!IsGrounded && !IsClimbing)
			{
                // This is related to the formula of: square root of ( -2 (gravity * height) )
                // https://en.wikipedia.org/wiki/Equations_for_a_falling_body#Example
                _calculatedVelocity.y -= Mathf.Sqrt( -2 * -CurrentPhysics.GravityScale * Time.deltaTime); 

				_calculatedVelocity.y = Mathf.Clamp(_calculatedVelocity.y,
					-CurrentPhysics.TerminalVelocity,
					CurrentPhysics.MaxUpForce);
			}

		}

		public virtual void CalculateSlope()
		{
			if(!_useSlopes)
				return;

			if(Direction.x > 0)
				_slopeNormal = Physics2D.Raycast(Bounds.BottomRight(), Vector2.down, .25f).normal;
			else
				_slopeNormal = Physics2D.Raycast(Bounds.BottomLeft(), Vector2.down, .25f).normal;

			StandingOnSlopeAngle = Vector2.Angle(_slopeNormal, Vector2.up);

			OnSlope = StandingOnSlopeAngle > 5;

			if(OnSlope)
			{
				IsGrounded = true;
				// TODO: Make the ability to walk up slopes.
				_calculatedVelocity = Vector3.ProjectOnPlane(_calculatedVelocity, _slopeNormal);
			}
		}

		protected override void Move()
		{
			//CalculateSlope();
			// Note to self: If we are standing on a platform and we move with the platform we gain the velocity of the platform ontop of our own. 
			// FIX THIS or the character becomes a mach 10 rocket sometimes.

			base.Move();
        }
		
		/// <summary>
		/// Calculates the current movement state that the player is currently in.
		/// </summary>
		/// <remarks>
		/// This needs to be moved into the Controller2d parent class.
		/// </remarks>
		protected override void CalculateMovementState()
		{

			if(CharacterState.CharacterStatus == CharacterStatus.Dead)
				return;


			// TODO: There are some places that set the values outside of this function. Find a way to make it where this function is the only needed one. Example IsJump in the Jumping Ability.

			if(IsClimbing)
			{
				if(_calculatedVelocity.y != 0)
					CharacterState.MovementState = MovementState.Climbing;
				else
					CharacterState.MovementState = MovementState.ClimbingIdle;
			}

			// If our velocity is negative we are either falling/gliding.
			if(_calculatedVelocity.y < 0 && !IsClimbing)
			{
				if(IsGliding)
					CharacterState.MovementState = MovementState.Gliding;
				else
				{
					// Need to remove the crouch check when I get the collider calculation more accurate on resizing.
					if(!IsCrouching)
						CharacterState.MovementState = MovementState.Falling;
				}
                IsFalling = true;
                IsJumping = false;
            }

			if(IsJumping)
				CharacterState.MovementState = MovementState.Jumping;

			if(IsGrounded)
			{
                IsFalling = false;

                if(IsCrouching)
				{
					CharacterState.MovementState = MovementState.Crouching;
					return;
				}
				
				CharacterState.MovementState = (Direction.x == 0) ? MovementState.Idle : MovementState.Walking;
			}
		}


		public virtual void ResizeCollider(Vector2 newSize)
		{
			// Need to keep track of the previous size if the collider was already resized once before, but wasn't reset to the default collider size.
			_previousColliderSize = _boxCollider.size;
			_modifiedColliderSize = newSize;
            _boxCollider.size = newSize;

            LowerToGround();
		}

        public void ResetColliderSize()
		{
			_boxCollider.size = _originalColliderSize;

			//TODO: Do checks if colliding on the sides or ceiling to make sure the default collider size doesn't click through them.

			// Put character above ground.
			if(IsGrounded)
			{
				transform.position += new Vector3(0, CollisionController.VerticalRayDistance, 0);

			}
		}

        public override void UpdatePhysics(MovementProperties movementProperties,
             PhysicsVolumeType volumeType = PhysicsVolumeType.None)
        {
			base.UpdatePhysics(movementProperties, volumeType);

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

#if UNITY_EDITOR

        private readonly List<Vector3> _listOfPoints = new();

        public void OnDrawGizmos()
        {
            _listOfPoints.Clear();
            _boxCollider = (_boxCollider == null) ? GetComponent<BoxCollider2D>() : _boxCollider;

            Vector2 startPosition;
            float stepPercent;
            int numberOfRays = CollisionController.VerticalRayAmount;
            Vector2 origin = Bounds.BottomLeft();
            Vector2 end = Bounds.BottomRight();

            numberOfRays = CollisionController.HoriztonalRayAmount;
            origin = Bounds.TopRight();
            end = Bounds.BottomRight();

            for(int x = 0; x < numberOfRays; x++) // Right
            {
                stepPercent = (float)x / (float)(numberOfRays - 1);
                startPosition = Vector2.Lerp(origin, end, stepPercent);
                _listOfPoints.Add(startPosition);
                _listOfPoints.Add(startPosition + new Vector2(CollisionController.HoriztonalRayDistance, 0));
            }

            for(int x = 0; x < numberOfRays; x++) // Left
            {
                stepPercent = (float)x / (float)(numberOfRays - 1);
                startPosition = Vector2.Lerp(Bounds.BottomLeft(), Bounds.TopLeft(), stepPercent);
                _listOfPoints.Add(startPosition);
                _listOfPoints.Add(startPosition - new Vector2(CollisionController.HoriztonalRayDistance, 0));
            }

            ReadOnlySpan<Vector3> pointsAsSpan = CollectionsMarshal.AsSpan(_listOfPoints);
            Gizmos.DrawLineList(pointsAsSpan);

            if(CollisionInfo.ClimbableSurfaceHit)
                Gizmos.DrawWireSphere(CollisionInfo.ClimbableSurfaceHit.point, .25f);

        }
#endif
    }
}