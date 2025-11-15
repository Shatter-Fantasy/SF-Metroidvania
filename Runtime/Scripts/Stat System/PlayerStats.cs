using SF.Events;
using SF.Experience;
using UnityEngine;

namespace SF.StatModule
{
    public class PlayerStats : CharacterStats
    {
        public LevelStats LevelStats;
        
        public void OnEvent(ExperienceEvent experienceEvent)
        {
            switch (experienceEvent.EventType)
            {
                case ExperienceEventTypes.Gain:
                {
                    LevelStats.CurrentExperience += experienceEvent.Experience;
                    break;
                }
            }
        }
    }
}
