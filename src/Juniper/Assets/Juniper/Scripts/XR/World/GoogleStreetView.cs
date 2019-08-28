using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Animation;
using Juniper.Data;
using Juniper.Google.Maps;
using Juniper.Google.Maps.Geocoding;
using Juniper.Google.Maps.StreetView;
using Juniper.Input;
using Juniper.Json;
using Juniper.Progress;
using Juniper.Security;
using Juniper.Serialization;
using Juniper.Units;
using Juniper.Unity;
using Juniper.Widgets;
using Juniper.World;
using Juniper.World.GIS;

using UnityEngine;
using UnityEngine.Events;

using Yarrow.Client;

namespace Juniper.Imaging
{
    public class GoogleStreetView : SubSceneController, ICredentialReceiver
    {
        private static readonly Regex GMAPS_URL_PANO_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@-?\\d+\\.\\d+,-?\\d+\\.\\d+(?:,[a-zA-Z0-9.]+)*/data=(?:![a-z0-9]+)*!1s([a-zA-Z0-9_\\-]+)(?:![a-z0-9]+)*", RegexOptions.Compiled);

        private static readonly Regex GMAPS_URL_LATLNG_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@(-?\\d+\\.\\d+,-?\\d+\\.\\d+)", RegexOptions.Compiled);

        private readonly Dictionary<string, MetadataResponse> metadataCache = new Dictionary<string, MetadataResponse>();

        public string yarrowServerHost = "http://localhost";

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
        private string lastSearchLocation = string.Empty;

        private MetadataResponse metadata;
        private Task dataSync;


        private Vector3 origin;
        private Vector3 navPointerPosition;

        private YarrowClient<Texture2D> yarrow;
        private FadeTransition fader;
        private GPSLocation gps;
        private PhotosphereManager photospheres;
        private Clickable navPlane;
        private Avatar avatar;
        private Transform navPointer;
        private UnifiedInputModule input;

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
            photospheres = ComponentExt.FindAny<PhotosphereManager>()
                ?? this.Ensure<PhotosphereManager>();
            navPlane = transform.Find("NavPlane").Ensure<Clickable>();
            avatar = ComponentExt.FindAny<Avatar>();
            navPointer = transform.Find("NavPointer");
            input = ComponentExt.FindAny<UnifiedInputModule>();
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
            var yarrowCacheDirName = Path.Combine(baseCachePath, "Yarrow");
            var yarrowCacheDir = new DirectoryInfo(yarrowCacheDirName);
            var gmapsCacheDirName = Path.Combine(baseCachePath, "GoogleMaps");
            var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
            var uri = new Uri(yarrowServerHost);
            var imageCodec = new UnityTextureCodec();
            var json = new JsonFactory();
            var metadataDecoder = json.Specialize<MetadataResponse>();
            var geocodingDecoder = json.Specialize<GeocodingResponse>();
            yarrow = new YarrowClient<Texture2D>(uri, yarrowCacheDir, imageCodec, metadataDecoder, geocodingDecoder, gmapsApiKey, gmapsSigningKey, gmapsCacheDir);

            photospheres.CubemapNeeded += Photospheres_CubemapNeeded;
            photospheres.ImageNeeded += Photospheres_ImageNeeded;
            photospheres.PhotosphereReady += Photospheres_PhotosphereReady;
            photospheres.codec = imageCodec;

            photospheres.SetDetailLevels(searchFOVs);

            navPlane.Click += NavPlane_Click;
            var renderer = navPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        private string Photospheres_CubemapNeeded(Photosphere source)
        {
            return StreamingAssets.FormatPath(Application.streamingAssetsPath, $"{source.name}.jpeg");
        }

