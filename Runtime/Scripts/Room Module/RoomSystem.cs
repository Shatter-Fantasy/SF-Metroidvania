using System.Collections.Generic;
using SF.CameraModule;
using UnityEngine;

namespace SF.RoomModule
{
    public static class RoomSystem
    {

        /// <summary>
        /// List of loaded rooms.
        /// </summary>
        private static List<Room> _loadedRooms = new();
        private static RoomDB _roomDB;
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

        public static void LoadRoom(int roomID)
        {
            if (RoomDB[roomID]?.RoomPrefab == null)
                return;
            
            var room = Object.Instantiate(RoomDB[roomID].RoomPrefab);
            
            CameraController.ChangeCameraConfiner(room.GetComponent<RoomController>().RoomConfiner);
        }
    }
    
    [System.Serializable]
    public class Room
    {
        public string Name;
        public int RoomID;
        public Regions Region;
        public GameObject RoomPrefab;
    }
}