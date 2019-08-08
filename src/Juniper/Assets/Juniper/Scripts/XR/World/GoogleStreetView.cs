using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Juniper.Animation;
using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.Imaging.JPEG;
using Juniper.Progress;
using Juniper.Units;
using Juniper.Unity;
using Juniper.Unity.Coroutines;
using Juniper.World;
using Juniper.World.GIS;

using UnityEngine;

using Yarrow.Client;

namespace Juniper.Imaging
{
    public class GoogleStreetView : SubSceneController
    {
        private const string LAT_LON = "_MAPPING_LATITUDE_LONGITUDE_LAYOUT";
        private const string SIDES_6 = "_MAPPING_6_FRAMES_LAYOUT";

        public TextureFormat textureFormat = TextureFormat.RGB24;
        public Color tint = Color.gray;

        [Range(0, 8)]
        public float exposure = 1;

        [Range(0, 360)]
        public float rotation;

        public bool useMipMap = true;

        private SkyboxManager skybox;

        private YarrowClient<ImageData> yarrow;

        public int searchRadius = 50;

        private string lastLocation;

        [ReadOnly]
        public string Location;

        private LatLngPoint LatLngLocation;

        [SerializeField]
        [HideInNormalInspector]
        private FadeTransition fader;

        [SerializeField]
        [HideInNormalInspector]
        private GPSLocation gps;

        private bool locked;

        private readonly Dictionary<PanoID, Texture> textureCache = new Dictionary<PanoID, Texture>();

#if UNITY_EDITOR

        private EditorTextInput locationInput;

        public void OnValidate()
        {
            locationInput = this.Ensure<EditorTextInput>();
            FindComponents();
        }

#endif

        private void FindComponents()
        {
            fader = ComponentExt.FindAny<FadeTransition>();
            gps = ComponentExt.FindAny<GPSLocation>();
            skybox = ComponentExt.FindAny<SkyboxManager>();
        }

        public override void Awake()
        {
            base.Awake();

            FindComponents();

#if UNITY_EDITOR
            locationInput = this.Ensure<EditorTextInput>();
            locationInput.OnSubmit.AddListener(SetLocation);
            if (!string.IsNullOrEmpty(locationInput.value))
            {
                SetLocation(locationInput.value);
            }
#endif

            yarrow = new YarrowClient<ImageData>(new JpegDecoder());
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

        public override void Update()
        {
            base.Update();

            if (IsEntered && !locked && Location != lastLocation)
            {
                GetImages(true);
            }
        }

        private Coroutine GetImages(bool fromNavigation, IProgress prog = null)
        {
            locked = true;
            lastLocation = Location;
            if (fromNavigation)
            {
                fader.Enter();
            }
            return StartCoroutine(GetImagesCoroutine(fromNavigation, prog));
        }

        private IEnumerator GetImagesCoroutine(bool fromNavigation, IProgress prog)
        {
            if (!string.IsNullOrEmpty(Location))
            {
                if (fader.IsRunning)
                {
                    yield return fader.Waiter;
                }

                var metadataProg = prog.Subdivide(0, 0.1f);
                var imageProg = prog.Subdivide(0.1f, 0.8f);
                var textureProg = prog.Subdivide(0.9f, 0.1f);
                Task<MetadataResponse> metadataTask;
                if (LatLngPoint.TryParseDecimal(Location, out var point))
                {
                    metadataTask = yarrow.GetMetadata(point, metadataProg);
                }
                else
                {
                    metadataTask = yarrow.GetMetadata((PlaceName)Location, metadataProg);
                }

                yield return new WaitForTask(metadataTask);
                var metadata = metadataTask.Result;

                if (metadata?.status != HttpStatusCode.OK)
                {
                    print("no metadata");
                }
                else
                {
                    print($"Pano ID = {metadata.pano_id}");
                    SetLatLngLocation(metadata.location);
                    lastLocation = Location;

                    if (!textureCache.ContainsKey(metadata.pano_id))
                    {
                        var imageTask = yarrow.GetImage(metadata.pano_id, imageProg);
                        yield return new WaitForTask(imageTask);
                        var image = imageTask.Result;

                        textureProg?.Report(0f);
                        var texture = new Texture2D(image.dimensions.width, image.dimensions.height, TextureFormat.RGB24, false);
                        if (image.format == ImageFormat.None)
                        {
                            texture.LoadRawTextureData(image.data);
                        }
                        else if (image.format != ImageFormat.Unsupported)
                        {
                            texture.LoadImage(image.data);
                        }
                        textureProg?.Report(0.3333f);
                        yield return null;
                        texture.Compress(true);
                        textureProg?.Report(0.66667f);
                        yield return null;
                        texture.Apply(false, true);
                        textureProg?.Report(1);

                        textureCache[metadata.pano_id] = texture;
                    }

                    skybox.layout = SkyboxManager.Mode.Cube;
                    skybox.imageType = SkyboxManager.ImageType.Degrees360;
                    skybox.stereoLayout = SkyboxManager.StereoLayout.None;
                    skybox.useMipMap = false;
                    skybox.SetTexture(textureCache[metadata.pano_id]);
                }

                locked = false;
                Complete();
                if (fromNavigation)
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