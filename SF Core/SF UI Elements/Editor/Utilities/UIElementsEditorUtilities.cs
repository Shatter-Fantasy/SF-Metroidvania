using System;

using static UnityEditor.AssetDatabase;
using System.Runtime.CompilerServices;
using System.IO;

namespace SFEditor.UIElements.Utilities
{

    public static class UIElementsEditorUtilities
    {
        /// <summary>
        /// Gets the first asset meeting the search filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public static T FindFirstAssetInPackages<T>(string searchFilter) where T : UnityEngine.Object
        {
            ReadOnlySpan<string> guids = FindAssets($"glob:\"Packages/**/*{searchFilter}\"");
            return LoadAssetAtPath<T>(GUIDToAssetPath(guids[0]));
        }

        public static bool TryFindFirstAssetInPackages<T>(string searchFilter, ref T asset) where T : UnityEngine.Object
        {
            ReadOnlySpan<string> guids = FindAssets($"glob:\"{searchFilter}\"");
            if(guids.IsEmpty)
                return false;
            asset = LoadAssetAtPath<T>(GUIDToAssetPath(guids[0]));
            return asset != null;
        }

        /// <summary>
        /// Uses the CallerFilePath attribute to get the absolute path of the calling script.
        /// Than removes the extention after the period.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetCallerPathWithoutExtension([CallerFilePath] string filePath = "")
        {
            // Nice built in C# function that when passing in null removes the extention at the end of a path.
            return Path.ChangeExtension(filePath, null);
        }

        /// <summary>
        /// Uses the CallerFilePath attribute to get the absolute path of the calling script.
        /// Than removes the extention after the period.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetCallerPathWithNewExtension(string newExtention,
            [CallerFilePath] string filePath = "")
        {
            // Nice built in C# function that when passing in null removes the extention at the end of a path.
            return Path.ChangeExtension(filePath, newExtention);
        }
    }
}
