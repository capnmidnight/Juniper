#if UNITY_XR_MAGICLEAP
using UnityEngine;
using UnityEngine.Audio;

namespace Juniper.Unity.Audio
{
    public abstract class MagicLeapInteractionAudio : AbstractInteractionAudio
    {
#if UNITY_MODULES_AUDIO
        protected override AudioSource InternalSpatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, loop, group);
            audioSource.EnsureComponent<MSA.MSASource>(null, (msa) =>
                msa.Override3DProperties = true);
            return audioSource;
        }
#endif
    }
}
#endif