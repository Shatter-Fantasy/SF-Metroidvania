using System;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Collections.Generic;

namespace SFEditor.Packages
{

    static class SFPlatformerPackages
    {
        /// <summary>
        /// A collection of packages installed in the project.
        /// </summary>
        static PackageCollection InstalledPackages;
        static List<string> TargetPackageNames = new();
        static List<string> ScriptingDefineSymbols = new();

        /// <summary>
        /// Required SF Package Git url for the AddRequest
        /// </summary>
        static string PackageBaseURl = "https://github.com/Shatter-Fantasy/SF-Utilities.git";
        /// <summary>
        /// The package version of any specific git change set to install.
        /// For getting the package being updated on a branch use the branch name here.
        /// If left blank it pulls the main branch.
        /// </summary>
        static string PackageVersion = "";

        static string PackageURl
        {
            get
            {
                if(string.IsNullOrEmpty(PackageVersion))
                    return PackageBaseURl;
                else
                    return $"{PackageBaseURl}#{PackageVersion}";
            }
        }


        static AddRequest AddRequest;
        static EmbedRequest EmbedRequest;
        static ListRequest ListRequest;


        [MenuItem("SF/Packages/Embed Required Packages")]
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

        [MenuItem("SF/Packages/Add Required Packages")]
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

        static void AddProgress()
        {
            if(AddRequest.IsCompleted)
            {
                if(AddRequest.Status == StatusCode.Success)
                {
                    Debug.Log("Installed the required packages successfully:" + AddRequest.Result.packageId);
                } 
                /*  The >= here checks the enum value as an int.
                    Technically this is a web request that gets returns. If the int is higher than 
                    2 than there was a reason it failed. Technically there are several failure codes
                    but Unity only has implmented an enum for 0 = InProgress, 
                    1 = Sucess, 2 = failed for some reason. */
                else if(AddRequest.Status == StatusCode.Failure)
                {
                    Debug.Log(AddRequest.Error.message);
                }

                // AddRequest is completed so remove the function tracking it's prgress.
                EditorApplication.update -= AddProgress;
            }
        }

        [MenuItem("SF/Scripting/Check Scripting Symbols")]
        static void CheckScriptingDefineSymbols()
        {
            Debug.Log(PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone));
        }

        [MenuItem("SF/Scripting/Add Scripting Symbols")]
        static void AddScriptingDefineSymbols()
        {
            
        }
    }
}