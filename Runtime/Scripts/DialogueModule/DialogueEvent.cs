using SF.Events;
using UnityEngine;

namespace SF.DialogueModule
{
    public enum DialogueEventTypes
    {
        DialoguePopup
    }
    public struct DialogueEvent
    {
        public DialogueEventTypes EventType;
        public string Dialogue;
        public DialogueEvent(DialogueEventTypes eventType, string dialogue)
        {
            EventType = eventType;
            Dialogue = dialogue;
        }
        static DialogueEvent dialogueEvent;

        public static void Trigger(DialogueEventTypes eventType, string dialogue)
        {
            dialogueEvent.EventType = eventType;
            dialogueEvent.Dialogue = dialogue;
            EventManager.TriggerEvent(dialogueEvent);
        }
    }
}
