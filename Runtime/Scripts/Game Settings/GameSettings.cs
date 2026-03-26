using System;
using System.Collections.Generic;
using Unity.Properties;
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
        
        [CreateProperty]
        public DisplaySettings DisplaySettings;
        
        private void OnEnable()
        {
            if(DisplaySettings != null)
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
    
    public enum VSyncCount : ushort
    {
        None = 0, // VSync is off.
        EveryFrame = 1,
        EveryTwoFrames = 2
    }
    
    [Serializable]
    public class DisplaySettings : SettingsBase
    {
        /// <summary>
        /// Is Vsync currently active or not.
        /// <remarks>
        /// This is true when the <see cref="VSyncCount"/> has not been set to VSyncCount.None.
        /// </remarks>
        /// </summary>
        public bool IsVsyncOn => VSyncCount != VSyncCount.None;
        public VSyncCount VSyncCount;
        public int FrameRateLimit = 60;
        
        /// <summary>
        /// Calculates the options that should be enabled or disabled.
        /// </summary>
        public override void ProcessSettings()
        {
            if (IsVsyncOn)
            {
                QualitySettings.vSyncCount  = (int)VSyncCount;
            }
            else
            {
                QualitySettings.vSyncCount  = 0;
                Application.targetFrameRate = FrameRateLimit;
            }
        }
    }
}
