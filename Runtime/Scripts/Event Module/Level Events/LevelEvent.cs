namespace SF
{
    public enum LevelEventTypes
    {
        LevelStart
    }
    public struct LevelEvent
    {
        public LevelEventTypes EventType;
        public LevelEvent(LevelEventTypes eventType)
        {
            EventType = eventType;
        }
        /*public LevelEvent(LevelEventTypes eventType, LevelData levelData)
        {
            // TODO Sooner or later a level data SO will need to be made so we can read level name, level music, level high score, and anything else.

            EventType = eventType;
        }*/
        static LevelEvent levelEvent;

         public static void Trigger(LevelEventTypes eventType)
         {
            levelEvent.EventType = eventType;
         }
    }
}
