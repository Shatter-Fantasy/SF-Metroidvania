using System.Collections.Generic;
using UnityEngine;

namespace SF.RoomModule.RegionModule
{
    using SF.DataModule;

    [System.Serializable]
    public struct RegionTransitionConnection
    {
        public RegionDataAsset RegionToTransitionTo;
        /// <summary>
        /// The <see cref="RegionDataAsset.RegionTransitionData.TransitionID"/> to use to calculate the new room being entered.
        /// </summary>
        public int TransitionIDToGoTo;

        public RegionTransitionConnection(int regionID = 0, int transitionIDToGoTo = 0 )
        {
            RegionToTransitionTo = RegionSystem.RegionDatabase != null 
                ? RegionSystem.RegionDatabase[regionID] 
                : null;

            TransitionIDToGoTo = transitionIDToGoTo;
        }
    }
    

    [CreateAssetMenu(menuName = "SF/Regions/Region Data", fileName = "Region Data")]
    public class RegionDataAsset : DTOAssetBase
    {
        [System.Serializable]
        public struct RegionTransitionData
        {
            /// <summary>
            /// The id for a set of transition data in a <see cref="RegionDataAsset"/>
            /// </summary>
            public int TransitionID;
            /// <summary>
            /// The other region this transition connects to.
            /// </summary>
            public RegionDataAsset RegionToTransitionTo;
            /// <summary>
            /// The <see cref="Room.RoomID"/>  of the room to spawn in when the new region is loaded.
            /// </summary>
            public int RoomID;

            /// <summary>
            /// The spawn position for the player when entering into the region.
            /// <remarks>
            /// Unlike the RoomID which needs the value for the room in the newly loading region.
            /// This value is for the spawn position of this <see cref="RegionDataAsset"/> when entering from the <see cref="RoomID"/>
            /// and coming into this <see cref="RegionDataAsset"/> region.
            /// </remarks>
            /// </summary>
            public Vector3 LocalSpawnPositionInCurrentRoom;
        }
        
        /// <summary>
        /// The scene index for the region.
        /// </summary>
        public int SceneIndex;

        public List<Room> Rooms = new();
        public List<RegionTransitionData> RegionTransitionDataSet = new();
        
        public void CleanUpRegion()
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                if(Rooms[i] == null)
                    continue;
                Rooms[i].SpawnedRoomController = null;
                RoomSystem.CleanUpRoom(Rooms[i].RoomID);
            }
        }
        
        public bool Contains(int roomID)
        {
            Room room = Rooms.Find(roomInDB => roomInDB.RoomID == roomID);
            return room != null;
        }

        public bool Contains(Room item)
        {
            Room room = Rooms.Find(roomInDB => roomInDB.RoomID == item?.RoomID);
            return room != null;
        }

        public void Add(Room newRoom)
        {
            Rooms.Add(newRoom);
        }

        public void Clear()
        {
            Rooms.Clear();
        }

        /// <summary>
        /// Tries to remove a room from the <see cref="Rooms"/> list.
        /// Return true if there was a room to remove or false if none matching the value to remove existed.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool Remove(Room room)
        {
            if (Rooms.Contains(room))
            {
                Rooms.Remove(room);
                return true;
            }

            return false;
        }

        public int RoomCount()
        {
            if (Rooms == null)
                return 0;
            return Rooms.Count;
        }
        
        /// <summary>
        /// Used to reset the room ids of the rooms in the database in cases the rooms values have been switch or reorganized in the list.
        /// </summary>
        public void ResetRoomIds()
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                Rooms[i].RoomID                                           = i;
                Rooms[i].RoomPrefab.GetComponent<RoomController>().RoomID = i;
            }
        }

        /// <summary>
        /// We search via the RoomID first. If the RoomID doesn;t exist than
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="NotImplementedException"></exception>
        public Room this[int index]
        {
            // We have to make sure we first have a value at that index.
            get
            {
                if (Rooms.Count < index)
                {
                    // If we try to return an index that is bigger than the size of the room collection return null for safety.
                    return null;
                }

                return !Contains(index)
                    ? null
                    : Rooms[index];
            }
            set { Rooms[index] = value; }
        }
    }
}
