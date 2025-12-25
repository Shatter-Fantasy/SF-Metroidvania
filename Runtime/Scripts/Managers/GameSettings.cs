using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.Settings
{
    /// <summary>
    /// The data object for game play settings that can be changed by the players.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "SF/Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {

        private List<SettingsBase> _settings = new();
        
        public DisplaySettings DisplaySettings;

        private void Awake()
        {
            Debug.Log("Awake");
            
        }
        
        private void OnEnable()
        {
            _settings.Add(DisplaySettings);
        }

        /// <summary>
        /// Calculates the options that should be enabled or disabled.
        /// </summary>
        public void ProcessSettings()
        {
            foreach (var settingBase in _settings)
            {
                settingBase.ProcessSettings();
            }
        }
    }


    /// <summary>
    /// Base class for any type of game settings the player can change in game..
    /// </summary>
    public abstract class SettingsBase
    {
        /// <summary>
        /// Calculates the options that should be enabled or disabled.
        /// </summary>
        public abstract void ProcessSettings();
    } 
    [Serializable]
    public class DisplaySettings : SettingsBase
    {
        public bool IsVsyncOn = true;
        public int FrameRateLimit = 60;
        
        /// <summary>
        /// Calculates the options that should be enabled or disabled.
        /// </summary>
        public override void ProcessSettings()
        {
            if (IsVsyncOn)
            {
                Application.targetFrameRate = FrameRateLimit;
            }
        }
    }
}
