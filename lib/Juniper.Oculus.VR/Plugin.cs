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
using System.Runtime.InteropServices;

using Juniper.Mathematics;

namespace Oculus.VR
{
    // Internal C# wrapper for OVRPlugin.
    public static class Plugin
    {
        public const bool isSupportedPlatform = true;

        public static readonly System.Version wrapperVersion = OVRP_1_45_0.version;

        private static System.Version _version;

        public static System.Version version
        {
            get
            {
                if (_version == null)
                {
                    try
                    {
                        var pluginVersion = OVRP_1_1_0.ovrp_GetVersion();

                        if (pluginVersion != null)
                        {
                            // Truncate unsupported trailing version info for System.Version. Original string is returned if not present.
                            pluginVersion = pluginVersion.Split('-')[0];
                            _version = new System.Version(pluginVersion);
                        }
                        else
                        {
                            _version = _versionZero;
                        }
                    }
                    catch
                    {
                        _version = _versionZero;
                    }

                    // Unity 5.1.1f3-p3 have OVRPlugin version "0.5.0", which isn't accurate.
                    if (_version == OVRP_0_5_0.version)
                    {
                        _version = OVRP_0_1_0.version;
                    }

                    if (_version > _versionZero && _version < OVRP_1_3_0.version)
                    {
                        throw new PlatformNotSupportedException("Oculus Utilities version " + wrapperVersion + " is too new for OVRPlugin version " + _version.ToString() + ". Update to the latest version of Unity.");
                    }
                }

                return _version;
            }
        }

