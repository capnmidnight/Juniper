using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Imaging
{
    public delegate bool CubemapAvailabilityNeeded(Photosphere source);
    public delegate Task<Texture2D> TextureNeeded(Photosphere source);
    public delegate float CubemapRotationNeeded(Photosphere source);

    public class Photosphere : MonoBehaviour, IProgress
    {
        public const string PHOTOSPHERE_LAYER = "Photospheres";
        public static readonly string[] PHOTOSPHERE_LAYER_ARR = { PHOTOSPHERE_LAYER };

        public string CubemapName;

        [Range(0, 1)]
        public float ProgressToReady;

        public event CubemapAvailabilityNeeded CheckIsCubemapAvailable;

        public UnityEvent OnEnter;

        protected bool? IsCubemapAvailable
        {
            get
            {
                return CheckIsCubemapAvailable?.Invoke(this);
            }
        }

        public event TextureNeeded GetCubemap;

        public event CubemapRotationNeeded GetRotation;
        private float Rotation
        {
            get
            {
                return GetRotation?.Invoke(this) ?? 0;
            }
        }

        public event Action<Photosphere> Ready;

        protected void OnReady()
        {
            this.Report(1);
            IsReady = true;
            Ready?.Invoke(this);
        }

        public bool IsReady
        {
            get;
            private set;
        }

        private Task readingTask;
        private SkyboxManager skybox;

        public virtual bool IsBusy
        {
            get
            {
                return readingTask.IsRunning();
            }
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

        public void ReportWithStatus(float progress, string status)
        {
            Status = status;
            ProgressToReady = progress;
        }

        public virtual void Awake()
        {
            if (!Find.Any(out skybox))
            {
                skybox = this.Ensure<SkyboxManager>();
            }
        }

        public virtual void OnEnable()
        {
            OnEnter?.Invoke();
        }

        public virtual void OnDisable()
        {
            this.Report(0);
            IsReady = false;
        }

        public virtual void Update()
        {
            if (!IsBusy
                && !IsReady
                && IsCubemapAvailable == true)
            {
                this.Report(0);
                readingTask = ReadCubemap();
            }
        }

        private async Task ReadCubemap()
        {
            try
            {
                var progs = this.Split("Load", "Decode");
                var textureTask = GetCubemap?.Invoke(this);
                if (textureTask == null)
                {
                    Debug.Log("No cubemap found " + CubemapName);
                }
                else
                {
                    var texture = await textureTask;

                    if (texture == null)
                    {
                        Debug.Log("No cubemap found " + CubemapName);
                    }
                    else
                    {
                        await JuniperSystem.OnMainThread(() =>
                        {
                            skybox.exposure = 1;
                            skybox.imageType = SkyboxManager.ImageType.Degrees360;
                            skybox.layout = SkyboxManager.Mode.Cube;
                            skybox.mirror180OnBack = false;
                            skybox.rotation = Rotation;
                            skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                            skybox.tint = Color.gray;
                            skybox.useMipMap = false;
                            skybox.SetTexture(texture);
                        });

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

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            ConfigurationManagement.TagManager.NormalizeLayer(PHOTOSPHERE_LAYER);
        }

        protected virtual void OnDrawGizmos()
        {
            if (!hideGizmo)
            {
                if (string.IsNullOrEmpty(CubemapName))
                {
                    CubemapName = name;
                }

                var gizmos = new FileCacheLayer(Path.Combine("Assets", "Gizmos"));
                var imageRef = CubemapName + MediaType.Image.Jpeg;
                if (gizmos.IsCached(imageRef))
                {
                    Gizmos.DrawIcon(transform.position + Vector3.up, (string)imageRef);
                }

                Gizmos.DrawSphere(transform.position, 0.5f);
            }
        }
        public bool hideGizmo;

#endif
    }
}