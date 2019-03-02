using Juniper.Widgets;

using UnityEngine;

using Juniper.Input;

#if VUFORIA
using Vuforia;
#elif ARKIT && !UNITY_EDITOR
using UnityEngine.XR.iOS;
#elif ARCORE
using GoogleARCore;
using UnityEngine.SpatialTracking;
using UnityEngine.Android;
#elif GOOGLEVR
#elif MAGIC_LEAP
using MSA;
#elif OCULUS
using System.Linq;
#elif WAVEVR

using wvr;

#endif

namespace Juniper
{
    /// <summary>
    /// Manages the camera FOV in the editor so that it matches the target system, or on desktop
    /// makes sure the FOV is a reasonable value for the current screen dimensions. Only one of these
    /// components is allowed on a gameObject. This component requires a Camera component to also be
    /// on the gameObject.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CameraExtensions : MonoBehaviour, IInstallable
    {/// <summary>
     /// On ARKit and ARCore systems, a special material for displaying the camera image needs to
     /// be configured. Vuforia does this on its own, and HoloLens has a clear display so it isn't needed.
     /// </summary>
        [Header("Augmented Reality")]
#if !ARKIT && !ARCORE
        [HideInInspector]
#endif
        public Material ARBackgroundMaterial;

        /// <summary>
        /// On ARKit and ARCore, perform an image analysis algorithm that renders a point cloud of
        /// feature points.
        /// </summary>
#if !ARKIT && !ARCORE

        [HideInInspector]
#endif
        public bool enablePointCloud = true;

        /// <summary>
        /// The main camera, cached so we don't incur a cost to look it up all the time.
        /// </summary>
        private static Camera cam;

        public static Camera MainCamera
        {
            get
            {
                if (cam != null)
                {
                    return cam;
                }
                else
                {
                    return cam = ComponentExt.FindAny<Camera>(camera => camera.tag == "MainCamera");
                }
            }
        }

        public virtual void Awake()
        {
            cam = GetComponent<Camera>();
            Install(false);
        }

        public virtual void Reinstall() =>
            Install(true);

#if UNITY_EDITOR
        public void Reset() =>
            Reinstall();
#endif

#if ARCORE
        /// <summary>
        /// On ARCore, this value is used to flag when the app is shutting down so we don't capture
        /// multiple errors that would cause multiple Toast messages to appear.
        /// </summary>
        bool isQuitting;
#endif

        public void Install(bool reset)
        {
            reset &= Application.isEditor;

#if UNITY_MODULES_AUDIO
            var listener = this.EnsureComponent<AudioListener>().Value;
#endif
            this.EnsureComponent<QualityDegrader>();
            this.EnsureComponent<CameraControl>();

#if ARKIT && !UNITY_EDITOR
            this.WithLock(() =>
            {
                var bgRenderer = this.EnsureComponent<UnityARVideo>();
                bgRenderer.m_ClearMaterial = ARBackgroundMaterial;

                this.EnsureComponent<UnityARCameraNearFar>();

                var camMgr = this.EnsureComponent<UnityARCameraManager>();
                camMgr.startAlignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;
                camMgr.planeDetection = UnityARPlaneDetection.None;
                camMgr.getPointCloud = enablePointCloud;
                camMgr.enableAutoFocus = true;
            });

#elif ARCORE
            this.WithLock(() =>
            {
                var arCoreSession = this.EnsureComponent<ARCoreSession>().Value;
                arCoreSession.SessionConfig = ScriptableObject.CreateInstance<ARCoreSessionConfig>();
                arCoreSession.SessionConfig.MatchCameraFramerate = true;

                var poseDriver = this.EnsureComponent<TrackedPoseDriver>().Value;
                poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.ColorCamera);
                poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                poseDriver.updateType = TrackedPoseDriver.UpdateType.BeforeRender;
                poseDriver.UseRelativeTransform = true;

                var bgRenderer = this.EnsureComponent<ARCoreBackgroundRenderer>().Value;

#if UNITY_EDITOR
                if (ARBackgroundMaterial == null || reset)
                {
                    ARBackgroundMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(
                        System.IO.PathExt.FixPath(
                            "Assets/GoogleARCore/SDK/Materials/ARBackground.mat"));
                }
#endif
                bgRenderer.BackgroundMaterial = ARBackgroundMaterial;
            });

#elif OCULUS
            var mgr = this.EnsureComponent<OVRManager>();
            if (mgr.IsNew)
            {
                mgr.Value.useRecommendedMSAALevel = true;
                mgr.Value.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
                mgr.Value.usePositionTracking = true;
                mgr.Value.useRotationTracking = true;
                mgr.Value.useIPDInPositionTracking = true;
                mgr.Value.resetTrackerOnLoad = false;
                mgr.Value.AllowRecenter = false;
                mgr.Value.chromatic = true;
            }

            if (Application.isPlaying)
            {
                if (OVRManager.tiledMultiResSupported)
                {
                    OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHigh;
                }

                if (OVRManager.display.displayFrequenciesAvailable.Length > 0)
                {
                    OVRManager.display.displayFrequency = OVRManager.display.displayFrequenciesAvailable.Max();
                }
            }

#elif GOOGLEVR
            this.EnsureComponent<GvrHeadset>();

#elif MAGIC_LEAP
            listener.EnsureComponent<MSAListener>();

#elif WAVEVR

            this.WithLock(() =>
            {
#if RESONANCEAUDIO
                this.RemoveComponent<ResonanceAudioListener>();
#endif

                this.RemoveComponent<AudioListener>();
                listener = ComponentExt.FindAny<AudioListener>();

                var rend = this.EnsureComponent<WaveVR_Render>();
                if (rend.IsNew)
                {
                    if (!rend.Value.isExpanded)
                    {
                        WaveVR_Render.Expand(rend);
                    }
                    rend.Value.cpuPerfLevel = WaveVR_Utils.WVR_PerfLevel.Maximum;
                    rend.Value.gpuPerfLevel = WaveVR_Utils.WVR_PerfLevel.Maximum;
                }

                rend.Value.origin = WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;

                var tracker = this.EnsureComponent<WaveVR_DevicePoseTracker>().Value;
                tracker.type = WVR_DeviceType.WVR_DeviceType_HMD;
                tracker.trackPosition = true;
                tracker.EnableNeckModel = true;
                tracker.trackRotation = true;
                tracker.timing = WVR_TrackTiming.WhenNewPoses;
            });
#endif

#if UNITY_MODULES_AUDIO && RESONANCEAUDIO
            if (listener != null)
            {
                var goog = listener.EnsureComponent<ResonanceAudioListener>().Value;

#if STEAMVR || CARDBOARD || GOOGLEVR || WAVEVR
                goog.stereoSpeakerModeEnabled = Application.isEditor;
#else
                goog.stereoSpeakerModeEnabled = true;
#endif
            }
#endif
#if VUFORIA
            this.WithLock(() =>
            {
                var vuforia = this.EnsureComponent<VuforiaBehaviour>().Value;
                vuforia.enabled = false;
            });
#endif
        }

