using UnityEngine;

namespace SF.Characters
{
    /// <summary>
    /// This class just holds a cached value for character animation states.
    /// By creating a cached of character animation states once we can prevent any garbage allocations down the line.
    /// </summary>
    public static class AnimationStateHashes
    {
        public static readonly int IdleAnimationHash = Animator.StringToHash("Idle");
        public static readonly int WalkingAnimationHash = Animator.StringToHash("Walking");
    }
}
