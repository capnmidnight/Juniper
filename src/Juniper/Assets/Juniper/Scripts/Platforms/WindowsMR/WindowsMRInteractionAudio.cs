#if UNITY_XR_WINDOWSMR_METRO
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.WSA;

namespace Juniper.Audio
{
    public abstract class WindowsMRInteractionAudio : AbstractInteractionAudio
    {
#if UNITY_MODULES_AUDIO
        protected override AudioSource InternalSpatialize(AudioSource audioSource, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, group);

            var ms = audioSource.Ensure<AudioSpatializerMicrosoft>();
            ms.Value.roomSize = AudioSpatializerMicrosoft.RoomSize.Medium;

            return audioSource;
        }

        protected override void UninstallSpatialization(AudioSource audioSource)
        {
            base.UninstallSpatialization(audioSource);
            audioSource.Remove<AudioSpatializerMicrosoft>();
        }
#endif
    }
}
#endif
