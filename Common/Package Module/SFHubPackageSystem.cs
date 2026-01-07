using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace SFEditor.Core.Packages
{
    public static class SFHubPackageSystem
    {
        public static List<SFPackageData> SFInstalledPackages { get; private set; } = new();

        /// <summary>
        /// A collection of packages installed in the project.
        /// </summary>
        internal static PackageCollection InstalledPackages;

        internal static List<string> TargetPackageNames = new();

        /// <summary>
        /// Required SF Package Git url for the AddRequest
        /// </summary>
        internal static string PackageBaseURl = "https://github.com/Shatter-Fantasy/SF-Utilities.git";
        /// <summary>
        /// The package version of any specific git change set to install.
        /// For getting the package being updated on a branch use the branch name here.
        /// If left blank it pulls the main branch.
        /// </summary>
        internal static string PackageVersion = "";

        internal static string PackageURl
        {
            get
            {
                if(string.IsNullOrEmpty(PackageVersion))
                    return PackageBaseURl;
                else
                    return $"{PackageBaseURl}#{PackageVersion}";
            }
        }

        internal static readonly string CurrentPackageName;

        internal const string PackageAlreadyInstalledMessage = "Package is already installed";

        internal static AddAndRemoveRequest RequiredPackageSetupRequest;
        internal static List<Request<UnityEditor.PackageManager.PackageInfo[]>> InProgressSearchRequests = new();
        internal static List<Request<UnityEditor.PackageManager.PackageInfo>> InProgressAddRequests = new();
        internal static AddRequest AddRequest;
        internal static AddRequest SearchRequest;
        internal static EmbedRequest EmbedRequest;
        internal static ListRequest ListRequest;


        /// <summary>
        /// Installs the SF Utilities and the SF UI Toolkit package if they are not already in the project.
        /// This is done when the editor first opens or when a recompile happens.
        /// </summary>
        [InitializeOnLoadMethod]
        static void OnCorePackageLoad()
        {
            List<string> neededPackages = new();
            
            // We check for the package name first just in case we have a local/git loaded version of the package.
            // If they are not found, load the GitHub version as fallback.
            
            if (!PackageInfo.IsPackageRegistered(SFPackageDefaults.SFCorePackage.PackageName))
            {
                neededPackages.Add(SFPackageDefaults.SFCorePackage.FullPackageURL);
                Debug.Log("SF UI Elements was not installed. Setting up a package install request.");
            }
            
            SFInstalledPackages.Add(SFPackageDefaults.SFCorePackage);
            
            // If any of the required packages are missing and in the needed package array installl them.
            if(neededPackages.Count > 0)
                RequiredPackageSetupRequest = Client.AddAndRemove(neededPackages.ToArray());
        }

        //[MenuItem("SF/Packages/Embed Required Packages")]
        static void EmbedPackaged()
        {
            // Get the name of the installed packages
            ListRequest = Client.List();
            EditorApplication.update += EmbedListProgress;
        }

        private static void EmbedListProgress()
        {
            if(ListRequest.IsCompleted)
            {
                // Clear any already chached package names from previous operations.
                TargetPackageNames.Clear();

                if(ListRequest.Status == StatusCode.Success)
                {

                    // Cache the target package requests if needed for other operations.
                    InstalledPackages = ListRequest.Result;

                    foreach(var package in InstalledPackages)
                    {
                        // Only retrieve packages that are currently installed in the
                        // project (and are neither Built-In nor already Embedded)
                        if(package.isDirectDependency 
                            && package.source != PackageSource.BuiltIn
                            && package.source != PackageSource.Embedded )
                        {
                            TargetPackageNames.Add(package.name);
                        }
                    }
                }
                else
                    Debug.Log(AddRequest.Error.message);

                EditorApplication.update -= EmbedListProgress;

                EmbedPackages();
            }
        }

        static void EmbedPackages()
        {
            // If we never got a package to target for operations return.
            if(TargetPackageNames == null || TargetPackageNames.Count < 1)
            {
                Debug.Log("When trying to embed packages there was no package names in the TargetPackageNames list.");
                return;
            }

            for(int i = 0; i < TargetPackageNames.Count; i++)
            {
                EmbedRequest = Client.Embed(TargetPackageNames[i]);
                EditorApplication.update += EmbeddedProgress;
            }
        }

        private static void EmbeddedProgress()
        {
            if(EmbedRequest.IsCompleted)
            {
                if(EmbedRequest.Status == StatusCode.Success)
                    Debug.Log("Embedded: " + EmbedRequest.Result.packageId);
                else if(EmbedRequest.Status >= StatusCode.Failure)
                    Debug.Log(EmbedRequest.Error.message);

                EditorApplication.update -= EmbeddedProgress;
            }
        }

        //[MenuItem("SF/Packages/Add Required Packages")]
        static void AddPackages()
        {
            // Don't due this when running the application.
            // This happens if someone runs the command while in play mode, but not in runtime builds.
            if(Application.isPlaying)
                return;

            // Add a package to the project via an asyncrounous request.
            AddRequest = Client.Add(PackageURl);

            // Register a function to keep track of the AddRequest
            // Due note this is updated during editor ticks only.
            EditorApplication.update += AddProgress;
        }

        /// <summary>
        /// Does validation checks on the passed in SFPackageData struct and returns if it is valid.
        /// If it is a valid SFPackageData it also adds the SF Package to the Unity Package Manager based on the passed in SFPackageData struct.
        /// </summary>
        /// <param name="packageData"></param>
        /// <returns></returns>
        public static bool AddSFPackage(SFPackageData packageData)
        {
            // Don't due this when running the application.
            // This happens if someone runs the command while in play mode, but not in runtime builds.
            // TODO: Add package validation method.
            if (Application.isPlaying)
            {
                return false;
            }
            
            // TODO: Check if the package already exists.

            // Checks to see if the package trying to be added is already installed.
            SearchRequest searchRequest = Client.Search(packageData.PackageName);
            InProgressSearchRequests.Add(searchRequest);
            
            // Add a package to the project via an asyncrounous request.
            AddRequest addRequest = Client.Add(packageData.FullPackageURL);
            InProgressAddRequests.Add(addRequest);
            
            // Register a function to keep track of the AddRequest
            // Due note this is updated during editor ticks only.
            EditorApplication.update += AddProgress;
            EditorApplication.update += SearchProgress;
            return true;
        }

        static void AddProgress()
        {
            if (InProgressAddRequests.Count < 1)
            {
                // TODO: Add debugging statements.
                EditorApplication.update -= AddProgress;
                return;
            }

            for (int i = 0; i < InProgressAddRequests.Count; i++)
            {
                if(InProgressAddRequests[i].IsCompleted)
                {
                    if(InProgressAddRequests[i].Status == StatusCode.Success)
                    {
                        Debug.Log("Finished searching for the passed in package:" + InProgressAddRequests[i]);
                    } 
                    
                    /*  The >= here checks the enum value as an int.
                        Technically this is a web request that gets returns. If the int is higher than
                        2 than there was a reason it failed. Technically there are several failure codes
                        but Unity only has implmented an enum for 0 = InProgress,
                        1 = Sucess, 2 = failed for some reason. */
                    else if(InProgressAddRequests[i].Status == StatusCode.Failure)
                    {
                        Debug.Log(InProgressAddRequests[i].Error.message);
                    }

                    InProgressAddRequests.Remove(InProgressAddRequests[i]);
                }
            }

            // At this point we know we started with at least once search request and have finished all of them.
            if (InProgressAddRequests.Count < 1)
            {
                EditorApplication.update -= AddProgress;
            }
        }

        static void SearchProgress()
        {
            if (InProgressSearchRequests.Count < 1)
            {
                // TODO: Add debugging statements.
                EditorApplication.update -= SearchProgress;
                return;
            }

            for (int i = 0; i < InProgressSearchRequests.Count; i++)
            {
                if(InProgressSearchRequests[i].IsCompleted)
                {
                    if(InProgressSearchRequests[i].Status == StatusCode.Success)
                    {
                        Debug.Log("Finished searching for the passed in package:" + InProgressSearchRequests[i]);
                    } 
                    
                    /*  The >= here checks the enum value as an int.
                        Technically this is a web request that gets returns. If the int is higher than
                        2 than there was a reason it failed. Technically there are several failure codes
                        but Unity only has implmented an enum for 0 = InProgress,
                        1 = Sucess, 2 = failed for some reason. */
                    else if(InProgressSearchRequests[i].Status == StatusCode.Failure)
                    {
                        Debug.Log(InProgressSearchRequests[i].Error.message);
                    }

                    InProgressSearchRequests.Remove(InProgressSearchRequests[i]);
                }
            }

            // At this point we know we started with at least once search request and have finished all of them.
            if (InProgressSearchRequests.Count < 1)
            {
                EditorApplication.update -= SearchProgress;
            }
        }
        
        //[MenuItem("SF/Scripting/Check Scripting Symbols")]
        static void CheckScriptingDefineSymbols()
        {
            Debug.Log(PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone));
        }

        //[MenuItem("SF/Scripting/Add Scripting Symbols")]
        static void AddScriptingDefineSymbols()
        {
            
        }
        
    }
}