        private static System.Version _nativeSDKVersion;
        public static System.Version nativeSDKVersion
        {
            get
            {
                if (_nativeSDKVersion == null)
                {
                    try
                    {
                        var sdkVersion = string.Empty;

                        if (version >= OVRP_1_1_0.version)
                        {
                            sdkVersion = OVRP_1_1_0.ovrp_GetNativeSDKVersion();
                        }
                        else
                        {
                            sdkVersion = _versionZero.ToString();
                        }

                        if (sdkVersion != null)
                        {
                            // Truncate unsupported trailing version info for System.Version. Original string is returned if not present.
                            sdkVersion = sdkVersion.Split('-')[0];
                            _nativeSDKVersion = new System.Version(sdkVersion);
                        }
                        else
                        {
                            _nativeSDKVersion = _versionZero;
                        }
                    }
                    catch
                    {
                        _nativeSDKVersion = _versionZero;
                    }
                }

                return _nativeSDKVersion;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private class GUID
        {
            public int a;
            public short b;
            public short c;
            public byte d0;
            public byte d1;
            public byte d2;
            public byte d3;
            public byte d4;
            public byte d5;
            public byte d6;
            public byte d7;
        }

        public enum Bool
        {
            False = 0,
            True
        }

        public enum Result
        {
            /// Success
            Success = 0,

            /// Failure
            Failure = -1000,
            Failure_InvalidParameter = -1001,
            Failure_NotInitialized = -1002,
            Failure_InvalidOperation = -1003,
            Failure_Unsupported = -1004,
            Failure_NotYetImplemented = -1005,
            Failure_OperationFailed = -1006,
            Failure_InsufficientSize = -1007,
        }

        public enum CameraStatus
        {
            None,
            Connected,
            Calibrating,
            CalibrationFailed,
            Calibrated,
            EnumSize = int.MaxValue
        }

        public enum Eye
        {
            None = -1,
            Left = 0,
            Right = 1,
            Count = 2
        }

        public enum Tracker
        {
            None = -1,
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Count,
        }

        public enum Node
        {
            None = -1,
            EyeLeft = 0,
            EyeRight = 1,
            EyeCenter = 2,
            HandLeft = 3,
            HandRight = 4,
            TrackerZero = 5,
            TrackerOne = 6,
            TrackerTwo = 7,
            TrackerThree = 8,
            Head = 9,
            DeviceObjectZero = 10,
            Count,
        }

        public enum Controller
        {
            None = 0,
            LTouch = 0x00000001,
            RTouch = 0x00000002,
            Touch = LTouch | RTouch,
            Remote = 0x00000004,
            Gamepad = 0x00000010,
            LHand = 0x00000020,
            RHand = 0x00000040,
            Hands = LHand | RHand,
            Touchpad = 0x08000000,
            LTrackedRemote = 0x01000000,
            RTrackedRemote = 0x02000000,
            Active = unchecked((int)0x80000000),
            All = ~None,
        }

        public enum Handedness
        {
            Unsupported = 0,
            LeftHanded = 1,
            RightHanded = 2,
        }

        public enum TrackingOrigin
        {
            EyeLevel = 0,
            FloorLevel = 1,
            Stage = 2,
            Count,
        }

        public enum RecenterFlags
        {
            Default = 0,
            Controllers = 0x40000000,
            IgnoreAll = unchecked((int)0x80000000),
            Count,
        }

        public enum BatteryStatus
        {
            Charging = 0,
            Discharging,
            Full,
            NotCharging,
            Unknown,
        }

        public enum EyeTextureFormat
        {
            Default = 0,
            R8G8B8A8_sRGB = 0,
            R8G8B8A8 = 1,
            R16G16B16A16_FP = 2,
            R11G11B10_FP = 3,
            B8G8R8A8_sRGB = 4,
            B8G8R8A8 = 5,
            R5G6B5 = 11,
            EnumSize = 0x7fffffff
        }

        public enum PlatformUI
        {
            None = -1,
            ConfirmQuit = 1,
            GlobalMenuTutorial, // Deprecated
        }

        public enum SystemRegion
        {
            Unspecified = 0,
            Japan,
            China,
        }

        public enum SystemHeadset
        {
            None = 0,
            GearVR_R320, // Note4 Innovator
            GearVR_R321, // S6 Innovator
            GearVR_R322, // Commercial 1
            GearVR_R323, // Commercial 2 (USB Type C)
            GearVR_R324, // Commercial 3 (USB Type C)
            GearVR_R325, // Commercial 4 (USB Type C)
            Oculus_Go,
            Oculus_Quest,

            Rift_DK1 = 0x1000,
            Rift_DK2,
            Rift_CV1,
            Rift_CB,
            Rift_S,
        }

        public enum OverlayShape
        {
            Quad = 0,
            Cylinder = 1,
            Cubemap = 2,
            OffcenterCubemap = 4,
            Equirect = 5,
        }

        public enum ProcessingStep
        {
            Render = -1,
            Physics = 0,
        }

        public enum CameraDevice
        {
            None = 0,
            WebCamera0 = 100,
            WebCamera1 = 101,
            ZEDCamera = 300,
        }

        public enum CameraDeviceDepthSensingMode
        {
            Standard = 0,
            Fill = 1,
        }

        public enum CameraDeviceDepthQuality
        {
            Low = 0,
            Medium = 1,
            High = 2,
        }

        public enum FixedFoveatedRenderingLevel
        {
            Off = 0,
            Low = 1,
            Medium = 2,
            High = 3,
            // High foveation setting with more detail toward the bottom of the view and more foveation near the top (Same as High on Oculus Go)
            HighTop = 4,
            EnumSize = 0x7FFFFFFF
        }

        [Obsolete("Please use FixedFoveatedRenderingLevel instead", false)]
        public enum TiledMultiResLevel
        {
            Off = 0,
            LMSLow = FixedFoveatedRenderingLevel.Low,
            LMSMedium = FixedFoveatedRenderingLevel.Medium,
            LMSHigh = FixedFoveatedRenderingLevel.High,
            // High foveation setting with more detail toward the bottom of the view and more foveation near the top (Same as High on Oculus Go)
            LMSHighTop = FixedFoveatedRenderingLevel.HighTop,
            EnumSize = 0x7FFFFFFF
        }

        public enum PerfMetrics
        {
            App_CpuTime_Float = 0,
            App_GpuTime_Float = 1,

            Compositor_CpuTime_Float = 3,
            Compositor_GpuTime_Float = 4,
            Compositor_DroppedFrameCount_Int = 5,

            System_GpuUtilPercentage_Float = 7,
            System_CpuUtilAveragePercentage_Float = 8,
            System_CpuUtilWorstPercentage_Float = 9,

            // 1.32.0
            Device_CpuClockFrequencyInMHz_Float = 10,
            Device_GpuClockFrequencyInMHz_Float = 11,
            Device_CpuClockLevel_Int = 12,
            Device_GpuClockLevel_Int = 13,

            Count,
            EnumSize = 0x7FFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CameraDeviceIntrinsicsParameters
        {
            private readonly float fx; /* Focal length in pixels along x axis. */
            private readonly float fy; /* Focal length in pixels along y axis. */
            private readonly float cx; /* Optical center along x axis, defined in pixels (usually close to width/2). */
            private readonly float cy; /* Optical center along y axis, defined in pixels (usually close to height/2). */
            private readonly double disto0; /* Distortion factor : [ k1, k2, p1, p2, k3 ]. Radial (k1,k2,k3) and Tangential (p1,p2) distortion.*/
            private readonly double disto1;
            private readonly double disto2;
            private readonly double disto3;
            private readonly double disto4;
            private readonly float v_fov; /* Vertical field of view after stereo rectification, in degrees. */
            private readonly float h_fov; /* Horizontal field of view after stereo rectification, in degrees.*/
            private readonly float d_fov; /* Diagonal field of view after stereo rectification, in degrees.*/
            private readonly int w; /* Resolution width */
            private readonly int h; /* Resolution height */
        }

        private const int OverlayShapeFlagShift = 4;
        private enum OverlayFlag
        {
            None = unchecked(0x00000000),
            OnTop = unchecked(0x00000001),
            HeadLocked = unchecked(0x00000002),
            NoDepth = unchecked(0x00000004),
            ExpensiveSuperSample = unchecked(0x00000008),

            // Using the 5-8 bits for shapes, total 16 potential shapes can be supported 0x000000[0]0 ->  0x000000[F]0
            ShapeFlag_Quad = unchecked(OverlayShape.Quad << OverlayShapeFlagShift),
            ShapeFlag_Cylinder = unchecked(OverlayShape.Cylinder << OverlayShapeFlagShift),
            ShapeFlag_Cubemap = unchecked(OverlayShape.Cubemap << OverlayShapeFlagShift),
            ShapeFlag_OffcenterCubemap = unchecked(OverlayShape.OffcenterCubemap << OverlayShapeFlagShift),
            ShapeFlagRangeMask = unchecked(0xF << OverlayShapeFlagShift),

            Hidden = unchecked(0x000000200),
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector2f
        {
            public float x;
            public float y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3f
        {
            public float x;
            public float y;
            public float z;
            public static readonly Vector3f zero = new Vector3f { x = 0.0f, y = 0.0f, z = 0.0f };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}", x, y, z);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector4f
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public static readonly Vector4f zero = new Vector4f { x = 0.0f, y = 0.0f, z = 0.0f, w = 0.0f };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector4s
        {
            public short x;
            public short y;
            public short z;
            public short w;
            public static readonly Vector4s zero = new Vector4s { x = 0, y = 0, z = 0, w = 0 };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Quatf
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public static readonly Quatf identity = new Quatf { x = 0.0f, y = 0.0f, z = 0.0f, w = 1.0f };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Posef
        {
            public Quatf Orientation;
            public Vector3f Position;
            public static readonly Posef identity = new Posef { Orientation = Quatf.identity, Position = Vector3f.zero };
            public override string ToString()
            {
                return string.Format("Position ({0}), Orientation({1})", Position, Orientation);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TextureRectMatrixf
        {
            public Rect leftRect;
            public Rect rightRect;
            public Vector4 leftScaleBias;
            public Vector4 rightScaleBias;
            public static readonly TextureRectMatrixf zero = new TextureRectMatrixf { leftRect = new Rect(0, 0, 1, 1), rightRect = new Rect(0, 0, 1, 1), leftScaleBias = new Vector4(1, 1, 0, 0), rightScaleBias = new Vector4(1, 1, 0, 0) };

            public override string ToString()
            {
                return string.Format("Rect Left ({0}), Rect Right({1}), Scale Bias Left ({2}), Scale Bias Right({3})", leftRect, rightRect, leftScaleBias, rightScaleBias);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PoseStatef
        {
            public Posef Pose;
            public Vector3f Velocity;
            public Vector3f Acceleration;
            public Vector3f AngularVelocity;
            public Vector3f AngularAcceleration;
            public double Time;

            public static readonly PoseStatef identity = new PoseStatef
            {
                Pose = Posef.identity,
                Velocity = Vector3f.zero,
                Acceleration = Vector3f.zero,
                AngularVelocity = Vector3f.zero,
                AngularAcceleration = Vector3f.zero
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ControllerState4
        {
            public uint ConnectedControllers;
            public uint Buttons;
            public uint Touches;
            public uint NearTouches;
            public float LIndexTrigger;
            public float RIndexTrigger;
            public float LHandTrigger;
            public float RHandTrigger;
            public Vector2f LThumbstick;
            public Vector2f RThumbstick;
            public Vector2f LTouchpad;
            public Vector2f RTouchpad;
            public byte LBatteryPercentRemaining;
            public byte RBatteryPercentRemaining;
            public byte LRecenterCount;
            public byte RRecenterCount;
            public byte Reserved_27;
            public byte Reserved_26;
            public byte Reserved_25;
            public byte Reserved_24;
            public byte Reserved_23;
            public byte Reserved_22;
            public byte Reserved_21;
            public byte Reserved_20;
            public byte Reserved_19;
            public byte Reserved_18;
            public byte Reserved_17;
            public byte Reserved_16;
            public byte Reserved_15;
            public byte Reserved_14;
            public byte Reserved_13;
            public byte Reserved_12;
            public byte Reserved_11;
            public byte Reserved_10;
            public byte Reserved_09;
            public byte Reserved_08;
            public byte Reserved_07;
            public byte Reserved_06;
            public byte Reserved_05;
            public byte Reserved_04;
            public byte Reserved_03;
            public byte Reserved_02;
            public byte Reserved_01;
            public byte Reserved_00;

            public ControllerState4(ControllerState2 cs)
            {
                ConnectedControllers = cs.ConnectedControllers;
                Buttons = cs.Buttons;
                Touches = cs.Touches;
                NearTouches = cs.NearTouches;
                LIndexTrigger = cs.LIndexTrigger;
                RIndexTrigger = cs.RIndexTrigger;
                LHandTrigger = cs.LHandTrigger;
                RHandTrigger = cs.RHandTrigger;
                LThumbstick = cs.LThumbstick;
                RThumbstick = cs.RThumbstick;
                LTouchpad = cs.LTouchpad;
                RTouchpad = cs.RTouchpad;
                LBatteryPercentRemaining = 0;
                RBatteryPercentRemaining = 0;
                LRecenterCount = 0;
                RRecenterCount = 0;
                Reserved_27 = 0;
                Reserved_26 = 0;
                Reserved_25 = 0;
                Reserved_24 = 0;
                Reserved_23 = 0;
                Reserved_22 = 0;
                Reserved_21 = 0;
                Reserved_20 = 0;
                Reserved_19 = 0;
                Reserved_18 = 0;
                Reserved_17 = 0;
                Reserved_16 = 0;
                Reserved_15 = 0;
                Reserved_14 = 0;
                Reserved_13 = 0;
                Reserved_12 = 0;
                Reserved_11 = 0;
                Reserved_10 = 0;
                Reserved_09 = 0;
                Reserved_08 = 0;
                Reserved_07 = 0;
                Reserved_06 = 0;
                Reserved_05 = 0;
                Reserved_04 = 0;
                Reserved_03 = 0;
                Reserved_02 = 0;
                Reserved_01 = 0;
                Reserved_00 = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ControllerState2
        {
            public uint ConnectedControllers;
            public uint Buttons;
            public uint Touches;
            public uint NearTouches;
            public float LIndexTrigger;
            public float RIndexTrigger;
            public float LHandTrigger;
            public float RHandTrigger;
            public Vector2f LThumbstick;
            public Vector2f RThumbstick;
            public Vector2f LTouchpad;
            public Vector2f RTouchpad;

            public ControllerState2(ControllerState cs)
            {
                ConnectedControllers = cs.ConnectedControllers;
                Buttons = cs.Buttons;
                Touches = cs.Touches;
                NearTouches = cs.NearTouches;
                LIndexTrigger = cs.LIndexTrigger;
                RIndexTrigger = cs.RIndexTrigger;
                LHandTrigger = cs.LHandTrigger;
                RHandTrigger = cs.RHandTrigger;
                LThumbstick = cs.LThumbstick;
                RThumbstick = cs.RThumbstick;
                LTouchpad = new Vector2f() { x = 0.0f, y = 0.0f };
                RTouchpad = new Vector2f() { x = 0.0f, y = 0.0f };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ControllerState
        {
            public uint ConnectedControllers;
            public uint Buttons;
            public uint Touches;
            public uint NearTouches;
            public float LIndexTrigger;
            public float RIndexTrigger;
            public float LHandTrigger;
            public float RHandTrigger;
            public Vector2f LThumbstick;
            public Vector2f RThumbstick;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HapticsBuffer
        {
            public IntPtr Samples;
            public int SamplesCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HapticsState
        {
            public int SamplesAvailable;
            public int SamplesQueued;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HapticsDesc
        {
            public int SampleRateHz;
            public int SampleSizeInBytes;
            public int MinimumSafeSamplesQueued;
            public int MinimumBufferSamplesCount;
            public int OptimalBufferSamplesCount;
            public int MaximumBufferSamplesCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AppPerfFrameStats
        {
            public int HmdVsyncIndex;
            public int AppFrameIndex;
            public int AppDroppedFrameCount;
            public float AppMotionToPhotonLatency;
            public float AppQueueAheadTime;
            public float AppCpuElapsedTime;
            public float AppGpuElapsedTime;
            public int CompositorFrameIndex;
            public int CompositorDroppedFrameCount;
            public float CompositorLatency;
            public float CompositorCpuElapsedTime;
            public float CompositorGpuElapsedTime;
            public float CompositorCpuStartToGpuEndElapsedTime;
            public float CompositorGpuEndToVsyncElapsedTime;
        }

        public const int AppPerfFrameStatsMaxCount = 5;

        [StructLayout(LayoutKind.Sequential)]
        public struct AppPerfStats
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AppPerfFrameStatsMaxCount)]
            public AppPerfFrameStats[] FrameStats;
            public int FrameStatsCount;
            public Bool AnyFrameStatsDropped;
            public float AdaptiveGpuPerformanceScale;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Sizei
        {
            public int w;
            public int h;

            public static readonly Sizei zero = new Sizei { w = 0, h = 0 };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Sizef
        {
            public float w;
            public float h;

            public static readonly Sizef zero = new Sizef { w = 0, h = 0 };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector2i
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Recti
        {
            private Vector2i Pos;
            private Sizei Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rectf
        {
            private Vector2f Pos;
            private Sizef Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Frustumf
        {
            public float zNear;
            public float zFar;
            public float fovX;
            public float fovY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Frustumf2
        {
            public float zNear;
            public float zFar;
            public Fovf Fov;
        }

        public enum BoundaryType
        {
            OuterBoundary = 0x0001,
            PlayArea = 0x0100,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BoundaryTestResult
        {
            public Bool IsTriggering;
            public float ClosestDistance;
            public Vector3f ClosestPoint;
            public Vector3f ClosestPointNormal;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BoundaryGeometry
        {
            public BoundaryType BoundaryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public Vector3f[] Points;
            public int PointsCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Colorf
        {
            public float r;
            public float g;
            public float b;
            public float a;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Fovf
        {
            public float UpTan;
            public float DownTan;
            public float LeftTan;
            public float RightTan;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CameraIntrinsics
        {
            public Bool IsValid;
            public double LastChangedTimeSeconds;
            public Fovf FOVPort;
            public float VirtualNearPlaneDistanceMeters;
            public float VirtualFarPlaneDistanceMeters;
            public Sizei ImageSensorPixelResolution;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CameraExtrinsics
        {
            public Bool IsValid;
            public double LastChangedTimeSeconds;
            public CameraStatus CameraStatusData;
            public Node AttachedToNode;
            public Posef RelativePose;
        }

        public enum LayerLayout
        {
            Stereo = 0,
            Mono = 1,
            DoubleWide = 2,
            Array = 3,
            EnumSize = 0xF
        }

        public enum LayerFlags
        {
            Static = (1 << 0),
            LoadingScreen = (1 << 1),
            SymmetricFov = (1 << 2),
            TextureOriginAtBottomLeft = (1 << 3),
            ChromaticAberrationCorrection = (1 << 4),
            NoAllocation = (1 << 5),
            ProtectedContent = (1 << 6),
            AndroidSurfaceSwapChain = (1 << 7),
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LayerDesc
        {
            public OverlayShape Shape;
            public LayerLayout Layout;
            public Sizei TextureSize;
            public int MipLevels;
            public int SampleCount;
            public EyeTextureFormat Format;
            public int LayerFlags;

            //Eye FOV-only members.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Fovf[] Fov;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Rectf[] VisibleRect;
            public Sizei MaxViewportSize;
            private readonly EyeTextureFormat DepthFormat;

            public override string ToString()
            {
                var delim = ", ";
                return Shape.ToString()
                    + delim + Layout.ToString()
                    + delim + TextureSize.w.ToString() + "x" + TextureSize.h.ToString()
                    + delim + MipLevels.ToString()
                    + delim + SampleCount.ToString()
                    + delim + Format.ToString()
                    + delim + LayerFlags.ToString();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LayerSubmit
        {
            private readonly int LayerId;
            private readonly int TextureStage;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            private readonly Recti[] ViewportRect;
            private Posef Pose;
            private readonly int LayerSubmitFlags;
        }

        public enum TrackingConfidence
        {
            Low = 0,
            High = 0x3f800000,
        }

        public enum Hand
        {
            None = -1,
            HandLeft = 0,
            HandRight = 1,
        }

        [Flags]
        public enum HandStatus
        {
            HandTracked = (1 << 0), // if this is set the hand pose and bone rotations data is usable
            InputStateValid = (1 << 1), // if this is set the pointer pose and pinch data is usable
            SystemGestureInProgress = (1 << 6), // if this is set the hand is currently processing a system gesture
        }

        public enum BoneId
        {
            Invalid = -1,

            Hand_Start = 0,
            Hand_WristRoot = Hand_Start + 0, // root frame of the hand, where the wrist is located
            Hand_ForearmStub = Hand_Start + 1, // frame for user's forearm
            Hand_Thumb0 = Hand_Start + 2, // thumb trapezium bone
            Hand_Thumb1 = Hand_Start + 3, // thumb metacarpal bone
            Hand_Thumb2 = Hand_Start + 4, // thumb proximal phalange bone
            Hand_Thumb3 = Hand_Start + 5, // thumb distal phalange bone
            Hand_Index1 = Hand_Start + 6, // index proximal phalange bone
            Hand_Index2 = Hand_Start + 7, // index intermediate phalange bone
            Hand_Index3 = Hand_Start + 8, // index distal phalange bone
            Hand_Middle1 = Hand_Start + 9, // middle proximal phalange bone
            Hand_Middle2 = Hand_Start + 10, // middle intermediate phalange bone
            Hand_Middle3 = Hand_Start + 11, // middle distal phalange bone
            Hand_Ring1 = Hand_Start + 12, // ring proximal phalange bone
            Hand_Ring2 = Hand_Start + 13, // ring intermediate phalange bone
            Hand_Ring3 = Hand_Start + 14, // ring distal phalange bone
            Hand_Pinky0 = Hand_Start + 15, // pinky metacarpal bone
            Hand_Pinky1 = Hand_Start + 16, // pinky proximal phalange bone
            Hand_Pinky2 = Hand_Start + 17, // pinky intermediate phalange bone
            Hand_Pinky3 = Hand_Start + 18, // pinky distal phalange bone
            Hand_MaxSkinnable = Hand_Start + 19,
            // Bone tips are position only. They are not used for skinning but are useful for hit-testing.
            // NOTE: Hand_ThumbTip == Hand_MaxSkinnable since the extended tips need to be contiguous
            Hand_ThumbTip = Hand_Start + Hand_MaxSkinnable + 0, // tip of the thumb
            Hand_IndexTip = Hand_Start + Hand_MaxSkinnable + 1, // tip of the index finger
            Hand_MiddleTip = Hand_Start + Hand_MaxSkinnable + 2, // tip of the middle finger
            Hand_RingTip = Hand_Start + Hand_MaxSkinnable + 3, // tip of the ring finger
            Hand_PinkyTip = Hand_Start + Hand_MaxSkinnable + 4, // tip of the pinky
            Hand_End = Hand_Start + Hand_MaxSkinnable + 5,

            // add new bones here

            Max = Hand_End + 0,
        }

        public enum HandFinger
        {
            Thumb = 0,
            Index = 1,
            Middle = 2,
            Ring = 3,
            Pinky = 4,
            Max = 5,
        }

        [Flags]
        public enum HandFingerPinch
        {
            Thumb = (1 << HandFinger.Thumb),
            Index = (1 << HandFinger.Index),
            Middle = (1 << HandFinger.Middle),
            Ring = (1 << HandFinger.Ring),
            Pinky = (1 << HandFinger.Pinky),
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HandState
        {
            public HandStatus Status;
            public Posef RootPose;
            public Quatf[] BoneRotations;
            public HandFingerPinch Pinches;
            public float[] PinchStrength;
            public Posef PointerPose;
            public float HandScale;
            public TrackingConfidence HandConfidence;
            public TrackingConfidence[] FingerConfidences;
            public double RequestedTimeStamp;
            public double SampleTimeStamp;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HandStateInternal
        {
            public HandStatus Status;
            public Posef RootPose;
            public Quatf BoneRotations_0;
            public Quatf BoneRotations_1;
            public Quatf BoneRotations_2;
            public Quatf BoneRotations_3;
            public Quatf BoneRotations_4;
            public Quatf BoneRotations_5;
            public Quatf BoneRotations_6;
            public Quatf BoneRotations_7;
            public Quatf BoneRotations_8;
            public Quatf BoneRotations_9;
            public Quatf BoneRotations_10;
            public Quatf BoneRotations_11;
            public Quatf BoneRotations_12;
            public Quatf BoneRotations_13;
            public Quatf BoneRotations_14;
            public Quatf BoneRotations_15;
            public Quatf BoneRotations_16;
            public Quatf BoneRotations_17;
            public Quatf BoneRotations_18;
            public Quatf BoneRotations_19;
            public Quatf BoneRotations_20;
            public Quatf BoneRotations_21;
            public Quatf BoneRotations_22;
            public Quatf BoneRotations_23;
            public HandFingerPinch Pinches;
            public float PinchStrength_0;
            public float PinchStrength_1;
            public float PinchStrength_2;
            public float PinchStrength_3;
            public float PinchStrength_4;
            public Posef PointerPose;
            public float HandScale;
            public TrackingConfidence HandConfidence;
            public TrackingConfidence FingerConfidences_0;
            public TrackingConfidence FingerConfidences_1;
            public TrackingConfidence FingerConfidences_2;
            public TrackingConfidence FingerConfidences_3;
            public TrackingConfidence FingerConfidences_4;
            public double RequestedTimeStamp;
            public double SampleTimeStamp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BoneCapsule
        {
            public short BoneIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Vector3f[] Points;
            public float Radius;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Bone
        {
            public BoneId Id;
            public short ParentBoneIndex;
            public Posef Pose;
        }

        public enum SkeletonConstants
        {
            MaxBones = BoneId.Max,
            MaxBoneCapsules = 19,
        }

        public enum SkeletonType
        {
            None = -1,
            HandLeft = 0,
            HandRight = 1,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Skeleton
        {
            public SkeletonType Type;
            public uint NumBones;
            public uint NumBoneCapsules;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)SkeletonConstants.MaxBones)]
            public Bone[] Bones;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)SkeletonConstants.MaxBoneCapsules)]
            public BoneCapsule[] BoneCapsules;
        }

        public enum MeshConstants
        {
            MaxVertices = 3000,
            MaxIndices = MaxVertices * 6,
        }

        public enum MeshType
        {
            None = -1,
            HandLeft = 0,
            HandRight = 1,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Mesh
        {
            public MeshType Type;
            public uint NumVertices;
            public uint NumIndices;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MeshConstants.MaxVertices)]
            public Vector3f[] VertexPositions;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MeshConstants.MaxIndices)]
            public short[] Indices;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MeshConstants.MaxVertices)]
            public Vector3f[] VertexNormals;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MeshConstants.MaxVertices)]
            public Vector2f[] VertexUV0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MeshConstants.MaxVertices)]
            public Vector4s[] BlendIndices;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MeshConstants.MaxVertices)]
            public Vector4f[] BlendWeights;
        }

        public static bool initialized => OVRP_1_1_0.ovrp_GetInitialized() == Plugin.Bool.True;

        public static bool chromatic
        {
            get
            {
                return version >= OVRP_1_7_0.version && initialized && OVRP_1_7_0.ovrp_GetAppChromaticCorrection() == Plugin.Bool.True;
            }

            set
            {
                if (initialized && version >= OVRP_1_7_0.version)
                {
                    OVRP_1_7_0.ovrp_SetAppChromaticCorrection(ToBool(value));
                }
            }
        }

        public static bool monoscopic
        {
            get
            {
                return initialized && OVRP_1_1_0.ovrp_GetAppMonoscopic() == Plugin.Bool.True;
            }
            set
            {
                if (initialized)
                {
                    OVRP_1_1_0.ovrp_SetAppMonoscopic(ToBool(value));
                }
            }
        }

        public static bool rotation
        {
            get
            {
                return initialized && OVRP_1_1_0.ovrp_GetTrackingOrientationEnabled() == Bool.True;
            }
            set
            {
                if (initialized)
                {
                    OVRP_1_1_0.ovrp_SetTrackingOrientationEnabled(ToBool(value));
                }
            }
        }

        public static bool position
        {
            get
            {
                return initialized && OVRP_1_1_0.ovrp_GetTrackingPositionEnabled() == Bool.True;
            }
            set
            {
                if (initialized)
                {
                    OVRP_1_1_0.ovrp_SetTrackingPositionEnabled(ToBool(value));
                }
            }
        }

        public static bool useIPDInPositionTracking
        {
            get
            {
                if (initialized && version >= OVRP_1_6_0.version)
                {
                    return OVRP_1_6_0.ovrp_GetTrackingIPDEnabled() == Plugin.Bool.True;
                }

                return true;
            }

            set
            {
                if (initialized && version >= OVRP_1_6_0.version)
                {
                    OVRP_1_6_0.ovrp_SetTrackingIPDEnabled(ToBool(value));
                }
            }
        }

        public static bool positionSupported => initialized && OVRP_1_1_0.ovrp_GetTrackingPositionSupported() == Bool.True;

        public static bool positionTracked => initialized && OVRP_1_1_0.ovrp_GetNodePositionTracked(Node.EyeCenter) == Bool.True;

        public static bool powerSaving => initialized && OVRP_1_1_0.ovrp_GetSystemPowerSavingMode() == Bool.True;

        public static bool hmdPresent => initialized && OVRP_1_1_0.ovrp_GetNodePresent(Node.EyeCenter) == Bool.True;

        public static bool userPresent => initialized && OVRP_1_1_0.ovrp_GetUserPresent() == Bool.True;

        public static bool headphonesPresent => initialized && OVRP_1_3_0.ovrp_GetSystemHeadphonesPresent() == Plugin.Bool.True;

        public static int recommendedMSAALevel
        {
            get
            {
                if (initialized && version >= OVRP_1_6_0.version)
                {
                    return OVRP_1_6_0.ovrp_GetSystemRecommendedMSAALevel();
                }
                else
                {
                    return 2;
                }
            }
        }

        public static SystemRegion systemRegion
        {
            get
            {
                if (initialized && version >= OVRP_1_5_0.version)
                {
                    return OVRP_1_5_0.ovrp_GetSystemRegion();
                }
                else
                {
                    return SystemRegion.Unspecified;
                }
            }
        }

        private static GUID _nativeAudioOutGuid = new Plugin.GUID();
        private static Guid _cachedAudioOutGuid;
        private static string _cachedAudioOutString;

        public static string audioOutId
        {
            get
            {
                try
                {
                    if (_nativeAudioOutGuid == null)
                    {
                        _nativeAudioOutGuid = new Plugin.GUID();
                    }

                    var ptr = OVRP_1_1_0.ovrp_GetAudioOutId();
                    if (ptr != IntPtr.Zero)
                    {
                        Marshal.PtrToStructure(ptr, _nativeAudioOutGuid);
                        var managedGuid = new Guid(
                            _nativeAudioOutGuid.a,
                            _nativeAudioOutGuid.b,
                            _nativeAudioOutGuid.c,
                            _nativeAudioOutGuid.d0,
                            _nativeAudioOutGuid.d1,
                            _nativeAudioOutGuid.d2,
                            _nativeAudioOutGuid.d3,
                            _nativeAudioOutGuid.d4,
                            _nativeAudioOutGuid.d5,
                            _nativeAudioOutGuid.d6,
                            _nativeAudioOutGuid.d7);

                        if (managedGuid != _cachedAudioOutGuid)
                        {
                            _cachedAudioOutGuid = managedGuid;
                            _cachedAudioOutString = _cachedAudioOutGuid.ToString();
                        }

                        return _cachedAudioOutString;
                    }
                }
                catch { }

                return string.Empty;
            }
        }

        private static GUID _nativeAudioInGuid = new Plugin.GUID();
        private static Guid _cachedAudioInGuid;
        private static string _cachedAudioInString;

        public static string audioInId
        {
            get
            {
                try
                {
                    if (_nativeAudioInGuid == null)
                    {
                        _nativeAudioInGuid = new Plugin.GUID();
                    }

                    var ptr = OVRP_1_1_0.ovrp_GetAudioInId();
                    if (ptr != IntPtr.Zero)
                    {
                        Marshal.PtrToStructure(ptr, _nativeAudioInGuid);
                        var managedGuid = new Guid(
                            _nativeAudioInGuid.a,
                            _nativeAudioInGuid.b,
                            _nativeAudioInGuid.c,
                            _nativeAudioInGuid.d0,
                            _nativeAudioInGuid.d1,
                            _nativeAudioInGuid.d2,
                            _nativeAudioInGuid.d3,
                            _nativeAudioInGuid.d4,
                            _nativeAudioInGuid.d5,
                            _nativeAudioInGuid.d6,
                            _nativeAudioInGuid.d7);

                        if (managedGuid != _cachedAudioInGuid)
                        {
                            _cachedAudioInGuid = managedGuid;
                            _cachedAudioInString = _cachedAudioInGuid.ToString();
                        }

                        return _cachedAudioInString;
                    }
                }
                catch { }

                return string.Empty;
            }
        }

        public static bool hasVrFocus => OVRP_1_1_0.ovrp_GetAppHasVrFocus() == Bool.True;

        public static bool hasInputFocus
        {
            get
            {
                if (version >= OVRP_1_18_0.version)
                {
                    var inputFocus = Bool.False;
                    var result = OVRP_1_18_0.ovrp_GetAppHasInputFocus(out inputFocus);
                    if (Result.Success == result)
                    {
                        return inputFocus == Bool.True;
                    }
                    else
                    {
                        //Console.WriteLine("WARNING: ovrp_GetAppHasInputFocus return " + result);
                        return false;
                    }
                }

                return true;
            }
        }

        public static bool shouldQuit => OVRP_1_1_0.ovrp_GetAppShouldQuit() == Bool.True;

        public static bool shouldRecenter => OVRP_1_1_0.ovrp_GetAppShouldRecenter() == Bool.True;

        public static string productName => OVRP_1_1_0.ovrp_GetSystemProductName();

        public static string latency
        {
            get
            {
                if (!initialized)
                {
                    return string.Empty;
                }

                return OVRP_1_1_0.ovrp_GetAppLatencyTimings();
            }
        }

        public static float eyeDepth
        {
            get
            {
                if (!initialized)
                {
                    return 0.0f;
                }

                return OVRP_1_1_0.ovrp_GetUserEyeDepth();
            }
            set
            {
                OVRP_1_1_0.ovrp_SetUserEyeDepth(value);
            }
        }

        public static float eyeHeight
        {
            get
            {
                return OVRP_1_1_0.ovrp_GetUserEyeHeight();
            }
            set
            {
                OVRP_1_1_0.ovrp_SetUserEyeHeight(value);
            }
        }

        public static float batteryLevel => OVRP_1_1_0.ovrp_GetSystemBatteryLevel();

        public static float batteryTemperature => OVRP_1_1_0.ovrp_GetSystemBatteryTemperature();

        public static int cpuLevel
        {
            get
            {
                return OVRP_1_1_0.ovrp_GetSystemCpuLevel();
            }
            set
            {
                OVRP_1_1_0.ovrp_SetSystemCpuLevel(value);
            }
        }

        public static int gpuLevel
        {
            get
            {
                return OVRP_1_1_0.ovrp_GetSystemGpuLevel();
            }
            set
            {
                OVRP_1_1_0.ovrp_SetSystemGpuLevel(value);
            }
        }

        public static int vsyncCount
        {
            get
            {
                return OVRP_1_1_0.ovrp_GetSystemVSyncCount();
            }
            set
            {
                OVRP_1_2_0.ovrp_SetSystemVSyncCount(value);
            }
        }

        public static float systemVolume => OVRP_1_1_0.ovrp_GetSystemVolume();

        public static float ipd
        {
            get
            {
                return OVRP_1_1_0.ovrp_GetUserIPD();
            }
            set
            {
                OVRP_1_1_0.ovrp_SetUserIPD(value);
            }
        }

        public static bool occlusionMesh
        {
            get
            {
                return initialized && (OVRP_1_3_0.ovrp_GetEyeOcclusionMeshEnabled() == Bool.True);
            }
            set
            {
                if (!initialized)
                {
                    return;
                }

                OVRP_1_3_0.ovrp_SetEyeOcclusionMeshEnabled(ToBool(value));
            }
        }

        public static BatteryStatus batteryStatus => OVRP_1_1_0.ovrp_GetSystemBatteryStatus();

        public static Frustumf GetEyeFrustum(Eye eyeId)
        {
            return OVRP_1_1_0.ovrp_GetNodeFrustum((Node)eyeId);
        }

        public static Sizei GetEyeTextureSize(Eye eyeId)
        {
            return OVRP_0_1_0.ovrp_GetEyeTextureSize(eyeId);
        }

        public static Posef GetTrackerPose(Tracker trackerId)
        {
            return GetNodePose((Node)((int)trackerId + (int)Node.TrackerZero), ProcessingStep.Render);
        }

        public static Frustumf GetTrackerFrustum(Tracker trackerId)
        {
            return OVRP_1_1_0.ovrp_GetNodeFrustum((Node)((int)trackerId + (int)Node.TrackerZero));
        }

        public static bool ShowUI(PlatformUI ui)
        {
            return OVRP_1_1_0.ovrp_ShowSystemUI(ui) == Bool.True;
        }

        public static bool EnqueueSubmitLayer(bool onTop, bool headLocked, bool noDepthBufferTesting, IntPtr leftTexture, IntPtr rightTexture, int layerId, int frameIndex, Posef pose, Vector3f scale, int layerIndex = 0, OverlayShape shape = OverlayShape.Quad,
                                            bool overrideTextureRectMatrix = false, TextureRectMatrixf textureRectMatrix = default(TextureRectMatrixf), bool overridePerLayerColorScaleAndOffset = false, Vector4 colorScale = default(Vector4), Vector4 colorOffset = default(Vector4),
                                            bool expensiveSuperSample = false, bool hidden = false)
        {
            if (!initialized)
            {
                return false;
            }

            if (version >= OVRP_1_6_0.version)
            {
                var flags = (uint)OverlayFlag.None;
                if (onTop)
                {
                    flags |= (uint)OverlayFlag.OnTop;
                }

                if (headLocked)
                {
                    flags |= (uint)OverlayFlag.HeadLocked;
                }

                if (noDepthBufferTesting)
                {
                    flags |= (uint)OverlayFlag.NoDepth;
                }

                if (expensiveSuperSample)
                {
                    flags |= (uint)OverlayFlag.ExpensiveSuperSample;
                }

                if (hidden)
                {
                    flags |= (uint)OverlayFlag.Hidden;
                }

                if (shape == OverlayShape.Cylinder || shape == OverlayShape.Cubemap)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix
                        && version >= OVRP_1_7_0.version)
                    {
                        flags |= (uint)(shape) << OverlayShapeFlagShift;
                    }
                    else if (shape == OverlayShape.Cubemap && version >= OVRP_1_10_0.version)
                    {
                        flags |= (uint)(shape) << OverlayShapeFlagShift;
                    }
                    else if (shape == OverlayShape.Cylinder && version >= OVRP_1_16_0.version)
                    {
                        flags |= (uint)(shape) << OverlayShapeFlagShift;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (shape == OverlayShape.OffcenterCubemap)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix
                        && version >= OVRP_1_11_0.version)
                    {
                        flags |= (uint)(shape) << OverlayShapeFlagShift;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (shape == OverlayShape.Equirect)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix
                        && version >= OVRP_1_21_0.version)
                    {
                        flags |= (uint)(shape) << OverlayShapeFlagShift;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (version >= OVRP_1_34_0.version && layerId != -1)
                {
                    return OVRP_1_34_0.ovrp_EnqueueSubmitLayer2(flags, leftTexture, rightTexture, layerId, frameIndex, ref pose, ref scale, layerIndex,
                    overrideTextureRectMatrix ? Bool.True : Bool.False, ref textureRectMatrix, overridePerLayerColorScaleAndOffset ? Bool.True : Bool.False, ref colorScale, ref colorOffset) == Result.Success;
                }
                else if (version >= OVRP_1_15_0.version && layerId != -1)
                {
                    return OVRP_1_15_0.ovrp_EnqueueSubmitLayer(flags, leftTexture, rightTexture, layerId, frameIndex, ref pose, ref scale, layerIndex) == Result.Success;
                }

                return OVRP_1_6_0.ovrp_SetOverlayQuad3(flags, leftTexture, rightTexture, IntPtr.Zero, pose, scale, layerIndex) == Bool.True;
            }

            if (layerIndex != 0)
            {
                return false;
            }

            return OVRP_0_1_1.ovrp_SetOverlayQuad2(ToBool(onTop), ToBool(headLocked), leftTexture, IntPtr.Zero, pose, scale) == Bool.True;
        }

        public static LayerDesc CalculateLayerDesc(OverlayShape shape, LayerLayout layout, Sizei textureSize,
            int mipLevels, int sampleCount, EyeTextureFormat format, int layerFlags)
        {
            var layerDesc = new LayerDesc();
            if (!initialized)
            {
                return layerDesc;
            }

            if (version >= OVRP_1_15_0.version)
            {
                OVRP_1_15_0.ovrp_CalculateLayerDesc(shape, layout, ref textureSize,
                    mipLevels, sampleCount, format, layerFlags, ref layerDesc);
            }

            return layerDesc;
        }

        public static bool EnqueueSetupLayer(LayerDesc desc, int compositionDepth, IntPtr layerID)
        {
            if (!initialized)
            {
                return false;
            }

            if (version >= OVRP_1_28_0.version)
            {
                return OVRP_1_28_0.ovrp_EnqueueSetupLayer2(ref desc, compositionDepth, layerID) == Result.Success;
            }
            else if (version >= OVRP_1_15_0.version)
            {
                if (compositionDepth != 0)
                {
                    Console.WriteLine("WARNING: Use Oculus Plugin 1.28.0 or above to support non-zero compositionDepth");
                }
                return OVRP_1_15_0.ovrp_EnqueueSetupLayer(ref desc, layerID) == Result.Success;
            }

            return false;
        }

        public static bool EnqueueDestroyLayer(IntPtr layerID)
        {
            if (!initialized)
            {
                return false;
            }

            if (version >= OVRP_1_15_0.version)
            {
                return OVRP_1_15_0.ovrp_EnqueueDestroyLayer(layerID) == Result.Success;
            }

            return false;
        }

        public static IntPtr GetLayerTexture(int layerId, int stage, Eye eyeId)
        {
            var textureHandle = IntPtr.Zero;
            if (!initialized)
            {
                return textureHandle;
            }

            if (version >= OVRP_1_15_0.version)
            {
                OVRP_1_15_0.ovrp_GetLayerTexturePtr(layerId, stage, eyeId, ref textureHandle);
            }

            return textureHandle;
        }

        public static int GetLayerTextureStageCount(int layerId)
        {
            if (!initialized)
            {
                return 1;
            }

            var stageCount = 1;

            if (version >= OVRP_1_15_0.version)
            {
                OVRP_1_15_0.ovrp_GetLayerTextureStageCount(layerId, ref stageCount);
            }

            return stageCount;
        }

        public static IntPtr GetLayerAndroidSurfaceObject(int layerId)
        {
            var surfaceObject = IntPtr.Zero;
            if (!initialized)
            {
                return surfaceObject;
            }

            if (version >= OVRP_1_29_0.version)
            {
                OVRP_1_29_0.ovrp_GetLayerAndroidSurfaceObject(layerId, ref surfaceObject);
            }

            return surfaceObject;
        }

        public static bool UpdateNodePhysicsPoses(int frameIndex, double predictionSeconds)
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_Update2((int)ProcessingStep.Physics, frameIndex, predictionSeconds) == Bool.True;
            }

            return false;
        }

        public static Posef GetNodePose(Node nodeId, ProcessingStep stepId)
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).Pose;
            }

            if (version >= OVRP_1_8_0.version && stepId == ProcessingStep.Physics)
            {
                return OVRP_1_8_0.ovrp_GetNodePose2(0, nodeId);
            }

            return OVRP_0_1_2.ovrp_GetNodePose(nodeId);
        }

        public static Vector3f GetNodeVelocity(Node nodeId, ProcessingStep stepId)
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).Velocity;
            }

            if (version >= OVRP_1_8_0.version && stepId == ProcessingStep.Physics)
            {
                return OVRP_1_8_0.ovrp_GetNodeVelocity2(0, nodeId).Position;
            }

            return OVRP_0_1_3.ovrp_GetNodeVelocity(nodeId).Position;
        }

        public static Vector3f GetNodeAngularVelocity(Node nodeId, ProcessingStep stepId)
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).AngularVelocity;
            }

            return new Vector3f(); //TODO: Convert legacy quat to vec3?
        }

        public static Vector3f GetNodeAcceleration(Node nodeId, ProcessingStep stepId)
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).Acceleration;
            }

            if (version >= OVRP_1_8_0.version && stepId == ProcessingStep.Physics)
            {
                return OVRP_1_8_0.ovrp_GetNodeAcceleration2(0, nodeId).Position;
            }

            return OVRP_0_1_3.ovrp_GetNodeAcceleration(nodeId).Position;
        }

        public static Vector3f GetNodeAngularAcceleration(Node nodeId, ProcessingStep stepId)
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId).AngularAcceleration;
            }

            return new Vector3f(); //TODO: Convert legacy quat to vec3?
        }

        public static bool GetNodePresent(Node nodeId)
        {
            return OVRP_1_1_0.ovrp_GetNodePresent(nodeId) == Bool.True;
        }

        public static bool GetNodeOrientationTracked(Node nodeId)
        {
            return OVRP_1_1_0.ovrp_GetNodeOrientationTracked(nodeId) == Bool.True;
        }

        public static bool GetNodeOrientationValid(Node nodeId)
        {
            if (version >= OVRP_1_38_0.version)
            {
                var orientationValid = Bool.False;
                var result = OVRP_1_38_0.ovrp_GetNodeOrientationValid(nodeId, ref orientationValid);
                return result == Result.Success && orientationValid == Bool.True;
            }
            else
            {
                return GetNodeOrientationTracked(nodeId);
            }

        }

        public static bool GetNodePositionTracked(Node nodeId)
        {
            return OVRP_1_1_0.ovrp_GetNodePositionTracked(nodeId) == Bool.True;
        }

        public static bool GetNodePositionValid(Node nodeId)
        {
            if (version >= OVRP_1_38_0.version)
            {
                var positionValid = Bool.False;
                var result = OVRP_1_38_0.ovrp_GetNodePositionValid(nodeId, ref positionValid);
                return result == Result.Success && positionValid == Bool.True;
            }
            else
            {
                return GetNodePositionTracked(nodeId);
            }
        }

        public static PoseStatef GetNodePoseStateRaw(Node nodeId, ProcessingStep stepId)
        {
            if (version >= OVRP_1_29_0.version)
            {
                var result = OVRP_1_29_0.ovrp_GetNodePoseStateRaw(stepId, -1, nodeId, out var nodePoseState);
                if (result == Result.Success)
                {
                    return nodePoseState;
                }
                else
                {
                    return PoseStatef.identity;
                }
            }
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetNodePoseState(stepId, nodeId);
            }
            else
            {
                return PoseStatef.identity;
            }
        }

        public static Posef GetCurrentTrackingTransformPose()
        {
            if (version >= OVRP_1_30_0.version)
            {
                var result = OVRP_1_30_0.ovrp_GetCurrentTrackingTransformPose(out var trackingTransformPose);
                if (result == Result.Success)
                {
                    return trackingTransformPose;
                }
                else
                {
                    return Posef.identity;
                }
            }
            else
            {
                return Posef.identity;
            }
        }

        public static Posef GetTrackingTransformRawPose()
        {
            if (version >= OVRP_1_30_0.version)
            {
                var result = OVRP_1_30_0.ovrp_GetTrackingTransformRawPose(out var trackingTransforRawPose);
                if (result == Result.Success)
                {
                    return trackingTransforRawPose;
                }
                else
                {
                    return Posef.identity;
                }
            }
            else
            {
                return Posef.identity;
            }
        }

        public static Posef GetTrackingTransformRelativePose(TrackingOrigin trackingOrigin)
        {
            if (version >= OVRP_1_38_0.version)
            {
                var trackingTransformRelativePose = Posef.identity;
                var result = OVRP_1_38_0.ovrp_GetTrackingTransformRelativePose(ref trackingTransformRelativePose, trackingOrigin);
                if (result == Result.Success)
                {
                    return trackingTransformRelativePose;
                }
                else
                {
                    return Posef.identity;
                }
            }
            else
            {
                return Posef.identity;
            }
        }

        public static ControllerState GetControllerState(uint controllerMask)
        {
            return OVRP_1_1_0.ovrp_GetControllerState(controllerMask);
        }

        public static ControllerState2 GetControllerState2(uint controllerMask)
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetControllerState2(controllerMask);
            }

            return new ControllerState2(OVRP_1_1_0.ovrp_GetControllerState(controllerMask));
        }

