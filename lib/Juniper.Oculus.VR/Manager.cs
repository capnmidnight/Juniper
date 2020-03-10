/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using System.Numerics;
namespace Oculus.VR
{
    /// <summary>
    /// Configuration data for Oculus virtual reality.
    /// </summary>
    public static class Manager
    {
#if DEBUG
        private const bool isDebugBuild = true;
#else
        private const bool debug = false;
#endif

        public enum TrackingOrigin
        {
            EyeLevel = Plugin.TrackingOrigin.EyeLevel,
            FloorLevel = Plugin.TrackingOrigin.FloorLevel,
            Stage = Plugin.TrackingOrigin.Stage,
        }

        public enum EyeTextureFormat
        {
            Default = Plugin.EyeTextureFormat.Default,
            R16G16B16A16_FP = Plugin.EyeTextureFormat.R16G16B16A16_FP,
            R11G11B10_FP = Plugin.EyeTextureFormat.R11G11B10_FP,
        }

        public enum FixedFoveatedRenderingLevel
        {
            Off = Plugin.FixedFoveatedRenderingLevel.Off,
            Low = Plugin.FixedFoveatedRenderingLevel.Low,
            Medium = Plugin.FixedFoveatedRenderingLevel.Medium,
            High = Plugin.FixedFoveatedRenderingLevel.High,
            HighTop = Plugin.FixedFoveatedRenderingLevel.HighTop,
        }

        public enum XRDevice
        {
            Unknown = 0,
            Oculus = 1,
            OpenVR = 2,
        }

        /// <summary>
        /// Gets a reference to the active display.
        /// </summary>
        public static Display display { get; } = new Display();

        static Manager()
        {
            // uncomment the following line to disable the callstack printed to log
            //Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);  // TEMPORARY

            Console.WriteLine("Oculus Utilities v" + Plugin.wrapperVersion + ", " +
                    "OVRPlugin v" + Plugin.version + ", " +
                    "SDK v" + Plugin.nativeSDKVersion + ".");

            if (resetTrackerOnLoad)
            {
                display.RecenterPose();
            }

            if (isDebugBuild)
            {
                Plugin.SetDeveloperMode(Plugin.Bool.True);
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Force OcculusionMesh on all the time, you can change the value to false if you really need it be off for some reasons,
                // be aware there are performance drops if you don't use occlusionMesh.
                Plugin.occlusionMesh = true;
            }
            OVRManagerinitialized = true;

        }

        /// <summary>
        /// Occurs when an HMD attached.
        /// </summary>
        public static event Action HMDAcquired;

        /// <summary>
        /// Occurs when an HMD detached.
        /// </summary>
        public static event Action HMDLost;

        /// <summary>
        /// Occurs when an HMD is put on the user's head.
        /// </summary>
        public static event Action HMDMounted;

        /// <summary>
        /// Occurs when an HMD is taken off the user's head.
        /// </summary>
        public static event Action HMDUnmounted;

        /// <summary>
        /// Occurs when VR Focus is acquired.
        /// </summary>
        public static event Action VrFocusAcquired;

        /// <summary>
        /// Occurs when VR Focus is lost.
        /// </summary>
        public static event Action VrFocusLost;

        /// <summary>
        /// Occurs when Input Focus is acquired.
        /// </summary>
        public static event Action InputFocusAcquired;

        /// <summary>
        /// Occurs when Input Focus is lost.
        /// </summary>
        public static event Action InputFocusLost;

        /// <summary>
        /// Occurs when the active Audio Out device has changed and a restart is needed.
        /// </summary>
        public static event Action AudioOutChanged;

        /// <summary>
        /// Occurs when the active Audio In device has changed and a restart is needed.
        /// </summary>
        public static event Action AudioInChanged;

        /// <summary>
        /// Occurs when the sensor gained tracking.
        /// </summary>
        public static event Action TrackingAcquired;

        /// <summary>
        /// Occurs when the sensor lost tracking.
        /// </summary>
        public static event Action TrackingLost;

        private static bool _isHmdPresentCached = false;
        private static bool _isHmdPresent = false;
        private static bool _wasHmdPresent = false;
        /// <summary>
        /// If true, a head-mounted display is connected and present.
        /// </summary>
        public static bool isHmdPresent
        {
            get
            {
                if (!_isHmdPresentCached)
                {
                    _isHmdPresentCached = true;
                    _isHmdPresent = NodeStateProperties.IsHmdPresent();
                }

                return _isHmdPresent;
            }

            private set
            {
                _isHmdPresentCached = true;
                _isHmdPresent = value;
            }
        }

