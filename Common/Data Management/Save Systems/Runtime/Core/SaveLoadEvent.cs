using SF.Events;
using UnityEngine;

namespace SF.DataManagement
{

    public enum SaveLoadEventTypes
    {
        Saving,
        Loading,
        CopyingFile,
        DeletingSaveFile
    }
    public struct SaveLoadEvent : IEvent
    {
        public SaveLoadEventTypes EventType;
        
        public SaveLoadEvent(SaveLoadEventTypes eventType)
        {
            EventType = eventType;
        }
        
        static SaveLoadEvent saveLoadEvent;

        public static void Trigger(SaveLoadEventTypes eventType)
        {
            saveLoadEvent.EventType = eventType;
            EventManager.TriggerEvent(saveLoadEvent);
        }
    }
}
