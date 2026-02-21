using UnityEngine;

namespace SF.Characters
{
    using Weapons;
    
    public class PlayerRenderer2D : CharacterRenderer2D
    {
        
        protected override void UpdateAnimatorParameters()
        {
            if (_controllerBody2D?.CharacterState.CharacterStatus == CharacterStatus.Dead)
            {
                Animator.Play(DeathAnimationHash,0);
                return;
            }
		
            if (_controllerBody2D?.CharacterState.AttackState != AttackState.NotAttacking)
                return;
		
            if (_controllerBody2D is null)
                return;
		
            /* All Controller2D have the next set of parameters*/
            Animator.SetFloat(XSpeedAnimationHash, Mathf.Abs(_controllerBody2D.Direction.x));

            // Grounded States
            Animator.SetBool(IsGroundedAnimationHash, _controllerBody2D.CollisionInfo.IsGrounded);
            Animator.SetBool("IsCrouching", _controllerBody2D.IsCrouching);
		
            // Jump/Air States
            Animator.SetBool(IsJumpingAnimationHash, _controllerBody2D.IsJumping);
            Animator.SetBool("IsFalling", _controllerBody2D.IsFalling);
            Animator.SetBool("IsGliding", _controllerBody2D.IsGliding);
        }
    }
}
