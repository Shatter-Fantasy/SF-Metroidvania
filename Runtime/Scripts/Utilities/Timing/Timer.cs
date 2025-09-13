using System;
using UnityEngine;

namespace SF
{
    [Serializable]
    public class Timer
    {
        public float Duration = 3;
        public float RemainingTime;
        public float ElapsedTimer = 0;
        /// <summary>
        /// An Action that is called at the end when the Timer hits 0.
        /// </summary>
        private Action _onTimerComplete;

        [NonSerialized] public bool TimerStopped;
        public Timer(Action onTimerComplete = null)
        {
            if (onTimerComplete == null)
             return;

            _onTimerComplete += onTimerComplete;
        }
        public Timer(float duration, Action onTimerComplete = null) : this(onTimerComplete)
        {
            Duration = duration;
            RemainingTime = duration;
        }
        public void ResetTimer()
        {
            TimerStopped = false;
            RemainingTime = Duration;
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
            TimerStopped = true;
        }

        /// <summary>
        /// Tells the timer to start updating and counting down asynchronously and invokes the <see cref="_onTimerComplete"/> event when the timer reaches zero.
        /// </summary>
        /// <returns></returns>
        public async Awaitable UpdateTimerAsync()
        {
            while (RemainingTime > 0)
            {
                if(TimerStopped)
                    break;
                
                RemainingTime -= Time.deltaTime;
                ElapsedTimer += Time.deltaTime;
                
                if (RemainingTime > 0)
                    await Awaitable.EndOfFrameAsync();
            }
            
            RemainingTime = 0;
            ElapsedTimer = 0;
            
            if(!TimerStopped)
                _onTimerComplete?.Invoke();
        }
    }
}