        /// <summary>
        /// Gets the audio output device identifier.
        /// </summary>
        /// <description>
        /// On Windows, this is a string containing the GUID of the IMMDevice for the Windows audio endpoint to use.
        /// </description>
        public static string audioOutId => Plugin.audioOutId;

        /// <summary>
        /// Gets the audio input device identifier.
        /// </summary>
        /// <description>
        /// On Windows, this is a string containing the GUID of the IMMDevice for the Windows audio endpoint to use.
        /// </description>
        public static string audioInId => Plugin.audioInId;

        private static bool _hasVrFocusCached = false;
        private static bool _hasVrFocus = false;
        private static bool _hadVrFocus = false;
        /// <summary>
        /// If true, the app has VR Focus.
        /// </summary>
        public static bool hasVrFocus
        {
            get
            {
                if (!_hasVrFocusCached)
                {
                    _hasVrFocusCached = true;
                    _hasVrFocus = Plugin.hasVrFocus;
                }

                return _hasVrFocus;
            }

            private set
            {
                _hasVrFocusCached = true;
                _hasVrFocus = value;
            }
        }

        private static bool _hadInputFocus = true;
        /// <summary>
        /// If true, the app has Input Focus.
        /// </summary>
        public static bool hasInputFocus => Plugin.hasInputFocus;

        /// <summary>
        /// If true, chromatic de-aberration will be applied, improving the image at the cost of texture bandwidth.
        /// </summary>
        public static bool chromatic
        {
            get
            {
                if (!isHmdPresent)
                {
                    return false;
                }

                return Plugin.chromatic;
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                Plugin.chromatic = value;
            }
        }

        /// <summary>
        /// If true, distortion rendering work is submitted a quarter-frame early to avoid pipeline stalls and increase CPU-GPU parallelism.
        /// </summary>
        public static bool queueAhead = true;

        /// <summary>
        /// If true, Unity will use the optimal antialiasing level for quality/performance on the current hardware.
        /// </summary>
        public static bool useRecommendedMSAALevel = true;

        public static bool monoscopic
        {
            get
            {
                if (!isHmdPresent)
                {
                    return false;
                }

                return Plugin.monoscopic;
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                Plugin.monoscopic = value;
            }
        }

        /// <summary>
        /// Set the relative offset rotation of head poses
        /// </summary>
        private static Vector3 _headPoseRelativeOffsetRotation;
        public static Vector3 headPoseRelativeOffsetRotation
        {
            get
            {
                return _headPoseRelativeOffsetRotation;
            }
            set
            {
                if (Plugin.GetHeadPoseModifier(out var rotation, out var translation))
                {
                    var finalRotation = Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z);
                    rotation = finalRotation.ToQuatf();
                    Plugin.SetHeadPoseModifier(ref rotation, ref translation);
                }
                _headPoseRelativeOffsetRotation = value;
            }
        }

        /// <summary>
        /// Set the relative offset translation of head poses
        /// </summary>
        private static Vector3 _headPoseRelativeOffsetTranslation;
        public static Vector3 headPoseRelativeOffsetTranslation
        {
            get
            {
                return _headPoseRelativeOffsetTranslation;
            }
            set
            {
                if (Plugin.GetHeadPoseModifier(out var rotation, out var translation))
                {
                    if (translation.FromFlippedZVector3f() != value)
                    {
                        translation = value.ToFlippedZVector3f();
                        Plugin.SetHeadPoseModifier(ref rotation, ref translation);
                    }
                }
                _headPoseRelativeOffsetTranslation = value;
            }
        }

        /// <summary>
        /// The number of expected display frames per rendered frame.
        /// </summary>
        public static int vsyncCount
        {
            get
            {
                if (!isHmdPresent)
                {
                    return 1;
                }

                return Plugin.vsyncCount;
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                Plugin.vsyncCount = value;
            }
        }

        public static string OCULUS_UNITY_NAME_STR = "Oculus";
        public static string OPENVR_UNITY_NAME_STR = "OpenVR";

        public static XRDevice loadedXRDevice;

        /// <summary>
        /// Gets the current battery level.
        /// </summary>
        /// <returns><c>battery level in the range [0.0,1.0]</c>
        /// <param name="batteryLevel">Battery level.</param>
        public static float batteryLevel
        {
            get
            {
                if (!isHmdPresent)
                {
                    return 1f;
                }

                return Plugin.batteryLevel;
            }
        }

