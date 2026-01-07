using UnityEngine;
using UnityEngine.InputSystem;

using SF.InputModule;
using SF.PhysicsLowLevel;

namespace SF.AbilityModule.Characters
{
    public class ClimbAbility : AbilityCore, IInputAbility
    {
        protected ClimbableSurface ClimableSurface => _controller2d.CollisionInfo.ClimbableSurface;
        protected CollisionInfoBase CollisionInfo => _controller2d.CollisionInfo;

        public Vector2 ClimbSpeed = new Vector2(5,5);
        public Vector2 ColliderBoxSize = new Vector2(1.22f, 2.31f);

        protected override void OnInitialize()
        {
            // TODO: Reset jumps on starting climb.
        }

        protected override void OnAbilityInterruption()
        {
            OnClimbEnd();
        }
        protected override bool CheckAbilityRequirements()
        {
            if(ClimableSurface == null)
                return false;

            return true;
        }

        protected override void OnUpdate()
        {
            if(!_controller2d.IsClimbing)
                return;

            Debug.Log("Doing the OnUpdate state checks.");
            if(!CollisionInfo.ClimbableSurface || _controller2d.IsJumping)
                OnAbilityInterruption();
        }

        private void OnClimbMove(InputAction.CallbackContext ctx)
        {

            if(!_controller2d.IsClimbing)
                return;

            Vector2 moveDirection = ctx.ReadValue<Vector2>();

            if(moveDirection.y != 0)
                _controller2d.Direction = moveDirection;
        }

        private void OnClimbMoveCancelled(InputAction.CallbackContext ctx)
        {
            if(!_controller2d.IsClimbing)
                return;

            Vector2 moveDirection = ctx.ReadValue<Vector2>();

            _controller2d.Direction = new Vector2(_controller2d.Direction.x, 0);
        }

        private void OnClimb(InputAction.CallbackContext ctx)
        {
            // Can we climb in the current character/game state.
            if(!CanStartAbility())
                return;
            
            if(!_controller2d.IsClimbing)
                OnClimbStart();
            else
                OnClimbEnd();
        }

        private void OnClimbStart()
        {
            /*
            Vector2 characterOffset = new Vector2
                (CollisionInfo.ClimbableSurfaceHit.point.x + _controller2d.CollisionController.HoriztonalRayDistance,
                CollisionInfo.ClimbableSurfaceHit.point.y);

            _controller2d.transform.position = (Vector3)characterOffset;
            */
            
            _controller2d.IsClimbing = true;
            _controller2d.IsFalling = false;
            _controller2d.IsGliding = false;
            _controller2d.IsJumping = false;
            _controller2d.SetVerticalVelocity(0);
            _controller2d.ResizeCollider(ColliderBoxSize);
        }

        private void OnClimbEnd()
        {
            _controller2d.IsClimbing = false;
            _controller2d.ResetColliderSize();
            _controller2d.CollisionInfo.WasClimbing = false;
        }

        private void OnEnable()
        {
            SFInputManager.Controls.Player.Enable();

            SFInputManager.Controls.Player.Interact.performed += OnClimb;
            SFInputManager.Controls.Player.Move.performed += OnClimbMove;
            SFInputManager.Controls.Player.Move.canceled += OnClimbMoveCancelled;
        }

        private void OnDisable()
        {
            if(SFInputManager.Controls == null) return;

            SFInputManager.Controls.Player.Interact.performed -= OnClimb;
            SFInputManager.Controls.Player.Move.performed -= OnClimbMove;
            SFInputManager.Controls.Player.Move.canceled -= OnClimbMoveCancelled;
        }
    }
}
