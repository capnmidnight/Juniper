using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Animation;
using Juniper.Data;
using Juniper.Display;
using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.Imaging.JPEG;
using Juniper.Progress;
using Juniper.Security;
using Juniper.Serialization;
using Juniper.Units;
using Juniper.Unity;
using Juniper.World;
using Juniper.World.GIS;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using Yarrow.Client;

namespace Juniper.Imaging
{
    public class GoogleStreetView : SubSceneController, ICredentialReceiver
    {
        private const int MAX_REQUESTS = 4;

        private static readonly float[] FOVs =
        {
            120,
            90,
            60,
            45,
            30
        };

        private static readonly Vector2[][] TEST_ANGLES = FOVs.Select(MakeTestAngles).ToArray();

        private static Vector2[] MakeTestAngles(float fov)
        {
            var list = new List<Vector2>();
            var minX = Mathf.Floor(-180 / fov) * fov;
            var maxX = Mathf.Ceil(180 / fov) * fov;
            var minY = Mathf.Floor(-90 / fov) * fov;
            var maxY = Mathf.Ceil(90 / fov) * fov;
            for (var ax = minX; ax <= maxX; ax += fov)
            {
                for (var ay = minY; ay <= maxY; ay += fov)
                {
                    list.Add(new Vector2(ax, ay));
                }
            }

            return list
                .Distinct()
                .OrderBy(x => x.magnitude)
                .ToArray();
        }

        public string yarrowServerHost = "http://localhost";

        [SerializeField]
        [HideInInspector]
        private string gmapsApiKey;

        [SerializeField]
        [HideInInspector]
        private string gmapsSigningKey;

        public TextureFormat textureFormat = TextureFormat.RGB24;
        public Color tint = Color.gray;

        [Range(0, 8)]
        public float exposure = 1;

        [Range(0, 360)]
        public float rotation;

        public bool useMipMap = true;

        public int searchRadius = 50;

        [ReadOnly]
        public string inputSearchLocation;

        private MetadataResponse lastMetadata;
        private string lastSearchLocation = string.Empty;
        private PanoID curPano = (PanoID)string.Empty;

        private bool trySkybox;
        private bool skyboxFound;
        private bool locked;
        private bool firstLoad;

        private YarrowClient<ImageData> yarrow;
        private FadeTransition fader;
        private GPSLocation gps;
        private Avatar avatar;
        private SkyboxManager skybox;
        private JpegDecoder encoder;

#if UNITY_EDITOR
        private EditorTextInput locationInput;

        public void OnValidate()
        {
            locationInput = this.Ensure<EditorTextInput>();
        }

#endif

        private readonly Dictionary<string, MetadataResponse> metadataCache = new Dictionary<string, MetadataResponse>();
        private readonly Dictionary<PanoID, Transform> panoContainerCache = new Dictionary<PanoID, Transform>();
        private readonly Dictionary<PanoID, Dictionary<int, Transform>> panoDetailContainerCache = new Dictionary<PanoID, Dictionary<int, Transform>>();
        private readonly Dictionary<PanoID, Dictionary<int, Dictionary<int, Transform>>> panoDetailSliceContainerCache = new Dictionary<PanoID, Dictionary<int, Dictionary<int, Transform>>>();
        private readonly Dictionary<PanoID, Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>> panoDetailSliceFrameContainerCache = new Dictionary<PanoID, Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>>();

        public string CredentialFile
        {
            get
            {
                var baseCachePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                var keyFile = Path.Combine(baseCachePath, "GoogleMaps", "keys.txt");
                return keyFile;
            }
        }

        public void ReceiveCredentials(string[] args)
        {
            if (args == null)
            {
                gmapsApiKey = null;
                gmapsSigningKey = null;
            }
            else
            {
                gmapsApiKey = args[0];
                gmapsSigningKey = args[1];
            }
        }

        private void FindComponents()
        {
            fader = ComponentExt.FindAny<FadeTransition>();
            gps = ComponentExt.FindAny<GPSLocation>();
            avatar = ComponentExt.FindAny<Avatar>();
            skybox = ComponentExt.FindAny<SkyboxManager>();
        }

