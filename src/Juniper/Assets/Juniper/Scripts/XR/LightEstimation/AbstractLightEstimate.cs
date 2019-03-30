using System;

using UnityEngine;

namespace Juniper.Unity.World.LightEstimation
{
    public abstract class AbstractLightEstimate : MonoBehaviour, IInstallable
    {/// <summary>
     /// When set to true, the lighting estimate will modify the global ambient light setting for the
     /// scene. </summary>
        public bool SetAmbientLight = true;

        /// <summary>
        /// When <see cref="SetAmbientLight"/> isSunRotation{getcales the ambient light intensity
        /// estimate to control the brightness of the overall scene.
        /// </summary>
        public float ambientScale = 1f;

        /// <summary>
        /// When set to true, the lighting estimate will modify the intensity and direction of the
        /// Light on which this component is attached.
        /// </summary>
        public bool SetDirectionalLight = true;

        /// <summary>
        /// When <see cref="SetDirectionalLight"/> is set to true, scales the directional light
        /// intensity estimate to control the brightness of incident light and cast shadows.
        /// </summary>
        public float directionalScale = 1f;

        /// <summary>
        /// This value scales the shadow strength estimate, making shadows stronger or lighter,
        /// depending on the balance between ambient and directional light.
        /// </summary>
        public float shadowScale = 1f;

        /// <summary>
        /// This value scales the cloud cover report value, modifying the balance of light between
        /// ambient and directional and potentially softening shadows.
        /// </summary>
        [Range(0, 1)]
        public float CloudCoverScale = 0.5f;

        /// <summary>
        /// When set to true, the Update loop will include printing out the internal values of the component;
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// A static orientation for the straight above zenith.
        /// </summary>
        protected static Vector3 overhead = new Vector3(90, 0, 0);

        /// <summary>
        /// The light to which this component is attached.
        /// </summary>
        protected Light sun;

        /// <summary>
        /// The AR subsystem for measuring light strength in a camera frame.
        /// </summary>
        protected LightMeasurement measurement;

        /// <summary>
        /// Gets the estimated cloud cover.
        /// </summary>
        /// <value>The cloud cover.</value>
        protected abstract float CloudCover
        {
            get;
        }

        /// <summary>
        /// Defaults to false. Overriding classes can indicate they have a rotation value to make the
        /// base class update the sun light's rotation value. Otherwise, it will not be modified from
        /// whatever the user set in the Unity Editor.
        /// </summary>
        /// <value><c>true</c> if has sun rotation; otherwise, <c>false</c>.</value>
        protected abstract bool HasSunRotation
        {
            get;
        }

        /// <summary>
        /// Gets a rotation quaternion that will put the "sun dot" of Unity's defauilt skybox into
        /// the correct position in the sky.
        /// </summary>
        /// <value>The sun rotation.</value>
        protected abstract Vector3 SunRotation
        {
            get;
        }

        /// <summary>
        /// Raw intensity values need to be ramped to make sense as a Unity scene light intensity value.
        /// </summary>
        /// <returns>The intensity.</returns>
        /// <param name="rawIntensityEstimate">Raw intensity estimate.</param>
        protected float RampIntensity(float rawIntensityEstimate)
        {
            const float linearRampThreshold = 0.8f;
            const float middleGray = 0.18f;
            const float inclination = 0.4f;
            const float beta = (linearRampThreshold / inclination) - 1.0f;
            const float alpha = (beta + 1.0f) / beta * linearRampThreshold;

            var normalizedIntensity = rawIntensityEstimate / middleGray;

            if (normalizedIntensity < 1.0f)
            {
                return normalizedIntensity * linearRampThreshold;
            }
            else
            {
                return alpha * (1.0f - (1.0f / ((beta * normalizedIntensity) + 1.0f)));
            }
        }

        /// <summary>
        /// Retrieves the light to which this component is attached, and makes sure there is a <see
        /// cref="LightMeasurement"/> component attached.
        /// </summary>
        protected virtual void Awake()
        {
            Install(false);
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        public virtual bool Install(bool reset)
        {
            measurement = this.Ensure<LightMeasurement>();
            sun = GetComponent<Light>();

            return sun != null;
        }

        public void Uninstall()
        {
        }

        /// <summary>
        /// Applies the light estimation process, and optionally prints a debug report.
        /// </summary>
        protected virtual void Update()
        {
            Color.RGBToHSV(measurement.ColorEstimate, out var hue, out var sat, out var val);

            var rampedIntensity = RampIntensity(measurement.IntensityEstimate);
            var cloudCover = CloudCover * CloudCoverScale * 2;
            var ambientIntensityEstimate = cloudCover * rampedIntensity;
            var directionalIntensityEstimate = (1 - cloudCover) * rampedIntensity;

            var ambientIntensity = ambientScale * ambientIntensityEstimate;
            var ambientColor = Color.HSVToRGB(hue, sat, ambientIntensity);

            var directionalIntensity = directionalScale * directionalIntensityEstimate;
            var directionalColor = Color.HSVToRGB(hue, sat, directionalIntensity);

            var shadowStrength = Mathf.Clamp(shadowScale * directionalIntensityEstimate * (1 - ambientIntensityEstimate), 0f, 1f);

            if (SetDirectionalLight && sun != null)
            {
                sun.color = directionalColor;
                sun.intensity = directionalIntensity;
                sun.shadowStrength = shadowStrength;

                if (HasSunRotation)
                {
                    sun.transform.localRotation = Quaternion.Euler(SunRotation);
                }
            }

            if (SetAmbientLight)
            {
                RenderSettings.ambientLight = ambientColor;
                RenderSettings.ambientIntensity = ambientIntensity;
            }

            Shader.SetGlobalFloat("_GlobalLightEstimation", ambientIntensityEstimate);

            if (DebugReport)
            {
                PrintDebugReport(ambientIntensity, ambientColor, directionalIntensity, directionalColor, shadowStrength);
            }
        }

        /// <summary>
        /// If <see cref="DebugReport"/> is set to true, prints a report that displays the status of
        /// the component and the values it is using for the light estimation.
        /// </summary>
        /// <param name="ambientIntensity">    Ambient intensity.</param>
        /// <param name="ambientColor">        Ambient color.</param>
        /// <param name="directionalIntensity">Directional intensity.</param>
        /// <param name="directionalColor">    Directional color.</param>
        /// <param name="shadowStrength">      Shadow strength.</param>
        private void PrintDebugReport(float ambientIntensity, Color ambientColor, float directionalIntensity, Color directionalColor, float shadowStrength)
        {
            if (measurement == null)
            {
                ScreenDebugger.Print("  no light measurement");
            }

            ScreenDebugger.Print($"  Sun Position: <{SunRotation.x.Label(UnitOfMeasure.Degrees, 4)}, {SunRotation.y.Label(UnitOfMeasure.Degrees, 4)}, {SunRotation.z.Label(UnitOfMeasure.Degrees, 4)}>");
            ScreenDebugger.PrintFormat("  Ambient: int {0:0.000} col {1}", ambientIntensity, ambientColor);
            ScreenDebugger.PrintFormat("  Directional: int {0:0.000} col {1}", directionalIntensity, directionalColor);
            ScreenDebugger.PrintFormat("  Shadow strength: {0:0.000}", shadowStrength);

            if (sun == null)
            {
                ScreenDebugger.Print("  no directional light");
            }
            else
            {
                ScreenDebugger.Print($"  sun orientation: {sun.transform.eulerAngles}");
            }
        }
    }
}