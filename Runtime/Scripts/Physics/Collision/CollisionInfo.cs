using System;
using System.Collections.Generic;

using UnityEngine;

namespace SF.Physics
{
    /// <summary>
    /// Keeps track of the current frames collision information.
    /// This is used in all character controllers to help with knowing when a collision action needs to be invoked.
    /// <see cref="SF.Characters.Controllers.GroundedController2D"/> for an example implementation. 
    /// </summary>
    /// <remarks>
    ///	These are collisions from Collision based callbacks that interact with a non-trigger collider.
    /// Used for platforms, walls, and physical objects that can stop the player.
    /// </remarks>
    [Serializable]
	public class CollisionInfo
	{

		/// <summary>
		/// The <see cref="BoxCollider2D"/> to use for collision and contacts checks.
		/// </summary>
		public BoxCollider2D Collider2D;
		
		public GameObject StandingOnObject;


		public int ContactHitCount;
        public List<ContactPoint2D> CeilingContacts = new();
        public List<ContactPoint2D> GroundContacts = new();
        public List<ContactPoint2D> RightContacts = new();
        public List<ContactPoint2D> LeftContacts = new();

        [NonSerialized] public ContactFilter2D GroundFilter2D = DefaultGroundFilter2D;
        [NonSerialized] public ContactFilter2D CeilingFilter2D = DefaultCeilingFilter2D;
        [NonSerialized] public ContactFilter2D RightFilter2D = DefaultRightFilter2D;
        [NonSerialized] public ContactFilter2D LeftFilter2D = DefaultLeftFilter2D;
        
        public static ContactFilter2D DefaultGroundFilter2D = new ContactFilter2D()
        {
	        useTriggers = false,
	        useLayerMask = true,
	        layerMask = (LayerMask)(-1),
	        useDepth = false,
	        useOutsideDepth = false,
	        minDepth = float.NegativeInfinity,
	        maxDepth = float.PositiveInfinity,
	        useNormalAngle = true,
	        useOutsideNormalAngle = false,
	        minNormalAngle = 85f,
	        maxNormalAngle = 95f
        };
        public static ContactFilter2D DefaultCeilingFilter2D = new ContactFilter2D()
        {
	        useTriggers = false,
	        useLayerMask = true,
	        layerMask = (LayerMask)(-1),
	        useDepth = false,
	        useOutsideDepth = false,
	        minDepth = float.NegativeInfinity,
	        maxDepth = float.PositiveInfinity,
	        useNormalAngle = true,
	        useOutsideNormalAngle = false,
	        minNormalAngle = 265f,
	        maxNormalAngle = 275f
        };
        public static ContactFilter2D DefaultRightFilter2D = new ContactFilter2D()
        {
	        useTriggers = false,
	        useLayerMask = true,
	        layerMask = (LayerMask)(-1),
	        useDepth = false,
	        useOutsideDepth = false,
	        minDepth = float.NegativeInfinity,
	        maxDepth = float.PositiveInfinity,
	        useNormalAngle = true,
	        useOutsideNormalAngle = false,
	        minNormalAngle = 175f,
	        maxNormalAngle = 185f
        };
        public static ContactFilter2D DefaultLeftFilter2D = new ContactFilter2D()
        {
	        useTriggers = false,
	        useLayerMask = true,
	        layerMask = (LayerMask)(-1),
	        useDepth = false,
	        useOutsideDepth = false,
	        minDepth = float.NegativeInfinity,
	        maxDepth = float.PositiveInfinity,
	        useNormalAngle = true,
	        useOutsideNormalAngle = false,
	        minNormalAngle = -5,
	        maxNormalAngle = 5f
        };
        
        
		/// <summary>
		/// Keeps track if the current character controller using this collision info is colliding with anything on the right that matches any of it's collision mask filters.
		/// </summary>
		public bool IsCollidingRight;
        /// <summary>
        /// Keeps track if the current character controller using this collision info is colliding with anything on the left that matches any of it's collision mask filters.
        /// </summary>
        public bool IsCollidingLeft;
        /// <summary>
        /// Keeps track if the current character controller using this collision info is colliding with anything above that matches any of it's collision mask filters.
        /// </summary>
        public bool IsCollidingAbove;
        /// <summary>
        /// Keeps track if the current character controller using this collision info is colliding with anything below that matches any of it platform collision filters.
        /// </summary>
        public bool IsGrounded;
        private bool _wasGroundedLastFrame;

