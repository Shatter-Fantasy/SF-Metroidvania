namespace SF.Events
{
    /// <summary>
    /// The categories of events in the project.
    /// </summary>
    public enum GameEventTypes
    {
        Score,
        Audio,
        Pause,
        UnPause,
        PauseToggle
    }

    public struct GameEvent
    {
        public GameEventTypes EventType;
        public GameEvent(GameEventTypes eventType)
        {
            EventType = eventType;
        }
        static GameEvent gameEvent;

        public static void Trigger(GameEventTypes eventType)
        {
            gameEvent.EventType = eventType;
            EventManager.TriggerEvent(gameEvent);
        }
    }
}
