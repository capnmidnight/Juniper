#if UNITY_XR_OCULUS

using UnityEngine;
using UnityEngine.Audio;

namespace Juniper.Audio
{
    public abstract class OculusInteractionAudio : AbstractInteractionAudio
    {
        protected override string DefaultAudioMixer
        {
            get
            {
                return "SpatializerMixer";
            }
        }

#if UNITY_MODULES_AUDIO

        protected override AudioSource InternalSpatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, loop, group);
            var oculus = audioSource.Ensure<ONSPAudioSource>().Value;
            oculus.EnableSpatialization = true;
            oculus.UseInvSqr = true;
            oculus.Near = audioSource.minDistance;
            oculus.Far = audioSource.maxDistance;
            oculus.VolumetricRadius = 0;
            return audioSource;
        }

        protected override void UninstallSpatialization(AudioSource audioSource)
        {
            base.UninstallSpatialization(audioSource);

            audioSource.Remove<ONSPAudioSource>();
        }

#endif
    }
}

#endif