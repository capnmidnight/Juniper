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
        private static readonly Regex GMAPS_URL_PATTERN =
            new Regex("https?://www\\.google\\.com/maps/@-?\\d+\\.\\d+,\\d+\\.\\d+(?:,[a-zA-Z0-9.]+)*/data=(?:![a-z0-9]+)*!1s([a-zA-Z0-9_\\-]+)(?:![a-z0-9]+)*", RegexOptions.Compiled);

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

        private MetadataResponse nextMetadata;
        private MetadataResponse curMetadata;

        private PanoID? nextPano { get { return nextMetadata?.pano_id; } }
        private PanoID? curPano { get { return curMetadata?.pano_id; } }

        private LatLngPoint? nextPoint { get { return nextMetadata?.location; } }
        private LatLngPoint? curPoint { get { return curMetadata?.location; } }


        private Vector3 origin;

        private bool locked;

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
            SynchronizeData(true, prog);
        }

        public override void Update()
        {
            base.Update();
            if (IsEntered && IsComplete && !locked)
            {
                SynchronizeData(false);
            }
        }

        private void SynchronizeData(bool firstLoad, IProgress prog = null)
        {
            locked = true;
            var gmapsMatch = GMAPS_URL_PATTERN.Match(searchLocation);
            if (gmapsMatch.Success)
            {
                searchLocation = gmapsMatch.Groups[1].Value;
            }

            if (!firstLoad)
            {
                var nextVec = input.mouse.probe.Cursor.position + origin;
                nextVec.y = 0;
                var curUTM = curPoint.Value.ToUTM();
                var nextPoint = nextVec.ToUTM(curUTM.Zone, curUTM.Hemisphere).ToLatLng();
                searchLocation = nextPoint.ToString();
            }

            if (!string.IsNullOrEmpty(searchLocation) && searchLocation != lastSearchLocation)
            {
                lastSearchLocation = searchLocation;
                StartCoroutine(SynchronizeDataCoroutine(firstLoad, prog));
            }
            else
            {
                locked = false;
            }
        }

        private IEnumerator SynchronizeDataCoroutine(bool firstLoad, IProgress prog)
        {
            var metadataProg = prog.Subdivide(0, 0.1f);
            var subProg = prog.Subdivide(0.1f, 0.9f);
            if (metadataCache.ContainsKey(searchLocation))
            {
                yield return SetMetadata(firstLoad, metadataCache[searchLocation], subProg);
            }
            else
            {
                Task<MetadataResponse> metadataTask;
                if (PanoID.TryParse(searchLocation, out var pano))
                {
                    metadataTask = yarrow.GetMetadata(pano, searchRadius, metadataProg);
                }
                else if (LatLngPoint.TryParseDecimal(searchLocation, out var point))
                {
                    metadataTask = yarrow.GetMetadata(point, searchRadius, metadataProg);
                }
                else
                {
                    metadataTask = yarrow.GetMetadata((PlaceName)searchLocation, searchRadius, metadataProg);
                }

                yield return metadataTask.Waiter();

                if (metadataTask.IsSuccessful())
                {
                    var metadata = metadataTask.Result;
                    if (metadata.status == HttpStatusCode.OK && metadata.pano_id != curPano)
                    {
                        yield return SetMetadata(firstLoad, metadata, subProg);
                    }
                }
            }
            locked = false;
        }

        private IEnumerator SetMetadata(bool firstLoad, MetadataResponse metadata, IProgress subProg)
        {
            metadataCache[metadata.pano_id.ToString()] = metadata;
            metadataCache[metadata.location.ToString()] = metadata;
            metadataCache[searchLocation] = metadata;
            nextMetadata = metadata;
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

            var nextVec = nextPoint.Value.ToVector3();
            if (firstLoad)
            {
                origin = nextVec;
                yield return LoadPhotosphere(firstLoad, subProg);
            }
            else if (nextPoint != null)
            {
                navPointer.position = nextVec - origin;
            }
        }

        private void NavPlane_Click(object sender, EventArgs e)
        {
            StartCoroutine(LoadPhotosphere(false));
        }

        private IEnumerator LoadPhotosphere(bool firstLoad, IProgress subProg = null)
        {
            if (nextPano != null)
            {
                if (!firstLoad)
                {
                    fader.Enter();
                    yield return fader.Waiter;
                }

                var photosphere = photospheres.GetPhotosphere(nextPano.Value.ToString());
                while (!photosphere.IsReady)
                {
                    subProg?.Report(photosphere.ProgressToReady, "Loading photosphere");
                    yield return null;
                }

                var nextVec = nextPoint.Value.ToVector3();
                var delta = nextVec - origin;
                transform.position = avatar.transform.position = delta;
                photosphere.transform.position = avatar.Head.position;
                curMetadata = nextMetadata;
                nextMetadata = null;

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