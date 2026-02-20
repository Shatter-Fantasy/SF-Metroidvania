using Unity.Properties;
using UnityEngine;

namespace SF.Experience
{
    [System.Serializable]
    public class ExperienceValue
    {
        public int BaseExperience;
        // TODO: Maybe add a system where killing in any with overkill damage or a certain way like element typed grants bonus exp. 
    }
    
    /// <summary>
    /// Keeps track of the player current level and also the players current experience.
    /// </summary>
    [System.Serializable]
    public class PlayerLevelStats
    {
        // TODO: Add the experience curve system.
        
        protected static PlayerLevelStats _instance;
        public static PlayerLevelStats Instance
        {
            get => _instance;
            set
            {
                if(_instance != null)
                    return;

                _instance = value;
            }
        }
        
        /// <summary>
        /// Creates a static instance of the class on compilation.
        /// </summary>
        /// <remarks>
        /// Only works with non-Unity <see cref="UnityEngine.Object"/> classes.
        /// </remarks>
        static PlayerLevelStats()
        {
            Instance = new PlayerLevelStats();
        }
        
        [Header("Level Data")]
        [CreateProperty] public int CurrentLevel = 1;
        public int MaxLevel = 100;

        [Header("Experience Data")]
        [CreateProperty] public int CurrentExperience
        {
            get => _currentExperience;
            set
            {
                _currentExperience = value;
                
                if(_currentExperience >= ExperienceToNextLevel)
                    LevelUp();
            }
        }
        [SerializeField] private int _currentExperience;
        
        [CreateProperty] public int ExperienceToNextLevel = 40;

        
        /// <summary>
        /// The percentage of the previous level's experience needed to level up to add for the next level up amount.
        /// </summary>
        public float LevelUpModifier = 1.2f;
        
        private void LevelUp()
        {
            ExperienceToNextLevel += Mathf.RoundToInt(ExperienceToNextLevel * LevelUpModifier);
            CurrentLevel++;
        }

        public static void GainExperience(int amount)
        {
            _instance._currentExperience += amount;
        }
    }
}