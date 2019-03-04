using Juniper.Statistics;

using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Juniper.Widgets
{
    /// <summary>
    /// This component manages the quality settings on the fly, making sure the application never
    /// falls below 50 FPS minimum. Only one of these components is allowed on a GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public class QualityDegrader : MonoBehaviour
    {
#if ARKIT || ARCORE || HOLOLENS || MAGIC_LEAP
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
                    while (QualityLevel < value)
                    {
                        QualitySettings.IncreaseLevel(true);
                    }
                    while (QualityLevel > value)
                    {
                        QualitySettings.DecreaseLevel(true);
                    }

#if ARKIT || ARCORE || HOLOLENS || MAGIC_LEAP
                    if (ground != null)
                    {
                        ground.spatialMappingFidelity = (Level)(QualityLevel / 2);
                    }
#endif

#if UNITY_POSTPROCESSING
                    prost.enabled = QualityLevel >= 4;
#endif

                    PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, QualityLevel);
                    PlayerPrefs.Save();
                }

                UpdateButtons();
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

        public Button[] qualityButtons;

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
#if ARKIT || ARCORE || HOLOLENS || MAGIC_LEAP
            ground = ComponentExt.FindAny<Ground.Ground>();
#endif

#if UNITY_POSTPROCESSING
            prost = GetComponent<PostProcessLayer>();
#endif

            if (qualityButtons != null)
            {
                for (var i = 0; i < qualityButtons.Length; ++i)
                {
                    qualityButtons[i].onClick.AddListener(QualityChanger(i));
                }
            }

            QualityLevel = PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY, QualityLevel);
            UpdateButtons();
        }

        private UnityAction QualityChanger(int i)
        {
            return () => QualityLevel = i;
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

        private void UpdateButtons()
        {
            if (qualityButtons != null)
            {
                var cur = QualityLevel;
                for (var i = 0; i < qualityButtons.Length; ++i)
                {
                    qualityButtons[i].interactable = i != cur;
                }
            }
        }
    }
}