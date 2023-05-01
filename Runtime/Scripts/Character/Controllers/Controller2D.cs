using SF.Physics.Helpers;

using UnityEngine;

namespace SF.Characters.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
	public class Controller2D : MonoBehaviour
	{
		[field:SerializeField] public Vector2 Velocity { get; protected set; }
		protected Vector2 _calculatedVelocity;
		protected Vector2 _controllerForce;
		#region Components  
		protected BoundsData _boundsData;
		protected Rigidbody2D _rigidbody2D;
		#endregion

		#region Lifecycle Methods
		private void Awake()
		{
			_rigidbody2D = GetComponent<Rigidbody2D>();
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
			OnStart();
		}
		protected virtual void OnStart()
		{

		}

		private void FixedUpdate()
		{
			CalculateHorizontal();
			CalculateVertical();
			OnFixedUpdate();
		}

		protected virtual void OnFixedUpdate()
		{
			Move();
		}
		#endregion

		#region Movement Calculations
		protected virtual void Move()
		{
			_rigidbody2D.AddForce(_calculatedVelocity, ForceMode2D.Impulse);
			Velocity = _rigidbody2D.velocity;
			_controllerForce = Vector2.zero;
			_calculatedVelocity = Vector2.zero;
		}
		protected virtual void CalculateHorizontal()
		{
			_calculatedVelocity += _controllerForce;
		}
		protected virtual void CalculateVertical()
		{
		}
		protected virtual void AddForce(Vector2 force)
		{
			_controllerForce += force;
		}
		protected virtual void AddHorizontalForce(float horizontalForce)
		{
			_controllerForce.x += horizontalForce;
		}
		protected virtual void AddVerticalForce(float verticalForce)
		{
			_controllerForce.y += verticalForce;
		}
		#endregion
	}
}
