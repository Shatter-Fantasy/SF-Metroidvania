using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable All

namespace SFEditor.Core.Packages
{
    
    /// <summary>
    /// This is the packages status for the branch the release is on.
    /// </summary>
    public enum PackageReleaseStatus {Stable, Alpha, Beta}
    
    /// <summary>
    /// The data struct used for keeping track of packages versions, other SF package dependencies,
    /// scripting defines, needed assembly definitions, and Unity packages dependencies.
    /// </summary>
    [System.Serializable]
    public class SFPackageData
    {
        /// <summary>
        /// This is the full name of the package in the package json.
        /// Think shatter-fantasy.sf-ui-elements
        /// </summary>
        public string PackageName;

        /// <summary>
        /// This is the shorthand display name of the package in the inspector.
        /// </summary>
        public string PackageDisplayName;
        
        public readonly string BasePackageURL;
        /// <summary>
        /// This is an optional string to allow people to pull specific versions of a package release or even choose the nightly  branch. 
        /// </summary>
        public string PackageReleaseTag;
        public string FullPackageURL => BasePackageURL + PackageReleaseTag;

        public PackageReleaseStatus ReleaseStatus;
        public List<string> ScriptingDefinesNames = new();
        public List<string> UnityPackageDependencies = new();
        
        public SFPackageData(string packageName, string basePackageURL, string packageDisplayName ,string packageReleaseTag = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                Debug.LogError("The package name value being passed into the SFPackageData constructor is not valid."
                    + $"Passed in package name is: {packageName}");
                packageName = "Invalid Package Name";
            }

            // TODO BasePackageURL Validation: Make sure the BasePackageURL value is a valid Git URL.
            
            PackageName = packageName;
            PackageDisplayName = packageDisplayName;
            
            BasePackageURL = basePackageURL;
            PackageReleaseTag = packageReleaseTag;

            ReleaseStatus = PackageReleaseStatus.Alpha;
        }
    }

    /// <summary>
    /// These are the URL for the current public GitHub packages
    /// </summary>
    public class SFPackageDefaults
    {
        #region Required Packages
        public static readonly SFPackageData SFCorePackage = new SFPackageData(SFCorePackageName,SFCoreBasePackageURL, "SF Core");
        
        public const string SFCorePackageName = "shatterfantasy.sf-core";
        public const string SFCoreBasePackageURL = "https://github.com/crowhound/SF-Core.git";
        #endregion
    }
}
