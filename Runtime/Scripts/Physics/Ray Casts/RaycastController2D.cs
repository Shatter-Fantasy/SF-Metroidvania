using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.Physics
{
    public class RaycastController2D : MonoBehaviour
    {
    
        /// <summary>
        /// If this is negative that means we are in the ground and need our position corrected.
        /// </summary>
        public float DistanceToGround;

        public List<RaycastHit2D> CollisionHits;

        /// <summary>
        /// The raycast hit detected during the ground collision check for below the character controller being used.
        /// This will return false when checked if null in an if statement when there is nothing being hit.
        /// </summary>
        public RaycastHit2D BelowHit;
        /// <summary>
        /// The raycast hit detected during the ceiling collision check for the character controller being used.
        /// This will return false when checked if null in an if statement when there is nothing being hit.
        /// </summary>
        public RaycastHit2D CeilingHit;
        /// <summary>
        /// The raycast hit detected during the side collision check on the right side for the character controller being used.
        /// This will return false when checked if null in an if statement when there is nothing being hit.
        /// </summary>
        public RaycastHit2D RightHit;
        /// <summary>
        /// The last raycast hit detected during the side collision check on the left side for the character controller being used.
        /// This will return false when checked if null in an if statement when there is nothing being hit.
        /// </summary>
        public RaycastHit2D LeftHit;

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
        /// Keeps track if the current character controller using this collision info is colliding with anything below that matches any of it's collision mask filters.
        /// </summary>
        public bool IsCollidingBelow;

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
        public Action OnCollidedRight;
		public Action OnCollidedLeft;
		public Action OnCollidedAbove;
		public Action OnCollidedBelow;
		
		/*
		protected RaycastHit2D DebugBoxCast(Vector2 origin, 
			Vector2 size,
			float angle,
			Vector2 direction, 
			float distance, 
			LayerMask layerMask)
		{
#if UNITY_EDITOR
			Debug.DrawLine(origin, origin + (direction * distance));
#endif
			return Physics2D.BoxCast(origin, size,angle,direction, distance, layerMask);
		}

		protected RaycastHit2D DebugRayCast(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask)
		{
#if UNITY_EDITOR

			Debug.DrawLine(origin, origin + (direction * distance));
#endif
			return Physics2D.Raycast(origin, direction, distance, layerMask);
		}

		public bool RaycastMultiple(Vector2 origin, Vector2 end, Vector2 direction, float distance, ContactFilter2D contactFilter2D, int numberOfRays = 4)
		{
			return RaycastMultiple(origin, end, direction, distance, contactFilter2D.layerMask, numberOfRays);
		}
		public bool RaycastMultiple(Vector2 origin, Vector2 end, Vector2 direction, float distance, LayerMask layerMask, int numberOfRays = 4)
		{
			RaycastHit2D hasHit;
			Vector2 startPosition;
			float stepPercent;
			for(int x = 0; x < numberOfRays; x++)
			{
				stepPercent = (float)x / (float)(numberOfRays - 1);
				startPosition = Vector2.Lerp(origin, end, stepPercent);
				hasHit = DebugRayCast(startPosition, direction, distance, layerMask);

				if(hasHit)
				{
					if(direction.x > 0 && direction.y == 0 )
						RightHit = hasHit;
					else if(direction.x < 0 && direction.y == 0)
						LeftHit = hasHit;

					if(direction.y > 0)
						CeilingHit = hasHit;
					else if(direction.y < 0)
						BelowHit = hasHit;

					CollisionHits.Add(hasHit);
					return true;
				}
			}

			return false;
		}
		*/
    }
}
