#if RESONANCEAUDIO
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
        protected ResonanceAudioListener goog;

        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                var js = ComponentExt.FindAny<JuniperSystem>();
                goog = listener.Ensure<ResonanceAudioListener>().Value;
                goog.stereoSpeakerModeEnabled = Application.isEditor || js.DisplayType != DisplayTypes.Stereo;
            }

            return false;
        }

        public override void Uninstall()
        {
            ComponentExt.FindAny<AudioListener>()
                ?.Remove<ResonanceAudioListener>();

            base.Uninstall();
        }

        protected override AudioSource InternalSpatialize(AudioSource audioSource, bool loop, AudioMixerGroup group)
        {
            audioSource = base.InternalSpatialize(audioSource, loop, group);

            var goog = audioSource.Ensure<ResonanceAudioSource>().Value;
            goog.quality = ResonanceAudioSource.Quality.High;

            return audioSource;
        }
#endif
    }
}
#endif
