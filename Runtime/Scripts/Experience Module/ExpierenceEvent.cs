using SF.Events;

namespace SF.Experience
{
    public enum ExperienceEventTypes
    {
        Gain,
        Lose,
        Set
    }
    public struct ExperienceEvent
    {
        public ExperienceEventTypes EventType;
        public int Experience;
        public ExperienceEvent(ExperienceEventTypes eventType, int experience = 0)
        {
            EventType = eventType;
            Experience = experience;
        }
        static ExperienceEvent experienceEvent;

        public static void Trigger(ExperienceEventTypes eventType, int experience = 0)
        {
            experienceEvent.EventType = eventType;
            experienceEvent.Experience = experience;
            EventManager.TriggerEvent(experienceEvent);
        }
    }
}
