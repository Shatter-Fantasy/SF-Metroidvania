using UnityEngine;


namespace SF.CommandModule
{
    [System.Serializable]
    [CommandMenu("Audio/Play Sound")]
    public class SFXCommand : CommandNode, ICommand
    {
        [SerializeField] private AudioClip _sfxClip;
        [SerializeField] private float _volume = .5f;
        [SerializeField] private AudioSource _sfxSource;

        protected override bool CanDoCommand()
        {
            return _sfxClip != null && _sfxSource != null;
        }

        protected override void DoCommand()
        {
            _sfxSource.PlayOneShot(_sfxClip, _volume);
        }

        protected override async Awaitable DoAsyncCommand()
        {
            _sfxSource.PlayOneShot(_sfxClip,_volume);
            await Awaitable.MainThreadAsync();
        }
    }
}
