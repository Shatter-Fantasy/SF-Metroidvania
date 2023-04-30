using SF.Physics.Helpers;

using UnityEngine;

namespace SF.Characters.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
	public class Controller2D : MonoBehaviour
	{
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
			OnFixedUpdate();
		}

		protected virtual void OnFixedUpdate()
		{

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
	}
}
