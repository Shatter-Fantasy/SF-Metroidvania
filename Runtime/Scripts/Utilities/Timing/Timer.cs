using System;
using UnityEngine;

namespace SF
{
    [Serializable]
    public class Timer
    {
        public float StartingTime = 3;
        public float RemainingTime;

        /// <summary>
        /// An Action that is called at the end when the Timer hits 0.
        /// </summary>
        private Action _onTimerComplete;

        private bool _timerStopped;
        public Timer(Action onTimerComplete = null)
        {
            if (onTimerComplete == null)
             return;

            _onTimerComplete += onTimerComplete;
        }
        public Timer(float startingTime, Action onTimerComplete = null) : this(onTimerComplete)
        {
            StartingTime = startingTime;
            RemainingTime = startingTime;
        }
        public void ResetTimer()
        {
            _timerStopped = false;
            RemainingTime = StartingTime;
        }
        /// <summary>
        /// Starts an async timer that when completed raises the 
        /// <see cref="_onTimerComplete"/> event.
        /// </summary>
        /// <returns>
        /// True when the timer is finished or false while the timer is currently counting down.
        /// </returns>
        public async Awaitable<bool> StartTimerAsync()
        {
            ResetTimer();
            await UpdateTimerAsync();
            return true;
        }

        public void StopTimer()
        {
            _timerStopped = true;
        }

        /// <summary>
        /// Tells the timer to start updating and counting down asynchronously and invokes the <see cref="_onTimerComplete"/> event when the timer reaches zero.
        /// </summary>
        /// <returns></returns>
        public async Awaitable UpdateTimerAsync()
        {
            while (RemainingTime > 0)
            {
                if(_timerStopped)
                    break;
                
                await Awaitable.EndOfFrameAsync();
                RemainingTime -= Time.deltaTime;
            }
            RemainingTime = 0;
            _onTimerComplete?.Invoke();
        }
    }
}
