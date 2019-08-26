using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Data;
using Juniper.Display;
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
        private const int MAX_REQUESTS = 4;

        private const string PHOTOSPHERE_LAYER = "Photospheres";
        private static readonly string[] PHOTOSPHERE_LAYER_ARR = { PHOTOSPHERE_LAYER };

        private readonly Dictionary<int, Transform> detailContainerCache = new Dictionary<int, Transform>();
        private readonly Dictionary<int, Dictionary<int, Transform>> detailSliceContainerCache = new Dictionary<int, Dictionary<int, Transform>>();
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, Transform>>> detailSliceFrameContainerCache = new Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>();

        private int[] FOVs;
        private Vector2[][] fovTestAngles;
        private int[] lodLevelRequirements;

        public float ProgressToReady;
        public float ProgressToComplete;

        private bool locked;
        private bool wasComplete;
        private bool wasReady;

        private Avatar avatar;
        private bool trySkybox = true;
        private SkyboxManager skybox;
        private Texture skyboxCubemap;

        public event CubemapImageNeeded CubemapNeeded;

        public event PhotosphereImageNeeded ImageNeeded;

        public event Action<Photosphere> Complete;

        public event Action<Photosphere> Ready;

        internal IImageCodec<Texture2D> codec;

        public void Awake()
        {
            avatar = ComponentExt.FindAny<Avatar>();
            skybox = ComponentExt.FindAny<SkyboxManager>()
                ?? this.Ensure<SkyboxManager>();
        }

        public void OnEnable()
        {
            wasReady = false;
            wasComplete = false;

            if (trySkybox)
            {
                trySkybox = false;
                locked = true;
                var filename = CubemapNeeded?.Invoke(this);
                StartCoroutine(ReadCubemapCoroutine(filename));
            }
            else if (skyboxCubemap != null)
            {
                skybox.exposure = 1;
                skybox.imageType = SkyboxManager.ImageType.Degrees360;
                skybox.layout = SkyboxManager.Mode.Cube;
                skybox.mirror180OnBack = false;
                skybox.rotation = 0;
                skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                skybox.tint = Color.gray;
                skybox.useMipMap = false;
                skybox.SetTexture(skyboxCubemap);
            }
            else if (lodLevelRequirements != null)
            {
                for (var f = lodLevelRequirements.Length - 1; f >= 0; --f)
                {
                    var lodLevel = FOVs[f];
                    if (detailContainerCache.ContainsKey(lodLevel))
                    {
                        detailContainerCache[lodLevel].Activate();
                        if (DetailLevelCompleteCount(f) == lodLevelRequirements[f])
                        {
                            break;
                        }
                    }
                }
            }
        }

        private IEnumerator ReadCubemapCoroutine(string filePath)
        {
            var imageTask = StreamingAssets.ReadImage(codec, Application.persistentDataPath, filePath, this);
            yield return imageTask.Waiter();

            if (imageTask.IsSuccessful()
                && imageTask.Result != null)
            {
                Debug.Log("Cubemap saved");
                skyboxCubemap = imageTask.Result;
                OnDisable();
                OnEnable();
                Update();
            }
            else if (imageTask.IsCanceled)
            {
                Debug.Log("Cubemap canceled");
            }
            else
            {
                Debug.Log("Cubemap save error");
            }

            locked = false;
        }

        public void Report(float progress, string status)
        {
            ProgressToReady = progress;
        }

        public void Report(float progress)
        {
            Report(progress, null);
        }

        public void OnDisable()
        {
            if (lodLevelRequirements != null)
            {
                for (var f = 0; f < lodLevelRequirements.Length; ++f)
                {
                    var lodLevel = FOVs[f];
                    if (detailContainerCache.ContainsKey(lodLevel))
                    {
                        detailContainerCache[lodLevel].Deactivate();
                    }
                }
            }
        }

        public void Update()
        {
            transform.position = avatar.Head.position;
            if (!wasComplete)
            {
                var isComplete = false;
                var isReady = wasReady;
                if (skyboxCubemap != null)
                {
                    isReady = true;
                    isComplete = true;
                }
                else if (lodLevelRequirements != null)
                {
                    var totalCompleted = 0;
                    var totalNeeded = 0;
                    for (var f = 0; f < lodLevelRequirements.Length; ++f)
                    {
                        var t = DetailLevelCompleteCount(f);
                        var n = lodLevelRequirements[f];

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
                                detailContainerCache[FOVs[f - 1]].Deactivate();
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

                if (!wasReady && isReady)
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
                wasReady = isReady;
            }
        }

        private IEnumerator UpdateSphereCoroutine()
        {
            var euler = (Vector2)avatar.Head.rotation.eulerAngles;

            var numRequests = 0;
            for (var f = 0; f < FOVs.Length && numRequests < MAX_REQUESTS; ++f)
            {
                var fov = FOVs[f];
                var overlap = FOVs.Length - f;
                var radius = 10 * overlap + 50;
                var overlapFOV = fov + 2f * overlap;
                var scale = 2 * radius * Mathf.Tan(Degrees.Radians(overlapFOV / 2));

                var testAngles = fovTestAngles[f];
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

                        yield return imageTask.Waiter();

                        if (imageTask.IsSuccessful())
                        {
                            var image = imageTask.Result;
                            if (image != null)
                            {
                                var frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                var renderer = frame.GetComponent<MeshRenderer>();
                                var material = new Material(Shader.Find("Unlit/Texture"));
                                var texture = codec.Deserialize(image);
                                material.SetTexture("_MainTex", texture);
                                renderer.SetMaterial(material);

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

        public void SetDetailRequirements(int[] FOVs, Vector2[][] testAngles, int[] lodLevels)
        {
            this.FOVs = FOVs;
            fovTestAngles = testAngles;
            lodLevelRequirements = lodLevels;
        }

        public int DetailLevelCompleteCount(int f)
        {
            if (lodLevelRequirements == null)
            {
                return 0;
            }

            var lodLevel = FOVs[f];
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
                else if (lodLevelRequirements == null)
                {
                    return false;
                }
                else
                {
                    for (var f = 0; f < lodLevelRequirements.Length; ++f)
                    {
                        if (DetailLevelCompleteCount(f) != lodLevelRequirements[f])
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
                throw new NotImplementedException();
            }
        }

#if UNITY_EDITOR
        private bool cubemapLock;

        public void OnValidate()
        {
            ConfigurationManagement.TagManager.NormalizeLayer(PHOTOSPHERE_LAYER);
        }

        private void CaptureCubemap()
        {
            if (skyboxCubemap == null && !cubemapLock)
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

        private IEnumerator CaptureCubemapCoroutine()
        {
            var fileName = Path.Combine("Assets", "StreamingAssets", $"{name}.jpeg");
            using (var prog = new UnityEditorProgressDialog("Saving cubemap " + name))
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

                var faces = new[]
                {
                    CubemapFace.NegativeY,
                    CubemapFace.NegativeX,
                    CubemapFace.PositiveZ,
                    CubemapFace.PositiveX,
                    CubemapFace.NegativeZ,
                    CubemapFace.PositiveY
                };

                var images = new Texture2D[faces.Length];

                for (var f = 0; f < faces.Length; ++f)
                {
                    subProgs[1].Report(f, faces.Length);
                    try
                    {
                        var pixels = cubemap.GetPixels(faces[f]);
                        var texture = new Texture2D(cubemap.width, cubemap.height);
                        texture.SetPixels(pixels);
                        texture.Apply();
                        images[f] = texture;
                    }
                    catch (Exception exp)
                    {
                        Debug.LogException(exp);
                        cubemapLock = false;
                        throw;
                    }
                    subProgs[1].Report(f + 1, faces.Length);
                    yield return null;
                }

                try
                {
                    var img = codec.Concatenate(ImageData.CubeCross(images), subProgs[2]);
                    codec.Save(fileName, img, subProgs[3]);
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