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

using UnityEngine;
using UnityEngine.Events;

using Yarrow.Client;

namespace Juniper.Imaging
{
    public class GoogleStreetView : SubSceneController, ICredentialReceiver
    {
        private static readonly float[] FOVs =
        {
            90,
            60,
            45,
            30
        };

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

        private bool locked;
        private bool firstLoad;

        private IImageDecoder<ImageData> encoder;
        private YarrowClient<ImageData> yarrow;
        private FadeTransition fader;
        private GPSLocation gps;
        private PhotosphereManager photospheres;

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
            photospheres = ComponentExt.FindAny<PhotosphereManager>()
                ?? this.Ensure<PhotosphereManager>();
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

            photospheres.CubemapNeeded += Photospheres_CubemapNeeded;
            photospheres.ImageNeeded += Photospheres_ImageNeeded;
            photospheres.PhotosphereReady += Photospheres_PhotosphereReady;
            photospheres.PhotosphereComplete += Photospheres_PhotosphereComplete;

            photospheres.SetDetailLevels(FOVs);
        }

        private async Task<ImageData> Photospheres_CubemapNeeded(Photosphere source)
        {
            var path = StreamingAssets.FormatPath(Application.streamingAssetsPath, $"{source.name}.jpeg");
            var response = await StreamingAssets.GetStream(Application.persistentDataPath, path);
            if (response?.Content != null)
            {
                using (var stream = response.Content)
                {
                    return encoder.Read(stream);
                }
            }
            else
            {
                return null;
            }
        }

        private void Photospheres_PhotosphereComplete(Photosphere obj)
        {
            curPano = (PanoID)obj.name;
        }

        private void Photospheres_PhotosphereReady(Photosphere obj)
        {
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

        private void SynchronizeData(IProgress prog = null)
        {
            var gmapsMatch = GMAPS_URL_PATTERN.Match(inputSearchLocation);
            if (gmapsMatch.Success)
            {
                inputSearchLocation = gmapsMatch.Groups[1].Value;
            }
            if (!string.IsNullOrEmpty(inputSearchLocation) && inputSearchLocation != lastSearchLocation)
            {
                locked = true;
                StartCoroutine(SynchronizeDataCoroutine(prog));
            }
        }

        private IEnumerator SynchronizeDataCoroutine(IProgress prog)
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
                var photosphere = photospheres[nextPano.ToString()];
                subProg?.Report(photosphere.ProgressToReady);
            }

            locked = false;
        }

        private Task<ImageData> Photospheres_ImageNeeded(Photosphere source, int fov, int heading, int pitch)
        {
            return yarrow.GetImage((PanoID)source.name, fov, heading, pitch);
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