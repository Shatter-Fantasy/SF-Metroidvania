using System;
using UnityEngine;

namespace SF.PhysicsLowLevel
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
    public abstract class CollisionInfoBase
    {
        public GameObject ControlledGameObject;
        public GameObject StandingOnObject;
        
        [Header("Collision Properties")]
        public bool CollisionActivated = true;

        /// <summary>
        /// The threshold for the normals of a contact to pass to be considered an actual collision. 
        /// </summary>
        /// <example>
        ///	Useful for removing ghost collisions and also collisions on physics shapes with rounded corners like capsules and circles.
        /// </example>
        public float CollisionContactThreshold = 0.5f;
        
        
        /// <summary>
        /// Keeps track if the current character controller using this collision info is colliding with anything on the right that matches any of it's collision mask filters.
        /// </summary>
        [Header("Current Colliding Direction")]
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
        /// Keeps track if there is anything below that can make a contact point callback on collision.
        /// </summary>
        /// <remarks>
        /// This is more than just platforms. It can be anything like projectiles, magic, gust of wind, enemies head for bouncing.
        /// <see cref="IsGrounded"/> is specifically for platforms.
        /// </remarks>
        public bool IsCollidingBelow;
        /// <summary>
        /// Keeps track if the current character controller using this collision info is colliding with anything below that matches any of it platform collision filters.
        /// </summary>
        public bool IsGrounded;
        /// <summary>
        /// Was the <see cref="ControlledGameObject"/> on the ground last frame. 
        /// </summary>
        /// <remarks>
        /// Note even flying objects can be grounded. Imagine an enemy that that is knocked down or stunned and it hits the ground. 
        /// </remarks>
        public bool WasGroundedLastFrame;
        
        //TODO: Make summary documentation notes for these so they appear in the documentation.
        
        [NonSerialized] public bool WasCollidingRight;
        [NonSerialized] public bool WasCollidingLeft;
        [NonSerialized] public bool WasCollidingAbove;
        /// <summary>
        /// Was the <see cref="ControlledGameObject"/> colliding with anything below.
        /// </summary>
        /// <remarks>
        /// This is not the same as grounded. Grounded is touching a platform below while this can be anything.
        /// Enemies head to bounce off of, spell, projectile, and so forth.
        /// </remarks>
        [NonSerialized] public bool WasCollidingBelow;
        [NonSerialized] public bool WasClimbing;
        
        // These are for invoking actions on the frame a new collision takes place.
        //TODO: Make summary documentation notes for these so they appear in the documentation.
        public Action OnCollidedRightHandler;
        public Action OnCollidedLeftHandler;
        public Action OnCeilingCollidedHandler;
        public Action OnGroundedHandler;
        
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
        
        public virtual void Initialize()
        {

        }

        
        public virtual void CheckCollisions()
        {
            WasCollidingLeft = IsCollidingLeft;
            WasCollidingRight = IsCollidingRight;
            WasCollidingAbove = IsCollidingAbove;
            WasCollidingBelow = IsGrounded;
			
            WasGroundedLastFrame = IsGrounded;
			
            GroundCollisionChecks();
            CeilingChecks();
            SideCollisionChecks();
            CheckOnCollisionActions();
        }
        public virtual void SideCollisionChecks()
        {
        }
        
        public virtual void GroundCollisionChecks()
		{
			// If not grounded last frame, but grounded this frame call OnGrounded
			if(!WasGroundedLastFrame && IsGrounded)
			{
				OnGroundedHandler?.Invoke();
			}
        }
		
		public virtual void CeilingChecks()
		{

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
			if(!WasGroundedLastFrame && IsGrounded)
				OnGroundedHandler?.Invoke();
		}
    }
}
