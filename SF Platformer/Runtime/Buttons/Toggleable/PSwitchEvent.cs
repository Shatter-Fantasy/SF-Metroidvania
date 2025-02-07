namespace SF.Events
{
    public enum PSwitchEventTypes
    {
        PSwitchOn,
        PSwitchOff
    }
    public struct PSwitchEvent
    {
        public PSwitchEventTypes EventType;
        static PSwitchEvent pSwitchEvent;

        public PSwitchEvent(PSwitchEventTypes eventType)
        {
            EventType = eventType;
        }

         public static void Trigger(PSwitchEventTypes eventType)
         {
            pSwitchEvent.EventType = eventType;
            EventManager.TriggerEvent(pSwitchEvent);
         }
    }
}
