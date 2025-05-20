using UnityEngine.Events;

namespace SF.Events
{
    [System.Serializable]
    public class SerializedEvent 
    {
        [field: UnityEngine.SerializeField] public string Name { get; private set; }
        public UnityEvent Event;

        public void Invoke()
        {
            Event?.Invoke();
        }
    }
}
