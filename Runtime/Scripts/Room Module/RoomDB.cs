using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.RoomModule
{
    [CreateAssetMenu(fileName = "Room DB", menuName = "SF/Data/Rooms/Room Database")]
    public class RoomDB : ScriptableObject , IList<Room>
    {
        public List<Room> Rooms = new();
        
        /// <summary>
        /// Will be called when the Rooms list value gets changed such as add/remove.
        /// This is also called when a new list is assigned into the Rooms value.
        /// </summary>
        public Action OnRoomsValueChanged;

        public IEnumerator<Room> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Room newRoom)
        {
            Rooms.Add(newRoom);
            OnRoomsValueChanged();
        }

        public void Clear()
        {
            Rooms.Clear();
            OnRoomsValueChanged();
        }
        
        public bool Contains(Room item)
        {
            Room room = Rooms.Find(roomInDB => roomInDB.RoomID == item?.RoomID);
            return room != null;
        }
        
        public bool Contains(int roomID)
        {
            Room room = Rooms.Find(roomInDB => roomInDB.RoomID == roomID);
            return room != null;
        }

        public void CopyTo(Room[] array, int arrayIndex)
        {
            throw new NotImplementedException();
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
                OnRoomsValueChanged();
                return true;
            }

            return false;
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
        public int IndexOf(Room item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Room item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to reset the room ids of the rooms in the database in cases the rooms values have been switch or reorganized in the list.
        /// </summary>
        public void ResetRoomIds()
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                Rooms[i].RoomID = i;
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

            set => throw new NotImplementedException();
        }
    }
}
