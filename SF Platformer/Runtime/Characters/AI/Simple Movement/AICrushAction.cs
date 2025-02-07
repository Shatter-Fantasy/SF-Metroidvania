using UnityEngine;

using SF.Characters.Controllers;
using SF.SpawnModule;

namespace SF.AIModule
{
	public enum CycleTypes
	{
		PingPong,
		OnFullCycle
	}


	[System.Serializable]
	public class AICrushAction : AIActionBase
	{

		/// <summary>
		/// Should the AI automatically being it's crush logic.
		/// </summary>
		[SerializeField] private bool _startAutomatically = false;
		[SerializeField] private CycleTypes _movementCycle;
        public float FallingSpeed = 10f;
		public float RisingSpeed = 6f;

		/// <summary>
		/// The physics layers that will take damagee crushed by this object.
		/// </summary>
		[SerializeField] LayerMask _damageableLayers;
		[SerializeField] private int _damage = 1;
		[SerializeField] private bool _isInstantKill = false;


		public AICrushAction(Controller2D controller)
		{
			Controller2D = controller;
		}

        private void Awake()
        {
			if(!IsControlledByAIBrain)
				Init();
        }

        public override void Init()
		{
			Controller2D = GetComponent<Controller2D>();

			if(Controller2D == null)
				return;

			Controller2D.CollisionInfo.OnCollidedAbove += OnCollidingAbove;
			Controller2D.CollisionInfo.OnCollidedBelow += OnCollidingBelow;

			// If this instance of AICrushAction is not being controlled by a brain and it should start automatically.
			// Start the action.
			if(!IsControlledByAIBrain && _startAutomatically)
				DoAction();
		}

		public override void DoAction()
		{
			if(Controller2D == null || !IsEnabled )
				return;

			Controller2D.SetVerticalVelocity(-FallingSpeed);
		}

		private void OnCollidingAbove()
		{
			Controller2D.SetVerticalVelocity(-FallingSpeed);
		}

		private void OnCollidingBelow()
		{
            if(Controller2D.CollisionInfo.BelowHit.collider.TryGetComponent(out Health health ) 
				&& CollisionMaskCheck())
			{
				
				if(_isInstantKill)
					health.InstantKill();
				else
					health.TakeDamage(_damage);
			}

			if(Controller2D is GroundedController2D controller2D)
			{
				controller2D.IsJumping = true;
			}
			Controller2D.SetVerticalVelocity(RisingSpeed);
		}

		private bool CollisionMaskCheck()
		{
			// This is a fun bitwise operator to see if the colliding object's layer is in the damageable layer mask.
			return _damageableLayers == (_damageableLayers | (1 << Controller2D.CollisionInfo.BelowHit.collider.gameObject.layer));
		}
	}
}
