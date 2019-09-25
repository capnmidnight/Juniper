using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Data;
using Juniper.Display;
using Juniper.Imaging.Unity;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Units;

using UnityEngine;

namespace Juniper.Imaging
{
    public delegate string CubemapImageNeeded(Photosphere source);

    public delegate Task<Stream> PhotosphereImageNeeded(Photosphere source, int fov, int heading, int pitch);

    public class Photosphere : MonoBehaviour, IProgress
    {
        private static Material material;
        private const int MAX_REQUESTS = 4;

        private const string PHOTOSPHERE_LAYER = "Photospheres";
        private static readonly string[] PHOTOSPHERE_LAYER_ARR = { PHOTOSPHERE_LAYER };

        private readonly Dictionary<int, Transform> detailContainerCache = new Dictionary<int, Transform>();
        private readonly Dictionary<int, Dictionary<int, Transform>> detailSliceContainerCache = new Dictionary<int, Dictionary<int, Transform>>();
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, Transform>>> detailSliceFrameContainerCache = new Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>();

        public string Key;
        public string CubemapPath;
        public float ProgressToReady;
        public float ProgressToComplete;

        private bool locked;
        private bool wasComplete;
        private bool trySkybox = true;

        private PhotosphereManager mgr;
        private Avatar avatar;
        private SkyboxManager skybox;

        public event CubemapImageNeeded CubemapNeeded;

        public event PhotosphereImageNeeded ImageNeeded;

        public event Action<Photosphere> Complete;

        public event Action<Photosphere> Ready;

        internal UnityTextureCodec codec;

        public bool IsReady { get; private set; }

        private bool hasStarted;

        public void Awake()
        {
            if (material == null)
            {
                material = new Material(Shader.Find("Unlit/Texture"));
            }

            mgr = this.FindClosest<PhotosphereManager>();
            Find.Any(out avatar);
            if (!Find.Any(out skybox))
            {
                skybox = this.Ensure<SkyboxManager>();
            }
        }

        public void Start()
        {
            ShowImage();
            hasStarted = true;
        }

        public virtual void OnEnable()
        {
            if (hasStarted)
            {
                ShowImage();
            }
        }

        public virtual void OnDisable()
        {
            foreach (var child in transform.Children())
            {
                child.Deactivate();
            }
        }

        private void ShowImage()
        {
            if (trySkybox)
            {
                locked = true;
                if (string.IsNullOrEmpty(CubemapPath))
                {
                    CubemapPath = CubemapNeeded?.Invoke(this);
                }
                var path = StreamingAssets.FormatPath(Application.streamingAssetsPath, CubemapPath);
                StartCoroutine(ReadCubemapCoroutine(path));
            }

            foreach (var child in transform.Children())
            {
                child.Activate();
            }
        }

        private IEnumerator ReadCubemapCoroutine(string filePath)
        {
            var streamTask = StreamingAssets.GetStream(Application.persistentDataPath, filePath, this);
            yield return streamTask.AsCoroutine();

            trySkybox = false;
            if (streamTask.IsSuccessful()
                && streamTask.Result != null)
            {
                trySkybox = true;
                if (mgr != null && mgr.lodLevelRequirements != null && mgr.FOVs != null)
                {
                    for (var f = 0; f < mgr.lodLevelRequirements.Length; ++f)
                    {
                        var lodLevel = mgr.FOVs[f];
                        if (detailContainerCache.ContainsKey(lodLevel))
                        {
                            detailContainerCache[lodLevel].Deactivate();
                        }
                    }
                }

                var texture = codec.Deserialize(streamTask.Result.Content);

                skybox.exposure = 1;
                skybox.imageType = SkyboxManager.ImageType.Degrees360;
                skybox.layout = SkyboxManager.Mode.Cube;
                skybox.mirror180OnBack = false;
                skybox.rotation = 0;
                skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                skybox.tint = UnityEngine.Color.gray;
                skybox.useMipMap = false;
                yield return skybox.SetTexture(texture);

                IsReady = wasComplete = true;
                Ready?.Invoke(this);
            }
            else if (streamTask.IsCanceled)
            {
                Debug.LogError("Cubemap canceled");
            }
            else if (streamTask.IsFaulted)
            {
                Debug.LogError("Cubemap load error");
                Debug.LogException(streamTask.Exception);
            }
            else
            {
                Debug.LogWarning("No cubemap found");
            }

            locked = false;
        }

