using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using ovrResult = System.Int32;

namespace Juniper
{
    public static class NativeMethods
    {
        /// ovrResult isn't actually an enum type and the the success / failure types are not
        /// defined anywhere for GearVR VrApi. This needs to be remedied. For now, I'm defining
        /// these here and will try to address this larger issue in a follow-on changeset.
        /// errors are < 0, successes are >= 0
        /// Except where noted, these match error codes from PC CAPI.
        public enum ovrSuccessResult
        {
            ovrSuccess = 0,
            ovrSuccess_BoundaryInvalid = 1001,
            ovrSuccess_EventUnavailable = 1002,
        }


        public enum ovrErrorResult
        {
            ovrError_MemoryAllocationFailure = -1000,
            ovrError_NotInitialized = -1004,
            ovrError_InvalidParameter = -1005,
            ovrError_DeviceUnavailable = -1010, //< device is not connected,
                                                // or not connected as input device
            ovrError_InvalidOperation = -1015,

            // enums not in CAPI
            ovrError_UnsupportedDeviceType = -1050, //< specified device type isn't supported on GearVR
            ovrError_NoDevice = -1051, //< specified device ID does not map to any current device
            ovrError_NotImplemented = -1052, //< executed an incomplete code path - this should not be
                                             // possible in public releases.

            ovrResult_EnumSize = 0x7fffffff
        }
        /// Java details about an activity
        public unsafe struct ovrJava
        {
            public JNIInvokeInterface** Vm; //< Java Virtual Machine
            public JNINativeInterface** Env; //< Thread specific environment
            public IntPtr ActivityObject; //< Java activity object
        }


        /// A 2D vector.
        public struct ovrVector2f
        {
            public float x, y;
        }



        /// A 3D vector.
        public struct ovrVector3f
        {
            public float x, y, z;
        }



        /// A 4D vector.
        public struct ovrVector4f
        {
            public float x, y, z, w;
        }



        public struct ovrVector4s
        {
            public short x, y, z, w;
        }



        /// Quaternion.
        public struct ovrQuatf
        {
            public float x, y, z, w;
        }



        /// Row-major 4x4 matrix.
        public struct ovrMatrix4f
        {
            public float[][] M;
        }

        [StructLayout(LayoutKind.Explicit)]
        /// Position and orientation together.
        public struct ovrPosef
        {
            [FieldOffset(0)]
            public ovrQuatf Orientation;

            [FieldOffset(16)]
            public ovrVector3f Position;

            [FieldOffset(16)]
            public ovrVector3f Translation;
        }



        /// A rectangle with 2D size and position.
        public struct ovrRectf
        {
            public float x;
            public float y;
            public float width;
            public float height;
        }



        /// True or false.
        public enum ovrBooleanResult { VRAPI_FALSE = 0, VRAPI_TRUE = 1 }


        /// One of the user's eyes.
        public enum ovrEye { VRAPI_EYE_LEFT = 0, VRAPI_EYE_RIGHT = 1, VRAPI_EYE_COUNT = 2 }


        //-----------------------------------------------------------------
        // Structure Types
        //-----------------------------------------------------------------

        /// Defines a layout for ovrInitParms, ovrModeParms, or ovrFrameParms.
        public enum ovrStructureType
        {
            VRAPI_STRUCTURE_TYPE_INIT_PARMS = 1,
            VRAPI_STRUCTURE_TYPE_MODE_PARMS = 2,
            VRAPI_STRUCTURE_TYPE_FRAME_PARMS = 3,
            VRAPI_STRUCTURE_TYPE_MODE_PARMS_VULKAN = 5,
        }


        //-----------------------------------------------------------------
        // System Properties and Status
        //-----------------------------------------------------------------

        /// A VR-capable device.
        public enum ovrDeviceType
        {

            // Standalone Devices
            VRAPI_DEVICE_TYPE_OCULUSGO_START = 64,
            VRAPI_DEVICE_TYPE_OCULUSGO = VRAPI_DEVICE_TYPE_OCULUSGO_START,
            VRAPI_DEVICE_TYPE_MIVR_STANDALONE = VRAPI_DEVICE_TYPE_OCULUSGO_START + 1, //< China-only SKU
            VRAPI_DEVICE_TYPE_OCULUSGO_END = 127,

            VRAPI_DEVICE_TYPE_OCULUSQUEST_START = 256,
            VRAPI_DEVICE_TYPE_OCULUSQUEST = VRAPI_DEVICE_TYPE_OCULUSQUEST_START + 3,
            VRAPI_DEVICE_TYPE_OCULUSQUEST_END = 319,



            VRAPI_DEVICE_TYPE_UNKNOWN = -1,
        }


        /// A headset, which typically includes optics and tracking hardware, but not necessarily the device
        /// itself.
        public enum ovrHeadsetType
        {

            // Standalone Headsets
            VRAPI_HEADSET_TYPE_OCULUSGO = 64, //< Oculus Go
            VRAPI_HEADSET_TYPE_MIVR_STANDALONE = 65, //< China-only SKU

            VRAPI_HEADSET_TYPE_OCULUSQUEST = 256,


            VRAPI_HEADSET_TYPE_UNKNOWN = -1,
        }


        /// A geographic region authorized for certain hardware and content.
        public enum ovrDeviceRegion
        {
            VRAPI_DEVICE_REGION_UNSPECIFIED = 0,
            VRAPI_DEVICE_REGION_JAPAN = 1,
            VRAPI_DEVICE_REGION_CHINA = 2,
        }


        /// The maximum resolution and framerate supported by a video decoder.
        public enum ovrVideoDecoderLimit
        {
            VRAPI_VIDEO_DECODER_LIMIT_4K_30FPS = 0,
            VRAPI_VIDEO_DECODER_LIMIT_4K_60FPS = 1,
        }


        /// Emulation mode for applications developed on different devices
        /// for determining if running in emulation mode at all test against !=
        /// VRAPI_DEVICE_EMULATION_MODE_NONE
        public enum ovrDeviceEmulationMode
        {
            VRAPI_DEVICE_EMULATION_MODE_NONE = 0,
            VRAPI_DEVICE_EMULATION_MODE_GO_ON_QUEST = 1,
        }


        /// System configuration properties.
        public enum ovrSystemProperty
        {
            VRAPI_SYS_PROP_DEVICE_TYPE = 0,
            VRAPI_SYS_PROP_MAX_FULLSPEED_FRAMEBUFFER_SAMPLES = 1,
            /// Physical width and height of the display in pixels.
            VRAPI_SYS_PROP_DISPLAY_PIXELS_WIDE = 2,
            VRAPI_SYS_PROP_DISPLAY_PIXELS_HIGH = 3,
            /// Returns the refresh rate of the display in cycles per second.
            VRAPI_SYS_PROP_DISPLAY_REFRESH_RATE = 4,
            /// With a display resolution of 2560x1440, the pixels at the center
            /// of each eye cover about 0.06 degrees of visual arc. To wrap a
            /// full 360 degrees, about 6000 pixels would be needed and about one
            /// quarter of that would be needed for ~90 degrees FOV. As such, Eye
            /// images with a resolution of 1536x1536 result in a good 1:1 mapping
            /// in the center, but they need mip-maps for off center pixels. To
            /// avoid the need for mip-maps and for significantly improved rendering
            /// performance this currently returns a conservative 1024x1024.
            VRAPI_SYS_PROP_SUGGESTED_EYE_TEXTURE_WIDTH = 5,
            VRAPI_SYS_PROP_SUGGESTED_EYE_TEXTURE_HEIGHT = 6,
            /// This is a product of the lens distortion and the screen size,
            /// but there is no truly correct answer.
            /// There is a tradeoff in resolution and coverage.
            /// Too small of an FOV will leave unrendered pixels visible, but too
            /// large wastes resolution or fill rate.  It is unreasonable to
            /// increase it until the corners are completely covered, but we do
            /// want most of the outside edges completely covered.
            /// Applications might choose to render a larger FOV when angular
            /// acceleration is high to reduce black pull in at the edges by
            /// the time warp.
            /// Currently symmetric 90.0 degrees.
            VRAPI_SYS_PROP_SUGGESTED_EYE_FOV_DEGREES_X = 7,
            VRAPI_SYS_PROP_SUGGESTED_EYE_FOV_DEGREES_Y = 8,
            /// Path to the external SD card. On Android-M, this path is dynamic and can
            /// only be determined once the SD card is mounted. Returns an empty string if
            /// device does not support an ext sdcard or if running Android-M and the SD card
            /// is not mounted.
            VRAPI_SYS_PROP_EXT_SDCARD_PATH = 9,
            VRAPI_SYS_PROP_DEVICE_REGION = 10,
            /// Video decoder limit for the device.
            VRAPI_SYS_PROP_VIDEO_DECODER_LIMIT = 11,
            VRAPI_SYS_PROP_HEADSET_TYPE = 12,

            // enum 13 used to be VRAPI_SYS_PROP_BACK_BUTTON_SHORTPRESS_TIME
            // enum 14 used to be VRAPI_SYS_PROP_BACK_BUTTON_DOUBLETAP_TIME

