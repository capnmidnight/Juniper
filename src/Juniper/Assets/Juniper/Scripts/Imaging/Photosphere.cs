using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.Imaging
{
    public delegate CachingStrategy CachingStrategyNeeded(Photosphere source);
    public delegate IImageCodec<Texture2D> TextureDecoderNeeded(Photosphere source);
    public delegate string CubemapImageNeeded(Photosphere source);

    public class Photosphere : MonoBehaviour, IProgress
    {
        public const string PHOTOSPHERE_LAYER = "Photospheres";
        public static readonly string[] PHOTOSPHERE_LAYER_ARR = { PHOTOSPHERE_LAYER };

        private SkyboxManager skybox;

        protected IImageCodec<Texture2D> codec;
        protected CachingStrategy cache;

        public string CubemapName;

        [Range(0, 1)]
        public float ProgressToReady;

        public event CachingStrategyNeeded CacheNeeded;
        public event TextureDecoderNeeded DecoderNeeded;
        public event CubemapImageNeeded CubemapNeeded;
        public event Action<Photosphere> Ready;
        private TaskFactory mainThread;

        public bool IsReady
        {
            get;
            private set;
        }

        public virtual void Awake()
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            mainThread = new TaskFactory(scheduler);

            if (!Find.Any(out skybox))
            {
                skybox = this.Ensure<SkyboxManager>();
            }
        }

        private void OnEnable()
        {
            foreach (var child in transform.Children())
            {
                child.Activate();
            }
        }

        public virtual void OnDisable()
        {
            foreach (var child in transform.Children())
            {
                child.Deactivate();
            }
        }

        private async Task ReadCubemap()
        {
            try
            {
                var progs = this.Split("Load", "Decode");
                var imageStream = await cache.Open(CubemapName + codec.ContentType, progs[0]);
                if (imageStream == null)
                {
                    Debug.Log("No cubemap found " + CubemapName);
                }
                else
                {
                    using (imageStream)
                    {
                        var co = await mainThread.StartNew(() =>
                        {
                            var texture = codec.Deserialize(imageStream, progs[1]);

                            skybox.exposure = 1;
                            skybox.imageType = SkyboxManager.ImageType.Degrees360;
                            skybox.layout = SkyboxManager.Mode.Cube;
                            skybox.mirror180OnBack = false;
                            skybox.rotation = 0;
                            skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                            skybox.tint = Color.gray;
                            skybox.useMipMap = false;
                            return skybox.SetTexture(texture);

                        });

                        await co.AsTask();

                        OnReady();
                    }
                }
            }
            catch (Exception exp)
            {
                Debug.LogError("Cubemap load error " + CubemapName, this);
                Debug.LogException(exp, this);
                throw;
            }
        }

        protected void OnReady()
        {
            this.Report(1);
            IsReady = true;
            Ready?.Invoke(this);
        }

        public void ReportWithStatus(float progress, string status)
        {
            Status = status;
            ProgressToReady = progress;
        }

        private Task readingTask;

        public virtual bool IsBusy
        {
            get
            {
                return readingTask.IsRunning();
            }
        }

        public virtual void Update()
        {
            if (!IsBusy)
            {
                if (cache == null)
                {
                    cache = CacheNeeded?.Invoke(this);
                }

                if (codec == null)
                {
                    codec = DecoderNeeded?.Invoke(this);
                }

                if (string.IsNullOrEmpty(CubemapName))
                {
                    CubemapName = CubemapNeeded?.Invoke(this);
                }

                if (cache != null
                    && codec != null
                    && !string.IsNullOrEmpty(CubemapName)
                    && !IsReady
                    && cache.IsCached(CubemapName + codec.ContentType))
                {
                    this.Report(0);
                    readingTask = ReadCubemap();
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (string.IsNullOrEmpty(CubemapName))
            {
                CubemapName = name;
            }

            var imageName = CubemapName + ".jpeg";
            var gizmoPath = Path.Combine("Assets", "Gizmos", imageName);
            if (File.Exists(gizmoPath))
            {
                Gizmos.DrawIcon(transform.position + Vector3.up, imageName);
            }
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        public float Progress
        {
            get
            {
                return ProgressToReady;
            }
        }

        public string Status
        {
            get;
            private set;
        }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            ConfigurationManagement.TagManager.NormalizeLayer(PHOTOSPHERE_LAYER);
        }

#endif
    }
}