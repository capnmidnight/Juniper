#if UNITY_XR_MAGICLEAP
using UnityEngine;
using UnityEngine.Audio;

namespace Juniper.Audio
{
    public abstract class MagicLeapInteractionAudio : AbstractInteractionAudio
    {
#if UNITY_MODULES_AUDIO
        protected override AudioSource InternalSpatialize(AudioSource audioSource, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, group);
            audioSource.Ensure<MSA.MSASource>(null, (msa) =>
                msa.Override3DProperties = true);
            return audioSource;
        }

        protected override void UninstallSpatialization(AudioSource audioSource)
        {
            base.UninstallSpatialization(audioSource);

            audioSource.Remove<MSA.MSASource>();
        }
#endif
    }
}
#endif