            /// Returns an ovrHandedness enum indicating left or right hand.
            VRAPI_SYS_PROP_DOMINANT_HAND = 15,

            /// Returns VRAPI_TRUE if the system supports orientation tracking.
            VRAPI_SYS_PROP_HAS_ORIENTATION_TRACKING = 16,
            /// Returns VRAPI_TRUE if the system supports positional tracking.
            VRAPI_SYS_PROP_HAS_POSITION_TRACKING = 17,

            /// Returns the number of display refresh rates supported by the system.
            VRAPI_SYS_PROP_NUM_SUPPORTED_DISPLAY_REFRESH_RATES = 64,
            /// Returns an array of the supported display refresh rates.
            VRAPI_SYS_PROP_SUPPORTED_DISPLAY_REFRESH_RATES = 65,

            /// Returns the number of swapchain texture formats supported by the system.
            VRAPI_SYS_PROP_NUM_SUPPORTED_SWAPCHAIN_FORMATS = 66,
            /// Returns an array of the supported swapchain formats.
            /// Formats are platform specific. For GLES, this is an array of
            /// GL internal formats.
            VRAPI_SYS_PROP_SUPPORTED_SWAPCHAIN_FORMATS = 67,

            /// Returns VRAPI_TRUE if Multiview rendering support is available for this system,
            /// otherwise VRAPI_FALSE.
            VRAPI_SYS_PROP_MULTIVIEW_AVAILABLE = 128,

            /// Returns VRAPI_TRUE if submission of SRGB Layers is supported for this system,
            /// otherwise VRAPI_FALSE.
            VRAPI_SYS_PROP_SRGB_LAYER_SOURCE_AVAILABLE = 129,

            /// Returns VRAPI_TRUE if on-chip foveated rendering of swapchains is supported
            /// for this system, otherwise VRAPI_FALSE.
            VRAPI_SYS_PROP_FOVEATION_AVAILABLE = 130,
        }


        /// Configurable VrApi properties.
        public enum ovrProperty
        {
            VRAPI_FOVEATION_LEVEL = 15, //< Used by apps that want to control swapchain foveation levels.
            VRAPI_REORIENT_HMD_ON_CONTROLLER_RECENTER =
            17, //< Used to determine if a controller recenter should also reorient the headset.
            VRAPI_LATCH_BACK_BUTTON_ENTIRE_FRAME =
                18, //< Used to determine if the 'short press' back button should lasts an entire frame.
            VRAPI_BLOCK_REMOTE_BUTTONS_WHEN_NOT_EMULATING_HMT =
                19, //< Used to not send the remote back button java events to the apps.
            VRAPI_EAT_NATIVE_GAMEPAD_EVENTS =
                20, //< Used to tell the runtime not to eat gamepad events.  If this is false on a native
                    // app, the app must be listening for the events.
            VRAPI_ACTIVE_INPUT_DEVICE_ID = 24, //< Used by apps to query which input device is most 'active'
                                               // or primary, a -1 means no active input device
            VRAPI_DEVICE_EMULATION_MODE = 29, //< Used by apps to determine if they are running in an
                                              // emulation mode. Is a ovrDeviceEmulationMode value

            VRAPI_DYNAMIC_FOVEATION_ENABLED =
                30, //< Used by apps to enable / disable dynamic foveation adjustments.
        }



        /// Specifies left or right handedness.
        public enum ovrHandedness
        {
            VRAPI_HAND_UNKNOWN = 0,
            VRAPI_HAND_LEFT = 1,
            VRAPI_HAND_RIGHT = 2
        }


        /// System status bits.
        public enum ovrSystemStatus
        {
            VRAPI_SYS_STATUS_DOCKED = 0, //< Device is docked.
            VRAPI_SYS_STATUS_MOUNTED = 1, //< Device is mounted.
            VRAPI_SYS_STATUS_THROTTLED = 2, //< Device is in powersave mode.

            // enum  3 used to be VRAPI_SYS_STATUS_THROTTLED2.

            // enum  4 used to be VRAPI_SYS_STATUS_THROTTLED_WARNING_LEVEL.

            VRAPI_SYS_STATUS_RENDER_LATENCY_MILLISECONDS =
                5, //< Average time between render tracking sample and scanout.
            VRAPI_SYS_STATUS_TIMEWARP_LATENCY_MILLISECONDS =
                6, //< Average time between timewarp tracking sample and scanout.
            VRAPI_SYS_STATUS_SCANOUT_LATENCY_MILLISECONDS = 7, //< Average time between Vsync and scanout.
            VRAPI_SYS_STATUS_APP_FRAMES_PER_SECOND =
                8, //< Number of frames per second delivered through vrapi_SubmitFrame.
            VRAPI_SYS_STATUS_SCREEN_TEARS_PER_SECOND = 9, //< Number of screen tears per second (per eye).
            VRAPI_SYS_STATUS_EARLY_FRAMES_PER_SECOND =
                10, //< Number of frames per second delivered a whole display refresh early.
            VRAPI_SYS_STATUS_STALE_FRAMES_PER_SECOND = 11, //< Number of frames per second delivered late.

            // enum 12 used to be VRAPI_SYS_STATUS_HEADPHONES_PLUGGED_IN

            VRAPI_SYS_STATUS_RECENTER_COUNT = 13, //< Returns the current HMD recenter count. Defaults to 0.
            VRAPI_SYS_STATUS_SYSTEM_UX_ACTIVE = 14, //< Returns VRAPI_TRUE if a system UX layer is active
            VRAPI_SYS_STATUS_USER_RECENTER_COUNT = 15, //< Returns the current HMD recenter count for user
                                                       // initiated recenters only. Defaults to 0.


            VRAPI_SYS_STATUS_FRONT_BUFFER_PROTECTED =
                128, //< VRAPI_TRUE if the front buffer is allocated in TrustZone memory.
            VRAPI_SYS_STATUS_FRONT_BUFFER_565 = 129, //< VRAPI_TRUE if the front buffer is 16-bit 5:6:5
            VRAPI_SYS_STATUS_FRONT_BUFFER_SRGB =
                130, //< VRAPI_TRUE if the front buffer uses the sRGB color space.

        }


        //-----------------------------------------------------------------
        // Initialization
        //-----------------------------------------------------------------

        /// Possible results of initialization.
        public enum ovrInitializeStatus
        {
            VRAPI_INITIALIZE_SUCCESS = 0,
            VRAPI_INITIALIZE_UNKNOWN_ERROR = -1,
            VRAPI_INITIALIZE_PERMISSIONS_ERROR = -2,
            VRAPI_INITIALIZE_ALREADY_INITIALIZED = -3,
            VRAPI_INITIALIZE_SERVICE_CONNECTION_FAILED = -4,
            VRAPI_INITIALIZE_DEVICE_NOT_SUPPORTED = -5,
        }


        /// Supported graphics APIs.
        public enum ovrGraphicsAPI
        {
            VRAPI_GRAPHICS_API_TYPE_OPENGL_ES = 0x10000,
            VRAPI_GRAPHICS_API_OPENGL_ES_2 =
                (VRAPI_GRAPHICS_API_TYPE_OPENGL_ES | 0x0200), //< OpenGL ES 2.x context
            VRAPI_GRAPHICS_API_OPENGL_ES_3 =
                (VRAPI_GRAPHICS_API_TYPE_OPENGL_ES | 0x0300), //< OpenGL ES 3.x context

            VRAPI_GRAPHICS_API_TYPE_OPENGL = 0x20000,
            VRAPI_GRAPHICS_API_OPENGL_COMPAT =
                (VRAPI_GRAPHICS_API_TYPE_OPENGL | 0x0100), //< OpenGL Compatibility Profile
            VRAPI_GRAPHICS_API_OPENGL_CORE_3 =
                (VRAPI_GRAPHICS_API_TYPE_OPENGL | 0x0300), //< OpenGL Core Profile 3.x
            VRAPI_GRAPHICS_API_OPENGL_CORE_4 =
                (VRAPI_GRAPHICS_API_TYPE_OPENGL | 0x0400), //< OpenGL Core Profile 4.x

            VRAPI_GRAPHICS_API_TYPE_VULKAN = 0x40000,
            VRAPI_GRAPHICS_API_VULKAN_1 = (VRAPI_GRAPHICS_API_TYPE_VULKAN | 0x0100), //< Vulkan 1.x
        }


        /// Configuration details specified at initialization.
        public struct ovrInitParms
        {
            ovrStructureType Type;
            int ProductVersion;
            int MajorVersion;
            int MinorVersion;
            int PatchVersion;
            ovrGraphicsAPI GraphicsAPI;
            ovrJava Java;
        }




        //-----------------------------------------------------------------
        // VR Mode
        //-----------------------------------------------------------------

