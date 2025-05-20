using UnityEngine.Events;

namespace SF.Interactables
{
    public enum CollectableEventType
    {
        ScorePickUp,
        HealthPickUp
    }

    [System.Serializable]
    public class CollectableEvent 
    {
        public CollectableEventType EventType;
        public UnityEvent CollectableUnityEvent;

        public void OnEventTrigger()
        {
            CollectableUnityEvent?.Invoke();
        }
    }
}
