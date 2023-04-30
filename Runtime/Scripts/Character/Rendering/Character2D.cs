using SF.Characters.Controllers;

using UnityEngine;

namespace SF.Characters
{
	[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class Character2D : MonoBehaviour
    {

		public bool IsPlayer = false;

		#region Components
		[Header("Components")]
		protected SpriteRenderer _spriteRend;
		protected Animator _animator;
		#endregion


		#region Lifecycle Functions  
		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_spriteRend = GetComponent<SpriteRenderer>();
			Init();
		}

		private void Start()
		{

		}
		#endregion
		private void Init()
		{
			OnInit();
		}

		protected virtual void OnInit()
		{

		}


		protected virtual void UpdateAnimator()
		{

		}
	}
}