        /// \note the first two flags use the first two bytes for backwards compatibility on little endian
        /// systems.
        public enum ovrModeFlags
        {
            /// When an application moves backwards on the activity stack,
            /// the activity window it returns to is no longer flagged as fullscreen.
            /// As a result, Android will also render the decor view, which wastes a
            /// significant amount of bandwidth.
            /// By setting this flag, the fullscreen flag is reset on the window.
            /// Unfortunately, this causes Android life cycle events that mess up
            /// several NativeActivity codebases like Stratum and UE4, so this
            /// flag should only be set for specific applications.
            /// Use "adb shell dumpsys SurfaceFlinger" to verify
            /// that there is only one HWC next to the FB_TARGET.
            VRAPI_MODE_FLAG_RESET_WINDOW_FULLSCREEN = 0x0000FF00,

            /// The WindowSurface passed in is an ANativeWindow.
            VRAPI_MODE_FLAG_NATIVE_WINDOW = 0x00010000,

            /// Create the front buffer in TrustZone memory to allow protected DRM
            /// content to be rendered to the front buffer. This functionality
            /// requires the WindowSurface to be allocated from TimeWarp, via
            /// specifying the nativeWindow via VRAPI_MODE_FLAG_NATIVE_WINDOW.
            VRAPI_MODE_FLAG_FRONT_BUFFER_PROTECTED = 0x00020000,

            /// Create a 16-bit 5:6:5 front buffer.
            VRAPI_MODE_FLAG_FRONT_BUFFER_565 = 0x00040000,

            /// Create a front buffer using the sRGB color space.
            VRAPI_MODE_FLAG_FRONT_BUFFER_SRGB = 0x00080000,

            /// If set, indicates the OpenGL ES Context was created with EGL_CONTEXT_OPENGL_NO_ERROR_KHR
            /// attribute. The same attribute would be applied when TimeWrap creates the shared context.
            /// More information could be found at:
            /// https://www.khronos.org/registry/EGL/extensions/KHR/EGL_KHR_create_context_no_error.txt
            VRAPI_MODE_FLAG_CREATE_CONTEXT_NO_ERROR = 0x00100000
        }


        /// Configuration details that stay constant between a vrapi_EnterVrMode()/vrapi_LeaveVrMode() pair.
        public struct ovrModeParms
        {
            ovrStructureType Type;

            /// Combination of ovrModeFlags flags.
            uint Flags;

            /// The Java VM is needed for the time warp thread to create a Java environment.
            /// A Java environment is needed to access various system services. The thread
            /// that enters VR mode is responsible for attaching and detaching the Java
            /// environment. The Java Activity object is needed to get the windowManager,
            /// packageName, systemService, etc.
            ovrJava Java;

            OVR_VRAPI_PADDING_32_BIT(4)

    /// Display to use for asynchronous time warp rendering.
    /// Using EGL this is an EGLDisplay.
            ulong Display;

            /// The window surface to use for asynchronous time warp rendering.
            /// Using EGL this can be the EGLSurface created by the application for the ANativeWindow.
            /// This should be the ANativeWIndow itself (requires VRAPI_MODE_FLAG_NATIVE_WINDOW).
            ulong WindowSurface;

            /// The resources from this context will be shared with the asynchronous time warp.
            /// Using EGL this is an EGLContext.
            ulong ShareContext;
        }



        /// Vulkan-specific mode paramaters.
        public struct ovrModeParmsVulkan
        {
            ovrModeParms ModeParms;

            /// For Vulkan, this should be the VkQueue created on the same Device as specified
            /// by vrapi_CreateSystemVulkan. An internally created VkFence object will be signaled
            /// by the completion of commands on the queue.
            ulong SynchronizationQueue;
        }



        /// VR context
        /// To allow multiple Android activities that live in the same address space
        /// to cooperatively use the VrApi, each activity needs to maintain its own
        /// separate contexts for a lot of the video related systems.
        struct ovrMobile { }

        //-----------------------------------------------------------------
        // Tracking
        //-----------------------------------------------------------------

        /// Full rigid body pose with first and second derivatives.
        public struct ovrRigidBodyPosef
        {
            ovrPosef Pose;
            ovrVector3f AngularVelocity;
            ovrVector3f LinearVelocity;
            ovrVector3f AngularAcceleration;
            ovrVector3f LinearAcceleration;
            OVR_VRAPI_PADDING(4)
            double TimeInSeconds; //< Absolute time of this pose.
            double PredictionInSeconds; //< Seconds this pose was predicted ahead.
        }



        /// Bit flags describing the current status of sensor tracking.
        public enum ovrTrackingStatus
        {
            VRAPI_TRACKING_STATUS_ORIENTATION_TRACKED = 1 << 0, //< Orientation is currently tracked.
            VRAPI_TRACKING_STATUS_POSITION_TRACKED = 1 << 1, //< Position is currently tracked.
            VRAPI_TRACKING_STATUS_ORIENTATION_VALID = 1 << 2, //< Orientation reported is valid.
            VRAPI_TRACKING_STATUS_POSITION_VALID = 1 << 3, //< Position reported is valid.
            VRAPI_TRACKING_STATUS_HMD_CONNECTED = 1 << 7 //< HMD is available & connected.
        }


        /// Tracking state at a given absolute time.
        public struct ovrTracking2
        {
            /// Sensor status described by ovrTrackingStatus flags.
            uint Status;

            OVR_VRAPI_PADDING(4)

    /// Predicted head configuration at the requested absolute time.
    /// The pose describes the head orientation and center eye position.
            ovrRigidBodyPosef HeadPose;
            struct {
        ovrMatrix4f ProjectionMatrix;
            ovrMatrix4f ViewMatrix;
        }
        Eye[VRAPI_EYE_COUNT];
}



    /// Reports the status and pose of a motion tracker.
    public struct ovrTracking
    {
        /// Sensor status described by ovrTrackingStatus flags.
        uint Status;

        OVR_VRAPI_PADDING(4)

    /// Predicted head configuration at the requested absolute time.
    /// The pose describes the head orientation and center eye position.
        ovrRigidBodyPosef HeadPose;
    }



    /// Specifies a reference frame for motion tracking data.
    public enum ovrTrackingTransform
    {
        VRAPI_TRACKING_TRANSFORM_IDENTITY = 0,
        VRAPI_TRACKING_TRANSFORM_CURRENT = 1,
        VRAPI_TRACKING_TRANSFORM_SYSTEM_CENTER_EYE_LEVEL = 2,
        VRAPI_TRACKING_TRANSFORM_SYSTEM_CENTER_FLOOR_LEVEL = 3,
    }


    public enum ovrTrackingSpace
    {
        VRAPI_TRACKING_SPACE_LOCAL = 0, // Eye level origin - controlled by system recentering
        VRAPI_TRACKING_SPACE_LOCAL_FLOOR = 1, // Floor level origin - controlled by system recentering
        VRAPI_TRACKING_SPACE_LOCAL_TILTED =
            2, // Tilted pose for "bed mode" - controlled by system recentering
        VRAPI_TRACKING_SPACE_STAGE = 3, // Floor level origin - controlled by Guardian setup
        VRAPI_TRACKING_SPACE_LOCAL_FIXED_YAW = 7, // Position of local space, but yaw stays constant
    }


    /// Tracked device type id used to simplify interaction checks with Guardian
    public enum ovrTrackedDeviceTypeId
    {
        VRAPI_TRACKED_DEVICE_NONE = -1,
        VRAPI_TRACKED_DEVICE_HMD = 0, //< Headset
        VRAPI_TRACKED_DEVICE_HAND_LEFT = 1, //< Left controller
        VRAPI_TRACKED_DEVICE_HAND_RIGHT = 2, //< Right controller
        VRAPI_NUM_TRACKED_DEVICES = 3,
    }


    /// Guardian boundary trigger state information based on a given tracked device type
    public struct ovrBoundaryTriggerResult
    {
        /// Closest point on the boundary surface.
        ovrVector3f ClosestPoint;

        /// Normal of the closest point on the boundary surface.
        ovrVector3f ClosestPointNormal;

        /// Distance to the closest guardian boundary surface.
        float ClosestDistance;

        /// True if the boundary system is being triggered. Note that due to fade in/out effects this
        /// may not exactly match visibility.
        bool IsTriggering;
    }




    //-----------------------------------------------------------------
    // Texture Swap Chain
    //-----------------------------------------------------------------

    /// A texture type, such as 2D, array, or cubemap.
    public enum ovrTextureType
    {
        VRAPI_TEXTURE_TYPE_2D = 0, //< 2D textures.
        VRAPI_TEXTURE_TYPE_2D_ARRAY = 2, //< Texture array.
        VRAPI_TEXTURE_TYPE_CUBE = 3, //< Cube maps.
        VRAPI_TEXTURE_TYPE_MAX = 4,
    }


    /// A texture format.
    /// DEPRECATED in favor of passing platform-specific formats to vrapi_CreateTextureSwapChain3.
    public enum ovrTextureFormat
    {
        VRAPI_TEXTURE_FORMAT_NONE = 0,
        VRAPI_TEXTURE_FORMAT_565 = 1,
        VRAPI_TEXTURE_FORMAT_5551 = 2,
        VRAPI_TEXTURE_FORMAT_4444 = 3,
        VRAPI_TEXTURE_FORMAT_8888 = 4,
        VRAPI_TEXTURE_FORMAT_8888_sRGB = 5,
        VRAPI_TEXTURE_FORMAT_RGBA16F = 6,
        VRAPI_TEXTURE_FORMAT_DEPTH_16 = 7,
        VRAPI_TEXTURE_FORMAT_DEPTH_24 = 8,
        VRAPI_TEXTURE_FORMAT_DEPTH_24_STENCIL_8 = 9,
        VRAPI_TEXTURE_FORMAT_RG16 = 10,

    }