        public override void Awake()
        {
            base.Awake();

            FindComponents();

#if UNITY_EDITOR
            this.ReceiveCredentials();

            locationInput = this.Ensure<EditorTextInput>();
            locationInput.OnSubmit.AddListener(new UnityAction<string>(SetLocation));
            if (!string.IsNullOrEmpty(locationInput.value))
            {
                SetLocation(locationInput.value);
            }

            var baseCachePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
#else
            var baseCachePath = Application.persistentDataPath;
#endif
            var yarrowCacheDirName = Path.Combine(baseCachePath, "Yarrow");
            var yarrowCacheDir = new DirectoryInfo(yarrowCacheDirName);
            var gmapsCacheDirName = Path.Combine(baseCachePath, "GoogleMaps");
            var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
            var uri = new Uri(yarrowServerHost);
            encoder = new JpegDecoder(80, 2);
            yarrow = new YarrowClient<ImageData>(uri, encoder, yarrowCacheDir, gmapsApiKey, gmapsSigningKey, gmapsCacheDir);
        }

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);
            if (string.IsNullOrEmpty(inputSearchLocation) && gps?.HasCoord == true)
            {
                SetLatLngLocation(gps.Coord);
            }

            firstLoad = true;
            SynchronizeData(prog);
        }

        public override void Update()
        {
            base.Update();
            if (IsEntered && IsComplete && !locked)
            {
                SynchronizeData();
            }
        }

        private Coroutine SynchronizeData(IProgress prog = null)
        {
            locked = true;
            var gmapsMatch = GMAPS_URL_PATTERN.Match(inputSearchLocation);
            if (gmapsMatch.Success)
            {
                inputSearchLocation = gmapsMatch.Groups[1].Value;
            }
            return StartCoroutine(SynchronizeDataCoroutine(prog));
        }

        private IEnumerator SynchronizeDataCoroutine(IProgress prog)
        {
            if (!string.IsNullOrEmpty(inputSearchLocation))
            {
                var metadataProg = prog.Subdivide(0, 0.1f);
                var subProg = prog.Subdivide(0.1f, 0.9f);

                if (lastMetadata == null || lastSearchLocation != inputSearchLocation)
                {
                    yield return GetMetadata(metadataProg);
                }

                if (lastMetadata != null && lastMetadata.pano_id != curPano)
                {
                    var nextPano = lastMetadata.pano_id;

                    if (trySkybox)
                    {
                        var path = StreamingAssets.FormatPath(Application.streamingAssetsPath, $"{nextPano}.jpeg");
                        var streamTask = StreamingAssets.GetStream(Application.persistentDataPath, path);
                        while (streamTask.IsRunning())
                        {
                            yield return null;
                        }

                        if (streamTask.IsCompleted && streamTask.Result != null)
                        {
                            using (var stream = streamTask.Result.Content)
                            {
                                var image = encoder.Read(stream);
                                var texture = image.ToTexture();
                                skybox.imageType = SkyboxManager.ImageType.Degrees360;
                                skybox.layout = SkyboxManager.Mode.Cube;
                                skybox.useMipMap = false;
                                skybox.SetTexture(texture);

                                skyboxFound = true;
                                curPano = nextPano;
                            }
                        }

                        trySkybox = false;
                    }

                    if (!skyboxFound)
                    {
                        var euler = (Vector2)avatar.Head.rotation.eulerAngles;

                        var lodProgs = subProg.Split(2);
                        var topLevelProgs = lodProgs[0].Split(TEST_ANGLES[0].Length);
                        var otherLevelProgs = lodProgs[1].Split(MAX_REQUESTS);

                        var numRequests = 0;
                        for (var f = 0; f < FOVs.Length && numRequests < MAX_REQUESTS; ++f)
                        {
                            var testAngles = TEST_ANGLES[f];
                            for (var a = 0; a < testAngles.Length && numRequests < MAX_REQUESTS; ++a)
                            {
                                var testAngle = euler + testAngles[a];
                                CalcRequestParameters(testAngle, f, out var fov, out var radius, out var scale, out var requestHeading, out var requestPitch);
                                if (FillCaches(nextPano, f, radius, requestHeading, requestPitch))
                                {
                                    IProgress imageProg;
                                    if (f == 0)
                                    {
                                        imageProg = topLevelProgs[a];
                                    }
                                    else
                                    {
                                        imageProg = otherLevelProgs[numRequests++];
                                    }
                                    yield return GetImage(nextPano, f, (int)fov, scale, requestHeading, requestPitch, imageProg);

                                    if (f > 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (numRequests == 0)
                        {
#if UNITY_EDITOR
                            yield return null;
                            CaptureCubemap(nextPano);
#endif
                            print("nothing left to do");
                            curPano = nextPano;
                        }
                    }

                    if (firstLoad)
                    {
                        Complete();
                        firstLoad = false;
                    }
                    else if (fader.IsEntered && fader.IsComplete)
                    {
                        fader.Exit();
                    }
                }
            }
            locked = false;
        }

        private static readonly Regex GMAPS_URL_PATTERN = new Regex("https?://www\\.google\\.com/maps/@-?\\d+\\.\\d+,\\d+\\.\\d+(?:,[a-zA-Z0-9.]+)*/data=(?:![a-z0-9]+)*!1s([a-zA-Z0-9_\\-]+)(?:![a-z0-9]+)*", RegexOptions.Compiled);

        private IEnumerator GetMetadata(IProgress metadataProg)
        {
            Task<MetadataResponse> metadataTask;

            if (metadataCache.ContainsKey(inputSearchLocation))
            {
                SetMetadata(inputSearchLocation, metadataCache[inputSearchLocation]);
            }
            else
            {
                print("Getting metadata " + inputSearchLocation);
                if (PanoID.TryParse(inputSearchLocation, out var pano2))
                {
                    metadataTask = yarrow.GetMetadata(pano2, metadataProg);
                }
                else if (LatLngPoint.TryParseDecimal(inputSearchLocation, out var point))
                {
                    metadataTask = yarrow.GetMetadata(point, metadataProg);
                }
                else
                {
                    metadataTask = yarrow.GetMetadata((PlaceName)inputSearchLocation, metadataProg);
                }

                while (metadataTask.IsRunning())
                {
                    yield return null;
                }

                if (metadataTask.IsCompleted)
                {
                    print("metadata found");
                    var curMetadata = metadataTask.Result;
                    metadataCache[inputSearchLocation] = curMetadata;
                    trySkybox = skybox != null;
                    skyboxFound = false;

                    if (curMetadata.status == HttpStatusCode.OK && curMetadata.pano_id != curPano)
                    {
                        if (!firstLoad)
                        {
                            fader.Enter();
                            yield return fader.Waiter;
                        }

                        if (lastMetadata != null)
                        {
                            if (panoContainerCache.ContainsKey(lastMetadata.pano_id))
                            {
                                var panoContainer = panoContainerCache[lastMetadata.pano_id];
                                panoContainer.Deactivate();
                            }
                        }

                        SetMetadata(inputSearchLocation, curMetadata);
                    }
                }
            }
        }

        private void SetMetadata(string inputSearchLocation, MetadataResponse metadata)
        {
            lastMetadata = metadata;
            lastSearchLocation = inputSearchLocation;
#if UNITY_EDITOR
            if (locationInput != null)
            {
                locationInput.value = inputSearchLocation;
            }
#endif
            SetLatLngLocation(metadata.location);
        }

        private static void CalcRequestParameters(Vector2 testAngle, int f, out float fov, out int radius, out float scale, out int requestHeading, out int requestPitch)
        {
            var overlap = FOVs.Length - f;
            radius = 5 * overlap + 50;
            if (f == 0)
            {
                overlap = 0;
            }

            var subFOV = FOVs[f];
            fov = subFOV + 2 * overlap;
            scale = 2 * radius * Mathf.Tan(Degrees.Radians(fov / 2));
            requestHeading = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.y / fov), 360);
            var unityPitch = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.x / fov), 360);
            requestPitch = -unityPitch;
            if (unityPitch >= 270)
            {
                requestPitch += 360;
            }
            else if (unityPitch > 90)
            {
                requestPitch += 180;
            }

            if (90 < unityPitch && unityPitch < 270)
            {
                requestHeading = (int)Mathf.Repeat(requestHeading + 180, 360);
            }
        }

        private IEnumerator GetImage(PanoID panoid, int f, int fov, float scale, int requestHeading, int requestPitch, IProgress imageProg)
        {
            var imageTask = yarrow.GetImage(panoid, fov, requestHeading, requestPitch, imageProg.Subdivide(0f, 0.9f));
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

                    frame.layer = LayerMask.NameToLayer("Photospheres");
                    frame.transform.SetParent(panoDetailSliceFrameContainerCache[panoid][f][requestHeading][requestPitch], false);
                    frame.transform.localScale = scale * Vector3.one;
                }
            }
        }

