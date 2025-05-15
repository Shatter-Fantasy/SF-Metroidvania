using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.RoomModule
{
    [CreateAssetMenu(fileName = "Room DB", menuName = "SF/Data/Rooms/Room Database")]
    public class RoomDB : ScriptableObject , IList<Room>
    {
        private static RoomDB _instance;

        public static RoomDB Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                if (value == null)
                    return;

                _instance = value;
            }
        }
        
        public List<Room> Rooms = new();
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void OnEnable()
        {
            if (Instance == null)
            {

                Instance = this;
            }
        }

        public IEnumerator<Room> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Room item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
        
        public bool Contains(Room item)
        {
            return true;
        }

        public void CopyTo(Room[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Room item)
        {
            throw new NotImplementedException();
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

        public Room this[int index]
        {
            get => Rooms.Find(x => x.RoomID == index);
            set => throw new NotImplementedException();
        }
    }
}
