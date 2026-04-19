using SF.DialogueModule.Nodes;
using SFEditor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.Audio;

namespace SFEditor.Dialogue.Graphs
{
    [System.Serializable]
    [UseWithContext(typeof(ConversationContextNode))]
    [UseWithGraph(typeof(DialogueGraph))]
    public class SFXBlockNode : BlockNode, INodeConvertor
    {
        public string AudioResourceOptionsName { get; } = "Audio Resource";
        public string AudioSourceOptionsName { get; } = "Audio Source";

        protected override void OnDefineOptions(IOptionDefinitionContext  context)
        {		    
            context.AddOption<AudioResource>(AudioResourceOptionsName);
            context.AddOption<AudioSource>(AudioSourceOptionsName);
        }

        public IRuntimeNode ConvertToRuntimeNode()
        {
            GetNodeOptionByName(AudioResourceOptionsName).TryGetValue(out AudioResource audioResource);
            GetNodeOptionByName(AudioSourceOptionsName).TryGetValue(out AudioSource audioSource);

            return new SFXRuntimeNode()
            {
                AudioResource = audioResource,
                AudioSource = audioSource
            };
        }
    }
}