#if UNITY_EDITOR
        private bool cubemapLock;

        private void CaptureCubemap(PanoID panoid)
        {
            if (!cubemapLock)
            {
                cubemapLock = true;
                StartCoroutine(CaptureCubemapCoroutine(panoid));
            }
        }

        private IEnumerator CaptureCubemapCoroutine(PanoID panoid)
        {
            var fileName = Path.Combine("Assets", "StreamingAssets", $"{panoid.ToString()}.jpeg");
            if (!File.Exists(fileName))
            {
                using (var prog = new UnityEditorProgressDialog("Saving cubemap " + panoid))
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
                    DisplayManager.MainCamera.cullingMask = LayerMask.GetMask("Photospheres");

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
                            var img = encoder.Concatenate(ImageData.CubeCross(images), subProgs[2]);
                            encoder.Save(fileName, img, subProgs[3]);
                        }
                        catch
                        {
                            cubemapLock = false;
                            throw;
                        }
                    });

                    while (saveTask.IsRunning())
                    {
                        yield return null;
                    }

                    if (saveTask.IsCompleted)
                    {
                        Debug.Log("Cubemap saved");
                    }
                    else if (saveTask.IsFaulted)
                    {
                        Debug.Log("Cubemap save error");
                    }
                    else
                    {
                        Debug.Log("Cubemap cancelled");
                    }
                }
            }

            cubemapLock = false;
        }