        public static ControllerState4 GetControllerState4(uint controllerMask)
        {
            if (version >= OVRP_1_16_0.version)
            {
                var controllerState = new ControllerState4();
                OVRP_1_16_0.ovrp_GetControllerState4(controllerMask, ref controllerState);
                return controllerState;
            }

            return new ControllerState4(GetControllerState2(controllerMask));
        }

        public static bool SetControllerVibration(uint controllerMask, float frequency, float amplitude)
        {
            return OVRP_0_1_2.ovrp_SetControllerVibration(controllerMask, frequency, amplitude) == Bool.True;
        }

        public static HapticsDesc GetControllerHapticsDesc(uint controllerMask)
        {
            if (version >= OVRP_1_6_0.version)
            {
                return OVRP_1_6_0.ovrp_GetControllerHapticsDesc(controllerMask);
            }
            else
            {
                return new HapticsDesc();
            }
        }

        public static HapticsState GetControllerHapticsState(uint controllerMask)
        {
            if (version >= OVRP_1_6_0.version)
            {
                return OVRP_1_6_0.ovrp_GetControllerHapticsState(controllerMask);
            }
            else
            {
                return new HapticsState();
            }
        }

        public static bool SetControllerHaptics(uint controllerMask, HapticsBuffer hapticsBuffer)
        {
            if (version >= OVRP_1_6_0.version)
            {
                return OVRP_1_6_0.ovrp_SetControllerHaptics(controllerMask, hapticsBuffer) == Bool.True;
            }
            else
            {
                return false;
            }
        }

