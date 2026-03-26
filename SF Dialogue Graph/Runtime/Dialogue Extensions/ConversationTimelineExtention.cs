using SF.DialogueModule;
using UnityEngine.Playables;

namespace SF.DialogueModule
{
    public class ConversationTimelineExtension : DialogueExtensionBase
    {
        public PlayableDirector Director;
        public override void ControlFlow()
        {
            Director?.Play();
        }
    }
}
