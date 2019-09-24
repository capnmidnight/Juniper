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

            resListener = listener.Ensure<ResonanceAudioListener>().Value;
            resListener.stereoSpeakerModeEnabled = Application.isEditor
                || !ComponentExt.FindAny<>(out JuniperSystem js)
                || js.DisplayType != DisplayTypes.Stereo;

            if(ComponentExt.FindAny(out DisplayManager dsp))
            {
                dsp.DisplayTypeChange += OnDisplayTypeChange;
            }
        }

        private void OnDisplayTypeChange(object sender, DisplayTypes e)
        {
            resListener.stereoSpeakerModeEnabled = Application.isEditor || e != DisplayTypes.Stereo;
        }

        public override void Uninstall()
        {
            if(ComponentExt.FindAny(out AudioListener aud))
            {
                aud.Remove<ResonanceAudioListener>();
            }

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
