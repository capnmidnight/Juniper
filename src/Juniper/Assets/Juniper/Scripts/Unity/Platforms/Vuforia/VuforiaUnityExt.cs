#if VUFORIA
using System;
namespace Vuforia
{
    /// <summary>
    /// Some configuration parameters concerning the current version of Vuforia on the current system.
    /// </summary>
    public static class VuforiaUnityExt
    {
        /// <summary>
        /// The current version of Vuforia.
        /// </summary>
        public static Version CurrentVersion;

        /// <summary>
        /// Set to true when the current version of Vuforia has the ability to provide light
        /// estimation values.
        /// </summary>
        public static bool HasLightEstimation;

        /// <summary>
        /// A flag indicating that the current Vuforia version is old enough to require Persistent
        /// Extended Tracking instead of Device Tracking, i.e. v 7.1.35 or older.
        /// </summary>
        public static bool HasPET;

        /// <summary>
        /// The last good version of Vuforia.
        /// </summary>
        static readonly Version VERSION_7_1_35 = new Version(7, 1, 35);

#if UNITY_ANDROID
        /// <summary>
        /// The version of Vuforia at which everything went off the rails.
        /// </summary>
        static readonly Version VERSION_7_2 = new Version(7, 2);
#endif

        /// <summary>
        /// Initializes the <see cref="CurrentVersion"/>, <see cref="HasPET"/>, and <see
        /// cref="HasLightEstimation"/> values.
        /// </summary>
        static VuforiaUnityExt()
        {
            CurrentVersion = new Version(VuforiaUnity.GetVuforiaLibraryVersion());
            HasPET = CurrentVersion <= VERSION_7_1_35;
#if UNITY_IOS
            HasLightEstimation = CurrentVersion >= VERSION_7_1_35;
#elif UNITY_ANDROID
            HasLightEstimation = CurrentVersion >= VERSION_7_2;
#endif
        }
    }
}
#endif