#endif

        private bool FillCaches(PanoID panoid, int f, float radius, int requestHeading, int requestPitch)
        {
            if (!panoDetailSliceFrameContainerCache.ContainsKey(panoid))
            {
                var pano = new GameObject(panoid.ToString()).transform;
                pano.position = avatar.Head.position;
                panoContainerCache[panoid] = pano;
                panoDetailContainerCache[panoid] = new Dictionary<int, Transform>();
                panoDetailSliceContainerCache[panoid] = new Dictionary<int, Dictionary<int, Transform>>();
                panoDetailSliceFrameContainerCache[panoid] = new Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>();
            }

            var panoContainer = panoContainerCache[panoid];
            var detailContainerCache = panoDetailContainerCache[panoid];
            var detailSliceContainerCache = panoDetailSliceContainerCache[panoid];
            var detailSliceFrameContainerCache = panoDetailSliceFrameContainerCache[panoid];

            panoContainer.Activate();

            if (!detailContainerCache.ContainsKey(f))
            {
                var detail = new GameObject(f.ToString()).transform;
                detail.SetParent(panoContainer, false);
                detailContainerCache[f] = detail;
                detailSliceContainerCache[f] = new Dictionary<int, Transform>();
                detailSliceFrameContainerCache[f] = new Dictionary<int, Dictionary<int, Transform>>();
            }

            var detailContainer = detailContainerCache[f];
            var sliceContainerCache = detailSliceContainerCache[f];
            var sliceFrameContainerCache = detailSliceFrameContainerCache[f];

            if (!sliceContainerCache.ContainsKey(requestHeading))
            {
                var slice = new GameObject(requestHeading.ToString()).transform;
                slice.SetParent(detailContainer, false);
                sliceContainerCache[requestHeading] = slice;
                sliceFrameContainerCache[requestHeading] = new Dictionary<int, Transform>();
            }

            var sliceContainer = sliceContainerCache[requestHeading];
            var frameContainerCache = sliceFrameContainerCache[requestHeading];

            if (!frameContainerCache.ContainsKey(requestPitch))
            {
                var frameContainer = new GameObject(requestPitch.ToString()).transform;
                frameContainer.rotation = Quaternion.Euler(-requestPitch, requestHeading, 0);
                frameContainer.position = frameContainer.rotation * (radius * Vector3.forward);
                frameContainer.SetParent(sliceContainer, false);
                frameContainerCache[requestPitch] = frameContainer;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            Complete();
        }

        public void SetLatLngLocation(LatLngPoint location)
        {
            if (gps != null)
            {
                gps.FakeCoord = true;
                gps.Coord = location;
            }
        }

        public void SetLocation(string location)
        {
            inputSearchLocation = location;
        }

        public void Move(Vector2 deltaMeters)
        {
            if (lastMetadata?.location != null)
            {
                yarrow.ClearError();
                deltaMeters /= 10f;
                var utm = lastMetadata.location.ToUTM();
                utm = new UTMPoint(utm.X + deltaMeters.x, utm.Y + deltaMeters.y, utm.Z, utm.Zone, utm.Hemisphere);
                inputSearchLocation = utm.ToLatLng().ToString();
            }
        }

        public void MoveNorth()
        {
            Move(Vector2.up * 2 * searchRadius);
        }

        public void MoveEast()
        {
            Move(Vector2.right * 2 * searchRadius);
        }

        public void MoveWest()
        {
            Move(Vector2.left * 2 * searchRadius);
        }

        public void MoveSouth()
        {
            Move(Vector2.down * 2 * searchRadius);
        }
    }
}