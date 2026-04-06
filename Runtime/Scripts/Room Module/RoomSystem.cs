using System;
using System.Collections.Generic;
using SF.LoggingModule;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SF.RoomModule
{
    using RegionModule;
    /// <summary>
    /// Used to allow interactions between the in game systems and the rooms in the current loaded region. <see cref="RegionSystem.LoadedRegionDataAsset"/>.
    /// Also includes helper functions for interacting with room management like room loading/unloading and keeping track of what rooms are already loaded.
    ///
    ///
    /// <remarks>
    ///     Room System Logic Flow
    ///     There are two ways to have the game take care of room loading that run depending on if DynamicRoomLoading is set to true or false.   
    ///  
    /// 
    /// </remarks>
    /// </summary>
    public static class RoomSystem
    {
        public static int StartingRoomId;
        public static bool DynamicRoomLoading = true;
        
        /// <summary>
        /// List of the loaded Rooms data.
        /// </summary>
        private static readonly List<int> LoadedRoomsIDs = new();
        public static RegionDataAsset LoadedRegion => RegionSystem.LoadedRegionDataAsset;
        

        /// <summary>
        /// The current room the player is moving in.
        /// </summary>
        /// <remarks>
        /// To set the CurrentRoom from other classes/C# objects call <see cref="SetCurrentRoom"/>.
        /// Calling SetCurrentRoom does checks to make sure the value being set to the CurrentRoom is valid.
        /// </remarks>
        public static Room CurrentRoom { get; private set; }
        public static int CurrentRoomID => CurrentRoom?.RoomID ?? 0;

        public static Vector3 CurrentRoomPosition => 
            CurrentRoom == null || CurrentRoom.SpawnedRoomController == null
                ? Vector3.zero
                : CurrentRoom.SpawnedRoomController.transform.position;
           
        
        /// <summary>
        /// Loads a connected room by its id. This is called in the room before it aka the connected room leading to other rooms.
        /// Allowing farther rooms to be loaded in the background to prevent pop up and lag.
        /// </summary>
        /// <param name="roomID"></param>
        public static Room LoadRoom(int roomID)
        {
            if (LoadedRegion[roomID]?.RoomPrefab == null)
                return null;
            
            // If the room is already loaded just refresh it object instances without spawning new ones.
            if (IsRoomLoaded(roomID))
            {
                RefreshRoom(roomID);
                return LoadedRegion[roomID];
            }
            
            // We can choose to skip the spawning of the instance. This is done for debugging reasons and to catch errors.
            if (DynamicRoomLoading)
            {
                // If no room instance with the passed in roomID is currently loaded spawn and load an instance. 
                // Also set it as the current SpawnedInstance in the RoomDB. This allows us to check if a room is already loaded later by checking 
                // if the SpawnedInstance is null or not. We should check the _loadedRoomsIDs first for performance reasons. 
                LoadedRegion[roomID].SpawnedInstance       = Object.Instantiate(LoadedRegion[roomID].RoomPrefab);
                LoadedRegion[roomID].SpawnedRoomController = LoadedRegion[roomID].SpawnedInstance?.GetComponent<RoomController>();
                LoadedRoomsIDs.Add(roomID);
            }
            else
            {
                LoadedRegion[roomID].SpawnedInstance       = Object.Instantiate(LoadedRegion[roomID].RoomPrefab);
                LoadedRegion[roomID].SpawnedRoomController = LoadedRegion[roomID].SpawnedInstance?.GetComponent<RoomController>();
                LoadedRoomsIDs.Add(roomID);
            }

            return LoadedRegion[roomID];
        }

        /// <summary>
        /// Only use this to manually add a RoomID into the loaded room ids list when the room will already exist in the scene at the start. 
        /// </summary>
        public static Room LoadRoomManually(int roomID, GameObject spawnedInstance = null)
        {
            // Don't duplicate the loaded room if it was already loaded.
            if (IsRoomLoaded(roomID))
                return LoadedRegion[roomID];
            
            LoadedRoomsIDs.Add(roomID);

            if (spawnedInstance != null)
            {
                LoadedRegion[roomID].SpawnedInstance = spawnedInstance;
                LoadedRegion[roomID].SpawnedRoomController = spawnedInstance.GetComponent<RoomController>();
            }

            return LoadedRegion[roomID];
        }
        /// <summary>
        /// Checks to see if the passed in room id belongs to one of the already loaded rooms.
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public static bool IsRoomLoaded(int roomID)
        {
            if (LoadedRegion == null || LoadedRegion.Rooms.Count < 1)
            {
                LoggingSystem.LogMessage("When checking if a room was loaded the LoadedRegion was either null or had no rooms in it's list.", LoadedRegion);
                return false;
            }
            
            for (int i = 0; i < LoadedRoomsIDs.Count; i++)
            {
                // We check the _loadedRoomsIDs first for performance reasons and
                // as a safety check just in case somehow a spawned instance hasn't been cleaned up fully yet.
                if (LoadedRoomsIDs[i] == roomID && LoadedRegion[roomID].SpawnedInstance != null)
                {
                    return true;
                }
            }
            
            // If we make it through the whole loop without finding a room with the roomID than no room instance is currently loaded.
            return false;
        }
        
        /// <summary>
        /// Refreshes the rooms spawned objects, but doesn't enable the game objects.
        /// They are enabled during the OnEnterRoom. This way we don't enable game objects two rooms away with enemy logic.
        /// </summary>
        public static void RefreshRoom(int roomID)
        {
            // Don't try to Refresh a room that hasn't loaded a spawned instance yet.
            if (!IsRoomLoaded(roomID))
                return;
        }

        /// <summary>
        /// Sets the current room
        /// Returns true if the <see cref="CurrentRoom"/> value was set properly and false if it failed to be set.
        /// </summary>
        public static bool SetCurrentRoom(int roomID)
        {
            // Make sure the room we are trying to set as the current room is already loaded
            if (!IsRoomLoaded(roomID))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No room matching RoomID: {roomID} is currently loaded in.");
#endif
                return false;
            }

            // Was able to set a valid room as the current one.
            CurrentRoom = LoadedRegion[roomID];
            return true;
        }

        /// <summary>
        /// This sets up the initial room when loading or starting a new file. Also happens when entering a new region.
        /// </summary>
        public static void SetInitialRoom(int roomID)
        {
            LoadRoom(roomID); 
            LoadedRegion[roomID]?.SpawnedRoomController?.MakeCurrentRoom();
        }
        public static void CleanUpRoom(int roomId)
        {
            // Can happen without error when unloading a scene and objects are destroyed on scene exit and also on exiting playmode.
            if (!IsRoomLoaded(roomId))
                return;
            
            
            LoadedRegion[roomId].SpawnedInstance = null;
            LoadedRoomsIDs.Remove(roomId);
        }
    }
    
    [Serializable]
    public class Room
    {
        public string Name;
        public int RoomID;
        /// <summary>
        /// The connected rooms that need to be loaded/deloaded when entering/existing 
        /// </summary>
        [Header("Room Ids")]
        public List<int> ConnectedRoomsIDs = new List<int>();

        /// <summary>
        /// The list of possible transition points into and out of a room. <see cref="RoomTransition"/>
        /// </summary>
        public List<int> TransitionsIDs = new List<int>();
        /// <summary>
        /// The Room Prefab asset to load into the game from the database.
        /// You should only get this when grabbing a Room reference directly from the RoomDB or you could risk a null value. 
        /// </summary>
        public GameObject RoomPrefab;
        /// <summary>
        /// This is only used during runtime. This allows for keeping track of the SpawnedInstances in the Room data itself.
        /// </summary>
        [NonSerialized] public GameObject SpawnedInstance;
        [NonSerialized] public RoomController SpawnedRoomController;
    }
}