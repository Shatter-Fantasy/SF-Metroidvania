using UnityEngine.SceneManagement;

namespace SF.RoomModule.RegionModule
{
    using SF.LevelModule;
    using SF.LoggingModule;
    public static class RegionSystem
    {
        /// <summary>
        /// The Region data asset for the scene the current <see cref="LevelLoader"/> is in.
        /// Set this in the inspector to update 
        /// </summary>
        public static RegionDataAsset LoadedRegionDataAsset;
        public static RegionDatabase RegionDatabase;

        public static void LoadRegionAsync(RegionDataAsset regionDataAsset, int roomToLoad = 0)
        {
            if (regionDataAsset == null)
            {
                LoggingSystem.LogMessage("There was no regionDataAsset passed in the LoadRegionAsync method when trying to load a region.",null);
                RoomSystem.InProgressTransitionRoomID = 0;
                SceneManager.LoadSceneAsync(1);
                return;
            }

            LoadedRegionDataAsset                 = regionDataAsset;
            RoomSystem.InProgressTransitionRoomID = roomToLoad;
            SceneManager.LoadSceneAsync(regionDataAsset.SceneIndex);
        }
    }
}
