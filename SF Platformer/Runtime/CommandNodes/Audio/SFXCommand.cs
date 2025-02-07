using UnityEngine;
using SF.CommandModule;

namespace SF
{
    [System.Serializable]
    public class SFXCommand : CommandNode, ICommand
    {
        public AudioClip SFXClip;
        public float Volume;
        public AudioSource SFXSource;


        public async override Awaitable Use() 
        {
            if (SFXClip == null)
                return;

            if(IsAsyncCommand)
                await PlayAudioAsync();
            else PlayAudio();
        }

        private void PlayAudio()
        {
            SFXSource.PlayOneShot(SFXClip, Volume);
        }

        private async Awaitable PlayAudioAsync()
        {
            SFXSource.PlayOneShot(SFXClip,Volume);
            await Awaitable.MainThreadAsync();
        }
    }
}
