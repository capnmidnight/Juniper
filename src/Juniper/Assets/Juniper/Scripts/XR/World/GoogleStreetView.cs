using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.Animation;
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

using UnityEngine;
using UnityEngine.Events;

using Yarrow.Client;

namespace Juniper.Imaging
{
    public class GoogleStreetView : SubSceneController, ICredentialReceiver
    {
        private const string LAT_LON = "_MAPPING_LATITUDE_LONGITUDE_LAYOUT";
        private const string SIDES_6 = "_MAPPING_6_FRAMES_LAYOUT";

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

        private Avatar avatar;

        private YarrowClient<ImageData> yarrow;

        public int searchRadius = 50;

        [ReadOnly]
        public string Location;

        private LatLngPoint LatLngLocation;
        private PanoID lastPano;

        [SerializeField]
        [HideInNormalInspector]
        private FadeTransition fader;

        [SerializeField]
        [HideInNormalInspector]
        private GPSLocation gps;

        private bool locked;

        private readonly Dictionary<string, MetadataResponse> metadataCache = new Dictionary<string, MetadataResponse>();
        private readonly Dictionary<PanoID, Transform> panoContainerCache = new Dictionary<PanoID, Transform>();
        private readonly Dictionary<PanoID, Dictionary<int, Transform>> panoDetailContainerCache = new Dictionary<PanoID, Dictionary<int, Transform>>();
        private readonly Dictionary<PanoID, Dictionary<int, Dictionary<int, Transform>>> panoDetailSliceContainerCache = new Dictionary<PanoID, Dictionary<int, Dictionary<int, Transform>>>();
        private readonly Dictionary<PanoID, Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>> panoDetailSliceFrameContainerCache = new Dictionary<PanoID, Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>>();

#if UNITY_EDITOR

        private EditorTextInput locationInput;

