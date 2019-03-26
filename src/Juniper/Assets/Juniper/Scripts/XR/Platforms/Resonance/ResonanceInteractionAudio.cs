#if RESONANCE
using UnityEngine;
using UnityEngine.Audio;

namespace Juniper.Unity.Audio
{
    public abstract class ResonanceInteractionAudio : AbstractInteractionAudio
    {
        protected override string DefaultAudioMixer
        {
            get
            {
                return "ResonanceAudioMixer";
            }
        }

#if UNITY_MODULES_AUDIO
        protected override AudioSource InternalSpatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, loop, group);

            var goog = audioSource.EnsureComponent<ResonanceAudioSource>().Value;
            goog.quality = ResonanceAudioSource.Quality.High;

            return audioSource;
        }
#endif
    }
}
#endif