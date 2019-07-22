using System;
using System.Collections;
using System.IO;
using System.Net;

using Juniper.Animation;
using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.Image;
using Juniper.Imaging;
using Juniper.Units;
using Juniper.Unity;
using Juniper.Unity.Coroutines;
using Juniper.World.GIS;

using UnityEngine;

namespace Juniper.Images
{
    public class GoogleStreetView : MonoBehaviour
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

        private Mode lastLayout = Mode.None;
        public Mode layout = Mode.Layout6Frames;

        private Material skyboxMaterial;

        private RawImage[] images;

        private Endpoint gmaps;

        public int searchRadius = 50;

        private string lastLocation;
        public string Location { get; set; }

        [ReadOnly]
        public LatLngPoint GPS;

        public enum Mode
        {
            None,
            IndividualImages,
            Layout6Frames,
            LayoutCross
        }

#if UNITY_EDITOR

        private EditorTextInput locationInput;
        private FadeTransition fader;

        public void OnValidate()
        {
            locationInput = this.Ensure<EditorTextInput>();
        }
#endif

        public void Awake()
        {
#if UNITY_EDITOR
            locationInput = this.Ensure<EditorTextInput>();
            locationInput.OnSubmit.AddListener(SetLocation);
            Location = locationInput.value;
#endif

            fader = ComponentExt.FindAny<FadeTransition>();

            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFile = Path.Combine(cacheDirName, "keys.txt");
            var lines = File.ReadAllLines(keyFile);
            var apiKey = lines[0];
            var signingKey = lines[1];
            var json = new Json.JsonFactory();
            gmaps = new API(json, apiKey, signingKey, cacheDir);
        }

        public void Update()
        {
            if (Location != lastLocation
                && (skyboxMaterial != null || images == null))
            {
                GetImages();
            }
            else if (Location == lastLocation
                && layout != lastLayout
                && images != null)
            {
                CreateSkyBox();
            }
            else if (Location == lastLocation
                && layout == lastLayout
                && skyboxMaterial != null)
            {
                UpdateSkyBox();
            }
        }

        public void SetLocation(string location)
        {
            Location = location;
        }

        private void UpdateSkyBox()
        {
            skyboxMaterial.SetColor("_Tint", tint);
            skyboxMaterial.SetFloat("_Exposure", exposure);
            skyboxMaterial.SetFloat("_Rotation", rotation);
        }

        private void GetImages()
        {
            fader.Enter();
            images = null;
            lastLayout = Mode.None;
            lastLocation = Location;
            StartCoroutine(GetImagesCoroutine());
        }

        private IEnumerator GetImagesCoroutine()
        {
            if (!string.IsNullOrEmpty(Location))
            {
                var metadataSearch = new MetadataSearch((PlaceName)Location);
                var metadataTask = gmaps.Get(metadataSearch);
                yield return new WaitForTask(metadataTask);
                var metadata = metadataTask.Result;
                if (metadata.status == HttpStatusCode.OK)
                {
                    GPS = metadata.location;
                    var imageSearch = new CubeMapSearch(GPS, 1024, 1024)
                    {
                        FlipImages = true
                    };
                    imageSearch.SetRadius(searchRadius);
                    var imageTask = gmaps.Get(imageSearch);
                    yield return new WaitForTask(imageTask);
                    images = imageTask.Result;
                }
                else
                {
                    Debug.LogError(metadata.error_message);
                }
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

        public void Move(Vector2 deltaMeters)
        {
            if (GPS != null)
            {
                deltaMeters /= 10f;
                var utm = GPS.ToUTM();
                utm = new UTMPoint(utm.X + deltaMeters.x, utm.Y + deltaMeters.y, utm.Z, utm.Zone, utm.Hemisphere);
                GPS = utm.ToLatLng();
                Location = $"{GPS.Latitude},{GPS.Longitude}";
#if UNITY_EDITOR
                locationInput.value = Location;
#endif
            }
        }

        private void CreateSkyBox()
        {
            RenderSettings.skybox = null;

            DestroyImmediate(skyboxMaterial);
            skyboxMaterial = null;

            lastLayout = layout;
            StartCoroutine(CreateSkyboxCoroutine());
        }

        private IEnumerator CreateSkyboxCoroutine()
        {
            if (layout != Mode.None)
            {
                if (layout == Mode.IndividualImages)
                {
                    var textures = new Texture[images.Length];
                    for (int i = 0; i < images.Length; ++i)
                    {
                        textures[i] = ImageLoader.ConstructTexture2D(images[i], textureFormat);
                        yield return null;
                    }
                    skyboxMaterial = new Material(Shader.Find("Skybox/6 Sided"));
                    skyboxMaterial.SetTexture("_FrontTex", textures[0]);
                    skyboxMaterial.SetTexture("_LeftTex", textures[1]);
                    skyboxMaterial.SetTexture("_RightTex", textures[2]);
                    skyboxMaterial.SetTexture("_BackTex", textures[3]);
                    skyboxMaterial.SetTexture("_UpTex", textures[4]);
                    skyboxMaterial.SetTexture("_DownTex", textures[5]);
                    RenderSettings.skybox = skyboxMaterial;
                    fader.Exit();
                }
                else if (layout == Mode.LayoutCross)
                {
                    var texture = ImageLoader.ConstructCubemap(images, textureFormat);
                    skyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
                    skyboxMaterial.SetTexture("_Tex", texture);
                    RenderSettings.skybox = skyboxMaterial;
                    fader.Exit();
                }
                else if (layout == Mode.Layout6Frames)
                {
                    var combinedTask = RawImage.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
                    yield return ImageLoader.ConstructTexture2D(combinedTask, textureFormat, texture =>
                    {
                        skyboxMaterial = new Material(Shader.Find("Skybox/Panoramic"));

                        if (skyboxMaterial.IsKeywordEnabled(LAT_LON))
                        {
                            skyboxMaterial.DisableKeyword(LAT_LON);
                        }
                        skyboxMaterial.EnableKeyword(SIDES_6);

                        skyboxMaterial.SetInt("_Mapping", 0);
                        skyboxMaterial.SetInt("_ImageType", 0);
                        skyboxMaterial.SetInt("_MirrorOnBack", 0);
                        skyboxMaterial.SetInt("_Layout", 0);
                        skyboxMaterial.SetTexture("_MainTex", texture);
                        RenderSettings.skybox = skyboxMaterial;
                        fader.Exit();
                    }, Debug.LogError);
                }
            }
        }
    }
}