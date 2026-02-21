using UnityEngine;
using UnityEngine.Events;

namespace SF.DialogueModule
{
    public class ConversationUnityEventExtension : DialogueExtensionBase
    {
        [SerializeField] private UnityEvent _conversationUnityEvent;
        public override void ControlFlow()
        {
            _conversationUnityEvent?.Invoke();
        }
    }
}
