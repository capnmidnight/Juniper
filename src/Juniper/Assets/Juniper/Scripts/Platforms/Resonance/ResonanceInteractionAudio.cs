#if UNITY_GOOGLE_RESONANCE_AUDIO
using System;
using Juniper.Display;
using Juniper.XR;
using UnityEngine;
using UnityEngine.Audio;

namespace Juniper.Audio
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
        protected ResonanceAudioListener resListener;

        public override void Install(bool reset)
        {
            base.Install(reset);

            var js = ComponentExt.FindAny<JuniperSystem>();
            var dsp = ComponentExt.FindAny<DisplayManager>();

            resListener = listener.Ensure<ResonanceAudioListener>().Value;
            resListener.stereoSpeakerModeEnabled = Application.isEditor || js.DisplayType != DisplayTypes.Stereo;
            dsp.DisplayTypeChange += OnDisplayTypeChange;
        }

        private void OnDisplayTypeChange(object sender, DisplayTypes e)
        {
            resListener.stereoSpeakerModeEnabled = Application.isEditor || e != DisplayTypes.Stereo;
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

            var resSource = audioSource.Ensure<ResonanceAudioSource>().Value;
            resSource.quality = ResonanceAudioSource.Quality.High;

            return audioSource;
        }

        protected override void UninstallSpatialization(AudioSource audioSource)
        {
            base.UninstallSpatialization(audioSource);

            audioSource.Remove<ResonanceAudioSource>();
        }
#endif
    }
}
#endif
