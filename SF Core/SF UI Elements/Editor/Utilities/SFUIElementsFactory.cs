using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SFEditor.UIElements.Utilities
{
    /// <summary>
    /// This is a factory class to help quickly set up new UI elements in the editor like inspectors, editor windows, and any other custom editor class.
    /// </summary>
    public class SFUIElementsFactory
    {
        public const string SFCommonStyleSheetName = "CommonUSS";
        /// <summary>
        /// Base path to the root folder of the SFUIElements package.
        /// <remarks>
        /// The root path to SF UI Elements is a sub package directory inside of the SF Core package.
        /// </remarks>
        /// </summary>
        public const string SFUIElementsRootPath = "Packages/shatterfantasy.sf-topdown-arpg/SF Core/SF UI Elements";
        public static readonly string SFCommonStyleSheetAssetPath = $"{SFUIElementsRootPath}/Runtime/Styles/CommonUSS.uss";

        private static StyleSheet _sfCommonStyleSheet;
        public static StyleSheet SFCommonStyleSheet
        {
            get
            {
                if(_sfCommonStyleSheet == null)
                {
                    _sfCommonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SFCommonStyleSheetAssetPath);
                    // If we can't find the Common Style Sheet for some reason do a safe fail.
                    if(_sfCommonStyleSheet == null)
                    {
                        _sfCommonStyleSheet = StyleSheet.CreateInstance<StyleSheet>();
                        Debug.LogWarning("No SF common style sheet was found so a blank style sheet was created and passed back to prevent null errors from breaking code.");
                    }
                }
                return _sfCommonStyleSheet;
            }
        }

        public static VisualElement InitializeSFStyles(VisualElement ve)
        {
            ve.styleSheets.Add(SFCommonStyleSheet);
            return ve;
        }
    }
}
