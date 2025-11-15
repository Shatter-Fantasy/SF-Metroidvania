using UnityEngine;

namespace SF.CombatModule
{
    [System.Serializable]
    public class ComboAttack
    {
        public string Name;
        public float AttackTimer;
        /// <summary>
        /// The delay before the hit box is enabled. This allows for matching damage with the animation visuals.
        /// </summary>
        public float HitBoxDelay;
        public float HitBoxAnimationFrame;

        /// <summary>
        /// The amount of allowable passed time from the previous combo attack to allow for continuing the combo set of attacks.
        /// </summary>
        public float ComboInputDelay = 1.5f;
        [field: SerializeField] public AnimationClip AttackAnimationClip { get; protected set; }
        
        public ComboAttack()
        {

        }

        public ComboAttack(AnimationClip attackAnimationClip)
        {
            AttackAnimationClip = attackAnimationClip;
            AttackTimer = attackAnimationClip.length;
        }
    }
}
