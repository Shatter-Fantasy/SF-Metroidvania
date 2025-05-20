using UnityEngine;

namespace SF.AbilityModule
{
    /// <summary>
    /// Not fully implemented yet.
    /// This will allow for abilities to have cooldowns for a variety of 
    /// cases such as ability reactivation, ability delay for going from one ability to another, and ability activation delay when leaving certain character states. Think of a delay on ability activation after being damaged of stunned.
    /// </summary>
    public class AbilityCooldown
    {
        /// <summary>
        /// The timer that controls the cooldown state.
        /// </summary>
        public Timer CoolDownTimer;

        /// <summary>
        /// Is the ability currently on cooldown.
        /// </summary>
        public bool IsOnCooldown = false;

        public void Start()
        {
            CoolDownTimer = new Timer(3);
            StartCooldown();
        }

        private async void StartCooldown()
        {
            // First set it to false
            IsOnCooldown = false;
            // The CooldownTimer awaitable will return true when the timer finishes.
            IsOnCooldown = await CoolDownTimer.StartTimerAsync();
        }
    }
}
