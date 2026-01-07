using SF.DialogueModule;
using UnityEngine;
using UnityEngine.Audio;

namespace SF.DialogueModule.Nodes
{
    /// <summary>
    /// A runtime node used to play and control sfx.
    /// </summary>
    [System.Serializable]
    public class SFXRuntimeNode : RuntimeNode, IRuntimeNode
    {
        /// <summary>
        /// The <see cref="AudioResource"/> to play.This can be an audio clip, <see cref="AudioRandomContainer"/>,
        /// or if using Unity 6.3 or newer the new Audio Scriptable Processors.
        /// </summary>
        public AudioResource AudioResource;

        /// <summary>
        ///  The in scene audio source to be played.
        /// </summary>
        public AudioSource AudioSource;

        public void ProcessNode()
        {
            // TODO: This should be hooked up to whatever audio manager we want to use.
            // Most likely FMOD.
            if (AudioResource == null || AudioSource == null)
                return;
            
            AudioSource.resource = AudioResource;
            AudioSource.Play();
        }
    }
}
