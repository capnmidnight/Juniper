using Juniper.Statistics;
using Juniper.Unity.Display;
using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Juniper.Unity.Widgets
{
    /// <summary>
    /// This component manages the quality settings on the fly, making sure the application never
    /// falls below 50 FPS minimum. Only one of these components is allowed on a GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public class QualityDegrader : MonoBehaviour
    {
#if UNITY_XR_ARKIT || UNITY_XR_ARCORE || HOLOLENS || UNITY_XR_MAGICLEAP
        private Ground.Ground ground;
#endif

        /// <summary>
        /// Calculates statistics for the frame delta time value, for knowing the mean value and
        /// rejecting outliers.
        /// </summary>
        private readonly SingleStatistics frameStats = new SingleStatistics(30);

        /// <summary>
        /// Get the current graphics quality level.
        /// </summary>
        /// <value>The quality level.</value>
        public int QualityLevel
        {
            get
            {
                return QualitySettings.GetQualityLevel();
            }

            set
            {
                if (value != QualityLevel)
                {
                    DisplayManager.MainCamera.enabled = false;
                    ChangeQuality(QualitySettings.IncreaseLevel, (a, b) => a < b, value);
                    ChangeQuality(QualitySettings.DecreaseLevel, (a, b) => a > b, value);
                    DisplayManager.MainCamera.enabled = true;

#if UNITY_XR_ARKIT || UNITY_XR_ARCORE || HOLOLENS || UNITY_XR_MAGICLEAP
                    if (ground != null)
                    {
                        ground.spatialMappingFidelity = (Level)(value / 2);
                    }
#endif

#if UNITY_POSTPROCESSING
                    prost.enabled = QualityLevel >= 4;
#endif

                    PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, value);
                    PlayerPrefs.Save();
                }

                UpdateSlider(value);
            }
        }

        private void ChangeQuality(Action<bool> changer, Func<int, int, bool> checker, int value)
        {
            var lastValue = QualityLevel;
            while (checker(lastValue, value))
            {
                changer(true);
                var newValue = QualityLevel;
                if (newValue == lastValue)
                {
                    throw new Exception("Could not change quality level");
                }
                else
                {
                    lastValue = newValue;
                }
            }
        }

        /// <summary>
        /// Get the name of the current graphics quality level.
        /// </summary>
        /// <value>The name of the quality.</value>
        private string QualityName
        {
            get
            {
                return QualitySettings.names[QualityLevel];
            }
        }

        /// <summary>
        /// Returns true when the frame rate drops below 30FPS.
        /// </summary>
        /// <value><c>true</c> if is bad; otherwise, <c>false</c>.</value>
        private bool IsBad
        {
            get
            {
                return frameStats.Mean / 30f < 1f;
            }
        }

        public Slider qualitySlider;

#if UNITY_POSTPROCESSING
        /// <summary>
        /// The post processing layer to disable as the last step of degrading quality.
        /// </summary>
        private PostProcessLayer prost;
#endif

        private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";

        /// <summary>
        /// Setup the statistics tracking and get the main camera.
        /// </summary>
        public void Awake()
        {
#if UNITY_XR_ARKIT || UNITY_XR_ARCORE || HOLOLENS || UNITY_XR_MAGICLEAP
            ground = ComponentExt.FindAny<Ground.Ground>();
#endif

#if UNITY_POSTPROCESSING
            prost = GetComponent<PostProcessLayer>();
#endif

            if (qualitySlider != null)
            {
                qualitySlider.onValueChanged.AddListener(QualityValueChange);
            }

            QualityLevel = PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY, QualityLevel);
            UpdateSlider(QualityLevel);
        }

        private void QualityValueChange(float i)
        {
            QualityLevel = (int)i;
        }

        /// <summary>
        /// Check to see if the average frame delta time (minus outliers) has dropped below 30FPS.
        /// </summary>
        public void Update()
        {
            if (frameStats.IsSaturated && IsBad)
            {
                if (QualityLevel > 0)
                {
                    --QualityLevel;
                }

                ScreenDebugger.Print(QualityName);
                frameStats.Clear();
            }
        }

        private void UpdateSlider(int value)
        {
            if (qualitySlider != null && !Mathf.Approximately(qualitySlider.value, value))
            {
                qualitySlider.value = value;
            }
        }
    }
}