        public void Uninstall()
        {
#if VUFORIA
            this.RemoveComponent<VuforiaBehaviour>();
#endif

#if RESONANCEAUDIO
            ComponentExt.FindAny<AudioListener>()
                ?.RemoveComponent<ResonanceAudioListener>();
#endif

#if ARKIT && !UNITY_EDITOR
            this.RemoveComponent<UnityARCameraManager>();
            this.RemoveComponent<UnityARCameraNearFar>();
            this.RemoveComponent<UnityARVideo>();

#elif ARCORE
            this.RemoveComponent<ARCoreBackgroundRenderer>();
            this.RemoveComponent<TrackedPoseDriver>();
            this.RemoveComponent<ARCoreSession>();

#elif OCULUS
            this.RemoveComponent<OVRManager>();

#elif GOOGLEVR
            this.RemoveComponent<GvrHeadset>();

#elif MAGIC_LEAP
            this.RemoveComponent<MSAListener>();

#elif WAVEVR
            this.RemoveComponent<WaveVR_DevicePoseTracker>();
            var render = GetComponent<WaveVR_Render>();
            if (render != null && render.isExpanded)
            {
                WaveVR_Render.Collapse(render);
                render.Destroy();
            }

#endif

#if UNITY_MODULES_AUDIO
            this.EnsureComponent<AudioListener>();
#endif
        }

        /// <summary>
        /// Gets the field of view value that should be targeted for the running application.
        /// </summary>
        /// <value>The default fov.</value>
        private static float DEFAULT_FOV
        {
            get
            {
                if (Application.isPlaying)
                {
                    return MainCamera.fieldOfView;
                }
                else
                {
#if HOLOLENS
                    return 26;
#elif MAGIC_LEAP
                    return 30;
#elif VUFORIA
                    return 31;
#else
                    return 50;
#endif
                }
            }
        }

