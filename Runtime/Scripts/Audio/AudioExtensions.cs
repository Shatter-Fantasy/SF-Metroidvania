using UnityEngine;

namespace SF.AudioModule
{
    /// <summary>
    /// A set of extension and helper methods for audio related stuff like converting float volumes to their decibal values that can be used in Unity's AudioMixers.
    /// </summary>
    public static class AudioExtensions
    {
        /// <summary>
        /// Converts a volume amplitude in float to decibal for loudness
        /// </summary>
        /// <param name="volumeAmplitude"></param>
        /// <returns>The a decibal as a float.</returns>
        public static float VolumeToDecibel(this float volumeAmplitude)
        {
            if (volumeAmplitude > 1)
                volumeAmplitude = 1;

            // Decibel = 20 * log10(amplitude aka float volume).
            return Mathf.Log10(volumeAmplitude) * 20;
        }

        // TODO: Write the DecibelToVolume extension method.
    }
}