        private Task<Stream> Photospheres_ImageNeeded(Photosphere source, int fov, int heading, int pitch)
        {
            return yarrow.GetImageStream((PanoID)source.name, fov, heading, pitch);
        }

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);
            SynchronizeData(true, Vector3.zero, prog);
        }

        public override void Update()
        {
            base.Update();
            if (IsEntered && IsComplete && !dataSync.IsRunning())
            {
                SynchronizeData(false, input.mouse.probe.Cursor.position);
            }

            navPointer.position = navPointerPosition;
        }

        private void SynchronizeData(bool firstLoad, Vector3 cursorPosition, IProgress prog = null)
        {
            dataSync = Task.Run((Func<Task>)(async () =>
            {
                PanoID? searchPano = null;
                var gmapsMatch = GMAPS_URL_PANO_PATTERN.Match(searchLocation);
                if (gmapsMatch.Success && PanoID.TryParse(gmapsMatch.Groups[1].Value, out var pano))
                {
                    searchPano = pano;
                }

                LatLngPoint? searchPoint = null;
                gmapsMatch = GMAPS_URL_LATLNG_PATTERN.Match(searchLocation);
                if (gmapsMatch.Success && LatLngPoint.TryParseDecimal(searchLocation, out var point))
                {
                    searchPoint = point;
                }

                var loadImmediately = true;
                if (!firstLoad && searchLocation == lastSearchLocation && this.metadata != null)
                {
                    var nextVec = cursorPosition + origin;
                    nextVec.y = 0;
                    var curUTM = LatLng.ToUTM(this.metadata.location);
                    var nextPoint = nextVec.ToUTM(curUTM.Zone, curUTM.Hemisphere).ToLatLng();
                    searchLocation = nextPoint.ToString();
                    loadImmediately = false;
                }

                if (!string.IsNullOrEmpty(searchLocation) && searchLocation != lastSearchLocation)
                {
                    var metadataProg = prog.Subdivide(0, 0.1f);
                    var subProg = prog.Subdivide(0.1f, 0.9f);
                    if (metadataCache.ContainsKey(searchLocation))
                    {
                        await SetMetadata(firstLoad, loadImmediately, metadataCache[searchLocation], subProg);
                    }
                    else
                    {
                        var metadata = await RequestMetadata(searchPano, metadataProg.Subdivide(0, 0.333f))
                            ?? await RequestMetadata(searchPoint, metadataProg.Subdivide(0.333f, 0.333f))
                            ?? await RequestMetadata(searchLocation, metadataProg.Subdivide(0.666f, 0.334f));
                        if (metadata != null)
                        {
                            await SetMetadata(firstLoad, loadImmediately, metadata, subProg);
                        }
                    }
                }
            })).ContinueWith(t => Debug.LogException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task<MetadataResponse> RequestMetadata(PanoID? searchPano, IProgress metadataProg)
        {
            if (searchPano == null)
            {
                return null;
            }
            else
            { 
                return ValidateMetadata(await yarrow.GetMetadata(searchPano.Value, searchRadius, metadataProg));
            }
        }

        private async Task<MetadataResponse> RequestMetadata(LatLngPoint? searchPoint, IProgress metadataProg)
        {
            if (searchPoint == null)
            {
                return null;
            }
            else 
            { 
                return ValidateMetadata(await yarrow.GetMetadata(searchPoint.Value, searchRadius, metadataProg));
            }
        }

        private async Task<MetadataResponse> RequestMetadata(string searchLocation, IProgress metadataProg)
        {
            if (string.IsNullOrEmpty(searchLocation))
            {
                return null;
            }
            else 
            {
                return ValidateMetadata(await yarrow.GetMetadata((PlaceName)searchLocation, searchRadius, metadataProg));
            }
        }

        private MetadataResponse ValidateMetadata(MetadataResponse metadata)
        {
            if(metadata?.status != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                return metadata;
            }
        }

        private async Task SetMetadata(bool firstLoad, bool loadImmediately, MetadataResponse metadata, IProgress subProg)
        {
            metadataCache[metadata.pano_id.ToString()] = metadata;
            metadataCache[metadata.location.ToString()] = metadata;
            metadataCache[searchLocation] = metadata;
            this.metadata = metadata;
            lastSearchLocation = searchLocation;
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

            var nextVec = metadata.location.ToVector3();
            if (firstLoad)
            {
                origin = nextVec;
            }
            else if (!loadImmediately)
            {
                navPointerPosition = nextVec - origin;
            }

            if (loadImmediately)
            {
                await LoadPhotosphere(firstLoad, subProg);
            }
        }

        private void NavPlane_Click(object sender, EventArgs e)
        {
            if (!dataSync.IsRunning())
            {
                dataSync = LoadPhotosphere(false);
            }
        }

        private async Task LoadPhotosphere(bool firstLoad, IProgress subProg = null)
        {
            if (metadata.pano_id != null)
            {
                if (!firstLoad)
                {
                    fader.Enter();
                    while (!fader.IsComplete)
                    {
                        await Task.Yield();
                    }
                }

                var photosphere = await photospheres.GetPhotosphere(metadata.pano_id.ToString());
                while (!photosphere.IsReady)
                {
                    subProg?.Report(photosphere.ProgressToReady, "Loading photosphere");
                    await Task.Yield();
                }

                if (firstLoad)
                {
                    Complete();
                }
                else
                {
                    fader.Exit();
                }
            }
        }

        private void Photospheres_PhotosphereReady(Photosphere obj)
        {
            var nextVec = metadata.location.ToVector3();
            var delta = nextVec - origin;
            transform.position = avatar.transform.position = delta;
            obj.transform.position = avatar.Head.position;
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
    }
}