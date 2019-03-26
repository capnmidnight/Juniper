#if UNITY_XR_WINDOWSMR_METRO
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.WSA;

namespace Juniper.Unity.Audio
{
    public abstract class WindowsMRInteractionAudio : AbstractInteractionAudio
    {
#if UNITY_MODULES_AUDIO
        protected override AudioSource InternalSpatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, loop, group);

            var ms = audioSource.EnsureComponent<AudioSpatializerMicrosoft>();
            ms.Value.roomSize = AudioSpatializerMicrosoft.RoomSize.Medium;

            return audioSource;
        }
#endif
    }
}
#endif