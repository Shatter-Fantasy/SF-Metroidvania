using SF.Events;
using UnityEngine;

namespace SF.SpawnModule
{
    public class Spawnable : MonoBehaviour, EventListener<RespawnEvent>
    {
        public void OnEvent(RespawnEvent eventType)
        {
            Spawn();
        }

        private void OnEnable()
        {
            this.EventStartListening<RespawnEvent>();
        }
        private void OnDisable()
        {
            this.EventStopListening<RespawnEvent>();
        }
        private void Spawn()
        {
            
        }
    }
}
