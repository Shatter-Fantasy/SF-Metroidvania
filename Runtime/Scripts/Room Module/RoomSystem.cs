using System;
using System.Collections.Generic;
using SF.CameraModule;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SF.RoomModule
{
    /// <summary>
    /// Used to allow interactions between the in game systems and the RoomDatabase. <see cref="RoomDB"/>.
    /// Also includes helper functions for interacting with room management like room loading/unloading and keeping track of what rooms are already loaded.
    /// </summary>
    public static class RoomSystem
    {
        
        /// <summary>
        /// List of the loaded Rooms data.
        /// </summary>
        private static List<int> _loadedRoomsIDs = new();
        
        private static RoomDB _roomDB;
        /// <summary>
        /// The RoomDatabase to be loaded into the game logic.
        /// This can be used to switch out RoomDatabases that have seperate set of rooms like debug room sets, certain game release room set, and next update room sets.
        /// </summary>
        public static RoomDB RoomDB
        {
            get
            {
                if (_roomDB == null)
                    _roomDB = RoomModule.RoomDB.Instance;
                
                // TODO: Check to make sure the static instance of room db is never somehow null.

                return _roomDB;
            }
        }

        /// <summary>
        /// The current room the player is moving in.
        /// </summary>
        public static Room CurrentRoom;
        
        /// <summary>
        /// Loads a connected room by its id. This is called in the room before it aka the connected room leading to other rooms.
        /// Allowing farther rooms to be loaded in the background to prevent pop up and lag.
        /// </summary>
        /// <param name="roomID"></param>
        public static void LoadConnectedRoom(int roomID)
        {
            if (RoomDB[roomID]?.RoomPrefab == null)
                return;
            
            // If the room is already loaded just refresh it object instances without spawning new ones.
            if (IsRoomLoaded(roomID))
            {
                RefreshRoom(roomID);
                return;
            }
            
            // If no room instance with the passed in roomID is currently loaded spawn and load an instance. 
            // Also set it as the current SpawnedInstance in the RoomDB. This allows us to check if a room is already loaded later by checking 
            // if the SpawnedInstance is null or not. We should check the _loadedRoomsIDs first for performance reasons. 
            _roomDB[roomID].SpawnedInstance = GameObject.Instantiate(RoomDB[roomID].RoomPrefab);
            _loadedRoomsIDs.Add(roomID);
        }

        /// <summary>
        /// Checks to see if the passed in room id belongs to one of the already loaded rooms.
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public static bool IsRoomLoaded(int roomID)
        {
            for (int i = 0; i < _loadedRoomsIDs.Count; i++)
            {
                // We check the _loadedRoomsIDs first for performance reasons and
                // as a safety check just in case somehow a spawned instance hasn't been cleaned up fully yet.
                if (_loadedRoomsIDs[i] == roomID && _roomDB[roomID].SpawnedInstance != null)
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
        /// This is called on the room being entered. 
        /// </summary>
        /// <param name="roomID"></param>
        public static void OnRoomEntered(int roomID)
        {
            SpawnRoomObjects(roomID);
        }

        /// <summary>
        /// This sets up the initial room when loading or starting a new file. Also happens when entering a new region.
        /// </summary>
        public static void SetInitialRoom(int roomID)
        {
            LoadConnectedRoom(roomID);
            RefreshRoom(roomID);
            
            SpawnRoomObjects(roomID);
        }
        
        /// <summary>
        /// Tells a room to spawn its game objects and allow them to call OnEnable.
        /// </summary>
        public static void SpawnRoomObjects(int roomID)
        {
            // Don't try to SpawnObjects in a room that hasn't loaded a spawned instance yet.
            if (!IsRoomLoaded(roomID))
                return;
        }

        public static void OnRoomExit(int roomID)
        {
           
        }

        public static void CleanUpRoom(int roomId)
        {
            _roomDB[roomId].SpawnedInstance = null;
            _loadedRoomsIDs.Remove(roomId);
        }
    }
    
    [Serializable]
    public class Room
    {
        public string Name;
        public int RoomID;
        public Regions Region;
        /// <summary>
        /// The connected rooms that need to be loaded/deloaded when entering/existing 
        /// </summary>
        public List<int> ConnectedRoomsIDs = new List<int>();
        
        /// <summary>
        /// The Room Prefab asset to load into the game from the database.
        /// You should only get this when grabbing a Room reference directly from the RoomDB or you could risk a null value. 
        /// </summary>
        public GameObject RoomPrefab;

        /// <summary>
        /// This is only used during runtime. This allows for keeping track of the SpawnedInstances in the Room data itself.
        /// </summary>
        [NonSerialized] public GameObject SpawnedInstance;
    }
}