    /// Built-in convenience swapchains.
    public enum ovrDefaultTextureSwapChain
    {
        VRAPI_DEFAULT_TEXTURE_SWAPCHAIN = 0x1,
        VRAPI_DEFAULT_TEXTURE_SWAPCHAIN_LOADING_ICON = 0x2
    }


    struct ovrTextureSwapChain { };

    //-----------------------------------------------------------------
    // Frame Submission
    //-----------------------------------------------------------------

    /// Per-frame configuration options.
    public enum ovrFrameFlags
    {
        // enum 1 << 0 used to be VRAPI_FRAME_FLAG_INHIBIT_SRGB_FRAMEBUFFER. See per-layer
        // flag VRAPI_FRAME_LAYER_FLAG_INHIBIT_SRGB_FRAMEBUFFER.

        /// Flush the warp swap pipeline so the images show up immediately.
        /// This is expensive and should only be used when an immediate transition
        /// is needed like displaying black when resetting the HMD orientation.
        VRAPI_FRAME_FLAG_FLUSH = 1 << 1,
        /// This is the final frame. Do not accept any more frames after this.
        VRAPI_FRAME_FLAG_FINAL = 1 << 2,

        /// enum 1 << 3 used to be VRAPI_FRAME_FLAG_TIMEWARP_DEBUG_GRAPH_SHOW.

        /// enum 1 << 4 used to be VRAPI_FRAME_FLAG_TIMEWARP_DEBUG_GRAPH_FREEZE.

        /// enum 1 << 5 used to be VRAPI_FRAME_FLAG_TIMEWARP_DEBUG_GRAPH_LATENCY_MODE.

        /// Don't show the volume layer when set.
        VRAPI_FRAME_FLAG_INHIBIT_VOLUME_LAYER = 1 << 6,

        /// enum 1 << 7 used to be VRAPI_FRAME_FLAG_SHOW_LAYER_COMPLEXITY.

        /// enum 1 << 8 used to be VRAPI_FRAME_FLAG_SHOW_TEXTURE_DENSITY.

    }


    /// Per-frame configuration options that apply to a particular layer.
    public enum ovrFrameLayerFlags
    {
        /// enum 1 << 0 used to be VRAPI_FRAME_LAYER_FLAG_WRITE_ALPHA.

        /// NOTE: On Oculus standalone devices, chromatic aberration correction is enabled
        /// by default.
        /// For non Oculus standalone devices, this must be explicitly enabled by specifying the layer
        /// flag as it is a quality / performance trade off.
        VRAPI_FRAME_LAYER_FLAG_CHROMATIC_ABERRATION_CORRECTION = 1 << 1,
        /// Used for some HUDs, but generally considered bad practice.
        VRAPI_FRAME_LAYER_FLAG_FIXED_TO_VIEW = 1 << 2,
        /// Spin the layer - for loading icons
        VRAPI_FRAME_LAYER_FLAG_SPIN = 1 << 3,
        /// Clip fragments outside the layer's TextureRect
        VRAPI_FRAME_LAYER_FLAG_CLIP_TO_TEXTURE_RECT = 1 << 4,

        /// To get gamma correct sRGB filtering of the eye textures, the textures must be
        /// allocated with GL_SRGB8_ALPHA8 format and the window surface must be allocated
        /// with these attributes:
        /// EGL_GL_COLORSPACE_KHR,  EGL_GL_COLORSPACE_SRGB_KHR
        ///
        /// While we can reallocate textures easily enough, we can't change the window
        /// colorspace without relaunching the entire application, so if you want to
        /// be able to toggle between gamma correct and incorrect, you must allocate
        /// the framebuffer as sRGB, then inhibit that processing when using normal
        /// textures.
        ///
        /// If the texture being read isn't an sRGB texture, the conversion
        /// on write must be inhibited or the colors are washed out.
        /// This is necessary for using external images on an sRGB framebuffer.
        VRAPI_FRAME_LAYER_FLAG_INHIBIT_SRGB_FRAMEBUFFER = 1 << 8,


        /// Allow Layer to use an expensive filtering mode. Only useful for 2D layers that are high
        /// resolution (e.g. a remote desktop layer), typically double or more the target resolution.
        VRAPI_FRAME_LAYER_FLAG_FILTER_EXPENSIVE = 1 << 19,


    }


    /// The user's eye (left or right) that can see a layer.
    public enum ovrFrameLayerEye
    {
        VRAPI_FRAME_LAYER_EYE_LEFT = 0,
        VRAPI_FRAME_LAYER_EYE_RIGHT = 1,
        VRAPI_FRAME_LAYER_EYE_MAX = 2
    }


    /// Selects an operation for alpha blending two images.
    public enum ovrFrameLayerBlend
    {
        VRAPI_FRAME_LAYER_BLEND_ZERO = 0,
        VRAPI_FRAME_LAYER_BLEND_ONE = 1,
        VRAPI_FRAME_LAYER_BLEND_SRC_ALPHA = 2,
        /// enum 3 used to be VRAPI_FRAME_LAYER_BLEND_DST_ALPHA.
        /// enum 4 used to be VRAPI_FRAME_LAYER_BLEND_ONE_MINUS_DST_ALPHA.
        VRAPI_FRAME_LAYER_BLEND_ONE_MINUS_SRC_ALPHA = 5
    }


    /// Extra latency mode pipelines app CPU work a frame ahead of VR composition.
    public enum ovrExtraLatencyMode
    {
        VRAPI_EXTRA_LATENCY_MODE_OFF = 0,
        VRAPI_EXTRA_LATENCY_MODE_ON = 1,
        VRAPI_EXTRA_LATENCY_MODE_DYNAMIC = 2
    }


    //-------------------------------------
    // Legacy monolithic FrameParm submission structures for vrapi_SubmitFrame.
    //-------------------------------------

    /// \deprecated The vrapi_SubmitFrame2 path with flexible layer types
    /// should be used instead.
    public enum ovrFrameLayerType { VRAPI_FRAME_LAYER_TYPE_MAX = 4 }


    /// A compositor layer.
    /// \note Any layer textures that are dynamic must be triple buffered.
    /// \deprecated The vrapi_SubmitFrame2 path with flexible layer types
    /// should be used instead.
    public unsafe struct ovrFrameLayerTexture
    {
        /// Because OpenGL ES does not support clampToBorder, it is the
        /// application's responsibility to make sure that all mip levels
        /// of the primary eye texture have a black border that will show
        /// up when time warp pushes the texture partially off screen.
        ovrTextureSwapChain* ColorTextureSwapChain;

        /// \deprecated The depth texture is optional for positional time warp.
        ovrTextureSwapChain* DepthTextureSwapChain;

        /// Index to the texture from the set that should be displayed.
        int TextureSwapChainIndex;

        /// Points on the screen are mapped by a distortion correction
        /// function into ( TanX, TanY, -1, 1 ) vectors that are transformed
        /// by this matrix to get ( S, T, Q, _ ) vectors that are looked
        /// up with texture2dproj() to get texels.
        ovrMatrix4f TexCoordsFromTanAngles;

        /// Only texels within this range should be drawn.
        /// This is a sub-rectangle of the [(0,0)-(1,1)] texture coordinate range.
        ovrRectf TextureRect;

        OVR_VRAPI_PADDING(4)

    /// The tracking state for which ModelViewMatrix is correct.
    /// It is ok to update the orientation for each eye, which
    /// can help minimize black edge pull-in, but the position
    /// must remain the same for both eyes, or the position would
    /// seem to judder "backwards in time" if a frame is dropped.
        ovrRigidBodyPosef HeadPose;

        /// \unused parameter.
        unsigned char Pad[8];
    }



    /// Per-frame state of a compositor layer.
    /// \deprecated The vrapi_SubmitFrame2 path with flexible layer types
    /// should be used instead.
    public struct ovrFrameLayer
    {
        /// Image used for each eye.
        ovrFrameLayerTexture Textures[VRAPI_FRAME_LAYER_EYE_MAX];

        /// Speed and scale of rotation when VRAPI_FRAME_LAYER_FLAG_SPIN is set in ovrFrameLayer::Flags
        float SpinSpeed; //< Radians/Second
        float SpinScale;

        /// Color scale for this layer (including alpha)
        float ColorScale;

        /// padding for deprecated variable.
        OVR_VRAPI_PADDING(4)

    /// Layer blend function.
        ovrFrameLayerBlend SrcBlend;
        ovrFrameLayerBlend DstBlend;

        /// Combination of ovrFrameLayerFlags flags.
        int Flags;

        /// explicit padding for x86
        OVR_VRAPI_PADDING_32_BIT(4)
    }



