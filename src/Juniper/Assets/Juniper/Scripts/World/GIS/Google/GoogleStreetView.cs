using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Display;
using Juniper.Imaging;
using Juniper.Input;
using Juniper.IO;
using Juniper.Progress;
using Juniper.Security;
using Juniper.Units;
using Juniper.Unity;
using Juniper.Widgets;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.World.GIS.Google
{
    public class GoogleStreetView : SubSceneController
#if UNITY_EDITOR
        , ICredentialReceiver
#endif
    {
        private static readonly Regex GMAPS_URL_PANO_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@-?\\d+\\.\\d+,-?\\d+\\.\\d+(?:,[a-zA-Z0-9.]+)*/data=(?:![a-z0-9]+)*!1s([a-zA-Z0-9_\\-]+)(?:![a-z0-9]+)*", RegexOptions.Compiled);

        private static readonly Regex GMAPS_URL_LATLNG_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@(-?\\d+\\.\\d+,-?\\d+\\.\\d+)", RegexOptions.Compiled);

        private readonly Dictionary<string, MetadataResponse> metadataCache = new Dictionary<string, MetadataResponse>();

        [SerializeField]
        [HideInInspector]
        private string gmapsApiKey;

        [SerializeField]
        [HideInInspector]
        private string gmapsSigningKey;

        public int searchRadius = 50;

        public float[] searchFOVs =
        {
            90,
            60,
            30
        };

        [ReadOnly]
        public string searchLocation;

        private MetadataResponse metadata;


        private LatLngPoint origin;
        private Vector3 cursorPosition;
        private Vector3 lastCursorPosition;
        private Vector3 navPointerPosition;
        private string navPointerPano;

        private GoogleMapsClient gmaps;
        private GPSLocation gps;
        private PhotosphereManager photospheres;
        private Clickable navPlane;
        private Avatar avatar;
        private Transform navPointer;
        private UnifiedInputModule input;
        private CachingStrategy cache;
        private IImageCodec<Texture2D> codec;
        private PhotosphereJig lastSphere;
        private UnityTexture2DProcessor processor;
        private Task searchTask;

#if UNITY_EDITOR
        private Task captureTask;
        private EditorTextInput locationInput;

        public void OnValidate()
        {
            locationInput = this.Ensure<EditorTextInput>();
        }

        public string CredentialFile
        {
            get
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "google-streetview.txt");
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
#endif

        public bool IsBusy
        {
            get
            {
                return searchTask.IsRunning()
#if UNITY_EDITOR
                    || captureTask.IsRunning()
#endif
                    || lastSphere != null
                        && lastSphere.IsBusy;
            }
        }

        public override void Awake()
        {
            base.Awake();

            processor = new UnityTexture2DProcessor();

            Find.Any(out gps);
            if (!this.FindClosest(out photospheres))
            {
                photospheres = this.Ensure<PhotosphereManager>();
            }

            navPlane = transform.Find("NavPlane").Ensure<Clickable>();
            Find.Any(out avatar);
            navPointer = transform.Find("NavPointer");
            Find.Any(out input);

#if UNITY_EDITOR
            this.ReceiveCredentials();

            locationInput = this.Ensure<EditorTextInput>();
            locationInput.OnSubmit.AddListener(new UnityAction<string>(SetLocation));
            if (!string.IsNullOrEmpty(locationInput.value))
            {
                SetLocation(locationInput.value);
            }
            else if (gps != null && gps.HasCoord)
            {
                SetLocation(gps.Coord.ToString());
            }

            var baseCachePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
#else
            if(gps != null && gps.HasCoord)
            {
                SetLocation(gps.Coord.ToString());
            }
            var baseCachePath = Application.persistentDataPath;
#endif
            cache = new GoogleMapsCachingStrategy(baseCachePath);
            codec = new UnityTexture2DCodec(MediaType.Image.Jpeg);
            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var geocodingDecoder = new JsonFactory<GeocodingResponse>();

            gmaps = new GoogleMapsClient(gmapsApiKey, gmapsSigningKey, metadataDecoder, geocodingDecoder, cache);

            foreach(var fileRef in cache.Get(metadataDecoder.ContentType))
            {
                if(cache.TryLoad(metadataDecoder, fileRef, out var metadata))
                {
                    if (metadata.location != null)
                    {
                        metadataCache[metadata.location.ToString()] = metadata;
                        metadataCache[metadata.pano_id] = metadata;
                    }
                    else
                    {
                        cache.Delete(fileRef);
                    }
                }
            }

            photospheres.CubemapNeeded += Photosphere_CubemapNeeded;

            photospheres.SetIO(cache, codec);
            photospheres.SetDetailLevels(searchFOVs);

            navPlane.Click += NavPlane_Click;
            var renderer = navPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

#if UNITY_EDITOR

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

        private async Task CaptureCubemap(PhotosphereJig photosphere)
        {
            try
            {
                if (cache.IsCached(photosphere.name + codec.ContentType))
                {
                    Debug.Log("Cubemap already cached");
                }
                else
                {
                    using (var prog = new UnityEditorProgressDialog("Saving cubemap " + photosphere.name))
                    {
                        var subProgs = prog.Split(CAPTURE_CUBEMAP_FIELDS);
                        const int dim = 2048;
                        var cubemap = await JuniperSystem.OnMainThread(() =>
                        {
                            subProgs[0].Report(0);
                            var cb = new Cubemap(dim, TextureFormat.RGB24, false);
                            cb.Apply();

                            var curMask = DisplayManager.MainCamera.cullingMask;
                            DisplayManager.MainCamera.cullingMask = LayerMask.GetMask(Photosphere.PHOTOSPHERE_LAYER_ARR);

                            var curRotation = DisplayManager.MainCamera.transform.rotation;
                            DisplayManager.MainCamera.transform.rotation = Quaternion.identity;

                            DisplayManager.MainCamera.RenderToCubemap(cb, 63);

                            DisplayManager.MainCamera.cullingMask = curMask;
                            DisplayManager.MainCamera.transform.rotation = curRotation;
                            subProgs[0].Report(1);

                            return cb;
                        });


                        for (var f = 0; f < CAPTURE_CUBEMAP_FACES.Length; ++f)
                        {
                            await JuniperSystem.OnMainThread(() =>
                            {
                                subProgs[1].Report(f, CAPTURE_CUBEMAP_FACES.Length, CAPTURE_CUBEMAP_FACES[f].ToString());
                                var pixels = cubemap.GetPixels(CAPTURE_CUBEMAP_FACES[f]);
                                var texture = new Texture2D(cubemap.width, cubemap.height);
                                texture.SetPixels(pixels);
                                texture.Apply();
                                CAPTURE_CUBEMAP_SUB_IMAGES[f] = texture;
                                subProgs[1].Report(f + 1, CAPTURE_CUBEMAP_FACES.Length);
                            });
                        }

                        var img = await JuniperSystem.OnMainThread(() =>
                            processor.Concatenate(ImageData.CubeCross(CAPTURE_CUBEMAP_SUB_IMAGES), subProgs[2]));

                        cache.Save(codec, photosphere.name + codec.ContentType, img, subProgs[3]);

                        Debug.Log("Cubemap saved " + photosphere.name);
                    }
                }

                await JuniperSystem.OnMainThread(photosphere.DestroyJig);

                var anyDestroyed = await JuniperSystem.OnMainThread(() =>
                {
                    var any = false;
                    foreach (var texture in CAPTURE_CUBEMAP_SUB_IMAGES)
                    {
                        if (texture != null)
                        {
                            any = true;
                            Destroy(texture);
                        }
                    }

                    return any;
                });

                if (anyDestroyed)
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                }

                photosphere.enabled = false;
                await Task.Yield();
                photosphere.enabled = true;
            }
            catch (Exception exp)
            {
                Debug.LogError("Cubemap capture error");
                Debug.LogException(exp, this);
                throw;
            }
        }

        private void Photosphere_Complete(PhotosphereJig jig, bool captureCubemap)
        {
            jig.ImageNeeded -= Photosphere_ImageNeeded;
            jig.Complete -= Photosphere_Complete;

            if (captureCubemap)
            {
                captureTask = CaptureCubemap(jig);
            }
        }
#endif

        private async Task<Texture2D> Photosphere_CubemapNeeded(Photosphere source)
        {
            var cubemapRef = source.CubemapName + codec.ContentType;
            if (cache == null
                || codec == null
                || string.IsNullOrEmpty(source.CubemapName)
                || !cache.IsCached(cubemapRef))
            {
                return null;
            }
            else
            {
                return await Decode(await cache.Open(cubemapRef));
            }
        }

        private async Task<Texture2D> Photosphere_ImageNeeded(PhotosphereJig source, int fov, int heading, int pitch)
        {
            if (gmaps == null
                || codec == null)
            {
                return null;
            }
            else
            {
                return await Decode(await gmaps.GetImage(source.CubemapName, fov, heading, pitch));
            }
        }

        private async Task<Texture2D> Decode(Stream imageStream)
        {
            if (imageStream == null)
            {
                return null;
            }
            else
            {
                using (imageStream)
                {
                    return codec.Deserialize(imageStream);
                }
            }
        }

        private void NavPlane_Click(object sender, EventArgs e)
        {
            if (!IsBusy && !string.IsNullOrEmpty(navPointerPano))
            {
                searchLocation = navPointerPano;
            }
        }

        public override void Enter(IProgress prog)
        {
            base.Enter(prog);
            SyncData(prog);
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            Complete();
        }

        public void SetLocation(string location)
        {
            searchLocation = location;
        }

        public void Update()
        {
            navPointer.position = navPointerPosition;
            if (IsEntered && IsComplete && !IsBusy)
            {
                SyncData(null);
            }
        }

        private static void LogError(Task erroredTask)
        {
            Debug.LogError(erroredTask.Exception);
            var stack = new Stack<Exception>(erroredTask.Exception.InnerExceptions);
            while (stack.Count > 0)
            {
                var here = stack.Pop();
                if (here != null)
                {
                    Debug.LogError(here);
                    stack.Push(here.InnerException);
                }
            }
        }

        private void SyncData(IProgress prog)
        {
            var isNewSearch = ParseSearchParams(searchLocation, out var searchPano, out var searchPoint);
            if (isNewSearch)
            {
                searchTask = SearchData(searchPano, searchPoint, prog)
                    .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted);
            }
            else if (origin != null && metadata != null)
            {
                cursorPosition = input.MouseEnabled
                    ? input.Mouse.CursorPosition
                    : input.ControllersEnabled
                        ? input.Controllers[0].IsConnected
                            ? input.Controllers[0].CursorPosition
                            : input.Controllers[1].CursorPosition
                        : input.GazeEnabled
                            ? input.Gaze.CursorPosition
                            : Vector3.zero;
                var cursorDelta = cursorPosition - lastCursorPosition;
                if (cursorDelta.magnitude > 0.1f)
                {
                    searchTask = SearchPoint();
                }
            }
        }

        private async Task SearchPoint()
        {
            lastCursorPosition = cursorPosition;
            var nextPoint = GetRelativeLatLng(cursorPosition);
            var nextMetadata = await gmaps.GetMetadata(nextPoint, searchRadius, null);
            nextMetadata = ValidateMetadata(nextMetadata);
            if (nextMetadata != null)
            {
                navPointerPano = nextMetadata.pano_id;
                navPointerPosition = GetRelativeVector3(nextMetadata);
            }
        }

        private async Task SearchData(string searchPano, LatLngPoint searchPoint, IProgress prog)
        {
            print("start new search");
            metadata = null;

            prog.Report(0);

            if (metadataCache.ContainsKey(searchLocation))
            {
                metadata = metadataCache[searchLocation];
            }
            else
            {
                var metaSubProgs = prog.Split("Searching by PanoID", "Searching by Lat/Lng", "Searching by Location Name");
                if (metadata == null && searchPano != null)
                {
                    metadata = await gmaps.GetMetadata(searchPano, searchRadius, metaSubProgs[0]);
                }

                if (metadata == null && searchPoint != null)
                {
                    metadata = await gmaps.GetMetadata(searchPoint, searchRadius, metaSubProgs[1]);
                }

                if (metadata == null && searchLocation != null)
                {
                    metadata = await gmaps.SearchMetadata(searchLocation, searchRadius, metaSubProgs[2]);
                }

                metadata = ValidateMetadata(metadata);

                if (metadata != null && !string.IsNullOrEmpty(metadata.pano_id))
                {
                    searchPoint = metadata.location;
                    searchLocation = searchPano = metadata.pano_id;

                    metadataCache[searchPano] = metadata;
                    metadataCache[searchPoint.ToString()] = metadata;
                    metadataCache[searchLocation] = metadata;
                }
            }

            if (metadata != null)
            {
                if (lastSphere == null)
                {
                    origin = metadata.location;
                }

                if (gps != null)
                {
                    gps.FakeCoord = true;
                    gps.Coord = metadata.location;
                }
#if UNITY_EDITOR
                if (locationInput != null)
                {
                    locationInput.value = searchLocation;
                }
#endif

                var curSphere = await GetPhotosphere();

                if (lastSphere != null)
                {
                    await JuniperSystem.OnMainThread(lastSphere.Deactivate);
                }

                await JuniperSystem.OnMainThread(curSphere.Activate);

                if (lastSphere == null)
                {
                    await prog.WaitOn(curSphere, "Loading photosphere");
                    Complete();
                }

                await JuniperSystem.OnMainThread(() =>
                {
                    var delta = GetRelativeVector3(metadata);

                    navPlane.transform.position
                        = avatar.transform.position
                        = delta;

                    curSphere.transform.position = avatar.Head.position;
                });

                prog.Report(1);

                lastSphere = curSphere;
            }
        }

        private readonly HashSet<string> imageNeededSet = new HashSet<string>();

        private async Task<PhotosphereJig> GetPhotosphere()
        {
            var jig = await JuniperSystem.OnMainThread(() =>
                photospheres.GetPhotosphere<PhotosphereJig>(metadata.pano_id));
            if (!imageNeededSet.Contains(metadata.pano_id))
            {
                imageNeededSet.Add(metadata.pano_id);
                jig.ImageNeeded += Photosphere_ImageNeeded;
#if UNITY_EDITOR
                jig.Complete += Photosphere_Complete;
#endif
            }
            return jig;
        }

        private bool ParseSearchParams(string searchLocation, out string searchPano, out LatLngPoint searchPoint)
        {
            searchPano = null;
            searchPoint = null;

            if (!string.IsNullOrEmpty(searchLocation))
            {
                var panoMatch = GMAPS_URL_PANO_PATTERN.Match(searchLocation);
                var latLngMatch = GMAPS_URL_LATLNG_PATTERN.Match(searchLocation);

                if (panoMatch.Success && MetadataResponse.IsPano(panoMatch.Groups[1].Value))
                {
                    searchPano = panoMatch.Groups[1].Value;
                }
                else if (MetadataResponse.IsPano(searchLocation))
                {
                    searchPano = searchLocation;
                }

                if (latLngMatch.Success && LatLngPoint.TryParseDecimal(latLngMatch.Groups[1].Value, out var point))
                {
                    searchPoint = point;
                }
                else if (LatLngPoint.TryParseDecimal(searchLocation, out var point2))
                {
                    searchPoint = point2;
                }
            }

            if (metadata == null)
            {
                Debug.Log("No existing search");
                return true;
            }
            else if (searchPano != null && metadata.pano_id != searchPano)
            {
                Debug.Log("Search pano changed " + searchPano);
                return true;
            }
            else if (searchPoint != null && searchPoint.Distance(metadata.location) > 3f)
            {
                Debug.Log($"Search point changed {metadata.location} -> {searchPoint}");
                return true;
            }
            else
            {
                return false;
            }
        }

        private LatLngPoint GetRelativeLatLng(Vector3 cursorPosition)
        {
            var curUTM = origin.ToUTM();
            var curVec = curUTM.ToVector3();
            var nextVec = cursorPosition + curVec;
            nextVec.y = 0;
            var nextPoint = nextVec.ToUTM(curUTM.Zone, curUTM.Hemisphere).ToLatLng();
            return nextPoint;
        }

        private Vector3 GetRelativeVector3(MetadataResponse metadata)
        {
            var nextUTM = metadata.location.ToUTM();
            var nextVec = nextUTM.ToVector3();
            var start = origin.ToVector3();
            var delta = nextVec - start;
            return delta;
        }

        private MetadataResponse ValidateMetadata(MetadataResponse metadata)
        {
            if (metadata?.status != HttpStatusCode.OK)
            {
                metadata = null;
            }

            return metadata;
        }
    }
}