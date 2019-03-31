#if VUFORIA
using System;
using System.Collections;
using Juniper.XR;
using Juniper.Unity.Imaging;
using Vuforia;
using UnityEngine;

namespace Juniper.Unity.World.LightEstimation
{
    public abstract class VuforiaLightMeasurement : AbstractLightMeasurement
    {
        /// <summary>
        /// Set to true to use the custom image analyzer when running Vuforia. This is mostly only
        /// useful on Android for Vuforia v7.1.
        /// </summary>
        public bool useCustomAnalyzer;

        /// <summary>
        /// If <see cref="useCustomAnalyzer"/> is set to true, this is the image analyzer that
        /// performs the light estimation.
        /// </summary>
        VuforiaImageAnalyzer analyzer;

        /// <summary>
        /// If <see cref="useCustomAnalyzer"/> is set to true, this is the number of rows in the
        /// camera feed that will be sampled to make the light estimation.
        /// </summary>
        const int NumSampleRows = 10;

        /// <summary>
        /// If <see cref="useCustomAnalyzer"/> is set to true, this is the number of columns in the
        /// camera feed that will be sampled to make the light estimation.
        /// </summary>
        const int NumSamplesPerRow = 10;

        /// <summary>
        /// If <see cref="useCustomAnalyzer"/> is set to false, Vuforia's built-in light estimation
        /// is used. This will be based on ARKit or ARCore, depending on the underlying runtime platform.
        /// </summary>
        IlluminationManager illumination;

        protected override bool UseFakeIntensity =>
            base.UseFakeIntensity
            || (illumination == null && analyzer == null)
            || (illumination != null && illumination.AmbientIntensity == null)
            || (analyzer != null
                && (!analyzer.Running || VuforiaImageAnalyzer.ImageFormat == null));

        public override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (VuforiaUnityExt.HasLightEstimation && !useCustomAnalyzer)
                {
                    StartCoroutine(GetIlluminationManagerCoroutine());
                }
                else
                {
                    analyzer.imageAnalyzer = GetAveragePixelIntensity;
                }
            }
        }

        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                analyzer = this.Ensure<VuforiaImageAnalyzer>();
                return true;
            }

            return false;
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.Remove<VuforiaImageAnalyzer>();
        }

        protected override void InternalUpdate()
        {
            base.InternalUpdate();

            if (illumination != null)
            {
                if (illumination.AmbientIntensity != null)
                {
                    lastIntensity = illumination.AmbientIntensity.Value / 1000f;
                }

                if (illumination.AmbientColorTemperature != null)
                {
                    lastColor = illumination.AmbientColorTemperature.Value.FromKelvinToColor();
                }
            }
        }

        /// <summary>
        /// Waits for a tracker manager, a state manager, and an illumination manager.
        /// </summary>
        /// <returns>The illumination manager coroutine.</returns>
        IEnumerator GetIlluminationManagerCoroutine()
        {
            yield return new WaitUntil(() =>
                TrackerManager.Instance != null);
            StateManager stateManager = null;
            yield return new WaitUntil(() =>
            {
                stateManager = TrackerManager.Instance.GetStateManager();
                return stateManager != null;
            });
            yield return new WaitUntil(() =>
            {
                illumination = stateManager.GetIlluminationManager();
                return illumination != null;
            });
        }

        /// <summary>
        /// Calculates an average pixel value in HSV space for the given image format.
        /// </summary>
        /// <param name="image">      Image.</param>
        /// <param name="pixelGetter">Pixel getter.</param>
        void GetAveragePixelIntensity(Vuforia.Image image, GetPixelFromBuffer pixelGetter)
        {
            float avgH, avgS, avgV;
            avgH = avgS = avgV = 0;

            int bytesPerPixel = image.Stride / image.Width;
            int numPixels = image.Width * image.Height;
            for (int r = 0; r < NumSampleRows; ++r)
            {
                int y = Random.Range(0, image.Height);
                for (int c = 0; c < NumSamplesPerRow; ++c)
                {
                    int x = Random.Range(0, image.Width);
                    int i = y * image.Stride + x * bytesPerPixel;

                    Color pixel = pixelGetter(image.Pixels, i);

                    float h, s, v;
                    Color.RGBToHSV(pixel, out h, out s, out v);

                    avgH += h / numPixels;
                    avgS += s / numPixels;
                    avgV += v / numPixels;
                }
            }

            lastIntensity = avgV;
            lastColor = Color.HSVToRGB(avgH, avgS, avgV);
        }

        protected override void PrintDebugReport()
        {
            base.PrintDebugReport();

            if (illumination == null)
            {
                if (analyzer == null)
                {
                    ScreenDebugger.Print("No illumination manager or image analyzer");
                }
                else
                {
                    if (!analyzer.Running)
                    {
                        ScreenDebugger.Print("Image analyzer is not running");
                    }

                    ScreenDebugger.Print("Image analyzer image format is {0}", VuforiaImageAnalyzer.ImageFormat);
                }
            }
            else
            {
                if (illumination.AmbientIntensity == null)
                {
                    ScreenDebugger.Print("Illumination manager doesn't have an estimate");
                }
            }
        }
    }
}
#endif