    /// Configuration parameters that affect system performance and scheduling behavior.
    /// \deprecated The vrapi_SubmitFrame2 path with flexible layer types
    /// should be used instead.
    public struct ovrPerformanceParms
    {
        /// These are fixed clock levels in the range [0, 3].
        int CpuLevel;
        int GpuLevel;

        /// These threads will get SCHED_FIFO.
        int MainThreadTid;
        int RenderThreadTid;
    }



    /// Per-frame details.
    /// \deprecated The vrapi_SubmitFrame2 path with flexible layer types
    /// should be used instead.
    public struct ovrFrameParms
    {
        ovrStructureType Type;

        OVR_VRAPI_PADDING(4)

    /// Layers composited in the time warp.
        ovrFrameLayer Layers[VRAPI_FRAME_LAYER_TYPE_MAX];
        int LayerCount;

        /// Combination of ovrFrameFlags flags.
        int Flags;

        /// Application controlled frame index that uniquely identifies this particular frame.
        /// This must be the same frame index that was passed to vrapi_GetPredictedDisplayTime()
        /// when synthesis of this frame started.
        long long FrameIndex;

        /// WarpSwap will not return until at least this many V-syncs have
        /// passed since the previous WarpSwap returned.
        /// Setting to 2 will reduce power consumption and may make animation
        /// more regular for applications that can't hold full frame rate.
        int SwapInterval;

        /// Latency Mode.
        ovrExtraLatencyMode ExtraLatencyMode;

        /// \unused parameter.
        ovrMatrix4f Reserved;

        /// \unused parameter.
        void* Reserved1;

        /// CPU/GPU performance parameters.
        ovrPerformanceParms PerformanceParms;

        /// For handling HMD events and power level state changes.
        ovrJava Java;
    }



    //-------------------------------------
    // Flexible Layer Type structures for vrapi_SubmitFrame2.
    //-------------------------------------

    enum { ovrMaxLayerCount = 16 };

    /// A layer type.
    public enum ovrLayerType2
    {
        VRAPI_LAYER_TYPE_PROJECTION2 = 1,
        VRAPI_LAYER_TYPE_CYLINDER2 = 3,
        VRAPI_LAYER_TYPE_CUBE2 = 4,
        VRAPI_LAYER_TYPE_EQUIRECT2 = 5,
        VRAPI_LAYER_TYPE_LOADING_ICON2 = 6,
        VRAPI_LAYER_TYPE_FISHEYE2 = 7,
    }


    /// Properties shared by any type of layer.
    public struct ovrLayerHeader2
    {
        ovrLayerType2 Type;
        uint32_t Flags;

        ovrVector4f ColorScale;
        ovrFrameLayerBlend SrcBlend;
        ovrFrameLayerBlend DstBlend;
        /// \unused parameter.
        void* Reserved;
    }



    /// ovrLayerProjection2 provides support for a typical world view layer.
    /// \note Any layer textures that are dynamic must be triple buffered.
    public struct ovrLayerProjection2
    {
        /// Header.Type must be VRAPI_LAYER_TYPE_PROJECTION2.
        ovrLayerHeader2 Header;
        OVR_VRAPI_PADDING_32_BIT(4)


    ovrRigidBodyPosef HeadPose;

        struct {
        ovrTextureSwapChain* ColorSwapChain;
        int SwapChainIndex;
        ovrMatrix4f TexCoordsFromTanAngles;
        ovrRectf TextureRect;
    }
    Textures[VRAPI_FRAME_LAYER_EYE_MAX];
}


/// ovrLayerCylinder2 provides support for a single 2D texture projected onto a cylinder shape.
///
/// For Cylinder, the vertex coordinates will be transformed as if the texture type was CUBE.
/// Additionally, the interpolated vec3 will be remapped to vec2 by a direction-to-hemicyl mapping.
/// This mapping is currently hard-coded to 180 degrees around and 60 degrees vertical FOV.
///
/// After the mapping to 2D, an optional textureMatrix is applied. In the monoscopic case, the
/// matrix will typically be the identity matrix (ie no scale, bias). In the stereo case, when the
/// image source comes from a single image, the transform is necessary to map the [0.0,1.0] output
/// to a different (sub)rect.
///
/// Regardless of how the textureMatrix transforms the vec2 output of the equirect transform, each
/// TextureRect clamps the resulting texture coordinates so that no coordinates are beyond the
/// specified extents. No guarantees are made about whether fragments will be shaded outside the
/// rect, so it is important that the subrect have a transparent border.
///
public struct ovrLayerCylinder2
{
    /// Header.Type must be VRAPI_LAYER_TYPE_CYLINDER2.
    ovrLayerHeader2 Header;
    OVR_VRAPI_PADDING_32_BIT(4)

    ovrRigidBodyPosef HeadPose;

    struct {
        /// Texture type used to create the swapchain must be a 2D target (VRAPI_TEXTURE_TYPE_2D_*).
        ovrTextureSwapChain* ColorSwapChain;
    int SwapChainIndex;
    ovrMatrix4f TexCoordsFromTanAngles;
    ovrRectf TextureRect;
    /// \note textureMatrix is set up like the following:
    ///	sx,  0, tx, 0
    ///	0,  sy, ty, 0
    ///	0,   0,  1, 0
    ///	0,   0,  0, 1
    /// since we do not need z coord for mapping to 2d texture.
    ovrMatrix4f TextureMatrix;
}
Textures[VRAPI_FRAME_LAYER_EYE_MAX];
} 


/// ovrLayerCube2 provides support for a single timewarped cubemap at infinity
/// with optional Offset vector (provided in normalized [-1.0,1.0] space).
///
/// Cube maps are an omni-directional layer source that are directly supported
/// by the graphics hardware. The nature of the cube map definition results in
/// higher resolution (in pixels per solid angle) at the corners and edges of
/// the cube and lower resolution at the center of each face. While the cube map
/// does have variability in sample density, the variability is spread symmetrically
/// around the sphere.
///
/// Sometimes it is valuable to have an omni-directional format that has a
/// directional bias where quality and sample density is better in a particular
/// direction or over a particular region. If we changed the cube map sampling
///
/// from:
///   color = texture( cubeLayerSampler, direction );
/// to:
///   color = texture( cubeLayerSampler, normalize( direction ) + offset );
///
/// we can provide a remapping of the cube map sample distribution such that
/// samples in the "offset" direction map to a smaller region of the cube map
/// (and are thus higher resolution).
///
/// A normal high resolution cube map can be resampled using the inverse of this
/// mapping to retain high resolution for one direction while signficantly reducing
/// the required size of the cube map.
///
public struct ovrLayerCube2
{
    /// Header.Type must be VRAPI_LAYER_TYPE_CUBE2.
    ovrLayerHeader2 Header;
    OVR_VRAPI_PADDING_32_BIT(4)

    ovrRigidBodyPosef HeadPose;
    ovrMatrix4f TexCoordsFromTanAngles;

    ovrVector3f Offset;

    struct {
        /// Texture type used to create the swapchain must be a cube target
        /// (VRAPI_TEXTURE_TYPE_CUBE).
    ovrTextureSwapChain* ColorSwapChain;
    int SwapChainIndex;
}
Textures[VRAPI_FRAME_LAYER_EYE_MAX];
#ifdef __i386__
    uint32_t Padding;
#endif
} 


/// ovrLayerEquirect2 provides support for a single Equirectangular texture at infinity.
///
/// For Equirectangular, the vertex coordinates will be transformed as if the texture type was CUBE,
/// and in the fragment shader, the interpolated vec3 will be remapped to vec2 by a
/// direction-to-equirect mapping.
///
/// After the mapping to 2D, an optional textureMatrix is applied. In the monoscopic case, the
/// matrix will typically be the identity matrix (ie no scale, bias). In the stereo case, when the
/// image source come from a single image, the transform is necessary to map the [0.0,1.0] output to
/// a different (sub)rect.
///
/// Regardless of how the textureMatrix transforms the vec2 output of the equirect transform, each
/// TextureRect clamps the resulting texture coordinates so that no coordinates are beyond the
/// specified extents. No guarantees are made about whether fragments will be shaded outside the
/// rect, so it is important that the subrect have a transparent border.
///
public struct ovrLayerEquirect2
{
    /// Header.Type must be VRAPI_LAYER_TYPE_EQUIRECT2.
    ovrLayerHeader2 Header;
    OVR_VRAPI_PADDING_32_BIT(4)

    ovrRigidBodyPosef HeadPose;
    ovrMatrix4f TexCoordsFromTanAngles;

    struct {
        /// Texture type used to create the swapchain must be a 2D target (VRAPI_TEXTURE_TYPE_2D_*).
        ovrTextureSwapChain* ColorSwapChain;
    int SwapChainIndex;
    ovrRectf TextureRect;
    /// \note textureMatrix is set up like the following:
    ///	sx,  0, tx, 0
    ///	0,  sy, ty, 0
    ///	0,   0,  1, 0
    ///	0,   0,  0, 1
    /// since we do not need z coord for mapping to 2d texture.
    ovrMatrix4f TextureMatrix;
}
Textures[VRAPI_FRAME_LAYER_EYE_MAX];
} 


/// ovrLayerLoadingIcon2 provides support for a monoscopic spinning layer.
///
public struct ovrLayerLoadingIcon2
{
    /// Header.Type must be VRAPI_LAYER_TYPE_LOADING_ICON2.
    ovrLayerHeader2 Header;

