using System;
using System.Collections;
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

        private GoogleMapsClient<Texture2D> gmaps;
        private GPSLocation gps;
        private PhotosphereManager photospheres;
        private Clickable navPlane;
        private Avatar avatar;
        private Transform navPointer;
        private UnifiedInputModule input;
        private CachingStrategy cache;
        private IImageCodec<Texture2D> codec;
        private PhotosphereJig lastSphere;
        private TaskFactory mainThread;

#if UNITY_EDITOR
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

        public bool Searching { get; private set; }

        public override void Awake()
        {
            base.Awake();

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            mainThread = new TaskFactory(scheduler);

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

            gmaps = new GoogleMapsClient<Texture2D>(gmapsApiKey, gmapsSigningKey, codec, metadataDecoder, geocodingDecoder, cache);

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

        private async Task CaptureCubemap(Photosphere photosphere)
        {
            try
            {
                var cubemapName = photosphere.name;
                var cubemapRef = cubemapName + codec.ContentType;
                if (cache.IsCached(cubemapRef))
                {
                    Debug.Log("Cubemap already cached");
                }
                else
                {
                    var prog = await mainThread.StartNew(() => new UnityEditorProgressDialog("Saving cubemap " + cubemapName));
                    using (prog)
                    {
                        var subProgs = prog.Split(CAPTURE_CUBEMAP_FIELDS);
                        const int dim = 2048;
                        var anyDestroyed = false;
                        var cubemap = await mainThread.StartNew(() =>
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

                            foreach (var texture in CAPTURE_CUBEMAP_SUB_IMAGES)
                            {
                                if (texture != null)
                                {
                                    anyDestroyed = true;
                                    Destroy(texture);
                                }
                            }

                            return cb;
                        });

                        if (anyDestroyed)
                        {
                            var co = await mainThread.StartNew(() => Resources.UnloadUnusedAssets().AsCoroutine());
                            await co.AsTask();
                            await Task.Run(GC.Collect);
                        }


                        for (var f = 0; f < CAPTURE_CUBEMAP_FACES.Length; ++f)
                        {
                            await mainThread.StartNew(() =>
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

                        await mainThread.StartNew(() =>
                        {
                            var processor = new UnityTexture2DProcessor();
                            var img = processor.Concatenate(ImageData.CubeCross(CAPTURE_CUBEMAP_SUB_IMAGES), subProgs[2]);
                            cache.Save(codec, cubemapRef, img, subProgs[3]);
                        });

                        Debug.Log("Cubemap saved " + cubemapName);
                    }
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
            finally
            {
                Searching = false;
            }
        }

        private void Photosphere_Complete(Photosphere obj, bool captureCubemap)
        {
            if (obj is PhotosphereJig jig)
            {
                jig.ImageNeeded -= Photosphere_ImageNeeded;
                jig.Complete -= Photosphere_Complete;
            }

            if (captureCubemap)
            {
                print("start cubemap capture");
                this.Run(CaptureCubemap(obj).AsCoroutine());
                print("started cubemap capture");
            }
            else
            {
                Searching = false;
            }
        }
#else
        private void Photosphere_Complete(Photosphere obj, bool captureCubemap)
        {
            Searching = false;
        }
#endif

        private string Photosphere_CubemapNeeded(Photosphere source)
        {
            return $"{source.name}.jpeg";
        }

        private Task<Texture2D> Photosphere_ImageNeeded(PhotosphereJig source, int fov, int heading, int pitch)
        {
            return gmaps.GetImage(source.name, fov, heading, pitch);
        }

        private void NavPlane_Click(object sender, EventArgs e)
        {
            if (!Searching && origin != null && metadata != null)
            {
                var searchPoint = GetRelativeLatLng(navPointerPosition);
                gmaps.GetMetadata(searchPoint, searchRadius)
                    .ContinueWith(task =>
                        searchLocation = task.Result.pano_id)
                    .ConfigureAwait(false);
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
            if (IsEntered && IsComplete && !Searching)
            {
                SyncData(null);
            }
        }

        private void SyncData(IProgress prog)
        {
            var isNewSearch = ParseSearchParams(searchLocation, out var searchPano, out var searchPoint);
            if (isNewSearch)
            {
                Searching = true;
                Task.Run(() => SearchData(searchPano, searchPoint, prog));
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
                    Searching = true;
                    Task.Run(SearchPoint);
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
                navPointerPosition = GetRelativeVector3(nextMetadata);
                await mainThread.StartNew(() =>
                    navPointer.position = navPointerPosition);
            }

            Searching = false;
        }

        private async Task SearchData(string searchPano, LatLngPoint searchPoint, IProgress prog)
        {
            print("start new search");
            metadata = null;

            prog.Report(0);

            if (metadataCache.ContainsKey(searchLocation))
            {
                Searching = false;
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
                    metadataCache[metadata.pano_id] = metadata;
                    metadataCache[metadata.location.ToString()] = metadata;
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
                    await mainThread.StartNew(lastSphere.Deactivate);
                }

                await mainThread.StartNew(curSphere.Activate);

                if (lastSphere == null)
                {
                    await prog.WaitOn(curSphere, "Loading photosphere");
                    Complete();
                }

                await mainThread.StartNew(() =>
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

        private Task<PhotosphereJig> GetPhotosphere()
        {
            return mainThread.StartNew(() =>
            {
                var sphere = photospheres.GetPhotosphere<PhotosphereJig>(metadata.pano_id);
                if (!imageNeededSet.Contains(metadata.pano_id))
                {
                    imageNeededSet.Add(metadata.pano_id);
                    sphere.ImageNeeded += Photosphere_ImageNeeded;
                    sphere.Complete += Photosphere_Complete;
                }
                return sphere;
            });
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