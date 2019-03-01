using System;

using UnityEngine;

namespace Juniper.World.LightEstimation
{
    public abstract class AbstractLightMeasurement :
        MonoBehaviour,
        IInstallable
    {
        /// <summary>
        /// Light estimate updates happen concurrently with frame updates. This value stores the last
        /// light intesity measurement that was calculated, for use in the current render frame.
        /// </summary>
        protected float lastIntensity;

        /// <summary>
        /// Light estimate updates happen concurrently with frame updates. This value stores the last
        /// light color measurement that was calculated, for use in the current render frame.
        /// </summary>
        protected Color lastColor;

        /// <summary>
        /// When set to true, the light estimate values this component returns will be fake values
        /// set in the Unity Editor below.
        /// </summary>
        public bool useFake;

        /// <summary>
        /// Returns true if the user has requested a fake intensity, or the application is running in
        /// the Unity Editor.
        /// </summary>
        /// <value><c>true</c> if use fake intensity; otherwise, <c>false</c>.</value>
        protected virtual bool UseFakeIntensity =>
            useFake || Application.isEditor;

        /// <summary>
        /// The fake intensity value to use if <see cref="UseFakeIntensity"/> is true.
        /// </summary>
        public float Intensity = 0.8f;

        /// <summary>
        /// The fake color to use if <see cref="UseFakeIntensity"/> is true.
        /// </summary>
        public Color Color = Color.white;

        /// <summary>
        /// When set to true, the Update loop will include printing out the internal values of the component;
        /// </summary>
        public bool DebugReport;

        /// <summary>
        /// Subsystems with light estimation capability should override this method to return the
        /// light temperature. Otherwise, a default value is returned that matches a typical, sunny day.
        /// </summary>
        /// <returns>The light color estimate.</returns>
        public Color ColorEstimate =>
            lastColor;

        /// <summary>
        /// Subsystems with light estimation capability should override this method to return the
        /// estimation. Otherwise, a default value is returned that matches a typical, sunny day.
        /// </summary>
        /// <returns>The light intensity estimate.</returns>
        public float IntensityEstimate =>
            lastIntensity;

        /// <summary>
        /// Find the current XR system configuration, and setup the correct light estimation
        /// functions for the current platform.
        /// </summary>
        public virtual void Awake()
        {
            Install(false);

            SetFakeValues();
        }

        public virtual void Reinstall() =>
            Install(true);

#if UNITY_EDITOR
        public void Reset() =>
            Reinstall();
#endif

        public virtual void Install(bool reset) =>
            reset &= Application.isEditor;

        public virtual void Uninstall() { }

        /// <summary>
        /// Sets the current Intensity and Color from the fake values set in the Unity Editor.
        /// </summary>
        private void SetFakeValues()
        {
            lastIntensity = Intensity;
            lastColor = Color.Scale(lastIntensity);
        }

        /// <summary>
        /// Check to see if the ARCore light estimation has been updated.
        /// </summary>
        public void Update()
        {
            if (UseFakeIntensity)
            {
                SetFakeValues();
            }
            else
            {
                UpdateMeasurement();
            }

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        protected virtual void UpdateMeasurement() { }

        /// <summary>
        /// If <see cref="DebugReport"/> is set to true, prints a report that displays the status of
        /// the component and the values it is using for the light estimation.
        /// </summary>
        protected virtual void PrintDebugReport()
        {
            if (UseFakeIntensity)
            {
                ScreenDebugger.Print("Using fake light estimate");
            }

            ScreenDebugger.PrintFormat("  Raw: int {0:0.000} col {1}", IntensityEstimate, ColorEstimate);
        }
    }
}
