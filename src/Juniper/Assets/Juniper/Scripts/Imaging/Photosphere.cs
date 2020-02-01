using System;
using System.IO;

using Juniper.IO;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Imaging
{
    public delegate bool CubemapAvailabilityNeeded(Photosphere source);
    public delegate Texture2D TextureNeeded(Photosphere source);
    public delegate float CubemapRotationNeeded(Photosphere source);
    public delegate void CubemapRotationUpdated(Photosphere source, float rotation);
    public delegate void CubemapPositionUpdated(Photosphere source, Vector3 position);

    public class Photosphere : MonoBehaviour, IProgress
    {
        public const string PHOTOSPHERE_LAYER = "Photospheres";
        public static readonly string[] PHOTOSPHERE_LAYER_ARR = { PHOTOSPHERE_LAYER };

        public string CubemapName;

        public virtual string CubemapCacheID
        {
            get
            {
                return CubemapName;
            }
        }

        [Range(0, 1)]
        public float ProgressToReady;

        public event CubemapAvailabilityNeeded CheckIsCubemapAvailable;

        public UnityEvent OnEnter;

        protected bool IsCubemapAvailable
        {
            get
            {
                return CheckIsCubemapAvailable?.Invoke(this) == true
                    && GetCubemap != null;
            }
        }

        public event TextureNeeded GetCubemap;

        public event CubemapRotationNeeded GetRotation;

#if UNITY_EDITOR
        public event CubemapRotationUpdated SetRotation;
        public event CubemapPositionUpdated SetPosition;
#endif

        public float rotation;
        private float LastRotation
        {
            get
            {
                return GetRotation?.Invoke(this) ?? 0;
            }
#if UNITY_EDITOR
            set
            {
                SetRotation?.Invoke(this, rotation);
            }
#endif
        }

        private Vector3 lastPosition;

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

        private Texture2D texture;
        private SkyboxManager skybox;

        public virtual bool IsBusy { get; protected set; }

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
                && IsCubemapAvailable
                && texture == null)
            {
                this.Report(0);
                texture = GetCubemap(this);

                if (texture != null)
                {
                    skybox.exposure = 1;
                    skybox.imageType = SkyboxManager.ImageType.Degrees360;
                    skybox.layout = SkyboxManager.Mode.Cube;
                    skybox.mirror180OnBack = false;
                    rotation = skybox.rotation = LastRotation;
                    skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                    skybox.tint = Color.gray;
                    skybox.useMipMap = false;
                    skybox.SetTexture(texture);

                    OnReady();
                }
            }
#if UNITY_EDITOR
            else if (rotation != LastRotation)
            {
                skybox.rotation = rotation;
                LastRotation = rotation;
            }
            else if (transform.localPosition != lastPosition)
            {
                SetPosition?.Invoke(this, transform.localPosition);
                lastPosition = transform.localPosition;
            }
#endif
        }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            ConfigurationManagement.TagManager.NormalizeLayer(PHOTOSPHERE_LAYER);
        }

        public bool hideGizmo;

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
#endif
    }
}