    float SpinSpeed; //< radians per second
    float SpinScale;

    /// Only monoscopic texture supported for spinning layer.
    ovrTextureSwapChain* ColorSwapChain;
    int SwapChainIndex;
}



/// An "equiangular fisheye" or "f-theta" lens can be used to capture photos or video
/// of around 180 degrees without stitching.
///
/// The cameras probably aren't exactly vertical, so a transformation may need to be applied
/// before performing the fisheye calculation.
/// A stereo fisheye camera rig will usually have slight misalignments between the two
/// cameras, so they need independent transformations.
///
/// Once in lens space, the ray is transformed into an ideal fisheye projection, where the
/// 180 degree hemisphere is mapped to a -1 to 1 2D space.
///
/// From there it can be mapped into actual texture coordinates, possibly two to an image for
/// stereo.
///
public struct ovrLayerFishEye2
{
    /// Header.Type must be VRAPI_LAYER_TYPE_FISHEYE2.
    ovrLayerHeader2 Header;
    OVR_VRAPI_PADDING_32_BIT(4)

    ovrRigidBodyPosef HeadPose;

    struct {
        ovrTextureSwapChain* ColorSwapChain;
    int SwapChainIndex;
    ovrMatrix4f LensFromTanAngles; //< transforms a tanAngle ray into lens space
    ovrRectf TextureRect; //< packed stereo images will need to clamp at the mid border
    ovrMatrix4f TextureMatrix; //< transform from a -1 to 1 ideal fisheye to the texture
    ovrVector4f Distortion; //< Not currently used.
}
Textures[VRAPI_FRAME_LAYER_EYE_MAX];
} 


/// Union that combines ovrLayer types in a way that allows them
/// to be used in a polymorphic way.
typedef union ovrLayer_Union2_ {
    ovrLayerHeader2 Header;
ovrLayerProjection2 Projection;
ovrLayerCylinder2 Cylinder;
ovrLayerCube2 Cube;
ovrLayerEquirect2 Equirect;
ovrLayerLoadingIcon2 LoadingIcon;
ovrLayerFishEye2 FishEye;
} ovrLayer_Union2;

/// Parameters for frame submission.
public struct ovrSubmitFrameDescription2
{
    uint32_t Flags;
    uint32_t SwapInterval;
    uint64_t FrameIndex;
    double DisplayTime;
    /// \unused parameter.
    unsigned char Pad[8];
    uint32_t LayerCount;
    const ovrLayerHeader2* const* Layers;
}



//-----------------------------------------------------------------
// Performance
//-----------------------------------------------------------------

/// Identifies a VR-related application thread.
public enum ovrPerfThreadType
{
    VRAPI_PERF_THREAD_TYPE_MAIN = 0,
    VRAPI_PERF_THREAD_TYPE_RENDERER = 1,
}



//-----------------------------------------------------------------
// Events
//-----------------------------------------------------------------

public enum ovrEventType
{
    // No event. This is returned if no events are pending.
    VRAPI_EVENT_NONE = 0,
    // Events were lost due to event queue overflow.
    VRAPI_EVENT_DATA_LOST = 1,
    // The application's frames are visible to the user.
    VRAPI_EVENT_VISIBILITY_GAINED = 2,
    // The application's frames are no longer visible to the user.
    VRAPI_EVENT_VISIBILITY_LOST = 3,
    // The current activity is in the foreground and has input focus.
    VRAPI_EVENT_FOCUS_GAINED = 4,
    // The current activity is in the background (but possibly still visible) and has lost input
    // focus.
    VRAPI_EVENT_FOCUS_LOST = 5,
}



public struct ovrEventHeader
{
    ovrEventType EventType;
}


// Event structure for VRAPI_EVENT_DATA_LOST
public struct ovrEventDataLost
{
    ovrEventHeader EventHeader;
}


// Event structure for VRAPI_EVENT_VISIBILITY_GAINED
public struct ovrEventVisibilityGained
{
    ovrEventHeader EventHeader;
}


// Event structure for VRAPI_EVENT_VISIBILITY_LOST
public struct ovrEventVisibilityLost
{
    ovrEventHeader EventHeader;
}


// Event structure for VRAPI_EVENT_FOCUS_GAINED
public struct ovrEventFocusGained
{
    ovrEventHeader EventHeader;
}


// Event structure for VRAPI_EVENT_FOCUS_LOST
public struct ovrEventFocusLost
{
    ovrEventHeader EventHeader;
}



public struct ovrEventDataBuffer
{
    ovrEventHeader EventHeader;
    unsigned char EventData[4000];
}


/// Returns the version + compile time stamp as a string.
/// Can be called any time from any thread.
[DllImport("vrapi")]
public static extern string vrapi_GetVersionString();

/// Returns global, absolute high-resolution time in seconds. This is the same value
/// as used in sensor messages and on Android also the same as Java's system.nanoTime(),
/// which is what the V-sync timestamp is based on.
/// \warning Do not use this time as a seed for simulations, animations or other logic.
/// An animation, for instance, should not be updated based on the "real time" the
/// animation code is executed. Instead, an animation should be updated based on the
/// time it will be displayed. Using the "real time" will introduce intra-frame motion
/// judder when the code is not executed at a consistent point in time every frame.
/// In other words, for simulations, animations and other logic use the time returned
/// by vrapi_GetPredictedDisplayTime().
/// Can be called any time from any thread.
[DllImport("vrapi")]
public static extern double vrapi_GetTimeInSeconds();

//-----------------------------------------------------------------
// Initialization/Shutdown
//-----------------------------------------------------------------

/// Initializes the API for application use.
/// This is lightweight and does not create any threads.
/// This is typically called from onCreate() or shortly thereafter.
/// Can be called from any thread.
/// Returns a non-zero value from ovrInitializeStatus on error.
[DllImport("vrapi")]
public static extern ovrInitializeStatus vrapi_Initialize(const ovrInitParms* initParms);

/// Shuts down the API on application exit.
/// This is typically called from onDestroy() or shortly thereafter.
/// Can be called from any thread.
public static extern void vrapi_Shutdown();

//-----------------------------------------------------------------
// VrApi Properties
//-----------------------------------------------------------------

/// Returns a VrApi property.
/// These functions can be called any time from any thread once the VrApi is initialized.
public static extern void vrapi_SetPropertyInt(const ovrJava* java, const ovrProperty propType, const int intVal);
public static extern void vrapi_SetPropertyFloat(const ovrJava* java, const ovrProperty propType, const float floatVal);

/// Returns false if the property cannot be read.
public static extern bool vrapi_GetPropertyInt(const ovrJava* java, const ovrProperty propType, int* intVal);

//-----------------------------------------------------------------
// System Properties
//-----------------------------------------------------------------

/// Returns a system property. These are constants for a particular device.
/// These functions can be called any time from any thread once the VrApi is initialized.
public static extern int vrapi_GetSystemPropertyInt(const ovrJava* java, const ovrSystemProperty propType);

public static extern float vrapi_GetSystemPropertyFloat(
    const ovrJava* java,
    const ovrSystemProperty propType);
/// Returns the number of elements written to values array.
public static extern int vrapi_GetSystemPropertyFloatArray(
    const ovrJava* java,
    const ovrSystemProperty propType,
float* values,
int numArrayValues);
public static extern int vrapi_GetSystemPropertyInt64Array(
    const ovrJava* java,
    const ovrSystemProperty propType,
int64_t* values,
int numArrayValues);

/// The return memory is guaranteed to be valid until the next call to
/// vrapi_GetSystemPropertyString.
public static extern string vrapi_GetSystemPropertyString(
    const ovrJava* java,
    const ovrSystemProperty propType);

//-----------------------------------------------------------------
// System Status
//-----------------------------------------------------------------

/// Returns a system status. These are variables that may change at run-time.
/// This function can be called any time from any thread once the VrApi is initialized.
public static extern int vrapi_GetSystemStatusInt(
    const ovrJava* java,
    const ovrSystemStatus statusType);
public static extern float vrapi_GetSystemStatusFloat(
    const ovrJava* java,
    const ovrSystemStatus statusType);

//-----------------------------------------------------------------
// Enter/Leave VR mode
//-----------------------------------------------------------------

/// Starts up the time warp, V-sync tracking, sensor reading, clock locking,
/// thread scheduling, and sets video options. The parms are copied, and are
/// not referenced after the function returns.
///
/// This should be called after vrapi_Initialize(), when the app is both
/// resumed and has a valid window surface (ANativeWindow).
///
/// On Android, an application cannot just allocate a new window surface
/// and render to it. Android allocates and manages the window surface and
/// (after the fact) notifies the application of the state of affairs through
/// life cycle events (surfaceCreated / surfaceChanged / surfaceDestroyed).
/// The application (or 3rd party engine) typically handles these events.
/// Since the VrApi cannot just allocate a new window surface, and the VrApi
/// does not handle the life cycle events, the VrApi somehow has to take over
/// ownership of the Android surface from the application. To allow this, the
/// application can explicitly pass the EGLDisplay, EGLContext and EGLSurface
/// or ANativeWindow to vrapi_EnterVrMode(). The EGLDisplay and EGLContext are
/// used to create a shared context used by the background time warp thread.
///
/// If, however, the application does not explicitly pass in these objects, then
/// vrapi_EnterVrMode() *must* be called from a thread with an OpenGL ES context
/// current on the Android window surface. The context of the calling thread is
/// then used to match the version and config for the context used by the background
/// time warp thread. The time warp will also hijack the Android window surface
/// from the context that is current on the calling thread. On return, the context
/// from the calling thread will be current on an invisible pbuffer, because the
/// time warp takes ownership of the Android window surface. Note that this requires
/// the config used by the calling thread to have an EGL_SURFACE_TYPE with EGL_PBUFFER_BIT.
///
/// New applications must always explicitly pass in the EGLDisplay, EGLContext
/// and ANativeWindow, otherwise vrapi_EnterVrMode will fail.
///
/// This function will return NULL when entering VR mode failed because the ANativeWindow
/// was not valid. If the ANativeWindow's buffer queue is abandoned
/// ("BufferQueueProducer: BufferQueue has been abandoned"), then the app can wait for a
/// new ANativeWindow (through SurfaceCreated). If another API is already connected to
/// the ANativeWindow ("BufferQueueProducer: already connected"), then the app has to first
/// disconnect whatever is connected to the ANativeWindow (typically an EGLSurface).
public static extern ovrMobile* vrapi_EnterVrMode(const ovrModeParms* parms);

/// Shut everything down for window destruction or when the activity is paused.
/// The ovrMobile object is freed by this function.
///
/// Must be called from the same thread that called vrapi_EnterVrMode(). If the
/// application did not explicitly pass in the Android window surface, then this
/// thread *must* have the same OpenGL ES context that was current on the Android
/// window surface before calling vrapi_EnterVrMode(). By calling this function,
/// the time warp gives up ownership of the Android window surface, and on return,
/// the context from the calling thread will be current again on the Android window
/// surface.
public static extern void vrapi_LeaveVrMode(ovrMobile* ovr);

//-----------------------------------------------------------------
// Tracking
//-----------------------------------------------------------------

/// Returns a predicted absolute system time in seconds at which the next set
/// of eye images will be displayed.
///
/// The predicted time is the middle of the time period during which the new
/// eye images will be displayed. The number of frames predicted ahead depends
/// on the pipeline depth of the engine and the minumum number of V-syncs in
/// between eye image rendering. The better the prediction, the less black will
/// be pulled in at the edges by the time warp.
///
/// The frameIndex is an application controlled number that uniquely identifies
/// the new set of eye images for which synthesis is about to start. This same
/// frameIndex must be passed to vrapi_SubmitFrame() when the new eye images are
/// submitted to the time warp. The frameIndex is expected to be incremented
/// once every frame before calling this function.
///
/// Can be called from any thread while in VR mode.
public static extern double vrapi_GetPredictedDisplayTime(ovrMobile* ovr, long long frameIndex);

/// Returns the predicted sensor state based on the specified absolute system time
/// in seconds. Pass absTime value of 0.0 to request the most recent sensor reading.
///
/// Can be called from any thread while in VR mode.
public static extern ovrTracking2 vrapi_GetPredictedTracking2(ovrMobile* ovr, double absTimeInSeconds);

public static extern ovrTracking vrapi_GetPredictedTracking(ovrMobile* ovr, double absTimeInSeconds);

/// Recenters the orientation on the yaw axis and will recenter the position
/// when position tracking is available.
///
/// \note This immediately affects vrapi_GetPredictedTracking() which may
/// be called asynchronously from the time warp. It is therefore best to
/// make sure the screen is black before recentering to avoid previous eye
/// images from being abrubtly warped across the screen.
///
/// Can be called from any thread while in VR mode.

// vrapi_RecenterPose() is being deprecated because it is supported at the user
// level via system interaction, and at the app level, the app is free to use
// any means it likes to control the mapping of virtual space to physical space.
OVR_VRAPI_DEPRECATED(public static extern void vrapi_RecenterPose(ovrMobile* ovr));

