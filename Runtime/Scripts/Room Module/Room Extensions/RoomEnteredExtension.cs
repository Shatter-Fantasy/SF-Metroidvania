using UnityEngine;
using UnityEngine.Events;

namespace SF.RoomModule
{
    /// <summary>
    /// Runs customizable logic when the connected <see cref="RoomController"/>  has been entered.
    /// </summary>
    public class RoomEnteredExtension : MonoBehaviour, IRoomExtension
    {
        public RoomExtensionType RoomExtensionType { get; protected set; } = RoomExtensionType.OnRoomEntered;

        [SerializeField] private UnityEvent _onRoomEnteredUnityEvent;
        public void Process()
        {
            _onRoomEnteredUnityEvent?.Invoke();
        }
    }
}
