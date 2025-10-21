using System;
using System.Collections.Generic;
using SF.Characters.Controllers;


using UnityEngine;

namespace SF.Physics
{
    /// <summary>
    /// Keeps track of the current frames collision information.
    /// This is used in all character controllers to help with knowing when a collision action needs to be invoked.
    /// <see cref="GroundedController2D"/> for an example implementation. 
    /// </summary>
    /// <remarks>
    /// Depending on if you are using the low or high level physics you will either use <see cref="Rigidbody2D"/> and <see cref="Collider2D"/>,
    /// or you will use <see cref="UnityEngine.LowLevelPhysics2D.PhysicsShape"/> and <see cref="UnityEngine.LowLevelPhysics2D.PhysicsBody"/>
    ///	These are collisions from Collision based callbacks that interact with a non-trigger collider.
    /// Used for platforms, walls, and physical objects that can stop the player.
    /// </remarks>
    [Serializable]
	public class CollisionInfo : CollisionInfoBase
	{
		/// <summary>
		/// The <see cref="BoxCollider2D"/> to use for collision and contacts checks.
		/// </summary>
		public BoxCollider2D Collider2D;
		
		[Header("Platform Layers")]
		[SerializeField] protected LayerMask _platformLayer;
		[SerializeField] protected LayerMask _movingPlatformLayer;
		[SerializeField] protected LayerMask _oneWayPlatformFilter;
		protected int OneWayFilterBitMask => _platformLayer & _oneWayPlatformFilter;

		// TODO: Add the Above and Below Filter now that grounded and below collisions are two different things.
		// TODO: Make a custom VisualElement for ReadOnly List so I can mark a list as readonly and still have the values show in the inspector.
#region MyRegion
		public List<ContactPoint2D> CeilingContacts = new();
		public List<ContactPoint2D> GroundContacts = new();
		public List<ContactPoint2D> RightContacts = new();
		public List<ContactPoint2D> LeftContacts = new();
#endregion
      

        // TODO: Add the Above and Below Filter now that grounded and below collisions are two different things.
#region ContactFilters
        [NonSerialized] public ContactFilter2D GroundFilter2D = DefaultGroundFilter2D;
        [NonSerialized] public ContactFilter2D CeilingFilter2D = DefaultCeilingFilter2D;
        [NonSerialized] public ContactFilter2D RightFilter2D = DefaultRightFilter2D;
        [NonSerialized] public ContactFilter2D LeftFilter2D = DefaultLeftFilter2D;
#endregion

		// TODO: Add the Above and Below Filter now that grounded and below collisions are two different things.
        #region DefaultFilter values
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
        #endregion
        

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
        
		public void Initialize(BoxCollider2D collider2d = null)
		{
			if (collider2d != null)
				Collider2D = collider2d;
			
			GroundFilter2D.layerMask = _platformLayer;
			CeilingFilter2D.layerMask = _platformLayer;
			RightFilter2D.layerMask = _platformLayer;
			LeftFilter2D.layerMask = _platformLayer;
		}
		
		public override void SideCollisionChecks()
		{
			Collider2D.GetContacts(RightFilter2D, RightContacts);
			Collider2D.GetContacts(LeftFilter2D, LeftContacts);
			
			IsCollidingRight = RightContacts.Count > 0;
			IsCollidingLeft = LeftContacts.Count > 0;
		}
		
		public override void GroundCollisionChecks()
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
			if(!WasGroundedLastFrame && IsGrounded)
			{
				OnGroundedHandler?.Invoke();
			}
        }
		
		public override void CeilingChecks()
		{
			Collider2D.GetContacts(CeilingFilter2D,CeilingContacts);
			IsCollidingAbove = CeilingContacts.Count > 0;
		}
		
		/// <summary>
		/// Checks to see what sides might have a new collision that was started the current frame. If a new collision is detected on the side invoke the action related to that sides collisions.
		/// </summary>
		protected override void CheckOnCollisionActions()
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
			if(!WasGroundedLastFrame && IsGrounded)
				OnGroundedHandler?.Invoke();
		}
	}
}
