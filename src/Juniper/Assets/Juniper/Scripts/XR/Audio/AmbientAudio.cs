#if UNITY_MODULES_AUDIO

using System;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.Unity.Audio
{
    public class AmbientAudio : SubSceneController
    {
        public StreamableAudioClip audioClip;

        private AudioSource player;

        public override void Awake()
        {
            base.Awake();

            var audio = ComponentExt.FindAny<InteractionAudio>();
            player = this.Ensure<AudioSource>();
            player.outputAudioMixerGroup = audio.defaultMixerGroup;
            player.playOnAwake = false;
            player.loop = true;
        }

        public override void Load(IProgress prog = null)
        {
            prog?.Report(0);
            Enter();
            if (player != null)
            {
                StartCoroutine(audioClip.Load(
                    clip =>
                    {
                        player.clip = clip;
                        player.Play();
                    },
                    exp =>
                    {
                        ScreenDebugger.PrintException(exp, "AmbientAudio");
                        prog?.Report(1);
                    },
                    prog));
            }
        }
    }
}

#endif