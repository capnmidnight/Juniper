using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Display;
using Juniper.Imaging;
using Juniper.Input;
using Juniper.IO;
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
    public abstract class AbstractGoogleStreetView<MetadataTypeT> : SubSceneController, ICredentialReceiver
        where MetadataTypeT : MetadataResponse
    {
        private static readonly Regex GMAPS_URL_PANO_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@-?\\d+\\.\\d+,-?\\d+\\.\\d+(?:,[a-zA-Z0-9.]+)*/data=(?:![a-z0-9]+)*!1s([a-zA-Z0-9_\\-]+)(?:![a-z0-9]+)*", RegexOptions.Compiled);

        private static readonly Regex GMAPS_URL_LATLNG_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@(-?\\d+\\.\\d+,-?\\d+\\.\\d+)", RegexOptions.Compiled);

        private readonly Dictionary<string, Transform> navPointers = new Dictionary<string, Transform>();

        [SerializeField]
        [HideInInspector]
        private string gmapsApiKey;

        [SerializeField]
        [HideInInspector]
        private string gmapsSigningKey;

        protected abstract string CachePrefix { get; }

        public int searchRadius = 50;

        public float[] searchFOVs =
        {
            90,
            60,
            30
        };

        [ReadOnly]
        public string searchLocation;



        private LatLngPoint origin;
        private Vector3 cursorPosition;
        private Vector3 lastCursorPosition;
        private string navPointerPano;

        private MetadataTypeT metadata;
        private GoogleMapsClient<MetadataTypeT> gmaps;
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
        private LoadingBar loadingBar;

        private Task captureTask;

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
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var assetsRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
                var keyFile = Path.Combine(assetsRoot, "DevKeys", "google-streetview.txt");
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

        public bool IsBusy
        {
            get
            {
                return searchTask.IsRunning()
                    || captureTask.IsRunning()
                    || lastSphere != null
                        && lastSphere.IsBusy;
            }
        }

        public override void Awake()
        {
            base.Awake();

            processor = new UnityTexture2DProcessor();

            Find.Any(out loadingBar);
            Find.Any(out gps);
            if (!this.FindClosest(out photospheres))
            {
                photospheres = this.Ensure<PhotosphereManager>();
            }

            Find.Any(out avatar);
            navPlane = avatar.GroundPlane.Ensure<Clickable>();
            navPlane.Activate();
            navPointer = transform.Find("NavPointer");
            if (navPointer != null)
            {
                navPointer.Deactivate();
            }

            Find.Any(out input);

            cache = new CachingStrategy();

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
            
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var assetsRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
            var oldCubemapPath = Path.Combine(assetsRoot, "GoogleMaps");
            var oldGmapsPath = Path.Combine(oldCubemapPath, "streetview", "maps", "api");

            cache.AddBackup(new FileCacheLayer(oldCubemapPath));
            cache.AddBackup(new FileCacheLayer(oldGmapsPath));
#else
            if (gps != null && gps.HasCoord)
            {
                SetLocation(gps.Coord.ToString());
            }
#endif

            var newGmapsPath = Path.Combine(CachePrefix, "Google", "StreetView");
            cache.Add(new StreamingAssetsCacheLayer(newGmapsPath));
            codec = new UnityTexture2DCodec(MediaType.Image.Jpeg);

            var metadataDecoder = new JsonFactory<MetadataTypeT>();
            var geocodingDecoder = new JsonFactory<GeocodingResponse>();

            gmaps = new GoogleMapsClient<MetadataTypeT>(gmapsApiKey, gmapsSigningKey, metadataDecoder, geocodingDecoder, cache);

            photospheres.CubemapNeeded += Photosphere_CubemapNeeded;

            photospheres.SetIO(cache, codec);
            photospheres.SetDetailLevels(searchFOVs);
        }

        public void OnEnable()
        {
            navPlane.Click += NavPlane_Click;
        }

        public void OnDisable()
        {
            navPlane.Click -= NavPlane_Click;
        }

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
                var fileRef = photosphere.name + codec.ContentType;
                if (cache.IsCached(fileRef))
                {
                    Debug.Log("Cubemap already cached");
                }
                else
                {
                    loadingBar.Activate();

                    const int dim = 2048;
                    Cubemap cubemap = null;
                    Texture2D img = null;

                    await loadingBar.RunAsync(
                        ("Rendering cubemap", async (prog) =>
                        {
                            cubemap = await JuniperSystem.OnMainThreadAsync(() =>
                            {
                                prog.Report(0);
                                var cb = new Cubemap(dim, TextureFormat.RGB24, false);
                                cb.Apply();

                                var curMask = DisplayManager.MainCamera.cullingMask;
                                DisplayManager.MainCamera.cullingMask = LayerMask.GetMask(Photosphere.PHOTOSPHERE_LAYER_ARR);

                                var curRotation = DisplayManager.MainCamera.transform.rotation;
                                DisplayManager.MainCamera.transform.rotation = Quaternion.identity;

                                DisplayManager.MainCamera.RenderToCubemap(cb, 63);

                                DisplayManager.MainCamera.cullingMask = curMask;
                                DisplayManager.MainCamera.transform.rotation = curRotation;
                                prog.Report(1);

                                return cb;
                            });
                        }
                    ),
                        ("Copying cubemap faces", async (prog) =>
                        {
                            for (var f = 0; f < CAPTURE_CUBEMAP_FACES.Length; ++f)
                            {
                                await JuniperSystem.OnMainThreadAsync(() =>
                                {
                                    prog.Report(f, CAPTURE_CUBEMAP_FACES.Length, CAPTURE_CUBEMAP_FACES[f].ToString());
                                    var pixels = cubemap.GetPixels(CAPTURE_CUBEMAP_FACES[f]);
                                    var texture = new Texture2D(cubemap.width, cubemap.height);
                                    texture.SetPixels(pixels);
                                    texture.Apply();
                                    CAPTURE_CUBEMAP_SUB_IMAGES[f] = texture;
                                    prog.Report(f + 1, CAPTURE_CUBEMAP_FACES.Length);
                                });
                            }
                        }
                    ),
                        ("Concatenating faces", async (prog) =>
                        {
                            img = await JuniperSystem.OnMainThreadAsync(() =>
                                processor.Concatenate(ImageData.CubeCross(CAPTURE_CUBEMAP_SUB_IMAGES), prog));
                        }
                    ),
                        ("Saving image", (prog) =>
                            JuniperSystem.OnMainThreadAsync(() => cache.Save(codec, fileRef, img))));

                    loadingBar.Deactivate();
                }

                await JuniperSystem.OnMainThreadAsync(photosphere.DestroyJig);

                var anyDestroyed = await JuniperSystem.OnMainThreadAsync(() =>
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
            loadingBar.Deactivate();
            jig.ImageNeeded -= Photosphere_ImageNeeded;
            jig.Complete -= Photosphere_Complete;

            if (captureCubemap)
            {
                captureTask = CaptureCubemap(jig);
            }
        }

        private Texture2D Photosphere_CubemapNeeded(Photosphere source)
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
                using (var stream = cache.OpenAsync(cubemapRef).Result)
                {
                    return Decode(stream);
                }
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
                return Decode(await gmaps.GetImageAsync(source.CubemapName, fov, heading, pitch));
            }
        }

        private Texture2D Decode(Stream imageStream)
        {
            return codec.Deserialize(imageStream);
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
            if (origin != null)
            {
                foreach (var metadata in gmaps.CachedMetadata)
                {
                    if (!navPointers.ContainsKey(metadata.Pano_ID))
                    {
                        var position = GetRelativeVector3(metadata);
                        if (position.magnitude < 1000)
                        {
                            var newPointer = Instantiate(navPointer);
                            newPointer.parent = navPointer.parent;
                            newPointer.position = position;
                            newPointer.name = "jump-to-" + metadata.Pano_ID;
                            newPointer.Activate();

                            navPointers[metadata.Pano_ID] = newPointer;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(navPointerPano))
            {
                foreach (var pointer in navPointers)
                {
                    var renderer = pointer.Value.GetComponentInChildren<MeshRenderer>();
                    var material = renderer.GetMaterial();
                    var color = Color.red;
                    if (pointer.Key == navPointerPano)
                    {
                        color = Color.green;
                    }
                    else if (gmaps.IsImageCached(pointer.Key))
                    {
                        color = Color.blue;
                    }

                    material.SetColor("_Color", color);
                }
            }

            if (IsEntered && IsComplete)
            {
                if (!IsBusy)
                {
                    SyncData(null);
                }
                else if (lastSphere != null)
                {
                    loadingBar.Report(lastSphere.ProgressToComplete);
                }
            }
        }

        private void SyncData(IProgress prog)
        {
            var isNewSearch = ParseSearchParams(searchLocation, out var searchPano, out var searchPoint);
            if (isNewSearch)
            {
                searchTask = SearchData(searchPano, searchPoint, prog)
                    .ContinueWith(JuniperSystem.LogError, TaskContinuationOptions.OnlyOnFaulted);
            }
            else if (origin != null
                && metadata != null
                && input.ActiveController != null)
            {
                cursorPosition = input.ActiveController.CursorPosition;
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
            var point = GetRelativeLatLng(cursorPosition);
            var closestMetadata = await gmaps.FindClosestMetadataAsync(point, searchRadius)
                .ConfigureAwait(false);
            if (closestMetadata != null)
            {
                navPointerPano = closestMetadata.Pano_ID;
            }
        }

        private async Task SearchData(string searchPano, LatLngPoint searchPoint, IProgress prog)
        {
            prog.Report(0);

            metadata = await gmaps.SearchMetadataAsync(searchLocation, searchPano, searchPoint, searchRadius, prog)
                .ConfigureAwait(false);

            if (metadata != null)
            {
                searchPoint = metadata.Location;
                searchLocation = searchPano = metadata.Pano_ID;

                if (lastSphere == null)
                {
                    origin = metadata.Location;
                }

                if (gps != null)
                {
                    gps.FakeCoord = true;
                    gps.Coord = metadata.Location;
                }

#if UNITY_EDITOR
                if (locationInput != null)
                {
                    locationInput.value = searchLocation;
                }
#endif

                loadingBar.Activate();
                var curSphere = await GetPhotosphere();

                if (lastSphere != null)
                {
                    await JuniperSystem.OnMainThreadAsync(lastSphere.Deactivate);
                }

                await JuniperSystem.OnMainThreadAsync(curSphere.Activate);

                if (lastSphere == null)
                {
                    await prog.WaitOnAsync(curSphere, "Loading photosphere");
                    Complete();
                }

                await JuniperSystem.OnMainThreadAsync(() =>
                {
                    avatar.transform.position = GetRelativeVector3(metadata);
                    curSphere.transform.position = avatar.Head.position;
                });

                prog.Report(1);

                lastSphere = curSphere;
            }
        }

        private readonly HashSet<string> imageNeededSet = new HashSet<string>();

        private async Task<PhotosphereJig> GetPhotosphere()
        {
            var jig = await JuniperSystem.OnMainThreadAsync(() =>
                photospheres.GetPhotosphere<PhotosphereJig>(metadata.Pano_ID));
            if (!imageNeededSet.Contains(metadata.Pano_ID))
            {
                imageNeededSet.Add(metadata.Pano_ID);
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

                if (latLngMatch.Success && LatLngPoint.TryParse(latLngMatch.Groups[1].Value, out var point))
                {
                    searchPoint = point;
                }
                else if (LatLngPoint.TryParse(searchLocation, out var point2))
                {
                    searchPoint = point2;
                }
            }

            return metadata == null
                || searchPano != null
                    && metadata.Pano_ID != searchPano
                || searchPoint != null
                    && searchPoint.Distance(metadata.Location) > 3f;
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

        private Vector3 GetRelativeVector3(MetadataTypeT metadata)
        {
            var nextUTM = metadata.Location.ToUTM();
            var nextVec = nextUTM.ToVector3();
            var start = origin.ToVector3();
            var delta = nextVec - start;
            return delta;
        }
    }
}