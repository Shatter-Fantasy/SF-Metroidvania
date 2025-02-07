using System.Collections;
using System.Collections.Generic;
using SF.CommandModule;
using SF.Events;
using UnityEngine;

namespace SF
{
    public class PSwitchListener : MonoBehaviour, EventListener<PSwitchEvent>
    {
        public CommandController POnCommandController;
        public CommandController POffCommandController;
        public void OnEvent(PSwitchEvent switchEvent)
        {
            switch (switchEvent.EventType)
            {
                case PSwitchEventTypes.PSwitchOn:
                    PSwitchTurnedOn();
                    break;
                case PSwitchEventTypes.PSwitchOff:
                    PSwitchTurnedOff();
                    break;
            }
        }

        private void PSwitchTurnedOn()
        {
            if (POnCommandController == null)
                return;

            POnCommandController.StartCommands();
        }

        private void PSwitchTurnedOff()
        {
            if (POffCommandController == null)
                return;
            POffCommandController.StartCommands();
        }

        private void OnEnable()
        {
            this.EventStartListening<PSwitchEvent>();
        }

        private void OnDestroy()
        {
            this.EventStopListening<PSwitchEvent>();         
        }
    }
}
