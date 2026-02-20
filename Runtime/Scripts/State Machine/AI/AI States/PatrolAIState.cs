using UnityEngine;

namespace SF.StateMachine
{
	using PhysicsLowLevel;
	using SF.StateMachine.Core;

    public class PatrolAIState : StateCore
    {
        public bool StartingRight = true;
		public bool DoesTurnOnHoles = true;
		
		[SerializeField] private bool _isHoleAhead;

        protected override void OnInit(ControllerBody2D controllerBody2D)
		{
			_controllerBody2D = controllerBody2D;
		}
        
        protected override void OnStart()
		{
			if(_controllerBody2D == null)
				return;
			
			_controllerBody2D.CollisionInfo.OnCollidedLeftHandler += OnCollidingLeft;
			_controllerBody2D.CollisionInfo.OnCollidedRightHandler += OnCollidingRight;
			
			_controllerBody2D.SetDirection(1);
		}

        protected override void OnUpdateState()
        {
			HoleDetection();
        }

        protected override void OnStateEnter()
        {
	        _controllerBody2D.Direction = StartingRight
		        ? Vector2.right : Vector2.left;
        }

		private void HoleDetection()
		{
			if(_controllerBody2D == null 
			   || _controllerBody2D.IsFalling 
			   || !DoesTurnOnHoles )
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
			if(_controllerBody2D.Direction == Vector2.left)
				_controllerBody2D.SetDirection(1);
		}
		private void OnCollidingRight()
		{
            if(_controllerBody2D.Direction == Vector2.right)
                _controllerBody2D.SetDirection(-1);
        }
	}
}