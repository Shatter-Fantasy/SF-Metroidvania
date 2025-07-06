using UnityEngine;

namespace SF.CommandModule
{
    /// <summary>
    /// A Command to change the games current playing background music.
    /// </summary>
    [System.Serializable]
    [CommandMenu("Audio/Music")]
    public class MusicCommand : CommandNode, ICommand
    {
        [Header("Audio Objects")]
        [SerializeField] private AudioClip _musicTrack;
        [SerializeField] private AudioSource _sfxSource;

        protected override bool CanDoCommand()
        {
            return _musicTrack != null && _sfxSource != null;
        }
        protected override void DoCommand()
        {
            _sfxSource.clip = _musicTrack;
            _sfxSource.Play();
        }

        protected override async Awaitable DoAsyncCommand()
        {
            _sfxSource.clip = _musicTrack;
            _sfxSource.Play();
            await Awaitable.MainThreadAsync();
        }
    }
}