        /// <summary>
        /// The camera field of view along the large screen axis in portrait mode, the narrow axis in
        /// landscape mode.
        /// </summary>
        /// <value>The vertical field of view.</value>
        public float VerticalFieldOfView
        {
            get
            {
                if (Screen.width < Screen.height)
                {
                    var tan = Mathf.Tan(Units.Degrees.Radians(DEFAULT_FOV / 2));
                    var max = Mathf.Max(Screen.width, Screen.height);
                    var min = Mathf.Min(Screen.width, Screen.height);

                    float aspect;
                    if (Screen.height < Screen.width)
                    {
                        aspect = min / max;
                    }
                    else
                    {
                        aspect = max / min;
                    }

                    return 2 * Units.Radians.Degrees(Mathf.Atan(tan * aspect));
                }
                else
                {
                    return DEFAULT_FOV;
                }
            }
        }

        /// <summary>
        /// Updates the camera FOV according to game window dimensions, and moves the camera into
        /// position at the avatar height.
        /// </summary>
        public void Update()
        {
            if (!Mathf.Approximately(MainCamera.fieldOfView, VerticalFieldOfView))
            {
                MainCamera.fieldOfView = VerticalFieldOfView;
            }

#if ARCORE
            if (!isQuitting)
            {
                // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
                if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
                {
                    Device.ShowToastMessage("Camera permission is needed to run this application.");
                    isQuitting = true;
                    MasterSceneController.QuitApp();
                }
                else if (Session.Status.IsError())
                {
                    Device.ShowToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                    isQuitting = true;
                    MasterSceneController.QuitApp();
                }
            }

#elif HOLOLENS && UNITY_5_3_OR_NEWER && (!UNITY_2017_1_OR_NEWER || (UNITY_2017_2_OR_NEWER && !UNITY_2017_3_OR_NEWER))
            bool focusFound = false;
            var camT = MainCamera.transform;
            var origin = camT.position;
            var fwd = camT.forward;
            var arFocus = (from obj in GameObject.FindGameObjectsWithTag("AR Focal Point")
                           where obj.activeInHierarchy
                           select obj.transform)
                .FirstOrDefault();
            if (arFocus != null)
            {
                var center = arFocus.Center();
                focusFound = SetFocus(origin, fwd, center, true);
                if (focusFound)
                {
                    focusPoint = center;

                    directions[0] = arFocus.forward;
                    directions[1] = -arFocus.forward;
                    directions[2] = arFocus.up;
                    directions[3] = -arFocus.up;
                    directions[4] = arFocus.right;
                    directions[5] = -arFocus.right;

                    focusDir = directions
                        .OrderByDescending(dir =>
                            Vector3.Dot(dir, -fwd))
                        .FirstOrDefault();
                }
            }

            if (!focusFound)
            {
                var att = ComponentExt.FindAny<AttentionDirector>();
                if (att != null)
                {
                    focusFound = SetFocus(origin, fwd, att.Target, false);
                }
            }

            if (!focusFound)
            {
                var ray = new Ray(origin, fwd);
                focusDir = -fwd;
                RaycastHit hitinfo;
                if (Physics.Raycast(ray, out hitinfo))
                {
                    focusPoint = hitinfo.point;
                }
                else
                {
                    focusPoint = origin + 10 * fwd;
                }
            }

            // NOTE: the SetFocusPointForFrame feature is currently defective - STM 2018-03-22
            // REF: https://forum.unity.com/threads/holographicsettings-setfocuspointforframe-seems-to-make-things-worse-hololens.514103/
            focusPoint = Vector3.Lerp(lastFocus, focusPoint, 0.1f);
            var rot = Quaternion.FromToRotation(lastNormal, focusDir);
            rot = Quaternion.Slerp(Quaternion.identity, rot, 0.1f);
            focusDir = rot * lastNormal;
            if (focusDir.sqrMagnitude > 0)
            {
                focusDir.Normalize();
                HolographicSettings.SetFocusPointForFrame(focusPoint, focusDir);
            }
            lastFocus = focusPoint;
            lastNormal = focusDir;
        }

        Vector3 focusPoint,
            focusDir,
            lastFocus,
            lastNormal;
        Vector3[] directions = new Vector3[6];

        bool SetFocus(Vector3 origin)
        {
            bool focusFound = MainCamera.IsInView(viewPost);
            if (focusFound)
            {
                focusPoint = viewPos;
            }

            focusDir = MainCamera.transform.position - origin;

            return focusFound;
#endif
        }
    }
}
