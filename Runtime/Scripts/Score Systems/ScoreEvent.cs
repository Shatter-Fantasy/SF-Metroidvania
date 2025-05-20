using SF.Events;

namespace SF
{
    public enum ScoreEventTypes
    {
        ScoreIncrease,
        ScoreDecrease,
        ScoreSet
    }
    public struct ScoreEvent : IEvent
    {
        public ScoreEventTypes EventType;
        public float ScoreChange;
        static ScoreEvent scoreEvent;

        public ScoreEvent(ScoreEventTypes eventType, float scoreChange = 0)
        {
            EventType = eventType;
            ScoreChange = scoreChange;
        }

         public static void Trigger(ScoreEventTypes eventType, float scoreChange = 0)
         {
            scoreEvent.EventType = eventType;
            scoreEvent.ScoreChange = scoreChange;
            EventManager.TriggerEvent(scoreEvent);
         }
    }
}
