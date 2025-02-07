namespace SF.Events
{
    public enum ApplicationEventTypes
    {
        ExitApplication
    }
    public struct ApplicationEvent
    {
        public ApplicationEventTypes EventType;
        public ApplicationEvent(ApplicationEventTypes eventType)
        {
            EventType = eventType;
        }
        static ApplicationEvent applicationEvent;

        public static void Trigger(ApplicationEventTypes eventType)
        {
            applicationEvent.EventType = eventType;
            EventManager.TriggerEvent(applicationEvent);
        }
    }
}
