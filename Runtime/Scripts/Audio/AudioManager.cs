using UnityEngine;

namespace SF.AudioModule
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _audioSource;


        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AudioManager>();

                    // If no AudioManager was found in the scene make one than set it as the instance for the AudioManager.
                    if(_instance == null)
                    {
                        GameObject go = new GameObject("Audio Manager", typeof(AudioManager));
                        Instantiate(go);
                        _instance = go.GetComponent<AudioManager>();
                    }

                    if(_instance._audioSource == null)
                    {
                        _instance._audioSource = _instance.GetComponent<AudioSource>();

                        // If no AudioSource was found, make one.
                        if(_instance._audioSource == null)
                        {
                            AudioSource audioSource = _instance.gameObject.AddComponent<AudioSource>();
                            _instance._audioSource = audioSource;
                        }
                    }
                }

                return _instance; 
            }
            set 
            {   
                if(_instance == null)
                    _instance = value;
            }
        }

        private void Awake()
        {
            Instance = this;

            if(_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays a sound effect through the cached Audio Source in the Audio Manager.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="volume"></param>
        public void PlayOneShot(AudioClip audioClip, float volume = 0.75f)
        {
            if(_audioSource == null)
                return;

            _audioSource.PlayOneShot(audioClip, volume);
        }
    }
}