        //-----------------------------------------------------------------
        // Tracking Transform
        //
        //-----------------------------------------------------------------

        /// The coordinate system used by the tracking system is defined in meters
        /// with its positive y axis pointing up, but its origin and yaw are unspecified.
        ///
        /// Applications generally prefer to operate in a well-defined coordinate system
        /// relative to some base pose. The tracking transform allows the application to
        /// specify the space that tracking poses are reported in.
        ///
        /// The tracking transform is specified as the ovrPosef of the base pose in tracking
        /// system coordinates.
        ///
        /// Head poses the application supplies in vrapi_SubmitFrame() are transformed
        /// by the tracking transform.
        /// Pose predictions generated by the system are transformed by the inverse of the
        /// tracking transform before being reported to the application.
        ///
        /// \note This immediately affects vrapi_SubmitFrame() and
        /// vrapi_GetPredictedTracking(). It is important for the app to make sure that
        /// the tracking transform does not change between the fetching of a pose prediction
        /// and the submission of poses in vrapi_SubmitFrame().
        ///
        /// The default Tracking Transform is VRAPI_TRACKING_TRANSFORM_SYSTEM_CENTER_EYE_LEVEL.

        /// Returns a pose suitable to use as a tracking transform.
        /// Applications that want to use an eye-level based coordinate system can fetch
        /// the VRAPI_TRACKING_TRANSFORM_SYSTEM_CENTER_EYE_LEVEL transform.
        /// Applications that want to use a floor-level based coordinate system can fetch
        /// the VRAPI_TRACKING_TRANSFORM_SYSTEM_CENTER_FLOOR_LEVEL transform.
        /// To determine the current tracking transform, applications can fetch the
        /// VRAPI_TRACKING_TRANSFORM_CURRENT transform.

        /// The TrackingTransform API has been deprecated because it was superceded by the
        /// TrackingSpace API. The key difference in the TrackingSpace API is that LOCAL
        /// and LOCAL_FLOOR spaces are mutable, so user/system recentering is transparently
        /// applied without app intervention.
        OVR_VRAPI_DEPRECATED(public static extern ovrPosef vrapi_GetTrackingTransform(
            ovrMobile* ovr,
            ovrTrackingTransform whichTransform));

        /// Sets the transform used to convert between tracking coordinates and a canonical
        /// application-defined space.
        /// Only the yaw component of the orientation is used.
        OVR_VRAPI_DEPRECATED(
            public static extern void vrapi_SetTrackingTransform(ovrMobile* ovr, ovrPosef pose));

