using SF.Characters.Controllers;

using UnityEngine;

namespace SF.Characters
{
	[RequireComponent(typeof(Controller2D), typeof(Animator), typeof(SpriteRenderer))]
    public class Character2D : MonoBehaviour
    {
        [Header("Components")]
		protected Controller2D _controller2D;
		protected Animator _animator;

		#region Lifecycle Functions  
		private void Awake()
		{
			_controller2D = GetComponent<Controller2D>();
			_animator = GetComponent<Animator>();
			Init();
		}

		private void Start()
		{

		}
		#endregion // end of Lifecycle Functions
		
		protected virtual void Init()
		{

		}

		protected virtual void UpdateAnimator()
		{

		}
	}
}
