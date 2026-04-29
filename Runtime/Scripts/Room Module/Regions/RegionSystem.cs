using UnityEngine.SceneManagement;

namespace SF.RoomModule.RegionModule
{
    using SF.LevelModule;
    using SF.LoggingModule;
    using SF.SpawnModule;
    public static class RegionSystem
    {
        /// <summary>
        /// The Region data asset for the scene the current <see cref="LevelLoader"/> is in.
        /// Set this in the inspector to update 
        /// </summary>
        public static RegionDataAsset LoadedRegionDataAsset;
        public static RegionDataAsset PreviousRegionDataAsset;
        public static RegionDatabase RegionDatabase;

        public static void LoadInitialRegionData()
        {
            if (RegionDatabase == null ||RegionDatabase.DataEntries == null || RegionDatabase.DataEntries.Count < 1)
                return;
            
            if (LoadedRegionDataAsset == null && RegionDatabase.DataEntries.Count > 0)
            {
                LoadedRegionDataAsset = GetRegionBySceneIndex();
                // If we can't find a region try to get the first scene to prevent the game breaking.
                if(LoadedRegionDataAsset == null)
                    LoadedRegionDataAsset = RegionDatabase.DataEntries[0];
            }

            if (LoadedRegionDataAsset.Rooms.Count > 1)
                RoomSystem.SetInitialRoom(RoomSystem.StartingRoomId);
        }
        public static void LoadRegionAsync(RegionTransitionConnection regionTransitionConnection)
        {
            
            if (regionTransitionConnection.RegionToTransitionTo == null)
            {
                LoggingSystem.LogMessage("There was no regionDataAsset passed in the LoadRegionAsync method when trying to load a region.",null);
                RoomSystem.StartingRoomId = 0;
                SceneManager.LoadSceneAsync(1);
                return;
            }

            PreviousRegionDataAsset = LoadedRegionDataAsset;
            // Remove all instanced room controllers in the previous region.
            PreviousRegionDataAsset?.CleanUpRegion();
            LoadedRegionDataAsset     = regionTransitionConnection.RegionToTransitionTo;
            var transitionData = LoadedRegionDataAsset.RegionTransitionDataSet
                                                          .Find(transitionData =>
                                                                  transitionData.TransitionID ==
                                                                  regionTransitionConnection.TransitionIDToGoTo);
            SpawnSystem.CurrentSpawnPosition = transitionData.LocalSpawnPositionInCurrentRoom;
            RoomSystem.StartingRoomId        = transitionData.RoomIDInLoadingRegion;
            
            SceneManager.LoadSceneAsync(regionTransitionConnection.RegionToTransitionTo.SceneIndex);
        }
        
        public static RegionDataAsset GetRegionBySceneIndex()
        {
            if (RegionDatabase == null 
                || RegionDatabase.DataEntries == null 
                || RegionDatabase.DataEntries.Count < 1)
                return null;
            
            var index = SceneManager.GetActiveScene().buildIndex;
            return RegionDatabase.DataEntries.Find(data => data.SceneIndex == index);
        }
    }
}
