#if UNITY_MODULES_AUDIO && UNITY_MODULES_VIDEO

using System;
using System.Collections;

using Juniper.Audio;
using Juniper.Imaging;
using Juniper.Progress;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Video;

namespace Juniper.Video
{
    public class Video360 : SubSceneController
    {
        private const string LAT_LON = "_MAPPING_LATITUDE_LONGITUDE_LAYOUT";
        private const string SIDES_6 = "_MAPPING_6_FRAMES_LAYOUT";

        public StreamableVideoClip videoClip;

        public RenderTextureFormat textureFormat;
        public Color tint = Color.gray;
        public UnityEvent onLoop;
        public EventHandler Loop;

        public float secondsToCache = Units.Days.Seconds(1);

        private VideoPlayer player;
        private AudioSource[] audioTracks;

        private RenderTexture renderTexture;
        public bool useMipMap = true;

        private SkyboxManager skybox;

        private InteractionAudio interaction;

        public override void Awake()
        {
            base.Awake();

            skybox = ComponentExt.FindAny<SkyboxManager>();
            interaction = ComponentExt.FindAny<InteractionAudio>();

            player = this.Ensure<VideoPlayer>();
            player.waitForFirstFrame = true;
            player.playOnAwake = false;
            player.loopPointReached += OnLoop;
            player.isLooping = true;
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.renderMode = VideoRenderMode.RenderTexture;

            CreateAudioTracks(1);
        }

        public override void Enter(IProgress prog)
        {
            base.Enter(prog);

            if (player != null)
            {
                void exec(VideoPlayer player)
                {
                    player.prepareCompleted -= exec;

                    renderTexture = new RenderTexture(
                        (int)player.width, (int)player.height,
                        24,
                        textureFormat)
                    {
                        dimension = TextureDimension.Tex2D,
                        antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing),
                        useMipMap = useMipMap,
                        depth = 16
                    };

                    renderTexture.Create();

                    player.targetTexture = renderTexture;
                    StartCoroutine(SetSkyboxTexture());
                }

                player.prepareCompleted += exec;

                player.source = VideoSource.Url;
                player.url = videoClip.LoadPath;
                player.Prepare();
            }
        }

        private IEnumerator SetSkyboxTexture()
        {
            skybox.useMipMap = useMipMap;
            yield return skybox.SetTexture(renderTexture);
            Complete();
            Play();
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            Complete();
        }

        public void Play()
        {
            if (audioTracks?.Length != player.audioTrackCount)
            {
                CreateAudioTracks(player.audioTrackCount);
            }

            player.Play();
            foreach (var aud in audioTracks)
            {
                aud.Play();
            }
        }

        private void CreateAudioTracks(ushort count)
        {
            var oldAudioTracks = audioTracks;

            player.controlledAudioTrackCount = count;

            audioTracks = new AudioSource[count];
            for (ushort i = 0; i < count; ++i)
            {
                var aud = oldAudioTracks?.MaybeGet(i);
                if (aud == null)
                {
                    if (i == 0)
                    {
                        aud = this.Ensure<AudioSource>();
                    }
                    else
                    {
                        aud = gameObject.AddComponent<AudioSource>();
                    }
                }

                interaction.Spatialize(aud);
                aud.playOnAwake = false;
                player.EnableAudioTrack(i, true);
                player.SetTargetAudioSource(i, aud);

                audioTracks[i] = aud;
            }
        }

        protected virtual void OnLoop(VideoPlayer source)
        {
            onLoop?.Invoke();
            Loop?.Invoke(this, EventArgs.Empty);
        }
    }
}

#endif