        public void ReportWithStatus(float progress, string status)
        {
            Status = status;
            ProgressToReady = progress;
        }

        public void Update()
        {
            if (mgr != null
                && mgr.lodLevelRequirements != null
                && mgr.lodLevelRequirements.Length > 0
                && !wasComplete)
            {
                var isComplete = false;
                var isReady = IsReady;
                if (mgr.lodLevelRequirements != null)
                {
                    var totalCompleted = 0;
                    var totalNeeded = 0;
                    for (var f = 0; f < mgr.lodLevelRequirements.Length; ++f)
                    {
                        var t = DetailLevelCompleteCount(f);
                        var n = mgr.lodLevelRequirements[f];

                        if (f == 0)
                        {
                            ProgressToReady = t / (float)n;

                            if (t == n)
                            {
                                isReady = true;
                            }
                        }

                        if (t == n)
                        {
                            if (f == 0)
                            {
                                isReady = true;
                            }
                            else
                            {
                                detailContainerCache[mgr.FOVs[f - 1]].Deactivate();
                            }
                        }

                        totalCompleted += t;
                        totalNeeded += n;
                    }

                    ProgressToComplete = totalCompleted / (float)totalNeeded;

                    if (totalCompleted == totalNeeded)
                    {
                        isComplete = true;
                    }
                }

                if (!IsReady && isReady)
                {
                    ProgressToReady = 1;
                    Ready?.Invoke(this);
                }

                if (isComplete)
                {
                    ProgressToComplete = 1;
                    Complete?.Invoke(this);
#if UNITY_EDITOR
                    CaptureCubemap();
#endif
                }
                else if (!locked)
                {
                    locked = true;
                    StartCoroutine(UpdateSphereCoroutine());
                }

                wasComplete = isComplete;
                IsReady = isReady;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            var gizmoPath = Path.Combine("Assets", "Gizmos", CubemapPath);
            if (File.Exists(gizmoPath))
            {
                Gizmos.DrawIcon(transform.position + Vector3.up, CubemapPath);
            }
            Gizmos.DrawSphere(transform.position, 1);
        }

        private IEnumerator UpdateSphereCoroutine()
        {
            var euler = (Vector2)avatar.Head.rotation.eulerAngles;

            var numRequests = 0;
            for (var f = 0; f < mgr.FOVs.Length && numRequests < MAX_REQUESTS; ++f)
            {
                var fov = mgr.FOVs[f];
                var overlap = mgr.FOVs.Length - f;
                var radius = 10 * overlap + 50;
                var overlapFOV = fov + 2f * overlap;
                var scale = 2 * radius * Mathf.Tan(Degrees.Radians(overlapFOV / 2));

                var testAngles = mgr.fovTestAngles[f];
                for (var a = 0; a < testAngles.Length && numRequests < MAX_REQUESTS; ++a)
                {
                    var testAngle = euler + testAngles[a];
                    var heading = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.y / fov), 360);
                    var unityPitch = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.x / fov), 360);
                    var pitch = -unityPitch;
                    if (unityPitch >= 270)
                    {
                        pitch += 360;
                    }
                    else if (unityPitch > 90)
                    {
                        pitch += 180;
                    }

                    if (90 < unityPitch && unityPitch < 270)
                    {
                        heading = (int)Mathf.Repeat(heading + 180, 360);
                    }

                    if (Mathf.Abs(pitch) == 90)
                    {
                        heading = 0;
                    }

