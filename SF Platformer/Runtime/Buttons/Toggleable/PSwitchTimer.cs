using SF.Events;
using UnityEngine;

namespace SF.SwitchesModule
{
    public class PSwitchTimer : ITimer
    {
        public float Timer;

        public PSwitchTimer(float timer)
            {
                Timer = timer;
            }
            public void StartTimer()
            {
                PTimer();
            }
            private async void PTimer()
            {
                PSwitch.IsActivated = true;
                PSwitchEvent.Trigger(PSwitchEventTypes.PSwitchOn);
                await UnityEngine.Awaitable.WaitForSecondsAsync(Timer);
                PSwitchEvent.Trigger(PSwitchEventTypes.PSwitchOff);
                PSwitch.IsActivated = false;
            } 
    }
}