        /// <summary>
        /// The result of the last raycast used to check if there is any climbable surfaces. If no raycast detected a climable surface this will return false when checked if null in an if statement. 
        /// </summary>
        [SerializeField] private RaycastHit2D _climbableSurfaceHit;
        public RaycastHit2D ClimbableSurfaceHit
        {
            get { return _climbableSurfaceHit; }
            set
            {
                if(value)
                {
					if(value.collider.TryGetComponent(out ClimbableSurface climable))
					{
                        _climbableSurfaceHit = value;
                        ClimbableSurface = climable;
					}
					else
					{
						ClimbableSurface = null;
					}
                }
				else
					ClimbableSurface = null;
            }
        }

        /// <summary>
        /// The last detected climable surface. If no surface is currently found that is climable this will be set to null. 
        /// </summary>
        [SerializeField] private ClimbableSurface _climableSurface;
        public ClimbableSurface ClimbableSurface
		{
			get { return _climableSurface; }
			set 
			{ 
				if(value == null)
					WasClimbing = false;
				_climableSurface = value;
			}
		}

		// The below is for seeing if we were colliding in a direction on the previous frame. These allow us to see when we need to invoke any of the oncolliding events by comparing them to the current frame after doing the current frames collision checks.

        //TODO: Make summary documentation notes for these so they appear in the documentation.
		[NonSerialized] public bool WasCollidingRight;
		[NonSerialized] public bool WasCollidingLeft;
		[NonSerialized] public bool WasCollidingAbove;
		[NonSerialized] public bool WasCollidingBelow;
		[NonSerialized] public bool WasClimbing;

        // These are for invoking actions on the frame a new collision takes place.
        //TODO: Make summary documentation notes for these so they appear in the documentation.
        public Action OnCollidedRightHandler;
		public Action OnCollidedLeftHandler;
		public Action OnCeilingCollidedHandler;
		public Action OnGroundedHandler;

		public void CheckCollisions()
		{
			_wasGroundedLastFrame = IsGrounded;
			
			GroundCollisionChecks();
			CeilingChecks();
			SideCollisionChecks();
		}
		
		public void SideCollisionChecks()
		{
			Collider2D.GetContacts(RightFilter2D, RightContacts);
			Collider2D.GetContacts(LeftFilter2D, LeftContacts);
			
			IsCollidingRight = RightContacts.Count > 0;
			IsCollidingLeft = LeftContacts.Count > 0;
		}
		
		public void GroundCollisionChecks()
		{
			Collider2D.GetContacts(GroundFilter2D,GroundContacts);
			
			// If we did collide with something below.
			if(GroundContacts.Count > 0)
			{
                // If we are standing on something keep track of it. This can be useful for things like moving platforms.
                StandingOnObject = GroundContacts[0].collider.gameObject;
                IsGrounded = true;
                /* Moving Platforms
                 
				// Only set the transform if we already are not a child of another game object.
				// If we don't do this than we will constantly be restuck to the moving platforms transform.
				if(transform.parent == null && LayerMask.LayerToName(CollisionInfo.BelowHit.collider.gameObject.layer) == "MovingPlatforms")
                    transform.SetParent(CollisionInfo.BelowHit.collider.gameObject.transform);
                */

            }
			else // If we are not colliding with anything below.
			{
				StandingOnObject = null;
				IsGrounded = false;
				/*
                if(transform.parent != null)
					transform.SetParent(null);
				*/
            }

			// If not grounded last frame, but grounded this frame call OnGrounded
			if(!_wasGroundedLastFrame && IsGrounded)
			{
				OnGroundedHandler?.Invoke();
			}
        }
		
		public void CeilingChecks()
		{
			Collider2D.GetContacts(CeilingFilter2D,CeilingContacts);
			IsCollidingAbove = CeilingContacts.Count > 0;
		}
		
		/// <summary>
		/// Checks to see what sides might have a new collision that was started the current frame. If a new collision is detected on the side invoke the action related to that sides collisions.
		/// </summary>
		protected virtual void CheckOnCollisionActions()
		{
			// If we were not colliding on a side with anything last frame, but is now Invoke the OnCollisionActions.

			// Right Side
			if(!WasCollidingRight && IsCollidingRight)
				OnCollidedRightHandler?.Invoke();

			// Left Side
			if(!WasCollidingLeft && IsCollidingLeft)
				OnCollidedLeftHandler?.Invoke();

			// Above Side
			if(!WasCollidingAbove && IsCollidingAbove)
				OnCeilingCollidedHandler?.Invoke();

			//Below Side
			if(!_wasGroundedLastFrame && IsGrounded)
				OnCollidedRightHandler?.Invoke();
		}
	}
}