        /// Returns the current tracking space
        public static extern ovrTrackingSpace vrapi_GetTrackingSpace(ovrMobile* ovr);

/// Set the tracking space. There are currently two options:
///   * VRAPI_TRACKING_SPACE_LOCAL (default)
///         The local tracking space's origin is at the nominal head position
///         with +y up, and -z forward. This space is volatile and will change
///         when system recentering occurs.
///   * VRAPI_TRACKING_SPACE_LOCAL_FLOOR
///         The local floor tracking space is the same as the local tracking
///         space, except its origin is translated down to the floor. The local
///         floor space differs from the local space only in its y translation.
///         This space is volatile and will change when system recentering occurs.
public static extern ovrResult vrapi_SetTrackingSpace(ovrMobile* ovr, ovrTrackingSpace whichSpace);

/// Returns pose of the requested space relative to the current space.
/// The returned value is not affected by the current tracking transform.
public static extern ovrPosef vrapi_LocateTrackingSpace(ovrMobile* ovr, ovrTrackingSpace target);

//-----------------------------------------------------------------
// Guardian System
//
//-----------------------------------------------------------------

/// Get the geometry of the Guardian System as a list of points that define the outer boundary
/// space. You can choose to get just the number of points by passing in a null value for points or
/// by passing in a pointsCountInput size of 0.  Otherwise pointsCountInput will be used to fetch
/// as many points as possible from the Guardian points data.  If the input size exceeds the
/// number of points that are currently stored off we only copy up to the number of points that we
/// have and pointsCountOutput will return the number of copied points
public static extern ovrResult vrapi_GetBoundaryGeometry(
    ovrMobile* ovr,
    const uint32_t pointsCountInput,
uint32_t* pointsCountOutput,
ovrVector3f* points);

/// Gets the dimension of the Oriented Bounding box for the Guardian System.  This is the largest
/// fit rectangle within the Guardian System boundary geometry. The pose value contains the forward
/// facing direction as well as the translation for the oriented box.  The scale return value
/// returns a scalar value for the width, height, and depth of the box.  These values are half the
/// actual size as they are scalars and in meters."
public static extern ovrResult
vrapi_GetBoundaryOrientedBoundingBox(ovrMobile* ovr, ovrPosef* pose, ovrVector3f* scale);

/// Tests collision/proximity of a 3D point against the Guardian System Boundary and returns whether
/// or not a given point is inside or outside of the boundary.  If a more detailed set of boundary
/// trigger information is requested a ovrBoundaryTriggerResult may be passed in.  However null may
/// also be passed in to just return whether a point is inside the boundary or not.
public static extern ovrResult vrapi_TestPointIsInBoundary(
    ovrMobile* ovr,
    const ovrVector3f point,
bool* pointInsideBoundary,
ovrBoundaryTriggerResult* result);

/// Tests collision/proximity of position tracked devices (e.g. HMD and/or Controllers) against the
/// Guardian System boundary. This function returns an ovrGuardianTriggerResult which contains
/// information such as distance and closest point based on collision/proximity test
public static extern ovrResult vrapi_GetBoundaryTriggerState(
    ovrMobile* ovr,
    const ovrTrackedDeviceTypeId deviceId,
    ovrBoundaryTriggerResult* result);

/// Used to force Guardian System mesh visibility to true.  Forcing to false will set the Guardian
/// System back to normal operation.
public static extern ovrResult vrapi_RequestBoundaryVisible(ovrMobile* ovr, const bool visible);

/// Used to access whether or not the Guardian System is visible or not
public static extern ovrResult vrapi_GetBoundaryVisible(ovrMobile* ovr, bool* visible);


//-----------------------------------------------------------------
// Texture Swap Chains
//
//-----------------------------------------------------------------

/// Texture Swap Chain lifetime is explicitly controlled by the application via calls
/// to vrapi_CreateTextureSwapChain* or vrapi_CreateAndroidSurfaceSwapChain and
/// vrapi_DestroyTextureSwapChain. Swap Chains are associated with the VrApi instance,
/// not the VrApi ovrMobile. Therefore, calls to vrapi_EnterVrMode and vrapi_LeaveVrMode
/// will not destroy or cause the Swap Chain to become invalid.

/// Create a texture swap chain that can be passed to vrapi_SubmitFrame.
/// Must be called from a thread with a valid OpenGL ES context current.
///
/// 'bufferCount' used to be a bool that selected either a single texture index
/// or a triple buffered index, but the new entry point vrapi_CreateTextureSwapChain2,
/// allows up to 16 buffers to be allocated, which is useful for maintaining a
/// deep video buffer queue to get better frame timing.
///
/// 'format' used to be an ovrTextureFormat but has been expanded to accept
/// platform specific format types. For GLES, this is the internal format.
/// If an unsupported format is provided, swapchain creation will fail.
///
/// SwapChain creation failures result in a return value of 'nullptr'.
public static extern ovrTextureSwapChain* vrapi_CreateTextureSwapChain3(
    ovrTextureType type,
    int64_t format,
    int width,
    int height,
    int levels,
    int bufferCount);

public static extern ovrTextureSwapChain* vrapi_CreateTextureSwapChain2(
    ovrTextureType type,
    ovrTextureFormat format,
    int width,
    int height,
    int levels,
    int bufferCount);

public static extern ovrTextureSwapChain* vrapi_CreateTextureSwapChain(
    ovrTextureType type,
    ovrTextureFormat format,
    int width,
    int height,
    int levels,
    bool buffered);

/// Create an Android SurfaceTexture based texture swap chain suitable for use with
/// vrapi_SubmitFrame. Updating of the SurfaceTexture is handled through normal Android platform
/// specific mechanisms from within the Compositor. A reference to the Android Surface object
/// associated with the SurfaceTexture may be obtained by calling
/// vrapi_GetTextureSwapChainAndroidSurface.
///
/// An optional width and height (ie width and height do not equal zero) may be provided in order to
/// set the default size of the image buffers. Note that the image producer may override the buffer
/// size, in which case the default values provided here will not be used (ie both video
/// decompression or camera preview override the size automatically).
///
/// If isProtected is true, the surface swapchain will be created as a protected surface, ie for
/// supporting secure video playback.
///
/// NOTE: These paths are not currently supported under Vulkan.
public static extern ovrTextureSwapChain* vrapi_CreateAndroidSurfaceSwapChain(int width, int height);
public static extern ovrTextureSwapChain*
vrapi_CreateAndroidSurfaceSwapChain2(int width, int height, bool isProtected);


/// Destroy the given texture swap chain.
/// Must be called from a thread with the same OpenGL ES context current when
/// vrapi_CreateTextureSwapChain was called.
public static extern void vrapi_DestroyTextureSwapChain(ovrTextureSwapChain* chain);

/// Returns the number of textures in the swap chain.
public static extern int vrapi_GetTextureSwapChainLength(ovrTextureSwapChain* chain);

/// Get the OpenGL name of the texture at the given index.
public static extern uint vrapi_GetTextureSwapChainHandle(
    ovrTextureSwapChain* chain,
    int index);


/// Get the Android Surface object associated with the swap chain.
public static extern jobject vrapi_GetTextureSwapChainAndroidSurface(ovrTextureSwapChain* chain);


//-----------------------------------------------------------------
// Frame Submission
//-----------------------------------------------------------------


/// Accepts new eye images plus poses that will be used for future warps.
/// The parms are copied, and are not referenced after the function returns.
///
/// This will block until the textures from the previous vrapi_SubmitFrame() have been
/// consumed by the background thread, to allow one frame of overlap for maximum
/// GPU utilization, while preventing multiple frames from piling up variable latency.
///
/// This will block until at least SwapInterval vsyncs have passed since the last
/// call to vrapi_SubmitFrame() to prevent applications with simple scenes from
/// generating completely wasted frames.
///
/// IMPORTANT: any dynamic textures that are passed to vrapi_SubmitFrame() must be
/// triple buffered to avoid flickering and performance problems.
///
/// The VrApi allows for one frame of overlap which is essential on tiled mobile GPUs.
/// Because there is one frame of overlap, the eye images have typically not completed
/// rendering by the time they are submitted to vrapi_SubmitFrame(). To allow the time
/// warp to check whether the eye images have completed rendering, vrapi_SubmitFrame()
/// adds a sync object to the current context. Therefore, vrapi_SubmitFrame() *must*
/// be called from a thread with an OpenGL ES context whose completion ensures that
/// frame rendering is complete. Generally this is the thread and context that was used
/// for the rendering.
public static extern void vrapi_SubmitFrame(ovrMobile* ovr, const ovrFrameParms* parms);

/// vrapi_SubmitFrame2 takes a frameDescription describing per-frame information such as:
/// a flexible list of layers which should be drawn this frame and a frame index.
public static extern ovrResult
vrapi_SubmitFrame2(ovrMobile* ovr, const ovrSubmitFrameDescription2* frameDescription);

//-----------------------------------------------------------------
// Performance
//-----------------------------------------------------------------

/// Set the CPU and GPU performance levels.
///
/// Increasing the levels increases performance at the cost of higher power consumption
/// which likely leads to a greater chance of overheating.
///
/// Levels will be clamped to the expected range. Default clock levels are cpuLevel = 2, gpuLevel
/// = 2.
public static extern ovrResult
vrapi_SetClockLevels(ovrMobile* ovr, const int32_t cpuLevel, const int32_t gpuLevel);

/// Specify which app threads should be given higher scheduling priority.
public static extern ovrResult
vrapi_SetPerfThread(ovrMobile* ovr, const ovrPerfThreadType type, const uint32_t threadId);

/// If VRAPI_EXTRA_LATENCY_MODE_ON specified, adds an extra frame of latency for full GPU
/// utilization. Default is VRAPI_EXTRA_LATENCY_MODE_OFF.
///
/// The latency mode specified will be applied on the next call to vrapi_SubmitFrame(2).
public static extern ovrResult
vrapi_SetExtraLatencyMode(ovrMobile* ovr, const ovrExtraLatencyMode mode);

//-----------------------------------------------------------------
// Display Refresh Rate
//-----------------------------------------------------------------

/// Set the Display Refresh Rate.
/// Returns ovrSuccess or an ovrError code.
/// Returns 'ovrError_InvalidParameter' if requested refresh rate is not supported by the device.
/// Returns 'ovrError_InvalidOperation' if the display refresh rate request was not allowed (such as
/// when the device is in low power mode).
public static extern ovrResult vrapi_SetDisplayRefreshRate(ovrMobile* ovr, const float refreshRate);

//-----------------------------------------------------------------
// Events
//-----------------------------------------------------------------

/// Returns VrApi state information to the application.
/// The application should read from the VrApi event queue with regularity.
///
/// The caller must pass a pointer to memory that is at least the size of the largest event
/// structure, VRAPI_LARGEST_EVENT_TYPE. On return, the structure is filled in with the current
/// event's data. All event structures start with the ovrEventHeader, which contains the
/// type of the event. Based on this type, the caller can cast the passed ovrEventHeader
/// pointer to the appropriate event type.
///
/// Returns ovrSuccess if no error occured.
/// If no events are pending the event header EventType will be VRAPI_EVENT_NONE.
public static extern ovrResult vrapi_PollEvent(ovrEventHeader* event);
        }
}