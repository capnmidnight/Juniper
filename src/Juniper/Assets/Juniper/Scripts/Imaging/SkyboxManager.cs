using System.Collections;

using UnityEngine;

namespace Juniper.Imaging
{
    public class SkyboxManager : MonoBehaviour
    {
        private const string LAT_LON = "_MAPPING_LATITUDE_LONGITUDE_LAYOUT";
        private const string SIDES_6 = "_MAPPING_6_FRAMES_LAYOUT";

        public enum Mode
        {
            Cube,
            Spherical
        }

        public enum ImageType
        {
            Degrees360,
            Degrees180
        }

        public enum StereoLayout
        {
            None,
            SideBySide,
            OverUnder
        }

        public Color tint = Color.gray;

        [Range(0, 8)]
        public float exposure = 1;

        [Range(0, 360)]
        public float rotation;

        public bool useMipMap;
        public Mode layout = Mode.Spherical;
        public ImageType imageType;
        public bool mirror180OnBack = true;
        public StereoLayout stereoLayout;

        private Material skyboxMaterial;
        private Texture skyboxTexture;

        public void Awake()
        {
            skyboxMaterial = new Material(Shader.Find("Skybox/Panoramic"));
        }

        public void Update()
        {
            if (skyboxMaterial != null)
            {
                skyboxMaterial.SetColor("_Tint", tint);
                skyboxMaterial.SetFloat("_Exposure", exposure);
                skyboxMaterial.SetFloat("_Rotation", rotation);
            }
        }

        public IEnumerator SetTexture(Texture value)
        {
            if (layout == Mode.Spherical)
            {
                if (skyboxMaterial.IsKeywordEnabled(SIDES_6))
                {
                    skyboxMaterial.DisableKeyword(SIDES_6);
                }
                skyboxMaterial.EnableKeyword(LAT_LON);
            }
            else
            {
                if (skyboxMaterial.IsKeywordEnabled(LAT_LON))
                {
                    skyboxMaterial.DisableKeyword(LAT_LON);
                }
                skyboxMaterial.EnableKeyword(SIDES_6);
            }

            skyboxMaterial.SetInt("_Mapping", (int)layout);
            skyboxMaterial.SetInt("_ImageType", (int)imageType);
            skyboxMaterial.SetInt("_MirrorOnBack", mirror180OnBack ? 1 : 0);
            skyboxMaterial.SetInt("_Layout", (int)stereoLayout);

            var curTexture = skyboxTexture;
            if (skyboxTexture != value)
            {
                skyboxTexture = value;
                skyboxMaterial.SetTexture("_MainTex", skyboxTexture);
            }

            if (RenderSettings.skybox != skyboxMaterial)
            {
                RenderSettings.skybox = skyboxMaterial;
            }

            if (curTexture != null && curTexture != value)
            {
                if (curTexture is RenderTexture rt)
                {
                    rt.Release();
                }

                Destroy(curTexture);
                yield return Resources.UnloadUnusedAssets().AsCoroutine();
            }

        }
    }
}