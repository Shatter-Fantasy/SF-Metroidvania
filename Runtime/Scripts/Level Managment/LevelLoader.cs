using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace SF.LevelModule
{
    using SF.RoomModule.RegionModule;
    /// <summary>
    /// Loads the required game objects for used in managers and core systems in playable levels.
    /// This is needed to be in each playable scene and make sure this does not persist between scenes,
    /// so start can run per at least once per scene.
    /// </summary>
    [DefaultExecutionOrder(-4)]
    public partial class LevelLoader : MonoBehaviour
    {
        /// <summary>
        /// These are the indexes that are considered not part of a playable game scene.
        /// Any scene that has an index in this array will not start the First Playable Scene Loaded sequence.
        /// </summary>
        [Header("Level Initialization")] [SerializeField]
        private int[] _gameStartingSceneIndexes = new int[1];
        
        /// <summary>
        /// This is called when the first playable is ready to give the player control.
        /// </summary>
        [AutoStaticsCleanup]
        public static event Action LevelReadyHandler;

        public static event Action LevelStartedHandler;
   
        private void Start()
        {
            RegionSystem.LoadInitialRegionData();
            LevelReadyHandler?.Invoke();
            
            LevelStartedHandler?.Invoke();
        }
    }
}
