#if UNITY_MODULES_AUDIO

using System;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.Audio
{
    public class AmbientAudio : SubSceneController
    {
        public StreamableAudioClip audioClip;

        [Header("Spatialization")]
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

            player = this.Ensure<AudioSource>();
            player.playOnAwake = false;
            player.loop = true;
            player.spatialize = spatialize;
            player.spatialBlend = spatialBlend;
            if (Find.Any(out InteractionAudio audio))
            {
                player.outputAudioMixerGroup = audio.defaultMixerGroup;
            }
        }

        public override void Enter(IProgress prog)
        {
            base.Enter(prog);
            StartCoroutine(audioClip.Load(
                clip =>
                {
                    player.clip = clip;
                    Complete();
                    player.Play();
                },
                exp =>
                {
                    ScreenDebugger.PrintException(exp, "AmbientAudio");
                },
                prog));
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            Complete();
        }
    }
}

#endif