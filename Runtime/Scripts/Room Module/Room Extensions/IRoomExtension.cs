using UnityEngine;

namespace SF.RoomModule
{
    /// <summary>
    /// 
    /// </summary>
    public enum RoomExtensionType
    {
        OnRoomEntered = 0,
        OnRoomEnteredFirstTime = 1,
        OnRoomExit = 2,
        OnRoomExitedFirstTime = 4,
        OnRoomLoaded = 8,
        OnRoomUnloaded = 16,
    }
    /// <summary>
    /// Implement to create a custom extension that adds logic for rooms.
    /// </summary>
    /// <example>
    /// Can be used to create custom room logic for boss rooms, first time entering a room.
    /// </example>
    public interface IRoomExtension
    {
        public RoomExtensionType RoomExtensionType { get;}
        void Process();
    }
}
