using UnityEngine;

namespace SF
{
    [System.Serializable]
    public class ComboAttack
    {
        public string Name;
        public float AttackTimer;

        /// <summary>
        /// The amount of allowable passed time from the previous combo attack to allow for continuing the combo set of attacks.
        /// </summary>
        [SerializeField] private float _comboInputDelay = 1.5f;
        [field: SerializeField] public AnimationClip AttackAnimationClip { get; protected set; }
        
        public ComboAttack()
        {

        }

        public ComboAttack(AnimationClip attackAnimationClip)
        {
            AttackAnimationClip = attackAnimationClip;
            AttackTimer = attackAnimationClip.averageDuration;
        }
    }
}
