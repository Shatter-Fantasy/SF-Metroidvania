using UnityEngine;

namespace SF.Events
{
    public enum RespawnEventTypes
    {
        InitialRespawn, // First Spawn of the scene loading.
        RespawnAll,
        PlayerRespawnAll, // This maily be used for multiplayer set ups.
        ItemsRespawnAll, //Items, Collectables, Interactables, and so forth.
        GameObjectRespawn, // This is used for respawning a single specific game object of any type.
    }
    public struct RespawnEvent : IEvent
    {
        public RespawnEventTypes EventType;
        public GameObject RespawnGameObject;
        public RespawnEvent(RespawnEventTypes eventType, GameObject respawnGameObject = null)
        {
            EventType = eventType;
            RespawnGameObject = respawnGameObject;
        }
        static RespawnEvent respawnEvent;

         public static void Trigger(RespawnEventTypes eventType)
         {
            respawnEvent.EventType = eventType;
            EventManager.TriggerEvent(respawnEvent);
         }
         public static void Trigger(RespawnEventTypes eventType, GameObject respawnGameObject)
         {
            respawnEvent.EventType = eventType;
            respawnEvent.RespawnGameObject = respawnGameObject;
            EventManager.TriggerEvent(respawnEvent);
         }
    }
}