        /// <summary>
        /// Gets the current battery temperature.
        /// </summary>
        /// <returns><c>battery temperature in Celsius</c>
        /// <param name="batteryTemperature">Battery temperature.</param>
        public static float batteryTemperature
        {
            get
            {
                if (!isHmdPresent)
                {
                    return 0f;
                }

                return Plugin.batteryTemperature;
            }
        }

        /// <summary>
        /// Gets the current battery status.
        /// </summary>
        /// <returns><c>battery status</c>
        /// <param name="batteryStatus">Battery status.</param>
        public static int batteryStatus
        {
            get
            {
                if (!isHmdPresent)
                {
                    return -1;
                }

                return (int)Plugin.batteryStatus;
            }
        }

        /// <summary>
        /// Gets the current volume level.
        /// </summary>
        /// <returns><c>volume level in the range [0,1].</c>
        public static float volumeLevel
        {
            get
            {
                if (!isHmdPresent)
                {
                    return 0f;
                }

                return Plugin.systemVolume;
            }
        }

        /// <summary>
        /// Gets or sets the current CPU performance level (0-2). Lower performance levels save more power.
        /// </summary>
        public static int cpuLevel
        {
            get
            {
                if (!isHmdPresent)
                {
                    return 2;
                }

                return Plugin.cpuLevel;
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                Plugin.cpuLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the current GPU performance level (0-2). Lower performance levels save more power.
        /// </summary>
        public static int gpuLevel
        {
            get
            {
                if (!isHmdPresent)
                {
                    return 2;
                }

                return Plugin.gpuLevel;
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                Plugin.gpuLevel = value;
            }
        }

        /// <summary>
        /// If true, the CPU and GPU are currently throttled to save power and/or reduce the temperature.
        /// </summary>
        public static bool isPowerSavingActive
        {
            get
            {
                if (!isHmdPresent)
                {
                    return false;
                }

                return Plugin.powerSaving;
            }
        }

        /// <summary>
        /// Gets or sets the eye texture format.
        /// </summary>
        public static EyeTextureFormat eyeTextureFormat
        {
            get
            {
                return (Manager.EyeTextureFormat)Plugin.GetDesiredEyeTextureFormat();
            }

            set
            {
                Plugin.SetDesiredEyeTextureFormat((Plugin.EyeTextureFormat)value);
            }
        }

        /// <summary>
        /// Gets if tiled-based multi-resolution technique is supported
        /// This feature is only supported on QCOMM-based Android devices
        /// </summary>
        public static bool fixedFoveatedRenderingSupported => Plugin.fixedFoveatedRenderingSupported;

        /// <summary>
        /// Gets or sets the tiled-based multi-resolution level
        /// This feature is only supported on QCOMM-based Android devices
        /// </summary>
        public static FixedFoveatedRenderingLevel fixedFoveatedRenderingLevel
        {
            get
            {
                if (!Plugin.fixedFoveatedRenderingSupported)
                {
                    Console.WriteLine("WARNING: Fixed Foveated Rendering feature is not supported");
                }
                return (FixedFoveatedRenderingLevel)Plugin.fixedFoveatedRenderingLevel;
            }
            set
            {
                if (!Plugin.fixedFoveatedRenderingSupported)
                {
                    Console.WriteLine("WARNING: Fixed Foveated Rendering feature is not supported");
                }
                Plugin.fixedFoveatedRenderingLevel = (Plugin.FixedFoveatedRenderingLevel)value;
            }
        }

        /// <summary>
        /// Gets if the GPU Utility is supported
        /// This feature is only supported on QCOMM-based Android devices
        /// </summary>
        public static bool gpuUtilSupported => Plugin.gpuUtilSupported;

        /// <summary>
        /// Gets the GPU Utilised Level (0.0 - 1.0)
        /// This feature is only supported on QCOMM-based Android devices
        /// </summary>
        public static float gpuUtilLevel
        {
            get
            {
                if (!Plugin.gpuUtilSupported)
                {
                    Console.WriteLine("WARNING: GPU Util is not supported");
                }
                return Plugin.gpuUtilLevel;
            }
        }

        /// <summary>
        /// Sets the Color Scale and Offset which is commonly used for effects like fade-to-black.
        /// In our compositor, once a given frame is rendered, warped, and ready to be displayed, we then multiply
        /// each pixel by colorScale and add it to colorOffset, whereby newPixel = oldPixel * colorScale + colorOffset.
        /// Note that for mobile devices (Quest, Go, etc.), colorOffset is not supported, so colorScale is all that can
        /// be used. A colorScale of (1, 1, 1, 1) and colorOffset of (0, 0, 0, 0) will lead to an identity multiplication
        /// and have no effect.
        /// </summary>
        public static void SetColorScaleAndOffset(Vector4 colorScale, Vector4 colorOffset, bool applyToAllLayers)
        {
            Plugin.SetColorScaleAndOffset(colorScale, colorOffset, applyToAllLayers);
        }

        private static Manager.TrackingOrigin _trackingOriginType = Manager.TrackingOrigin.EyeLevel;
        /// <summary>
        /// Defines the current tracking origin type.
        /// </summary>
        public static Manager.TrackingOrigin trackingOriginType
        {
            get
            {
                if (!isHmdPresent)
                {
                    return _trackingOriginType;
                }

                return (Manager.TrackingOrigin)Plugin.GetTrackingOriginType();
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                if (Plugin.SetTrackingOriginType((Plugin.TrackingOrigin)value))
                {
                    // Keep the field exposed in the Unity Editor synchronized with any changes.
                    _trackingOriginType = value;
                }
            }
        }

        /// <summary>
        /// If true, head tracking will affect the position of each OVRCameraRig's cameras.
        /// </summary>
        public static bool usePositionTracking = true;

        /// <summary>
        /// If true, head tracking will affect the rotation of each OVRCameraRig's cameras.
        /// </summary>
        public static bool useRotationTracking = true;

        /// <summary>
        /// If true, the distance between the user's eyes will affect the position of each OVRCameraRig's cameras.
        /// </summary>
        public static bool useIPDInPositionTracking = true;

        /// <summary>
        /// If true, each scene load will cause the head pose to reset.
        /// </summary>
        public static bool resetTrackerOnLoad = false;

        /// <summary>
        /// If true, the Reset View in the universal menu will cause the pose to be reset. This should generally be
        /// enabled for applications with a stationary position in the virtual world and will allow the View Reset
        /// command to place the person back to a predefined location (such as a cockpit seat).
        /// Set this to false if you have a locomotion system because resetting the view would effectively teleport
        /// the player to potentially invalid locations.
        /// </summary>
        public static bool AllowRecenter = true;

        /// <summary>
        /// Defines the recentering mode specified in the tooltip above.
        /// </summary>
        public static bool reorientHMDOnControllerRecenter
        {
            get
            {
                if (!isHmdPresent)
                {
                    return false;
                }

                return Plugin.GetReorientHMDOnControllerRecenter();
            }

            set
            {
                if (!isHmdPresent)
                {
                    return;
                }

                Plugin.SetReorientHMDOnControllerRecenter(value);

            }
        }

        /// <summary>
        /// If true, a lower-latency update will occur right before rendering. If false, the only controller pose update will occur at the start of simulation for a given frame.
        /// Selecting this option lowers rendered latency for controllers and is often a net positive; however, it also creates a slight disconnect between rendered and simulated controller poses.
        /// Visit online Oculus documentation to learn more.
        /// </summary>
        public static bool LateControllerUpdate = true;

        /// <summary>
        /// True if the current platform supports virtual reality.
        /// </summary>
        public const bool isSupportedPlatform = true;

        private static bool _isUserPresentCached = false;
        private static bool _isUserPresent = false;
        private static bool _wasUserPresent = false;

        /// <summary>
        /// True if the user is currently wearing the display.
        /// </summary>
        public static bool isUserPresent
        {
            get
            {
                if (!_isUserPresentCached)
                {
                    _isUserPresentCached = true;
                    _isUserPresent = Plugin.userPresent;
                }

                return _isUserPresent;
            }

            private set
            {
                _isUserPresentCached = true;
                _isUserPresent = value;
            }
        }

        private static bool prevAudioOutIdIsCached = false;
        private static bool prevAudioInIdIsCached = false;
        private static string prevAudioOutId = string.Empty;
        private static string prevAudioInId = string.Empty;
        private static bool wasPositionTracked = false;

        public static System.Version utilitiesVersion => Plugin.wrapperVersion;

        public static System.Version pluginVersion => Plugin.version;

        public static System.Version sdkVersion => Plugin.nativeSDKVersion;

        public static bool OVRManagerinitialized = false;

        public static void Update()
        {
            if (Plugin.shouldQuit)
            {
                Console.WriteLine("[OVRManager] OVRPlugin.shouldQuit detected");
                Environment.Exit(0);
            }

            if (AllowRecenter && Plugin.shouldRecenter)
            {
                display.RecenterPose();
            }

            if (trackingOriginType != _trackingOriginType)
            {
                trackingOriginType = _trackingOriginType;
            }

            Tracker.IsEnabled = usePositionTracking;

            Plugin.rotation = useRotationTracking;

            Plugin.useIPDInPositionTracking = useIPDInPositionTracking;

            // Dispatch HMD events.

            isHmdPresent = NodeStateProperties.IsHmdPresent();

            if (headPoseRelativeOffsetRotation != _headPoseRelativeOffsetRotation)
            {
                headPoseRelativeOffsetRotation = _headPoseRelativeOffsetRotation;
            }

            if (headPoseRelativeOffsetTranslation != _headPoseRelativeOffsetTranslation)
            {
                headPoseRelativeOffsetTranslation = _headPoseRelativeOffsetTranslation;
            }

            if (_wasHmdPresent && !isHmdPresent)
            {
                try
                {
                    Console.WriteLine("[OVRManager] HMDLost event");
                    HMDLost?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            if (!_wasHmdPresent && isHmdPresent)
            {
                try
                {
                    Console.WriteLine("[OVRManager] HMDAcquired event");
                    HMDAcquired?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            _wasHmdPresent = isHmdPresent;

            // Dispatch HMD mounted events.

            isUserPresent = Plugin.userPresent;

            if (_wasUserPresent && !isUserPresent)
            {
                try
                {
                    Console.WriteLine("[OVRManager] HMDUnmounted event");
                    HMDUnmounted?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            if (!_wasUserPresent && isUserPresent)
            {
                try
                {
                    Console.WriteLine("[OVRManager] HMDMounted event");
                    if (HMDMounted != null)
                    {
                        HMDMounted();
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            _wasUserPresent = isUserPresent;

            // Dispatch VR Focus events.

            hasVrFocus = Plugin.hasVrFocus;

            if (_hadVrFocus && !hasVrFocus)
            {
                try
                {
                    Console.WriteLine("[OVRManager] VrFocusLost event");
                    VrFocusLost?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            if (!_hadVrFocus && hasVrFocus)
            {
                try
                {
                    Console.WriteLine("[OVRManager] VrFocusAcquired event");
                    VrFocusAcquired?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            _hadVrFocus = hasVrFocus;

            // Dispatch VR Input events.

            var hasInputFocus = Plugin.hasInputFocus;

            if (_hadInputFocus && !hasInputFocus)
            {
                try
                {
                    Console.WriteLine("[OVRManager] InputFocusLost event");
                    InputFocusLost?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            if (!_hadInputFocus && hasInputFocus)
            {
                try
                {
                    Console.WriteLine("[OVRManager] InputFocusAcquired event");
                    InputFocusAcquired?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            _hadInputFocus = hasInputFocus;

            // Dispatch Audio Device events.

            var audioOutId = Plugin.audioOutId;
            if (!prevAudioOutIdIsCached)
            {
                prevAudioOutId = audioOutId;
                prevAudioOutIdIsCached = true;
            }
            else if (audioOutId != prevAudioOutId)
            {
                try
                {
                    Console.WriteLine("[OVRManager] AudioOutChanged event");
                    AudioOutChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }

                prevAudioOutId = audioOutId;
            }

            var audioInId = Plugin.audioInId;
            if (!prevAudioInIdIsCached)
            {
                prevAudioInId = audioInId;
                prevAudioInIdIsCached = true;
            }
            else if (audioInId != prevAudioInId)
            {
                try
                {
                    Console.WriteLine("[OVRManager] AudioInChanged event");
                    AudioInChanged?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }

                prevAudioInId = audioInId;
            }

            // Dispatch tracking events.

            if (wasPositionTracked && !Tracker.IsPositionTracked)
            {
                try
                {
                    Console.WriteLine("[OVRManager] TrackingLost event");
                    TrackingLost?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            if (!wasPositionTracked && Tracker.IsPositionTracked)
            {
                try
                {
                    Console.WriteLine("[OVRManager] TrackingAcquired event");
                    TrackingAcquired?.Invoke();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Caught Exception: " + e);
                }
            }

            wasPositionTracked = Tracker.IsPositionTracked;

            display.Update();
        }

        public static void PlatformUIConfirmQuit()
        {
            if (!isHmdPresent)
            {
                return;
            }

            Plugin.ShowUI(Plugin.PlatformUI.ConfirmQuit);
        }
    }
}