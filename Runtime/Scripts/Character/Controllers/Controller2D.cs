using SF.Physics;

using UnityEngine;

namespace SF.Characters.Controllers
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Character2D))]
	public class Controller2D : MonoBehaviour
	{
		[Header("Physics Properties")]
		public MovementProperties DefaultPhysics;
		public MovementProperties CurrentPhysics;

		[Header("Contact Filters")]
		public ContactFilter2D GroundFilter;
		public Rigidbody2D.SlideMovement SlideMovement;

		#region Components  
		public Bounds Bounds;
		protected BoxCollider2D _boxCollider;
		protected Rigidbody2D _rigidbody2D;
		#endregion // end of Components

		#region Booleans
		public bool IsGrounded = false;
		public bool IsFalling = false;
		#endregion

		#region Bounds Shorthands
		protected Vector2 TopCenterBounds;
		protected Vector2 BottomCenterBounds;
		protected Vector2 LeftCenterBounds;
		protected Vector2 RightCenterBounds;
		#endregion

		#region Lifecycle Methods
		private void Awake()
		{
			_boxCollider = GetComponent<BoxCollider2D>();
			_rigidbody2D = GetComponent<Rigidbody2D>();
			OnAwake();
		}
		protected virtual void OnAwake()
		{
			_rigidbody2D.gravityScale = 0;
		}
		private void Start()
		{
			OnStart();
		}
		protected virtual void OnStart()
		{

		}

		protected virtual void FixedUpdate()
		{
			CalculateBounds();
			GroundChecks();
		}
		#endregion

		#region Movement Calculations
		protected virtual void Move()
		{

		}
		protected virtual void CalculateHorizontal()
		{

		}
		protected virtual void CalculateVertical()
		{

		}
		#endregion

		#region Collision Calculation
		protected void CalculateBounds()
		{
			Bounds = _boxCollider.bounds;
			TopCenterBounds = new Vector2(Bounds.center.x, Bounds.max.y);
			BottomCenterBounds = new Vector2(Bounds.center.x, Bounds.min.y);
			LeftCenterBounds = new Vector2(Bounds.min.x, Bounds.center.y);
			RightCenterBounds = new Vector2(Bounds.max.x, Bounds.center.y);
		}

		protected virtual void GroundChecks()
		{
		}
		#endregion

		public void UpdatePhysics(MovementProperties movementProperties)
		{
			CurrentPhysics = movementProperties;
		}
	}
}
