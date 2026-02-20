using UnityEngine;

namespace SF.CombatModule
{
    [System.Serializable]
    public class AttackDefinition
    {
        public string Name;
        public float AttackTimer;
        [field: SerializeField] public AnimationClip AttackAnimationClip { get; protected set; }

        /// <summary>
        /// The delay before the hit box is enabled. This allows for matching damage with the animation visuals.
        /// </summary>
        public float HitBoxDelay;
        public float HitBoxAnimationFrame;
    }
    
    [System.Serializable]
    public class ComboAttack : AttackDefinition
    {
        /// <summary>
        /// The amount of allowable passed time from the previous combo attack to allow for continuing the combo set of attacks.
        /// </summary>
        public float ComboInputDelay = 1.5f;

        public ComboAttack(AnimationClip attackAnimationClip)
        {
            AttackAnimationClip = attackAnimationClip;
            AttackTimer = attackAnimationClip.length;
        }
    }
}
