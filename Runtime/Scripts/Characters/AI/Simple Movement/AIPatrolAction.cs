using UnityEngine;
using SF.Characters.Controllers;
namespace SF.AIModule
{
	[System.Serializable]
    public class AIPatrolAction : AIActionBase
	{
		
		[SerializeField] private bool _startingRight = true;
		public AIPatrolAction(Controller2D controller)
		{
			Controller2D = controller;
		}

		public override void Init()
		{
			if(Controller2D == null)
				return;

			Controller2D.CollisionInfo.OnCollidedLeft += OnCollidingLeft;
			Controller2D.CollisionInfo.OnCollidedRight += OnCollidingRight;
		}

		public override void DoAction()
		{
			if(Controller2D == null)
				return;

			Controller2D.Direction = _startingRight
				? Vector2.right : Vector2.left;
		}

		private void OnCollidingLeft()
		{
			if(Controller2D.Direction == Vector2.left)
				Controller2D.ChangeDirection();
		}

		private void OnCollidingRight()
		{
			if(Controller2D.Direction == Vector2.right)
				Controller2D.ChangeDirection();
		}
	}
}