        public void OnValidate()
        {
            locationInput = this.Ensure<EditorTextInput>();
        }

#endif

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
            var decoder = new JpegDecoder();
            yarrow = new YarrowClient<ImageData>(uri, decoder, yarrowCacheDir, gmapsApiKey, gmapsSigningKey, gmapsCacheDir);
        }

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);
            if (string.IsNullOrEmpty(Location) && gps?.HasCoord == true)
            {
                SetLatLngLocation(gps.Coord);
            }
            GetImages(false, prog);
        }

        private readonly string lastStatus;

        public override void Update()
        {
            base.Update();
            if (IsEntered && IsComplete && !locked)
            {
                GetImages(true);
            }
        }

        private Coroutine GetImages(bool fromNavigation, IProgress prog = null)
        {
            locked = true;
            return StartCoroutine(GetImagesCoroutine(fromNavigation, prog));
        }

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
            for (float ax = 0; ax <= 360; ax += fov)
            {
                for (float ay = 0; ay <= 180; ay += fov)
                {
                    list.Add(new Vector2(ax, ay));
                }
            }

            return list
                .Distinct()
                .OrderBy(x => x.magnitude)
                .ToArray();
        }

        private MetadataResponse lastMetadata;
        private const int MAX_REQUESTS = 1;

        private IEnumerator GetImagesCoroutine(bool fromNavigation, IProgress prog)
        {
            if (!string.IsNullOrEmpty(Location))
            {
                var metadataProg = prog.Subdivide(0, 0.1f);
                var subProg = prog.Subdivide(0.1f, 0.9f);

                if (!metadataCache.ContainsKey(Location))
                {
                    Task<MetadataResponse> metadataTask;
                    if (LatLngPoint.TryParseDecimal(Location, out var point))
                    {
                        metadataTask = yarrow.GetMetadata(point, metadataProg);
                    }
                    else
                    {
                        metadataTask = yarrow.GetMetadata((PlaceName)Location, metadataProg);
                    }

                    while (!metadataTask.IsCompleted && !metadataTask.IsCanceled && !metadataTask.IsFaulted)
                    {
                        yield return null;
                    }

                    if (metadataTask.IsCompleted)
                    {
                        var curMetadata = metadataTask.Result;
                        metadataCache[Location] = curMetadata;

                        if (curMetadata.pano_id != lastPano)
                        {
                            if (fromNavigation)
                            {
                                fader.Enter();
                                yield return fader.Waiter;
                            }

                            if (lastMetadata != null && panoContainerCache.ContainsKey(lastMetadata.pano_id))
                            {
                                var panoContainer = panoContainerCache[lastMetadata.pano_id];
                                panoContainer.Deactivate();
                            }

                            lastPano = curMetadata.pano_id;
                            print($"Pano ID = {lastPano.ToString()}");

                            var curUTM = curMetadata.location.ToUTM();
                            if (lastMetadata != null)
                            {
                                var lastUTM = lastMetadata.location.ToUTM();
                                var delta = 20 * curUTM.Subtract(lastUTM);
                                if (delta.magnitude > 0)
                                {
                                    avatar.transform.position += delta;
                                    transform.position += delta;
                                }
                            }

                            lastMetadata = curMetadata;
                        }
                    }
                }

                if (metadataCache.ContainsKey(Location))
                {
                    var metadata = metadataCache[Location];

                    if (metadata?.status != HttpStatusCode.OK)
                    {
                        print("no metadata");
                    }
                    else
                    {
                        SetLatLngLocation(metadata.location);

                        var panoid = metadata.pano_id;
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
                                if (FillCaches(panoid, f, radius, requestHeading, requestPitch))
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
                                    yield return GetImage(panoid, f, (int)fov, scale, requestHeading, requestPitch, imageProg);

                                    if (f > 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

#if UNITY_EDITOR
                        if (numRequests == 0)
                        {
                            CaptureCubemap(panoid);
                        }
#endif

                        if (!fromNavigation)
                        {
                            Complete();
                        }
                        else if (fader.IsEntered && fader.IsComplete)
                        {
                            fader.Exit();
                        }
                    }
                }
            }
            locked = false;
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
            var imageTask = yarrow.GetImage(panoid, (int)fov, requestHeading, requestPitch, imageProg);
            while (!imageTask.IsCanceled && !imageTask.IsFaulted && !imageTask.IsCompleted)
            {
                yield return null;
            }

            if (imageTask.IsCompleted)
            {
                var image = imageTask.Result;
                if (image != null)
                {
                    //var textureProg = patchProg.Subdivide(0.9f, 0.1f);
                    var texture = new Texture2D(image.dimensions.width, image.dimensions.height, TextureFormat.RGB24, false);
                    if (image.format == ImageFormat.None)
                    {
                        texture.LoadRawTextureData(image.data);
                    }
                    else if (image.format != ImageFormat.Unsupported)
                    {
                        texture.LoadImage(image.data);
                    }
                    //textureProg?.Report(0.3333f);
                    yield return null;
                    texture.Compress(true);
                    //textureProg?.Report(0.66667f);
                    yield return null;
                    texture.Apply(false, true);
                    //textureProg?.Report(1);

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

        private void CaptureCubemap(PanoID panoid)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var fileName = Path.Combine(dir, "GoogleMaps", "streetview", $"{panoid.ToString()}.cubemap.jpeg");
            if (!File.Exists(fileName))
            {
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

                var encoder = new JpegDecoder(80, 2);
                var img = encoder.Concatenate(new ImageData[,]
                {
                    { null, images[0], null, null },
                    { images[1], images[2], images[3], images[4] },
                    { null, images[5], null, null }
                });
                encoder.Save(fileName, img);
            }
        }

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
            LatLngLocation = location;
            Location = LatLngLocation.ToString();

            if (gps != null)
            {
                gps.FakeCoord = true;
                gps.Coord = location;
            }
#if UNITY_EDITOR
            if (locationInput != null)
            {
                locationInput.value = Location;
            }
#endif
        }

        public void SetLocation(string location)
        {
            Location = location;
        }

        public void Move(Vector2 deltaMeters)
        {
            if (LatLngLocation != null)
            {
                yarrow.ClearError();
                deltaMeters /= 10f;
                var utm = LatLngLocation.ToUTM();
                utm = new UTMPoint(utm.X + deltaMeters.x, utm.Y + deltaMeters.y, utm.Z, utm.Zone, utm.Hemisphere);
                SetLatLngLocation(utm.ToLatLng());
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