                    var needLodLevel = !detailContainerCache.ContainsKey(fov);
                    var needSlice = needLodLevel || !detailSliceContainerCache[fov].ContainsKey(heading);
                    var needFrame = needSlice || !detailSliceFrameContainerCache[fov][heading].ContainsKey(pitch);
                    if (needLodLevel || needSlice || needFrame)
                    {
                        if (needLodLevel)
                        {
                            var detail = new GameObject(fov.ToString()).transform;
                            detail.SetParent(transform, false);
                            detailContainerCache[fov] = detail;
                            detailSliceContainerCache[fov] = new Dictionary<int, Transform>();
                            detailSliceFrameContainerCache[fov] = new Dictionary<int, Dictionary<int, Transform>>();
                        }

                        var detailContainer = detailContainerCache[fov];
                        var sliceContainerCache = detailSliceContainerCache[fov];
                        var sliceFrameContainerCache = detailSliceFrameContainerCache[fov];

                        if (needSlice)
                        {
                            var slice = new GameObject(heading.ToString()).transform;
                            slice.SetParent(detailContainer, false);
                            sliceContainerCache[heading] = slice;
                            sliceFrameContainerCache[heading] = new Dictionary<int, Transform>();
                        }

                        var sliceContainer = sliceContainerCache[heading];
                        var frameContainerCache = sliceFrameContainerCache[heading];

                        if (needFrame)
                        {
                            var frame = new GameObject(pitch.ToString()).transform;
                            frame.rotation = Quaternion.Euler(-pitch, heading, 0);
                            frame.position = frame.rotation * (radius * Vector3.forward);
                            frame.SetParent(sliceContainer, false);
                            frameContainerCache[pitch] = frame;
                        }

                        var frameContainer = frameContainerCache[pitch];

                        var imageTask = ImageNeeded?.Invoke(this, (int)overlapFOV, heading, pitch);

                        yield return imageTask.AsCoroutine();

                        if (imageTask.IsSuccessful())
                        {
                            var stream = imageTask.Result;
                            if (stream != null)
                            {
                                var frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                var renderer = frame.GetComponent<MeshRenderer>();
                                var image = codec.Deserialize(stream);
                                var properties = new MaterialPropertyBlock();
                                properties.SetTexture("_MainTex", image);
                                renderer.SetMaterial(material);
                                renderer.SetPropertyBlock(properties);
                                frame.layer = LayerMask.NameToLayer(PHOTOSPHERE_LAYER);
                                frame.transform.SetParent(frameContainer, false);
                                frame.transform.localScale = scale * Vector3.one;
                            }
                        }

                        // For the lowest detail level, we fill out all of the image angles immediately.
                        // For all other detail levels, we break out of testing angles and continue to
                        // next highest detail level.
                        if (f > 0)
                        {
                            break;
                        }
                    }
                }
            }

            locked = false;
        }

        public int DetailLevelCompleteCount(int f)
        {
            if (mgr == null || mgr.lodLevelRequirements == null)
            {
                return 0;
            }

            var lodLevel = mgr.FOVs[f];
            if (!detailSliceFrameContainerCache.ContainsKey(lodLevel))
            {
                return 0;
            }
            else
            {
                var frameCount = 0;
                foreach (var sliceFrameContainer in detailSliceFrameContainerCache[lodLevel])
                {
                    frameCount += sliceFrameContainer.Value.Count;
                }

                return frameCount;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (wasComplete)
                {
                    return true;
                }
                else if (mgr == null || mgr.lodLevelRequirements == null)
                {
                    return false;
                }
                else
                {
                    for (var f = 0; f < mgr.lodLevelRequirements.Length; ++f)
                    {
                        if (DetailLevelCompleteCount(f) != mgr.lodLevelRequirements[f])
                        {
                            return false;
                        }
                    }
                    return true;
                }
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

#if UNITY_EDITOR
        private bool cubemapLock;

        public virtual void OnValidate()
        {
            ConfigurationManagement.TagManager.NormalizeLayer(PHOTOSPHERE_LAYER);
        }

        private void CaptureCubemap()
        {
            if (string.IsNullOrEmpty(CubemapPath) && !cubemapLock)
            {
                cubemapLock = true;
                StartCoroutine(CaptureCubemapCoroutine());
            }
        }

        private static readonly string[] CAPTURE_CUBEMAP_FIELDS = {
            "Rendering cubemap",
            "Copying cubemap faces",
            "Concatenating faces",
            "Saving image"
        };

        private static readonly CubemapFace[] CAPTURE_CUBEMAP_FACES = new[] {
            CubemapFace.NegativeY,
            CubemapFace.NegativeX,
            CubemapFace.PositiveZ,
            CubemapFace.PositiveX,
            CubemapFace.NegativeZ,
            CubemapFace.PositiveY
        };

        private static readonly Texture2D[] CAPTURE_CUBEMAP_SUB_IMAGES = new Texture2D[CAPTURE_CUBEMAP_FACES.Length];

        private IEnumerator CaptureCubemapCoroutine()
        {
            var fileName = StreamingAssets.FormatPath(Application.streamingAssetsPath, Key + ".jpeg");
            using (var prog = new UnityEditorProgressDialog("Saving cubemap " + Key))
            {
                var subProgs = prog.Split(CAPTURE_CUBEMAP_FIELDS);

                subProgs[0].Report(0);
                const int dim = 2048;
                var cubemap = new Cubemap(dim, TextureFormat.RGB24, false);
                cubemap.Apply();

                var curMask = DisplayManager.MainCamera.cullingMask;
                DisplayManager.MainCamera.cullingMask = LayerMask.GetMask(PHOTOSPHERE_LAYER_ARR);

                var curRotation = DisplayManager.MainCamera.transform.rotation;
                DisplayManager.MainCamera.transform.rotation = Quaternion.identity;

                DisplayManager.MainCamera.RenderToCubemap(cubemap, 63);

                DisplayManager.MainCamera.cullingMask = curMask;
                DisplayManager.MainCamera.transform.rotation = curRotation;
                subProgs[0].Report(1);

                var anyDestroyed = false;
                foreach (var texture in CAPTURE_CUBEMAP_SUB_IMAGES)
                {
                    if (texture != null)
                    {
                        anyDestroyed = true;
                        Destroy(texture);
                    }
                }

                if (anyDestroyed)
                {
                    yield return JuniperSystem.Cleanup();
                }

                for (var f = 0; f < CAPTURE_CUBEMAP_FACES.Length; ++f)
                {
                    subProgs[1].Report(f, CAPTURE_CUBEMAP_FACES.Length, CAPTURE_CUBEMAP_FACES[f].ToString());
                    try
                    {
                        var pixels = cubemap.GetPixels(CAPTURE_CUBEMAP_FACES[f]);
                        var texture = new Texture2D(cubemap.width, cubemap.height);
                        texture.SetPixels(pixels);
                        texture.Apply();
                        CAPTURE_CUBEMAP_SUB_IMAGES[f] = texture;
                    }
                    catch (Exception exp)
                    {
                        Debug.LogException(exp);
                        cubemapLock = false;
                        throw;
                    }
                    subProgs[1].Report(f + 1, CAPTURE_CUBEMAP_FACES.Length);
                    yield return null;
                }

                try
                {
                    var img = codec.Concatenate(ImageData.CubeCross(CAPTURE_CUBEMAP_SUB_IMAGES), subProgs[2]);
                    codec.Save(fileName, img, subProgs[3]);
                    Debug.Log("Cubemap saved " + fileName);
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                    cubemapLock = false;
                    throw;
                }

                yield return ReadCubemapCoroutine(fileName);
            }

            cubemapLock = false;
        }

#endif
    }
}