        public static float GetEyeRecommendedResolutionScale()
        {
            if (version >= OVRP_1_6_0.version)
            {
                return OVRP_1_6_0.ovrp_GetEyeRecommendedResolutionScale();
            }
            else
            {
                return 1.0f;
            }
        }

        public static float GetAppCpuStartToGpuEndTime()
        {
            if (version >= OVRP_1_6_0.version)
            {
                return OVRP_1_6_0.ovrp_GetAppCpuStartToGpuEndTime();
            }
            else
            {
                return 0.0f;
            }
        }

        public static bool GetBoundaryConfigured()
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_GetBoundaryConfigured() == Plugin.Bool.True;
            }
            else
            {
                return false;
            }
        }

        public static BoundaryTestResult TestBoundaryNode(Node nodeId, BoundaryType boundaryType)
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_TestBoundaryNode(nodeId, boundaryType);
            }
            else
            {
                return new BoundaryTestResult();
            }
        }

        public static BoundaryTestResult TestBoundaryPoint(Vector3f point, BoundaryType boundaryType)
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_TestBoundaryPoint(point, boundaryType);
            }
            else
            {
                return new BoundaryTestResult();
            }
        }

        public static BoundaryGeometry GetBoundaryGeometry(BoundaryType boundaryType)
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_GetBoundaryGeometry(boundaryType);
            }
            else
            {
                return new BoundaryGeometry();
            }
        }

        public static bool GetBoundaryGeometry2(BoundaryType boundaryType, IntPtr points, ref int pointsCount)
        {
            if (version >= OVRP_1_9_0.version)
            {
                return OVRP_1_9_0.ovrp_GetBoundaryGeometry2(boundaryType, points, ref pointsCount) == Plugin.Bool.True;
            }
            else
            {
                pointsCount = 0;

                return false;
            }
        }

        public static AppPerfStats GetAppPerfStats()
        {
            if (version >= OVRP_1_9_0.version)
            {
                return OVRP_1_9_0.ovrp_GetAppPerfStats();
            }
            else
            {
                return new AppPerfStats();
            }
        }

        public static bool ResetAppPerfStats()
        {
            if (version >= OVRP_1_9_0.version)
            {
                return OVRP_1_9_0.ovrp_ResetAppPerfStats() == Plugin.Bool.True;
            }
            else
            {
                return false;
            }
        }

        public static float GetAppFramerate()
        {
            if (version >= OVRP_1_12_0.version)
            {
                return OVRP_1_12_0.ovrp_GetAppFramerate();
            }
            else
            {
                return 0.0f;
            }
        }

        public static bool SetHandNodePoseStateLatency(double latencyInSeconds)
        {
            if (version >= OVRP_1_18_0.version)
            {
                var result = OVRP_1_18_0.ovrp_SetHandNodePoseStateLatency(latencyInSeconds);
                if (result == Result.Success)
                {
                    return true;
                }
                else
                {
                    //Console.WriteLine("WARNING: ovrp_SetHandNodePoseStateLatency return " + result);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static double GetHandNodePoseStateLatency()
        {
            if (version >= OVRP_1_18_0.version)
            {
                if (OVRP_1_18_0.ovrp_GetHandNodePoseStateLatency(out var value) == Plugin.Result.Success)
                {
                    return value;
                }
                else
                {
                    return 0.0;
                }
            }
            else
            {
                return 0.0;
            }
        }

        public static EyeTextureFormat GetDesiredEyeTextureFormat()
        {
            if (version >= OVRP_1_11_0.version)
            {
                var eyeTextureFormatValue = (uint)OVRP_1_11_0.ovrp_GetDesiredEyeTextureFormat();

                // convert both R8G8B8A8 and R8G8B8A8_SRGB to R8G8B8A8 here for avoid confusing developers
                if (eyeTextureFormatValue == 1)
                {
                    eyeTextureFormatValue = 0;
                }

                return (EyeTextureFormat)eyeTextureFormatValue;
            }
            else
            {
                return EyeTextureFormat.Default;
            }
        }

        public static bool SetDesiredEyeTextureFormat(EyeTextureFormat value)
        {
            if (version >= OVRP_1_11_0.version)
            {
                return OVRP_1_11_0.ovrp_SetDesiredEyeTextureFormat(value) == Plugin.Bool.True;
            }
            else
            {
                return false;
            }
        }

        public static int GetExternalCameraCount()
        {
            if (version >= OVRP_1_15_0.version)
            {
                var result = OVRP_1_15_0.ovrp_GetExternalCameraCount(out var cameraCount);
                if (result != Plugin.Result.Success)
                {
                    //Console.WriteLine("WARNING: ovrp_GetExternalCameraCount return " + result);
                    return 0;
                }

                return cameraCount;
            }
            else
            {
                return 0;
            }
        }

        public static bool UpdateExternalCamera()
        {
            if (version >= OVRP_1_15_0.version)
            {
                var result = OVRP_1_15_0.ovrp_UpdateExternalCamera();
                if (result != Result.Success)
                {
                    //Console.WriteLine("WARNING: ovrp_UpdateExternalCamera return " + result);
                }
                return result == Result.Success;
            }
            else
            {
                return false;
            }
        }

        public static bool GetMixedRealityCameraInfo(int cameraId, out CameraExtrinsics cameraExtrinsics, out CameraIntrinsics cameraIntrinsics, out Posef calibrationRawPose)
        {
            cameraExtrinsics = default(CameraExtrinsics);
            cameraIntrinsics = default(CameraIntrinsics);
            calibrationRawPose = Posef.identity;

            if (version >= OVRP_1_15_0.version)
            {
                var retValue = true;

                var result = OVRP_1_15_0.ovrp_GetExternalCameraExtrinsics(cameraId, out cameraExtrinsics);
                if (result != Result.Success)
                {
                    retValue = false;
                    //Console.WriteLine("WARNING: ovrp_GetExternalCameraExtrinsics return " + result);
                }

                result = OVRP_1_15_0.ovrp_GetExternalCameraIntrinsics(cameraId, out cameraIntrinsics);
                if (result != Result.Success)
                {
                    retValue = false;
                    //Console.WriteLine("WARNING: ovrp_GetExternalCameraIntrinsics return " + result);
                }

                return retValue;
            }
            else
            {
                return false;
            }
        }

        public static bool OverrideExternalCameraFov(int cameraId, bool useOverriddenFov, Fovf fov)
        {
            if (version >= OVRP_1_44_0.version)
            {
                var retValue = true;
                var result = OVRP_1_44_0.ovrp_OverrideExternalCameraFov(cameraId, useOverriddenFov ? Bool.True : Bool.False, ref fov);
                if (result != Result.Success)
                {
                    retValue = false;
                }
                return retValue;
            }
            else
            {
                return false;
            }
        }


        public static bool GetUseOverriddenExternalCameraFov(int cameraId)
        {
            if (version >= OVRP_1_44_0.version)
            {
                var retValue = true;
                var useOverriddenFov = Bool.False;
                var result = OVRP_1_44_0.ovrp_GetUseOverriddenExternalCameraFov(cameraId, out useOverriddenFov);
                if (result != Result.Success)
                {
                    retValue = false;
                }
                if (useOverriddenFov == Bool.False)
                {
                    retValue = false;
                }
                return retValue;
            }
            else
            {
                return false;
            }
        }

        public static bool OverrideExternalCameraStaticPose(int cameraId, bool useOverriddenPose, Posef pose)
        {
            if (version >= OVRP_1_44_0.version)
            {
                var retValue = true;
                var result = OVRP_1_44_0.ovrp_OverrideExternalCameraStaticPose(cameraId, useOverriddenPose ? Bool.True : Bool.False, ref pose);
                if (result != Result.Success)
                {
                    retValue = false;
                }
                return retValue;
            }
            else
            {
                return false;
            }
        }

        public static bool GetUseOverriddenExternalCameraStaticPose(int cameraId)
        {
            if (version >= OVRP_1_44_0.version)
            {
                var retValue = true;
                var useOverriddenStaticPose = Bool.False;
                var result = OVRP_1_44_0.ovrp_GetUseOverriddenExternalCameraStaticPose(cameraId, out useOverriddenStaticPose);
                if (result != Result.Success)
                {
                    retValue = false;
                }
                if (useOverriddenStaticPose == Bool.False)
                {
                    retValue = false;
                }
                return retValue;
            }
            else
            {
                return false;
            }
        }

        public static bool ResetDefaultExternalCamera()
        {
            if (version >= OVRP_1_44_0.version)
            {
                var result = OVRP_1_44_0.ovrp_ResetDefaultExternalCamera();
                if (result != Result.Success)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SetDefaultExternalCamera(string cameraName, ref CameraIntrinsics cameraIntrinsics, ref CameraExtrinsics cameraExtrinsics)
        {
            if (version >= OVRP_1_44_0.version)
            {
                var result = OVRP_1_44_0.ovrp_SetDefaultExternalCamera(cameraName, ref cameraIntrinsics, ref cameraExtrinsics);
                if (result != Result.Success)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vector3f GetBoundaryDimensions(BoundaryType boundaryType)
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_GetBoundaryDimensions(boundaryType);
            }
            else
            {
                return new Vector3f();
            }
        }

        public static bool GetBoundaryVisible()
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_GetBoundaryVisible() == Plugin.Bool.True;
            }
            else
            {
                return false;
            }
        }

        public static bool SetBoundaryVisible(bool value)
        {
            if (version >= OVRP_1_8_0.version)
            {
                return OVRP_1_8_0.ovrp_SetBoundaryVisible(ToBool(value)) == Plugin.Bool.True;
            }
            else
            {
                return false;
            }
        }

        public static SystemHeadset GetSystemHeadsetType()
        {
            if (version >= OVRP_1_9_0.version)
            {
                return OVRP_1_9_0.ovrp_GetSystemHeadsetType();
            }

            return SystemHeadset.None;
        }

        public static Controller GetActiveController()
        {
            if (version >= OVRP_1_9_0.version)
            {
                return OVRP_1_9_0.ovrp_GetActiveController();
            }

            return Controller.None;
        }

        public static Controller GetConnectedControllers()
        {
            if (version >= OVRP_1_9_0.version)
            {
                return OVRP_1_9_0.ovrp_GetConnectedControllers();
            }

            return Controller.None;
        }

        private static Bool ToBool(bool b)
        {
            return (b) ? Plugin.Bool.True : Plugin.Bool.False;
        }

        public static TrackingOrigin GetTrackingOriginType()
        {
            return OVRP_1_0_0.ovrp_GetTrackingOriginType();
        }

        public static bool SetTrackingOriginType(TrackingOrigin originType)
        {
            return OVRP_1_0_0.ovrp_SetTrackingOriginType(originType) == Bool.True;
        }

        public static Posef GetTrackingCalibratedOrigin()
        {
            return OVRP_1_0_0.ovrp_GetTrackingCalibratedOrigin();
        }

        public static bool SetTrackingCalibratedOrigin()
        {
            return OVRP_1_2_0.ovrpi_SetTrackingCalibratedOrigin() == Bool.True;
        }

        public static bool RecenterTrackingOrigin(RecenterFlags flags)
        {
            return OVRP_1_0_0.ovrp_RecenterTrackingOrigin((uint)flags) == Bool.True;
        }

        public static bool fixedFoveatedRenderingSupported
        {
            get
            {
                if (version >= OVRP_1_21_0.version)
                {
                    var result = OVRP_1_21_0.ovrp_GetTiledMultiResSupported(out var supported);
                    if (result == Result.Success)
                    {
                        return supported == Bool.True;
                    }
                    else
                    {
                        //Console.WriteLine("WARNING: ovrp_GetTiledMultiResSupported return " + result);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static FixedFoveatedRenderingLevel fixedFoveatedRenderingLevel
        {
            get
            {
                if (version >= OVRP_1_21_0.version && fixedFoveatedRenderingSupported)
                {
                    var result = OVRP_1_21_0.ovrp_GetTiledMultiResLevel(out var level);
                    if (result != Result.Success)
                    {
                        //Console.WriteLine("WARNING: ovrp_GetTiledMultiResLevel return " + result);
                    }
                    return level;
                }
                else
                {
                    return FixedFoveatedRenderingLevel.Off;
                }
            }
            set
            {
                if (version >= OVRP_1_21_0.version && fixedFoveatedRenderingSupported)
                {
                    var result = OVRP_1_21_0.ovrp_SetTiledMultiResLevel(value);
                    if (result != Result.Success)
                    {
                        //Console.WriteLine("WARNING: ovrp_SetTiledMultiResLevel return " + result);
                    }
                }
            }
        }

        public static bool gpuUtilSupported
        {
            get
            {
                if (version >= OVRP_1_21_0.version)
                {
                    var result = OVRP_1_21_0.ovrp_GetGPUUtilSupported(out var supported);
                    if (result == Result.Success)
                    {
                        return supported == Bool.True;
                    }
                    else
                    {
                        //Console.WriteLine("WARNING: ovrp_GetGPUUtilSupported return " + result);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static float gpuUtilLevel
        {
            get
            {
                if (version >= OVRP_1_21_0.version && gpuUtilSupported)
                {
                    var result = OVRP_1_21_0.ovrp_GetGPUUtilLevel(out var level);
                    if (result == Result.Success)
                    {
                        return level;
                    }
                    else
                    {
                        //Console.WriteLine("WARNING: ovrp_GetGPUUtilLevel return " + result);
                        return 0.0f;
                    }
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        private static NativeBuffer _nativeSystemDisplayFrequenciesAvailable = null;
        private static float[] _cachedSystemDisplayFrequenciesAvailable = null;

        public static float[] systemDisplayFrequenciesAvailable
        {
            get
            {
                if (_cachedSystemDisplayFrequenciesAvailable == null)
                {
                    _cachedSystemDisplayFrequenciesAvailable = new float[0];

                    if (version >= OVRP_1_21_0.version)
                    {
                        var numFrequencies = 0;
                        var result = OVRP_1_21_0.ovrp_GetSystemDisplayAvailableFrequencies(IntPtr.Zero, ref numFrequencies);
                        if (result == Result.Success)
                        {
                            if (numFrequencies > 0)
                            {
                                var maxNumElements = numFrequencies;
                                _nativeSystemDisplayFrequenciesAvailable = new NativeBuffer(sizeof(float) * maxNumElements);
                                result = OVRP_1_21_0.ovrp_GetSystemDisplayAvailableFrequencies(_nativeSystemDisplayFrequenciesAvailable.GetPointer(), ref numFrequencies);
                                if (result == Result.Success)
                                {
                                    var numElementsToCopy = (numFrequencies <= maxNumElements) ? numFrequencies : maxNumElements;
                                    if (numElementsToCopy > 0)
                                    {
                                        _cachedSystemDisplayFrequenciesAvailable = new float[numElementsToCopy];
                                        Marshal.Copy(_nativeSystemDisplayFrequenciesAvailable.GetPointer(), _cachedSystemDisplayFrequenciesAvailable, 0, numElementsToCopy);
                                    }
                                }
                            }
                        }
                    }
                }

                return _cachedSystemDisplayFrequenciesAvailable;
            }
        }

        public static float systemDisplayFrequency
        {
            get
            {
                if (version >= OVRP_1_21_0.version)
                {
                    var result = OVRP_1_21_0.ovrp_GetSystemDisplayFrequency2(out var displayFrequency);
                    if (result == Result.Success)
                    {
                        return displayFrequency;
                    }

                    return 0.0f;
                }
                else if (version >= OVRP_1_1_0.version)
                {
                    return OVRP_1_1_0.ovrp_GetSystemDisplayFrequency();
                }
                else
                {
                    return 0.0f;
                }
            }
            set
            {
                if (version >= OVRP_1_21_0.version)
                {
                    OVRP_1_21_0.ovrp_SetSystemDisplayFrequency(value);
                }
            }
        }

        public static bool GetNodeFrustum2(Node nodeId, out Frustumf2 frustum)
        {
            frustum = default(Frustumf2);

            if (version >= OVRP_1_15_0.version)
            {
                var result = OVRP_1_15_0.ovrp_GetNodeFrustum2(nodeId, out frustum);
                if (result != Result.Success)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool AsymmetricFovEnabled
        {
            get
            {
                if (version >= OVRP_1_21_0.version)
                {
                    var asymmetricFovEnabled = Bool.False;
                    var result = OVRP_1_21_0.ovrp_GetAppAsymmetricFov(out asymmetricFovEnabled);

                    if (result != Result.Success)
                    {
                        return false;
                    }
                    else
                    {
                        return asymmetricFovEnabled == Bool.True;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool EyeTextureArrayEnabled
        {
            get
            {
                if (version >= OVRP_1_15_0.version)
                {
                    var enabled = Bool.False;
                    enabled = OVRP_1_15_0.ovrp_GetEyeTextureArrayEnabled();
                    return enabled == Bool.True;
                }
                else
                {
                    return false;
                }
            }
        }


        public static Handedness GetDominantHand()
        {

            if (version >= OVRP_1_28_0.version && OVRP_1_28_0.ovrp_GetDominantHand(out var dominantHand) == Result.Success)
            {
                return dominantHand;
            }

            return Handedness.Unsupported;
        }

        public static bool GetReorientHMDOnControllerRecenter()
        {
            if (version < OVRP_1_28_0.version || OVRP_1_28_0.ovrp_GetReorientHMDOnControllerRecenter(out var recenterMode) != Result.Success)
            {
                return false;
            }

            return (recenterMode == Bool.True);
        }

        public static bool SetReorientHMDOnControllerRecenter(bool recenterSetting)
        {
            var ovrpBoolRecenterSetting = recenterSetting ? Bool.True : Bool.False;
            if (version < OVRP_1_28_0.version || OVRP_1_28_0.ovrp_SetReorientHMDOnControllerRecenter(ovrpBoolRecenterSetting) != Result.Success)
            {
                return false;
            }

            return true;
        }

        public static bool SendEvent(string name, string param = "", string source = "")
        {
            if (version >= OVRP_1_30_0.version)
            {
                return OVRP_1_30_0.ovrp_SendEvent2(name, param, source.Length == 0 ? "integration" : source) == Result.Success;
            }
            else if (version >= OVRP_1_28_0.version)
            {
                return OVRP_1_28_0.ovrp_SendEvent(name, param) == Result.Success;
            }
            else
            {
                return false;
            }
        }

        public static bool SetHeadPoseModifier(ref Quatf relativeRotation, ref Vector3f relativeTranslation)
        {
            if (version >= OVRP_1_29_0.version)
            {
                return OVRP_1_29_0.ovrp_SetHeadPoseModifier(ref relativeRotation, ref relativeTranslation) == Result.Success;
            }
            else
            {
                return false;
            }
        }

        public static bool GetHeadPoseModifier(out Quatf relativeRotation, out Vector3f relativeTranslation)
        {
            if (version >= OVRP_1_29_0.version)
            {
                return OVRP_1_29_0.ovrp_GetHeadPoseModifier(out relativeRotation, out relativeTranslation) == Result.Success;
            }
            else
            {
                relativeRotation = Quatf.identity;
                relativeTranslation = Vector3f.zero;
                return false;
            }
        }

        public static bool IsPerfMetricsSupported(PerfMetrics perfMetrics)
        {
            if (version >= OVRP_1_30_0.version)
            {
                var result = OVRP_1_30_0.ovrp_IsPerfMetricsSupported(perfMetrics, out var isSupported);
                if (result == Result.Success)
                {
                    return isSupported == Bool.True;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static float? GetPerfMetricsFloat(PerfMetrics perfMetrics)
        {
            if (version >= OVRP_1_30_0.version)
            {
                var result = OVRP_1_30_0.ovrp_GetPerfMetricsFloat(perfMetrics, out var value);
                if (result == Result.Success)
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static int? GetPerfMetricsInt(PerfMetrics perfMetrics)
        {
            if (version >= OVRP_1_30_0.version)
            {
                var result = OVRP_1_30_0.ovrp_GetPerfMetricsInt(perfMetrics, out var value);
                if (result == Result.Success)
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static double GetTimeInSeconds()
        {
            if (version >= OVRP_1_31_0.version)
            {
                var result = OVRP_1_31_0.ovrp_GetTimeInSeconds(out var value);
                if (result == Result.Success)
                {
                    return value;
                }
                else
                {
                    return 0.0;
                }
            }
            else
            {
                return 0.0;
            }
        }

        public static bool SetColorScaleAndOffset(Vector4 colorScale, Vector4 colorOffset, bool applyToAllLayers)
        {
            if (version >= OVRP_1_31_0.version)
            {
                var ovrpApplyToAllLayers = applyToAllLayers ? Bool.True : Bool.False;
                return OVRP_1_31_0.ovrp_SetColorScaleAndOffset(colorScale, colorOffset, ovrpApplyToAllLayers) == Result.Success;
            }
            else
            {
                return false;
            }
        }

        public static bool AddCustomMetadata(string name, string param = "")
        {
            if (version >= OVRP_1_32_0.version)
            {
                return OVRP_1_32_0.ovrp_AddCustomMetadata(name, param) == Result.Success;
            }
            else
            {
                return false;
            }
        }

        public static bool SetDeveloperMode(Bool active)
        {
            if (version >= OVRP_1_38_0.version)
            {
                return OVRP_1_38_0.ovrp_SetDeveloperMode(active) == Result.Success;
            }
            else
            {
                return false;
            }
        }

        public static float GetAdaptiveGPUPerformanceScale()
        {
            if (version >= OVRP_1_42_0.version)
            {
                var adaptiveScale = 1.0f;
                if (OVRP_1_42_0.ovrp_GetAdaptiveGpuPerformanceScale2(ref adaptiveScale) == Result.Success)
                {
                    return adaptiveScale;
                }
                return 1.0f;
            }
            else
            {
                return 1.0f;
            }
        }

        public static bool GetHandTrackingEnabled()
        {
            if (version >= OVRP_1_44_0.version)
            {
                var val = Plugin.Bool.False;
                var res = OVRP_1_44_0.ovrp_GetHandTrackingEnabled(ref val);
                if (res == Result.Success)
                {
                    return val == Plugin.Bool.True;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private static HandStateInternal cachedHandState = new HandStateInternal();
        public static bool GetHandState(ProcessingStep stepId, Hand hand, ref HandState handState)
        {
            if (version >= OVRP_1_44_0.version)
            {
                var res = OVRP_1_44_0.ovrp_GetHandState(stepId, hand, out cachedHandState);
                if (res == Result.Success)
                {
                    // attempt to avoid allocations if client provides appropriately pre-initialized HandState
                    if (handState.BoneRotations == null || handState.BoneRotations.Length != ((int)BoneId.Hand_End - (int)BoneId.Hand_Start))
                    {
                        handState.BoneRotations = new Quatf[(int)BoneId.Hand_End - (int)BoneId.Hand_Start];
                    }
                    if (handState.PinchStrength == null || handState.PinchStrength.Length != (int)HandFinger.Max)
                    {
                        handState.PinchStrength = new float[(int)HandFinger.Max];
                    }
                    if (handState.FingerConfidences == null || handState.FingerConfidences.Length != (int)HandFinger.Max)
                    {
                        handState.FingerConfidences = new TrackingConfidence[(int)HandFinger.Max];
                    }

                    // unrolling the arrays is necessary to avoid per-frame allocations during marshaling
                    handState.Status = cachedHandState.Status;
                    handState.RootPose = cachedHandState.RootPose;
                    handState.BoneRotations[0] = cachedHandState.BoneRotations_0;
                    handState.BoneRotations[1] = cachedHandState.BoneRotations_1;
                    handState.BoneRotations[2] = cachedHandState.BoneRotations_2;
                    handState.BoneRotations[3] = cachedHandState.BoneRotations_3;
                    handState.BoneRotations[4] = cachedHandState.BoneRotations_4;
                    handState.BoneRotations[5] = cachedHandState.BoneRotations_5;
                    handState.BoneRotations[6] = cachedHandState.BoneRotations_6;
                    handState.BoneRotations[7] = cachedHandState.BoneRotations_7;
                    handState.BoneRotations[8] = cachedHandState.BoneRotations_8;
                    handState.BoneRotations[9] = cachedHandState.BoneRotations_9;
                    handState.BoneRotations[10] = cachedHandState.BoneRotations_10;
                    handState.BoneRotations[11] = cachedHandState.BoneRotations_11;
                    handState.BoneRotations[12] = cachedHandState.BoneRotations_12;
                    handState.BoneRotations[13] = cachedHandState.BoneRotations_13;
                    handState.BoneRotations[14] = cachedHandState.BoneRotations_14;
                    handState.BoneRotations[15] = cachedHandState.BoneRotations_15;
                    handState.BoneRotations[16] = cachedHandState.BoneRotations_16;
                    handState.BoneRotations[17] = cachedHandState.BoneRotations_17;
                    handState.BoneRotations[18] = cachedHandState.BoneRotations_18;
                    handState.BoneRotations[19] = cachedHandState.BoneRotations_19;
                    handState.BoneRotations[20] = cachedHandState.BoneRotations_20;
                    handState.BoneRotations[21] = cachedHandState.BoneRotations_21;
                    handState.BoneRotations[22] = cachedHandState.BoneRotations_22;
                    handState.BoneRotations[23] = cachedHandState.BoneRotations_23;
                    handState.Pinches = cachedHandState.Pinches;
                    handState.PinchStrength[0] = cachedHandState.PinchStrength_0;
                    handState.PinchStrength[1] = cachedHandState.PinchStrength_1;
                    handState.PinchStrength[2] = cachedHandState.PinchStrength_2;
                    handState.PinchStrength[3] = cachedHandState.PinchStrength_3;
                    handState.PinchStrength[4] = cachedHandState.PinchStrength_4;
                    handState.PointerPose = cachedHandState.PointerPose;
                    handState.HandScale = cachedHandState.HandScale;
                    handState.HandConfidence = cachedHandState.HandConfidence;
                    handState.FingerConfidences[0] = cachedHandState.FingerConfidences_0;
                    handState.FingerConfidences[1] = cachedHandState.FingerConfidences_1;
                    handState.FingerConfidences[2] = cachedHandState.FingerConfidences_2;
                    handState.FingerConfidences[3] = cachedHandState.FingerConfidences_3;
                    handState.FingerConfidences[4] = cachedHandState.FingerConfidences_4;
                    handState.RequestedTimeStamp = cachedHandState.RequestedTimeStamp;
                    handState.SampleTimeStamp = cachedHandState.SampleTimeStamp;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool GetSkeleton(SkeletonType skeletonType, out Skeleton skeleton)
        {
            if (version >= OVRP_1_44_0.version)
            {
                return OVRP_1_44_0.ovrp_GetSkeleton(skeletonType, out skeleton) == Result.Success;
            }
            else
            {
                skeleton = default(Skeleton);
                return false;
            }
        }

        public static bool GetMesh(MeshType meshType, out Mesh mesh)
        {
            if (version >= OVRP_1_44_0.version)
            {
                mesh = new Mesh();
                var meshSize = Marshal.SizeOf(mesh);
                var meshPtr = Marshal.AllocHGlobal(meshSize);
                var result = OVRP_1_44_0.ovrp_GetMesh(meshType, meshPtr);
                if (result == Result.Success)
                {
                    Marshal.PtrToStructure(meshPtr, mesh);
                }
                Marshal.FreeHGlobal(meshPtr);

                return (result == Result.Success);
            }
            else
            {
                mesh = new Mesh();
                return false;
            }
        }

        public static bool GetSystemHmd3DofModeEnabled()
        {
            if (version >= OVRP_1_45_0.version)
            {
                var val = Bool.False;
                var res = OVRP_1_45_0.ovrp_GetSystemHmd3DofModeEnabled(ref val);
                if (res == Result.Success)
                {
                    return val == Bool.True;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private const string pluginName = "OVRPlugin";
        private static readonly System.Version _versionZero = new System.Version(0, 0, 0);

        private static class OVRP_0_1_0
        {
            public static readonly System.Version version = new System.Version(0, 1, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Sizei ovrp_GetEyeTextureSize(Eye eyeId);
        }

        private static class OVRP_0_1_1
        {
            public static readonly System.Version version = new System.Version(0, 1, 1);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetOverlayQuad2(Bool onTop, Bool headLocked, IntPtr texture, IntPtr device, Posef pose, Vector3f scale);
        }

        private static class OVRP_0_1_2
        {
            public static readonly System.Version version = new System.Version(0, 1, 2);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetNodePose(Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetControllerVibration(uint controllerMask, float frequency, float amplitude);
        }

        private static class OVRP_0_1_3
        {
            public static readonly System.Version version = new System.Version(0, 1, 3);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetNodeVelocity(Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetNodeAcceleration(Node nodeId);
        }

        private static class OVRP_0_5_0
        {
            public static readonly System.Version version = new System.Version(0, 5, 0);
        }

        private static class OVRP_1_0_0
        {
            public static readonly System.Version version = new System.Version(1, 0, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern TrackingOrigin ovrp_GetTrackingOriginType();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetTrackingOriginType(TrackingOrigin originType);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetTrackingCalibratedOrigin();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_RecenterTrackingOrigin(uint flags);
        }

        private static class OVRP_1_1_0
        {
            public static readonly System.Version version = new System.Version(1, 1, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetInitialized();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetVersion")]
            private static extern IntPtr _ovrp_GetVersion();
            public static string ovrp_GetVersion() { return Marshal.PtrToStringAnsi(_ovrp_GetVersion()); }

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetNativeSDKVersion")]
            private static extern IntPtr _ovrp_GetNativeSDKVersion();
            public static string ovrp_GetNativeSDKVersion() { return Marshal.PtrToStringAnsi(_ovrp_GetNativeSDKVersion()); }

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ovrp_GetAudioOutId();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr ovrp_GetAudioInId();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetEyeTextureScale();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetEyeTextureScale(float value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetTrackingOrientationSupported();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetTrackingOrientationEnabled();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetTrackingOrientationEnabled(Bool value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetTrackingPositionSupported();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetTrackingPositionEnabled();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetTrackingPositionEnabled(Bool value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetNodePresent(Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetNodeOrientationTracked(Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetNodePositionTracked(Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Frustumf ovrp_GetNodeFrustum(Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern ControllerState ovrp_GetControllerState(uint controllerMask);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetSystemCpuLevel();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetSystemCpuLevel(int value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetSystemGpuLevel();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetSystemGpuLevel(int value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetSystemPowerSavingMode();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetSystemDisplayFrequency();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetSystemVSyncCount();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetSystemVolume();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern BatteryStatus ovrp_GetSystemBatteryStatus();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetSystemBatteryLevel();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetSystemBatteryTemperature();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetSystemProductName")]
            private static extern IntPtr _ovrp_GetSystemProductName();
            public static string ovrp_GetSystemProductName() { return Marshal.PtrToStringAnsi(_ovrp_GetSystemProductName()); }

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_ShowSystemUI(PlatformUI ui);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetAppMonoscopic();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetAppMonoscopic(Bool value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetAppHasVrFocus();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetAppShouldQuit();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetAppShouldRecenter();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetAppLatencyTimings")]
            private static extern IntPtr _ovrp_GetAppLatencyTimings();
            public static string ovrp_GetAppLatencyTimings() { return Marshal.PtrToStringAnsi(_ovrp_GetAppLatencyTimings()); }

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetUserPresent();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetUserIPD();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetUserIPD(float value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetUserEyeDepth();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetUserEyeDepth(float value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetUserEyeHeight();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetUserEyeHeight(float value);
        }

        private static class OVRP_1_2_0
        {
            public static readonly System.Version version = new System.Version(1, 2, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetSystemVSyncCount(int vsyncCount);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrpi_SetTrackingCalibratedOrigin();
        }

        private static class OVRP_1_3_0
        {
            public static readonly System.Version version = new System.Version(1, 3, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetEyeOcclusionMeshEnabled();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetEyeOcclusionMeshEnabled(Bool value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetSystemHeadphonesPresent();
        }

        private static class OVRP_1_5_0
        {
            public static readonly System.Version version = new System.Version(1, 5, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern SystemRegion ovrp_GetSystemRegion();
        }

        private static class OVRP_1_6_0
        {
            public static readonly System.Version version = new System.Version(1, 6, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetTrackingIPDEnabled();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetTrackingIPDEnabled(Bool value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern HapticsDesc ovrp_GetControllerHapticsDesc(uint controllerMask);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern HapticsState ovrp_GetControllerHapticsState(uint controllerMask);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetControllerHaptics(uint controllerMask, HapticsBuffer hapticsBuffer);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetOverlayQuad3(uint flags, IntPtr textureLeft, IntPtr textureRight, IntPtr device, Posef pose, Vector3f scale, int layerIndex);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetEyeRecommendedResolutionScale();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetAppCpuStartToGpuEndTime();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ovrp_GetSystemRecommendedMSAALevel();
        }

        private static class OVRP_1_7_0
        {
            public static readonly System.Version version = new System.Version(1, 7, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetAppChromaticCorrection();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetAppChromaticCorrection(Bool value);
        }

        private static class OVRP_1_8_0
        {
            public static readonly System.Version version = new System.Version(1, 8, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetBoundaryConfigured();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern BoundaryTestResult ovrp_TestBoundaryNode(Node nodeId, BoundaryType boundaryType);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern BoundaryTestResult ovrp_TestBoundaryPoint(Vector3f point, BoundaryType boundaryType);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern BoundaryGeometry ovrp_GetBoundaryGeometry(BoundaryType boundaryType);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Vector3f ovrp_GetBoundaryDimensions(BoundaryType boundaryType);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetBoundaryVisible();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetBoundaryVisible(Bool value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_Update2(int stateId, int frameIndex, double predictionSeconds);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetNodePose2(int stateId, Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetNodeVelocity2(int stateId, Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Posef ovrp_GetNodeAcceleration2(int stateId, Node nodeId);
        }

        private static class OVRP_1_9_0
        {
            public static readonly System.Version version = new System.Version(1, 9, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern SystemHeadset ovrp_GetSystemHeadsetType();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Controller ovrp_GetActiveController();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Controller ovrp_GetConnectedControllers();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetBoundaryGeometry2(BoundaryType boundaryType, IntPtr points, ref int pointsCount);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern AppPerfStats ovrp_GetAppPerfStats();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_ResetAppPerfStats();
        }

        private static class OVRP_1_10_0
        {
            public static readonly System.Version version = new System.Version(1, 10, 0);
        }

        private static class OVRP_1_11_0
        {
            public static readonly System.Version version = new System.Version(1, 11, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_SetDesiredEyeTextureFormat(EyeTextureFormat value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern EyeTextureFormat ovrp_GetDesiredEyeTextureFormat();
        }

        private static class OVRP_1_12_0
        {
            public static readonly System.Version version = new System.Version(1, 12, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern float ovrp_GetAppFramerate();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern PoseStatef ovrp_GetNodePoseState(ProcessingStep stepId, Node nodeId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern ControllerState2 ovrp_GetControllerState2(uint controllerMask);
        }

        private static class OVRP_1_15_0
        {
            public static readonly System.Version version = new System.Version(1, 15, 0);

            public const int OVRP_EXTERNAL_CAMERA_NAME_SIZE = 32;

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_InitializeMixedReality();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_ShutdownMixedReality();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetMixedRealityInitialized();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_UpdateExternalCamera();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetExternalCameraCount(out int cameraCount);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetExternalCameraName(int cameraId, [MarshalAs(UnmanagedType.LPArray, SizeConst = OVRP_EXTERNAL_CAMERA_NAME_SIZE)] char[] cameraName);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetExternalCameraIntrinsics(int cameraId, out CameraIntrinsics cameraIntrinsics);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetExternalCameraExtrinsics(int cameraId, out CameraExtrinsics cameraExtrinsics);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_CalculateLayerDesc(OverlayShape shape, LayerLayout layout, ref Sizei textureSize,
                int mipLevels, int sampleCount, EyeTextureFormat format, int layerFlags, ref LayerDesc layerDesc);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_EnqueueSetupLayer(ref LayerDesc desc, IntPtr layerId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_EnqueueDestroyLayer(IntPtr layerId);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetLayerTextureStageCount(int layerId, ref int layerTextureStageCount);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetLayerTexturePtr(int layerId, int stage, Eye eyeId, ref IntPtr textureHandle);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_EnqueueSubmitLayer(uint flags, IntPtr textureLeft, IntPtr textureRight, int layerId, int frameIndex, ref Posef pose, ref Vector3f scale, int layerIndex);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetNodeFrustum2(Node nodeId, out Frustumf2 nodeFrustum);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_GetEyeTextureArrayEnabled();
        }

        private static class OVRP_1_16_0
        {
            public static readonly System.Version version = new System.Version(1, 16, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_UpdateCameraDevices();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_IsCameraDeviceAvailable(CameraDevice cameraDevice);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetCameraDevicePreferredColorFrameSize(CameraDevice cameraDevice, Sizei preferredColorFrameSize);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_OpenCameraDevice(CameraDevice cameraDevice);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_CloseCameraDevice(CameraDevice cameraDevice);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_HasCameraDeviceOpened(CameraDevice cameraDevice);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Bool ovrp_IsCameraDeviceColorFrameAvailable(CameraDevice cameraDevice);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceColorFrameSize(CameraDevice cameraDevice, out Sizei colorFrameSize);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceColorFrameBgraPixels(CameraDevice cameraDevice, out IntPtr colorFrameBgraPixels, out int colorFrameRowPitch);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetControllerState4(uint controllerMask, ref ControllerState4 controllerState);
        }

        private static class OVRP_1_17_0
        {
            public static readonly System.Version version = new System.Version(1, 17, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetExternalCameraPose(CameraDevice camera, out Posef cameraPose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_ConvertPoseToCameraSpace(CameraDevice camera, ref Posef trackingSpacePose, out Posef cameraSpacePose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceIntrinsicsParameters(CameraDevice camera, out Bool supportIntrinsics, out CameraDeviceIntrinsicsParameters intrinsicsParameters);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_DoesCameraDeviceSupportDepth(CameraDevice camera, out Bool supportDepth);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceDepthSensingMode(CameraDevice camera, out CameraDeviceDepthSensingMode depthSensoringMode);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetCameraDeviceDepthSensingMode(CameraDevice camera, CameraDeviceDepthSensingMode depthSensoringMode);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDevicePreferredDepthQuality(CameraDevice camera, out CameraDeviceDepthQuality depthQuality);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetCameraDevicePreferredDepthQuality(CameraDevice camera, CameraDeviceDepthQuality depthQuality);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_IsCameraDeviceDepthFrameAvailable(CameraDevice camera, out Bool available);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceDepthFrameSize(CameraDevice camera, out Sizei depthFrameSize);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceDepthFramePixels(CameraDevice cameraDevice, out IntPtr depthFramePixels, out int depthFrameRowPitch);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCameraDeviceDepthConfidencePixels(CameraDevice cameraDevice, out IntPtr depthConfidencePixels, out int depthConfidenceRowPitch);
        }

        private static class OVRP_1_18_0
        {
            public static readonly System.Version version = new System.Version(1, 18, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetHandNodePoseStateLatency(double latencyInSeconds);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetHandNodePoseStateLatency(out double latencyInSeconds);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetAppHasInputFocus(out Bool appHasInputFocus);
        }

        private static class OVRP_1_19_0
        {
            public static readonly System.Version version = new System.Version(1, 19, 0);
        }

        private static class OVRP_1_21_0
        {
            public static readonly System.Version version = new System.Version(1, 21, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetTiledMultiResSupported(out Bool foveationSupported);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetTiledMultiResLevel(out FixedFoveatedRenderingLevel level);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetTiledMultiResLevel(FixedFoveatedRenderingLevel level);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetGPUUtilSupported(out Bool gpuUtilSupported);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetGPUUtilLevel(out float gpuUtil);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetSystemDisplayFrequency2(out float systemDisplayFrequency);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetSystemDisplayAvailableFrequencies(IntPtr systemDisplayAvailableFrequencies, ref int numFrequencies);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetSystemDisplayFrequency(float requestedFrequency);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetAppAsymmetricFov(out Bool useAsymmetricFov);
        }

        private static class OVRP_1_28_0
        {
            public static readonly System.Version version = new System.Version(1, 28, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetDominantHand(out Handedness dominantHand);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetReorientHMDOnControllerRecenter(out Bool recenter);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetReorientHMDOnControllerRecenter(Bool recenter);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SendEvent(string name, string param);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_EnqueueSetupLayer2(ref LayerDesc desc, int compositionDepth, IntPtr layerId);
        }

        private static class OVRP_1_29_0
        {
            public static readonly System.Version version = new System.Version(1, 29, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetLayerAndroidSurfaceObject(int layerId, ref IntPtr surfaceObject);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetHeadPoseModifier(ref Quatf relativeRotation, ref Vector3f relativeTranslation);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetHeadPoseModifier(out Quatf relativeRotation, out Vector3f relativeTranslation);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetNodePoseStateRaw(ProcessingStep stepId, int frameIndex, Node nodeId, out PoseStatef nodePoseState);
        }

        private static class OVRP_1_30_0
        {
            public static readonly System.Version version = new System.Version(1, 30, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetCurrentTrackingTransformPose(out Posef trackingTransformPose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetTrackingTransformRawPose(out Posef trackingTransformRawPose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SendEvent2(string name, string param, string source);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_IsPerfMetricsSupported(PerfMetrics perfMetrics, out Bool isSupported);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetPerfMetricsFloat(PerfMetrics perfMetrics, out float value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetPerfMetricsInt(PerfMetrics perfMetrics, out int value);
        }

        private static class OVRP_1_31_0
        {
            public static readonly System.Version version = new System.Version(1, 31, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetTimeInSeconds(out double value);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetColorScaleAndOffset(Vector4 colorScale, Vector4 colorOffset, Bool applyToAllLayers);
        }

        private static class OVRP_1_32_0
        {
            public static readonly System.Version version = new System.Version(1, 32, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_AddCustomMetadata(string name, string param);
        }

        private static class OVRP_1_34_0
        {
            public static readonly System.Version version = new System.Version(1, 34, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_EnqueueSubmitLayer2(uint flags, IntPtr textureLeft, IntPtr textureRight, int layerId, int frameIndex, ref Posef pose, ref Vector3f scale, int layerIndex,
            Bool overrideTextureRectMatrix, ref TextureRectMatrixf textureRectMatrix, Bool overridePerLayerColorScaleAndOffset, ref Vector4 colorScale, ref Vector4 colorOffset);

        }

        private static class OVRP_1_35_0
        {
            public static readonly System.Version version = new System.Version(1, 35, 0);
        }

        private static class OVRP_1_36_0
        {
            public static readonly System.Version version = new System.Version(1, 36, 0);
        }

        private static class OVRP_1_37_0
        {
            public static readonly System.Version version = new System.Version(1, 37, 0);
        }

        private static class OVRP_1_38_0
        {
            public static readonly System.Version version = new System.Version(1, 38, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetTrackingTransformRelativePose(ref Posef trackingTransformRelativePose, TrackingOrigin trackingOrigin);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetExternalCameraCalibrationRawPose(int cameraId, out Posef rawPose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetDeveloperMode(Bool active);


            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetNodeOrientationValid(Node nodeId, ref Bool nodeOrientationValid);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetNodePositionValid(Node nodeId, ref Bool nodePositionValid);
        }

        private static class OVRP_1_39_0
        {
            public static readonly System.Version version = new System.Version(1, 39, 0);
        }

        private static class OVRP_1_40_0
        {
            public static readonly System.Version version = new System.Version(1, 40, 0);
        }

        private static class OVRP_1_41_0
        {
            public static readonly System.Version version = new System.Version(1, 41, 0);
        }

        private static class OVRP_1_42_0
        {
            public static readonly System.Version version = new System.Version(1, 42, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetAdaptiveGpuPerformanceScale2(ref float adaptiveGpuPerformanceScale);
        }

        private static class OVRP_1_43_0
        {
            public static readonly System.Version version = new System.Version(1, 43, 0);
        }

        private static class OVRP_1_44_0
        {
            public static readonly System.Version version = new System.Version(1, 44, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetHandTrackingEnabled(ref Bool handTrackingEnabled);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetHandState(ProcessingStep stepId, Hand hand, out HandStateInternal handState);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetSkeleton(SkeletonType skeletonType, out Skeleton skeleton);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetMesh(MeshType meshType, System.IntPtr meshPtr);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_OverrideExternalCameraFov(int cameraId, Bool useOverriddenFov, ref Fovf fov);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetUseOverriddenExternalCameraFov(int cameraId, out Bool useOverriddenFov);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_OverrideExternalCameraStaticPose(int cameraId, Bool useOverriddenPose, ref Posef pose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetUseOverriddenExternalCameraStaticPose(int cameraId, out Bool useOverriddenStaticPose);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_ResetDefaultExternalCamera();

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_SetDefaultExternalCamera(string cameraName, ref CameraIntrinsics cameraIntrinsics, ref CameraExtrinsics cameraExtrinsics);

        }

        private static class OVRP_1_45_0
        {
            public static readonly System.Version version = new System.Version(1, 45, 0);

            [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
            public static extern Result ovrp_GetSystemHmd3DofModeEnabled(ref Bool enabled);
        }
    }
}