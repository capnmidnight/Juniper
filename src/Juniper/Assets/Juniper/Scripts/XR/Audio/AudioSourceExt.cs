#if UNITY_MODULES_AUDIO

using Juniper.Unity.Audio;

using UnityEngine.Audio;

namespace UnityEngine
{
    /// <summary>
    /// Extensions methods for <see cref="AudioSource"/>
    /// </summary>
    public static class AudioSourceExt
    {
        /// <summary>
        /// Set the common spatialization parameters for an AudioSource, including adding the
        /// necessary spatialization component for the platforms spatialization plugin.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="group">      </param>
        /// <returns></returns>
        public static AudioSource Spatialize(this AudioSource audioSource, AudioMixerGroup group)
        {
            return InteractionAudio.Spatialize(audioSource, false, group);
        }

        /// <summary>
        /// Set the common spatialization parameters for an AudioSource, including adding the
        /// necessary spatialization component for the platforms spatialization plugin.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="loop">       </param>
        /// <returns></returns>
        public static AudioSource Spatialize(this AudioSource audioSource, bool loop)
        {
            return InteractionAudio.Spatialize(audioSource, loop, null);
        }

        /// <summary>
        /// Set the common spatialization parameters for an AudioSource, including adding the
        /// necessary spatialization component for the platforms spatialization plugin.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <returns></returns>
        public static AudioSource Spatialize(this AudioSource audioSource)
        {
            return InteractionAudio.Spatialize(audioSource, false, null);
        }
    }
}

#endif