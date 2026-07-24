using SF.Graphs.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.Audio;

namespace SFEditor.Graphs.Graphs
{
    using SFEditor.Graphs.Nodes;

    [System.Serializable]
    public class SFXBlockNode : BlockNode, INodeConvertor
    {
        public string AudioResourceOptionsName { get; } = "Audio Resource";
        public string AudioSourceOptionsName { get; } = "Audio Source";

        protected override void OnDefineOptions(IOptionDefinitionContext  context)
        {		    
            context.AddOption<AudioResource>(AudioResourceOptionsName);
            context.AddOption<AudioSource>(AudioSourceOptionsName);
        }

        public SFRuntimeNode ConvertToRuntimeNode()
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