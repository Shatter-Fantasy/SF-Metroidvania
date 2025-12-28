using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.LowLevelPhysics2D;
namespace SFEditor
{
    /// <summary>
    /// Class that contains the static methods for creating helper tool bar buttons in Unity's main toolbar where the play/pause buttons are.
    /// </summary>
    public class SFEditorMainToolbar
    {
        /// <summary>
        /// Adds a toolbar to the main toolbar layout inside of the Unity editor to quickly open project settings.
        /// </summary>
        /// <returns></returns>
        [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement ProjectSettingsButton()
        {
            var icon    = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
            var content = new MainToolbarContent(icon);
            
            return new MainToolbarButton(content, () =>
            {
                SettingsService.OpenProjectSettings();
            });
        }
        
        /// <summary>
        /// Adds a toolbar to the main toolbar layout inside of the Unity editor to quickly open project physics 2D settings.
        /// </summary>
        /// <returns></returns>
        [MainToolbarElement("Project/Open Physics 2D Settings", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement Physics2DSettingsButton()
        {
            const string iconPath     = "Profiler.Physics2D@2x";
            const string settingsPath = "Project/Physics 2D";
            
            var icon    = EditorGUIUtility.IconContent(iconPath).image as Texture2D;
            var content = new MainToolbarContent(icon);
            
            return new MainToolbarButton(content, () =>
            {
                SettingsService.OpenProjectSettings(settingsPath);
            });
        }
        
        /// <summary>
        /// Adds a toolbar to the main toolbar layout inside of the Unity editor to quickly open project physics 2D settings.
        /// </summary>
        /// <returns></returns>
        [MainToolbarElement("Project/Open Preferences", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement PreferencesButton()
        {
            const string iconPath             = "SettingsIcon";
            
            var icon    = EditorGUIUtility.IconContent(iconPath).image as Texture2D;
            var content = new MainToolbarContent(icon, "Opens the Preferences Window.");
            
            return new MainToolbarButton(content, () =>
            {
                SettingsService.OpenUserPreferences();
            });
        }
    }
}
