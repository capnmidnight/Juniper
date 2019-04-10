#if UNITY_MODULES_AUDIO

using System;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.Unity.Audio
{
    public class AmbientAudio : SubSceneController
    {
        public StreamableAudioClip audioClip;

        [Header("Spatializationi")]
        public bool spatialize;

        [Range(0, 1)]
        public float spatialBlend;

        private AudioSource player;

#if UNITY_EDITOR
        private bool wasSpatialize;
        public void OnValidate()
        {
            if (spatialize != wasSpatialize)
            {
                wasSpatialize = spatialize;
                spatialBlend = spatialize ? 1 : 0;
            }
            else if (spatialBlend > 0)
            {
                wasSpatialize = spatialize = true;
            }
        }
#endif

        public override void Awake()
        {
            base.Awake();

            var audio = ComponentExt.FindAny<InteractionAudio>();
            player = this.Ensure<AudioSource>();
            player.outputAudioMixerGroup = audio.defaultMixerGroup;
            player.playOnAwake = false;
            player.loop = true;
            player.spatialize = spatialize;
            player.spatialBlend = spatialBlend;
        }

        public override void Load(IProgress prog = null)
        {
            prog?.Report(0);
            Enter();
            if (player == null)
            {
                ScreenDebugger.Print("No audio player for AmbientAudio!");
            }
            else
            {
                ScreenDebugger.Print("Starting ambient audio");
                StartCoroutine(audioClip.Load(
                    clip =>
                    {
                        ScreenDebugger.Print("Playing ambient audio");
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