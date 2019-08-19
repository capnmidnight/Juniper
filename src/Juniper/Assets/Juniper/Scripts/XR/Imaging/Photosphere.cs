using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Display;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Units;

using UnityEngine;

namespace Juniper.Imaging
{
    public delegate Task<ImageData> CubemapImageNeeded(Photosphere source);

    public delegate Task<ImageData> PhotosphereImageNeeded(Photosphere source, int fov, int heading, int pitch);

    public class Photosphere : MonoBehaviour
    {
        private const int MAX_REQUESTS = 4;

        private const string PHOTOSPHERE_LAYER = "Photospheres";

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

        public void Awake()
        {
            avatar = ComponentExt.FindAny<Avatar>();
            skybox = ComponentExt.FindAny<SkyboxManager>()
                ?? this.Ensure<SkyboxManager>();
        }

        public void OnEnable()
        {
            if (skybox != null && skyboxCubemap != null)
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
            else if (trySkybox)
            {
                trySkybox = false;
                locked = true;
                var imageTask = CubemapNeeded?.Invoke(this);
                StartCoroutine(ReadCubemapCoroutine(imageTask));
            }
            else if (lodLevelRequirements != null)
            {
                for (var f = lodLevelRequirements.Length - 1; f >= 0; ++f)
                {
                    var lodLevel = FOVs[f];
                    if (detailContainerCache.ContainsKey(lodLevel))
                    {
                        detailContainerCache[lodLevel].Activate();
                        if (DetailLevelCompleteCount(f) == lodLevelRequirements[lodLevel])
                        {
                            break;
                        }
                    }
                }
            }
        }

        private IEnumerator ReadCubemapCoroutine(Task<ImageData> imageTask)
        {
            while (imageTask.IsRunning())
            {
                yield return null;
            }

            if (imageTask.IsCompleted && imageTask.Result != null)
            {
                Debug.Log("Cubemap saved");
                skyboxCubemap = imageTask.Result.ToTexture();
                OnDisable();
                OnEnable();
                Update();
            }
            else if (imageTask.IsFaulted)
            {
                Debug.Log("Cubemap save error");
            }
            else
            {
                Debug.Log("Cubemap canceled");
            }

            locked = false;
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
                        totalCompleted = DetailLevelCompleteCount(f);
                        totalNeeded = lodLevelRequirements[f];

                        if (f == 0)
                        {
                            ProgressToReady = totalCompleted / (float)totalNeeded;

                            if (totalCompleted == totalNeeded)
                            {
                                isReady = true;
                            }
                        }
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

                        while (imageTask.IsRunning())
                        {
                            yield return null;
                        }

                        if (imageTask.IsCompleted)
                        {
                            var image = imageTask.Result;
                            if (image != null)
                            {
                                var texture = image.ToTexture();
                                var frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                var renderer = frame.GetComponent<MeshRenderer>();
                                var material = new Material(Shader.Find("Unlit/Texture"));
                                material.SetTexture("_MainTex", texture);
                                renderer.SetMaterial(material);

                                frame.layer = LayerMask.NameToLayer(PHOTOSPHERE_LAYER);
                                frame.transform.SetParent(frameContainer, false);
                                frame.transform.localScale = scale * Vector3.one;
                            }
                        }

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

        private IEnumerator CaptureCubemapCoroutine()
        {
            var fileName = Path.Combine("Assets", "StreamingAssets", $"{name}.jpeg");
            if (!File.Exists(fileName))
            {
                using (var prog = new UnityEditorProgressDialog("Saving cubemap " + name))
                {
                    var subProgs = prog.Split(
                        "Rendering cubemap",
                        "Copying cubemap faces",
                        "Concatenating faces",
                        "Saving image");

                    subProgs[0].Report(0);
                    const int dim = 2048;
                    var cubemap = new Cubemap(dim, TextureFormat.RGB24, false);
                    cubemap.Apply();

                    var curMask = DisplayManager.MainCamera.cullingMask;
                    DisplayManager.MainCamera.cullingMask = LayerMask.GetMask(PHOTOSPHERE_LAYER);

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

                    var images = new ImageData[faces.Length];

                    for (var f = 0; f < faces.Length; ++f)
                    {
                        subProgs[1].Report(f, faces.Length);
                        try
                        {
                            var pixels = cubemap.GetPixels(faces[f]);
                            var buf = new byte[pixels.Length * 3];
                            for (var y = 0; y < cubemap.height; ++y)
                            {
                                for (var x = 0; x < cubemap.width; ++x)
                                {
                                    var bufI = y * cubemap.width + x;
                                    var pixI = (cubemap.height - 1 - y) * cubemap.width + x;
                                    buf[bufI * 3 + 0] = (byte)(255 * pixels[pixI].r);
                                    buf[bufI * 3 + 1] = (byte)(255 * pixels[pixI].g);
                                    buf[bufI * 3 + 2] = (byte)(255 * pixels[pixI].b);
                                }
                            }

                            images[f] = new ImageData(DataSource.None, cubemap.width, cubemap.height, 3, ImageFormat.None, buf);
                        }
                        catch
                        {
                            cubemapLock = false;
                            throw;
                        }
                        subProgs[1].Report(f + 1, faces.Length);
                        yield return null;
                    }

                    var saveTask = Task.Run(() =>
                    {
                        try
                        {
                            var encoder = new JPEG.JpegDecoder(80);
                            var img = encoder.Concatenate(ImageData.CubeCross(images), subProgs[2]);
                            encoder.Save(fileName, img, subProgs[3]);
                            return encoder.Read(fileName);
                        }
                        catch
                        {
                            cubemapLock = false;
                            throw;
                        }
                    });

                    yield return ReadCubemapCoroutine(saveTask);
                }
            }

            cubemapLock = false;
        }

#endif
    }
}