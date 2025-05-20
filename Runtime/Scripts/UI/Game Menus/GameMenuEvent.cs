namespace SF.Events
{

    public enum GameMenuEventTypes
    {
        OpenGameMenu,
        CloseGameMenu,
        OpenOptionsMenu,
        CloseOptionsMenu
    }
    public struct GameMenuEvent
    {
        public GameMenuEventTypes EventType;

        public GameMenuEvent(GameMenuEventTypes eventType)
        {
            EventType = eventType;
        }
        static GameMenuEvent gameMenuEvent;

        public static void Trigger(GameMenuEventTypes eventType)
        {
            gameMenuEvent.EventType = eventType;
            EventManager.TriggerEvent(gameMenuEvent);
        }
    }
}
