using System;
using System.Collections;
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
        private static readonly Regex GMAPS_URL_PATTERN = new Regex("https?://www\\.google\\.com/maps/@-?\\d+\\.\\d+,\\d+\\.\\d+(?:,[a-zA-Z0-9.]+)*/data=(?:![a-z0-9]+)*!1s([a-zA-Z0-9_\\-]+)(?:![a-z0-9]+)*", RegexOptions.Compiled);

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
        private PanoID lastPano = (PanoID)string.Empty;
        private PanoID curPano = (PanoID)string.Empty;
        private PanoID nextPano = (PanoID)string.Empty;
        public LatLngPoint curPoint;
        public LatLngPoint nextPoint;

        private bool locked;
        private bool firstLoad;

        private IImageCodec<Texture2D> imageCodec;
        private YarrowClient<Texture2D> yarrow;
        private FadeTransition fader;
        private GPSLocation gps;
        private PhotosphereManager photospheres;
        private Clickable navPlane;
        private UnifiedInputModule input;
        private Transform navPointer;

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
            input = ComponentExt.FindAny<UnifiedInputModule>();
            navPointer = transform.Find("NavPointer");
        }

        public override void Awake()
        {
            base.Awake();

            FindComponents();

#if UNITY_EDITOR
            this.ReceiveCredentials();

            locationInput = this.Ensure<EditorTextInput>();
            locationInput.OnSubmit.AddListener(new UnityAction<string>(LocationInput_SetLocation));
            if (!string.IsNullOrEmpty(locationInput.value))
            {
                LocationInput_SetLocation(locationInput.value);
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
            imageCodec = new UnityTextureCodec();
            var json = new JsonFactory();
            var metadataDecoder = json.Specialize<MetadataResponse>();
            var geocodingDecoder = json.Specialize<GeocodingResponse>();
            yarrow = new YarrowClient<Texture2D>(uri, yarrowCacheDir, imageCodec, metadataDecoder, geocodingDecoder, gmapsApiKey, gmapsSigningKey, gmapsCacheDir, true);

            photospheres.CubemapNeeded += Photospheres_CubemapNeeded;
            photospheres.ImageNeeded += Photospheres_ImageNeeded;
            photospheres.PhotosphereReady += Photospheres_PhotosphereReady;
            photospheres.PhotosphereComplete += Photospheres_PhotosphereComplete;
            photospheres.codec = imageCodec;

            photospheres.SetDetailLevels(searchFOVs);

            var renderer = navPlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            navPlane.Click += NavPlane_Click;
        }

        public void LocationInput_SetLocation(string location)
        {
            searchLocation = location;
        }

        private string Photospheres_CubemapNeeded(Photosphere source)
        {
            return StreamingAssets.FormatPath(Application.streamingAssetsPath, $"{source.name}.jpeg");
        }

        private Task<Stream> Photospheres_ImageNeeded(Photosphere source, int fov, int heading, int pitch)
        {
            return yarrow.GetImageStream((PanoID)source.name, fov, heading, pitch);
        }

        private void Photospheres_PhotosphereReady(Photosphere obj)
        {
            lastPano = curPano;
#if UNITY_EDITOR
            if (locationInput != null)
            {
                locationInput.value = searchLocation;
            }
#endif
            curPoint = nextPoint;
            SetLatLngLocation(curPoint);
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

        private void Photospheres_PhotosphereComplete(Photosphere obj)
        {
            lastPano = curPano = nextPano = (PanoID)obj.name;
        }

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);
            if (string.IsNullOrEmpty(searchLocation) && gps?.HasCoord == true)
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

        private void SynchronizeData(IProgress prog = null)
        {
            var gmapsMatch = GMAPS_URL_PATTERN.Match(searchLocation);
            if (gmapsMatch.Success)
            {
                searchLocation = gmapsMatch.Groups[1].Value;
            }
            if (!string.IsNullOrEmpty(searchLocation) && searchLocation != lastSearchLocation)
            {
                print("searching");
                locked = true;
                lastSearchLocation = searchLocation;
                StartCoroutine(GetMetadataCoroutine(prog));
            }
            else if (curPano == lastPano)
            {
                StartCoroutine(GetMetadataCoroutine(input.mouse.SmoothedWorldPoint));
            }
            else
            {
                var photosphere = photospheres[curPano.ToString()];
            }
        }

        private IEnumerator GetMetadataCoroutine(IProgress prog)
        {
            print("Getting metadata");
            var metadataProg = prog.Subdivide(0, 0.1f, "Getting metadata");
            var imageProg = prog.Subdivide(0.1f, 0.9f, "Loading image");
            if (metadataCache.ContainsKey(searchLocation))
            {
                yield return SetMetadataCoroutine(metadataCache[searchLocation], imageProg);
            }
            else if (PanoID.TryParse(searchLocation, out var pano))
            {
                yield return GetMetadataCoroutine(pano, metadataProg, imageProg);
            }
            else if (LatLngPoint.TryParseDecimal(searchLocation, out var point))
            {
                yield return GetMetadataCoroutine(point, metadataProg, imageProg);
            }
            else
            {
                yield return GetMetadataCoroutine((PlaceName)searchLocation, metadataProg, imageProg);
            }
        }

        private IEnumerator GetMetadataCoroutine(Vector3 point)
        {
            print("GetMetadata:Vector3");
            yield return GetMetadataCoroutine(Vector3ToLatLngPoint(point), null, null);
        }

        private IEnumerator GetMetadataCoroutine(PanoID pano, IProgress metadataProg, IProgress imageProg)
        {
            print("GetMetadata:PanoID");
            yield return ResolveMetadataCoroutine(yarrow.GetMetadata(pano, searchRadius, metadataProg), imageProg);
        }

        private IEnumerator GetMetadataCoroutine(LatLngPoint point, IProgress metadataProg, IProgress imageProg)
        {
            print("GetMetadata:LatLngPoint");
            yield return ResolveMetadataCoroutine(yarrow.GetMetadata(point, searchRadius, metadataProg), imageProg);
        }

        private IEnumerator GetMetadataCoroutine(PlaceName place, IProgress metadataProg, IProgress imageProg)
        {
            print("GetMetadata:PlaceName");
            yield return ResolveMetadataCoroutine(yarrow.GetMetadata(place, searchRadius, metadataProg), imageProg);
        }

        private IEnumerator ResolveMetadataCoroutine(Task<MetadataResponse> metadataTask, IProgress imageProg)
        {
            print("ResolveMetadata");
            yield return metadataTask.Waiter();

            if (metadataTask.IsSuccessful())
            {
                yield return SetMetadataCoroutine(metadataTask.Result, imageProg);
            }
            else
            {
                print("Failed to get metadata");
            }
        }

        private IEnumerator SetMetadataCoroutine(MetadataResponse metadata, IProgress imageProg)
        {
            print("metadata found " + metadata.pano_id);
            if (metadata.status == HttpStatusCode.OK)
            {
                nextPano = metadata.pano_id;
                nextPoint = metadata.location;
                navPointer.position = LatLngPointToVector3(nextPoint);

                if (firstLoad)
                {
                    print("Loading next pano");
                    curPano = nextPano;
                    curPoint = nextPoint;
                    var photosphere = photospheres[curPano.ToString()];
                    yield return new WaitUntil(() =>
                    {
                        imageProg?.Report(photosphere.ProgressToReady, "Loading photosphere");
                        return lastPano == curPano;
                    });
                }
                else
                {
                    print("Not first load");
                }
            }
            locked = false;
        }

        private void NavPlane_Click(object sender, EventArgs e)
        {
            StartCoroutine(LoadNewPanoCoroutine());
        }

        private IEnumerator LoadNewPanoCoroutine()
        {
            fader.Enter();
            yield return fader.Waiter;
            curPano = nextPano;
        }

        public void SetLatLngLocation(LatLngPoint location)
        {
            if (gps != null)
            {
                gps.FakeCoord = true;
                gps.Coord = location;
            }
        }

        private LatLngPoint Vector3ToLatLngPoint(Vector3 vec)
        {
            var delta = vec - transform.position;
            var curUtm = curPoint.ToUTM();
            var curVec = curUtm.ToVector3();
            var nextVec = curVec + delta;
            var nextUtm = nextVec.ToUTM(curUtm.Zone, curUtm.Hemisphere);
            var nextPoint = nextUtm.ToLatLng();
            return nextPoint;
        }

        private Vector3 LatLngPointToVector3(LatLngPoint nextPoint)
        {
            var curUtm = curPoint.ToUTM();
            var curVec = curUtm.ToVector3();
            var nextUtm = nextPoint.ToUTM();
            var nextVec = nextUtm.ToVector3();
            var delta = nextVec - curVec;
            var vec = transform.position + delta;
            return vec;
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            Complete();
        }
    }
}