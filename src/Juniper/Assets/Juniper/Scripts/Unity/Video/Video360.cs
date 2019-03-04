#if UNITY_MODULES_AUDIO && UNITY_MODULES_VIDEO

using Juniper.Progress;

using System;

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

        [Range(0, 8)]
        public float exposure = 1;

        [Range(0, 360)]
        public float rotation;

        public bool useMipMap = true;
        public Mode layout = Mode.Spherical;
        public ImageType imageType;
        public bool mirror180OnBack = true;
        public StereoLayout stereoLayout;
        public UnityEvent onLoop;
        public EventHandler Loop;

        public float secondsToCache = Units.Days.Seconds(1);

        [HideInNormalInspector]
        [SerializeField]
        private uint width;

        [HideInNormalInspector]
        [SerializeField]
        private uint height;

        private VideoPlayer player;
        private AudioSource[] audioTracks;

        private RenderTexture renderTexture;

        private Material skyboxMaterial;

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (videoClip?.Asset != null)
            {
                width = videoClip.Asset.width;
                height = videoClip.Asset.height;
            }
        }

#endif

        public enum Mode
        {
            Cube,
            Spherical
        }

        public enum ImageType
        {
            Degrees360,
            Degrees180
        }

        public enum StereoLayout
        {
            None,
            SideBySide,
            OverUnder
        }

        public override void Awake()
        {
            base.Awake();

            if (skyboxMaterial == null)
            {
                skyboxMaterial = new Material(Shader.Find("Skybox/Panoramic"));
            }

            if (layout == Mode.Spherical)
            {
                if (skyboxMaterial.IsKeywordEnabled(SIDES_6))
                {
                    skyboxMaterial.DisableKeyword(SIDES_6);
                }
                skyboxMaterial.EnableKeyword(LAT_LON);
            }
            else
            {
                if (skyboxMaterial.IsKeywordEnabled(LAT_LON))
                {
                    skyboxMaterial.DisableKeyword(LAT_LON);
                }
                skyboxMaterial.EnableKeyword(SIDES_6);
            }

            skyboxMaterial.SetInt("_Mapping", (int)layout);
            skyboxMaterial.SetInt("_ImageType", (int)imageType);
            skyboxMaterial.SetInt("_MirrorOnBack", mirror180OnBack ? 1 : 0);
            skyboxMaterial.SetInt("_Layout", (int)stereoLayout);

            player = this.EnsureComponent<VideoPlayer>();
            player.waitForFirstFrame = true;
            player.playOnAwake = false;
            player.loopPointReached += OnLoop;
            player.isLooping = true;
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.renderMode = VideoRenderMode.RenderTexture;

            CreateAudioTracks(1);
        }

        public override void Load(IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
            Enter();
            if (player != null)
            {
                VideoPlayer.EventHandler exec = null;
                exec = player =>
                {
                    player.prepareCompleted -= exec;

                    renderTexture = new RenderTexture(
#if UNITY_2018_2_OR_NEWER
                        (int)player.width, (int)player.height,
#else
                        (int)width, (int)height,
#endif
                        24, textureFormat)
                    {
                        dimension = TextureDimension.Tex2D,
                        antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing),
                        useMipMap = useMipMap,
                        depth = 16
                    };

                    renderTexture.Create();

                    skyboxMaterial.SetTexture("_MainTex", renderTexture);

                    player.targetTexture = renderTexture;
                    Play();
                    prog?.SetProgress(1);
                };

                player.prepareCompleted += exec;

                player.source = VideoSource.Url;
                player.url = videoClip.LoadPath;
                player.Prepare();
            }
        }

        public void Play()
        {
            if (audioTracks?.Length != player.audioTrackCount)
            {
                CreateAudioTracks(player.audioTrackCount);
            }

            RenderSettings.skybox = skyboxMaterial;

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
                        aud = this.EnsureComponent<AudioSource>();
                    }
                    else
                    {
                        aud = gameObject.AddComponent<AudioSource>();
                    }
                }

                aud.Spatialize();
                aud.playOnAwake = false;
                player.EnableAudioTrack(i, true);
                player.SetTargetAudioSource(i, aud);

                audioTracks[i] = aud;
            }
        }

        public override void Update()
        {
            base.Update();
            if (skyboxMaterial != null)
            {
                skyboxMaterial.SetColor("_Tint", tint);
                skyboxMaterial.SetFloat("_Exposure", exposure);
                skyboxMaterial.SetFloat("_Rotation", rotation);
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