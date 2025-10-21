using SF.Characters.Controllers;
using SF.Platformer.Utilities;
using SF.StateMachine.Core;

using UnityEngine;

namespace SF.StateMachine
{
    public class PatrolAIState : StateCore
    {
        public bool StartingRight = true;
		public bool DoesTurnOnHoles = true;
		
		[SerializeField] private bool _isHoleAhead;

        protected override void OnInit(RigidbodyController2D rigidbodyController2D)
		{
			_rigidbodyController = rigidbodyController2D;
		}
        
        protected override void OnStart()
		{
			if(_rigidbodyController == null)
				return;
			
			_rigidbodyController.CollisionInfo.OnCollidedLeftHandler += OnCollidingLeft;
			_rigidbodyController.CollisionInfo.OnCollidedRightHandler += OnCollidingRight;
		}

        protected override void OnUpdateState()
        {
			HoleDetection();
        }

        protected override void OnStateEnter()
        {
	        _rigidbodyController.Direction = StartingRight
		        ? Vector2.right : Vector2.left;
        }

		private void HoleDetection()
		{
			if(_rigidbodyController == null || _rigidbodyController is GroundedController2D { IsFalling:true } || !DoesTurnOnHoles )
				return;
			
			/*
			RaycastHit2D hit2D = new RaycastHit2D();
			
			
			if(_controller.Direction == Vector2.left)
			{
                hit2D = Physics2D.Raycast(_controller.Bounds.BottomLeft(),
						Vector2.down,
						_controller.CollisionController.VerticalRayDistance + _controller.CollisionController.SkinWidth,
						_controller.PlatformFilter.layerMask
					);
			}
			else if(_controller.Direction == Vector2.right)
			{

                hit2D = Physics2D.Raycast(_controller.Bounds.BottomRight(),
						Vector2.down,
						_controller.CollisionController.VerticalRayDistance + _controller.CollisionController.SkinWidth,
						_controller.PlatformFilter.layerMask
					);
			}

			_isHoleAhead = !hit2D;

            if(_isHoleAhead)
            {
                _controller.ChangeDirection();
            }
            */
        }
        
		private void OnCollidingLeft()
		{
			if(_rigidbodyController.Direction == Vector2.left)
				_rigidbodyController.SetDirection(1);
		}
		private void OnCollidingRight()
		{
            if(_rigidbodyController.Direction == Vector2.right)
                _rigidbodyController.SetDirection(-1);
        }
	}
}