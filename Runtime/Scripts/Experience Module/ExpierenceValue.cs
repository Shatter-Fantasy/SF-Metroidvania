using UnityEngine;

namespace SF.Experience
{
    [System.Serializable]
    public class ExperienceValue
    {
        public int BaseExperience;
        // TODO: Maybe add a system where killing in any with overkill damage or a certain way like element typed grants bonus exp. 
    }
    
    [System.Serializable]
    public class LevelStats
    {
        [Header("Level Data")]
        public int CurrentLevel = 1;
        public int MaxLevel = 100;

        [Header("Experience Data")]
        public int CurrentExperience
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
        
        public int ExperienceToNextLevel = 40;

        // TODO: Add the experience curve system.
        /// <summary>
        /// The percentage of the previous level's experience needed to level up to add for the next level up amount.
        /// </summary>
        public float LevelUpModifier = 1.2f;

        private void LevelUp()
        {
            int leftOverExperience = _currentExperience - ExperienceToNextLevel;
            _currentExperience = leftOverExperience;

            ExperienceToNextLevel = Mathf.RoundToInt(ExperienceToNextLevel * LevelUpModifier);
            CurrentLevel++;
        }
    }
}