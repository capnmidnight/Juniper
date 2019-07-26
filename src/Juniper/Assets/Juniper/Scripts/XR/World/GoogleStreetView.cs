using System;
using System.Collections;
using System.IO;
using System.Net;

using Juniper.Animation;
using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.Imaging;
using Juniper.Progress;
using Juniper.Units;
using Juniper.Unity;
using Juniper.Unity.Coroutines;
using Juniper.World;
using Juniper.World.GIS;

using UnityEngine;

namespace Juniper.Images
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

        private Material skyboxMaterial;
        private Material newMaterial;

        private Endpoint gmaps;

        public int searchRadius = 50;

        private string lastLocation;

        public string Location;

        [ReadOnly]
        public LatLngPoint LatLngLocation;

        [SerializeField]
        [HideInNormalInspector]
        private FadeTransition fader;

        [SerializeField]
        [HideInNormalInspector]
        private GPSLocation gps;

        public enum Mode
        {
            None,
            IndividualImages,
            Layout6Frames,
            LayoutCross
        }

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
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFile = Path.Combine(cacheDirName, "keys.txt");
            var lines = File.ReadAllLines(keyFile);
            var apiKey = lines[0];
            var signingKey = lines[1];
            var json = new Json.JsonFactory();
            gmaps = new Endpoint(json, apiKey, signingKey, cacheDir);
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

            if (IsEntered)
            {
                if (Location != lastLocation)
                {
                    print("location changed");
                    GetImages(true);
                }
                else if (Location == lastLocation
                    && skyboxMaterial != null)
                {
                    UpdateSkyBox();
                }
            }
        }

        private Coroutine GetImages(bool fromNavigation, IProgress prog = null)
        {
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
                MetadataRequest metadataRequest;

                if (LatLngPoint.TryParseDecimal(Location, out var point))
                {
                    metadataRequest = new MetadataRequest(point);
                }
                else
                {
                    metadataRequest = new MetadataRequest((PlaceName)Location);
                }

                if (metadataRequest != null)
                {
                    var metadataTask = gmaps.Get(metadataRequest);
                    yield return new WaitForTask(metadataTask);
                    var metadata = metadataTask.Result;
                    if (metadata.status != HttpStatusCode.OK)
                    {
                        print("no metadata");
                    }
                    else
                    {
                        SetLatLngLocation(metadata.location);
                        lastLocation = Location;
                        newMaterial = null;

                        if (true)
                        {
                            yield return Build6SidedMaterial();
                        }
                        else
                        {
                            yield return BuildCubemapMaterial();
                        }

                        if (newMaterial != null)
                        {
                            RenderSettings.skybox = newMaterial;
                            DestroyImmediate(skyboxMaterial);
                            skyboxMaterial = newMaterial;
                        }

                        Complete();
                        if (fromNavigation)
                        {
                            fader.Exit();
                        }
                    }
                }
            }
        }

        private static readonly string[] TEXTURE_NAMES =
        {
            "_FrontTex",
            "_LeftTex",
            "_RightTex",
            "_BackTex",
            "_UpTex",
            "_DownTex"
        };

        private IEnumerator Build6SidedMaterial()
        {
            var imageRequest = new CubeMapRequest(LatLngLocation, 1024, 1024)
            {
                FlipImage = true,
                Radius = searchRadius
            };

            var imageTask = gmaps.Get(imageRequest);
            yield return new WaitForTask(imageTask);
            var images = imageTask.Result;

            newMaterial = new Material(Shader.Find("Skybox/6 Sided"));

            for (int i = 0; i < images.Length; ++i)
            {
                var texture = ImageLoader.ConstructTexture2D(images[i], textureFormat);
                yield return null;
                newMaterial.SetTexture(TEXTURE_NAMES[i], texture);
                yield return null;
            }
        }

        private IEnumerator BuildCubemapMaterial()
        {
            var imageRequest = new CubeMapRequest(LatLngLocation, 1024, 1024);

            var imageTask = gmaps.Get(imageRequest);
            yield return new WaitForTask(imageTask);
            var images = imageTask.Result;

            var texture = ImageLoader.ConstructCubemap(images, TextureFormat.RGB24);

            newMaterial = new Material(Shader.Find("Skybox/Cubemap"));
            newMaterial.SetTexture("_Tex", texture);
        }

        private IEnumerator BuildPanoramicMaterial()
        {
            yield return null;
            //var imageRequest = new PanoramicCubeMapRequest(LatLngLocation, 1024, 1024, Image.ImageFormat.JPEG);
            //var imageTask = gmaps.Get(imageRequest);
            //yield return new WaitForTask(imageTask);

            //skyboxMaterial = new Material(Shader.Find("Skybox/Panoramic"));

            //if (skyboxMaterial.IsKeywordEnabled(LAT_LON))
            //{
            //    skyboxMaterial.DisableKeyword(LAT_LON);
            //}
            //skyboxMaterial.EnableKeyword(SIDES_6);

            //skyboxMaterial.SetInt("_Mapping", 0);
            //skyboxMaterial.SetInt("_ImageType", 0);
            //skyboxMaterial.SetInt("_MirrorOnBack", 0);
            //skyboxMaterial.SetInt("_Layout", 0);
            //skyboxMaterial.SetTexture("_MainTex", texture);
        }

        private void UpdateSkyBox()
        {
            skyboxMaterial.SetColor("_Tint", tint);
            skyboxMaterial.SetFloat("_Exposure", exposure);
            skyboxMaterial.SetFloat("_Rotation", rotation);
        }

        public void SetLatLngLocation(LatLngPoint location)
        {
            LatLngLocation = location;
            Location = LatLngLocation.ToCSV();

            if (gps != null)
            {
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