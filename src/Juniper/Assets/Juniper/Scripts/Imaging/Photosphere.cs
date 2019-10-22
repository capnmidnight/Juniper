using System;
using System.Collections;
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

        protected bool locked;
        protected bool trySkybox = true;
        protected IImageCodec<Texture2D> codec;
        protected CachingStrategy cache;

        public string CubemapName;

        [Range(0, 1)]
        public float ProgressToReady;

        public event CachingStrategyNeeded CacheNeeded;
        public event TextureDecoderNeeded DecoderNeeded;
        public event CubemapImageNeeded CubemapNeeded;
        public event Action<Photosphere> Ready;

        public bool IsReady
        {
            get;
            private set;
        }

        public virtual void Awake()
        {
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
            trySkybox = true;
            IsReady = false;
            this.Report(0);

            foreach (var child in transform.Children())
            {
                child.Deactivate();
            }
        }

        private IEnumerator ReadCubemapCoroutine(string filePath)
        {
            var textureTask = cache.Load(codec, filePath + codec.ContentType, this);
            yield return textureTask.AsCoroutine();

            if (textureTask.IsCanceled)
            {
                Debug.LogError("Cubemap canceled");
            }
            else if (textureTask.IsFaulted)
            {
                Debug.LogError("Cubemap load error");
                Debug.LogException(textureTask.Exception);
            }
            else if (textureTask.Result == null)
            {
                Debug.Log("No cubemap found");
            }
            else
            {
                var texture = textureTask.Result;

                skybox.exposure = 1;
                skybox.imageType = SkyboxManager.ImageType.Degrees360;
                skybox.layout = SkyboxManager.Mode.Cube;
                skybox.mirror180OnBack = false;
                skybox.rotation = 0;
                skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                skybox.tint = UnityEngine.Color.gray;
                skybox.useMipMap = false;
                yield return skybox.SetTexture(texture);

                OnReady();
            }

            locked = false;
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

        public virtual void Update()
        {
            if (!locked)
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
                    && trySkybox)
                {
                    this.Report(0);
                    trySkybox = false;
                    locked = true;
                    this.Run(ReadCubemapCoroutine(CubemapName));
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