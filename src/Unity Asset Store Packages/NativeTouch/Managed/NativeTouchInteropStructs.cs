using UnityEngine;
using System.Runtime.InteropServices;

namespace E7.Native
{
    /// <summary>
    /// This C# struct can interop with native side. Each platform does not use all the allocated memory of this struct, but since you access them with properties which has been surrounded by preprocessor directive you don't have to worry about accessing invalid/blank data by mistake.
    /// 
    /// The struct contains only private fields and public property. The struct will be created from native side and so at C# we don't need the constructor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeTouchData
    {
        private int callbackType;
        private float x;
        private float y;
        private float previousX;
        private float previousY;
        private int phase;
        private double timestamp;
        private int pointerId;
        private int nativelyGenerated;

        /// <summary>
        /// A platform-dependent <see cref="ToString">. It will print only valid data on a platform you are on right now.
        /// </summary>
        public override string ToString()
        {
#if UNITY_IOS
            return string.Format( "X {0} Y {1} PrvX {2} PrvY {3} Phase {4} Time {5}", X,Y,PreviousX, PreviousY, Phase, Timestamp);
#elif UNITY_ANDROID
            return string.Format( "X {0} Y {1} Phase {2} Time {3} ID {4}", X,Y,Phase, Timestamp, PointerId);
#else
            return base.ToString();
#endif
        }

        internal TouchPhase CallbackType { get { return (TouchPhase)callbackType; } }

        /// <summary>
        /// The struct that is NOT created at native side is a fake, and considered a warm up touch.
        /// 
        /// Since the struct does not have any constructor and all of its field private, this must be true if the struct was
        /// created at C# side.
        /// 
        /// If you are using <see cref="NativeTouch.WarmUp"> you must have an `if` to ignore the warm up touch in your callback.
        /// </summary>
        public bool WarmUpTouch { get { return nativelyGenerated == 0; } }

        /// <summary>
        /// [All platforms] 0 is the left most (same as Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// [iOS] Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// 
        /// [Android] It is possible to get fractions. The fraction is for sub-pixel precision, the whole pixel must move. There is no movement of only sub-pixel level (e.g. a series of movements from 255.04 to 255.94 can never happen) . See [the official google docs](https://developer.android.com/reference/android/view/MotionEvent#getX(int))
        /// </summary>
        public float X { get { return x; } }

        /// <summary>
        /// [All platforms] 0 is the left most (same as Unity)
        /// 
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// 
        /// [Android] Converted to `int` by rounding DOWN, discarding the sub-pixel precision.
        /// </summary>
        public int IntegerX { get { return (int)X; } }

        /// <summary>
        /// [All platforms] 0 is the top (DIFFERENT from Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// [iOS] Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// 
        /// [Android] It is possible to get fractions. The fraction is for sub-pixel precision, the whole pixel must move. There is no movement of only sub-pixel level (e.g. a series of movements from 255.04 to 255.94 can never happen) . See [the official google docs](https://developer.android.com/reference/android/view/MotionEvent#getX(int))
        /// </summary>
        public float Y { get { return y; } }

        /// <summary>
        /// [All platforms] 0 is the top (DIFFERENT from Unity)
        /// 
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// 
        /// [Android] Converted to `int` by rounding DOWN, discarding the sub-pixel precision.
        /// </summary>
        public int IntegerY { get { return (int)Y; } }

        /// <summary>
        /// Casted from `int` to <see cref="TouchPhase"> on getter of this property.
        /// 
        /// [iOS] Enum signature of Unity matches the native iOS side so it is the same. <see cref="TouchPhase.Stationary"> signifying this touch was staying still while the other touch moves. For explanation please see [Callback Details](http://exceed7.com/native-touch/callback-details.html)
        /// 
        /// [Android] You can never get  <see cref="TouchPhase.Stationary"> because Google Android API does not have one, at the same time  <see cref="TouchPhase.Moved"> might signify either stationary or really moved.
        /// For explanation please see [Callback Details](http://exceed7.com/native-touch/callback-details.html)
        /// </summary>
        /// <returns></returns>
        public TouchPhase Phase { get { return (TouchPhase)phase; } }

        /// <summary>
        /// Here's one thing Native Touch can give you more than Unity's. In Unity we don't have a timestamp indicating when the touch really happens and forced to use the in-frame time even though those touches likely occur out of the frame earlier, and it unnecessary punish the late-pressing player in a game that need timing. (Or vise-versa help players that likes to push early) But this number is a time since phone system start up and cannot be compare to <see cref="Time.realTimeSinceStartUp"> of Unity. To convert to Unity time use <see cref="NativeTouch.GetNativeTouchTime()"> static method to ask for respective native time, then at that moment you remember the <see cref="Time.realTimeSinceStartUp"> in Unity side. You can then be able to relate both times.
        /// 
        /// [iOS] Based on `ProcessInfo.systemUptime` in iOS's API. The unit is SECONDS with sub-millisecond precision, already in `double`.
        /// 
        /// [Android] Based on `SystemClock.uptimeMillis();` in Android's API. The unit is MILLISECONDS converted to `double`. (Originally `long`)
        /// 
        /// Android can report multiple touches per `MotionEvent` and even touch that stayed still are reported as `ACTION_MOVE`. But the one staying still also have the same new timestamp copied from the moved/up/down ones (the "main pointer" that cause the action)
        /// </summary>
        public double Timestamp { get { return timestamp; } }

#if UNITY_IOS
        /// <summary>
        /// [iOS] 0 is the left most (same as Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// </summary>
        public float PreviousX { get { return previousX; } }

        /// <summary>
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// </summary>
        public int PreviousIntegerX { get { return (int)PreviousX; } }

        /// <summary>
        /// [iOS] 0 is the top (DIFFERENT from Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// </summary>
        public float PreviousY { get { return previousY; } }

        /// <summary>
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// </summary>
        public int PreviousIntegerY { get { return (int)PreviousY; } }
#endif

#if UNITY_ANDROID
        /// <summary>
        /// [Android] Android does not have previous position for each touch, therefore you must use pointer ID to relate the touch movement.
        /// </summary>
        public int PointerId { get { return pointerId; } }
#endif
    }

    /// <summary>
    /// Full version of <see cref="NativeTouchData">. This C# struct can interop with native side. Each platform does not use all the allocated memory of this struct, but since you access them with properties which has been surrounded by preprocessor directive you don't have to worry about accessing invalid/blank data by mistake.
    /// **This struct along with full mode functionality is in BETA!** Because I don't have proper tools (pressure sensitive touch screen of BOTH iOS and Android, etc.) to test the correctness of data but I do think they are correct because it came straight from native side. Drop me a message on Discord if you do find the error. Sorry for your inconvenience.
    /// 
    /// The struct contains only private fields and public property. The struct will be created from native side and so at C# we don't need the constructor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeTouchDataFull
    {
        //NativeTouchData
        private int callbackType;
        private float x;
        private float y;
        private float previousX;
        private float previousY;
        private int phase;
        private double timestamp;
        private int pointerId;
        private int nativelyGenerated;

        //NativeTouchDataFull only. The name after _ is for Android data to be in, before is for iOS.
        private int tapCount;
        private int type;
        private float force_pressure;
        private float maximumPossibleForce;
        private float majorRadius_touchMajor;
        private float majorRadiusTolerance_touchMinor;
        private float altitudeAngle_size;
        private float azimuthAngle_orientation;

        internal TouchPhase CallbackType { get { return (TouchPhase)callbackType; } }

        /// <summary>
        /// The struct that is NOT created at native side is a fake, and considered a warm up touch.
        /// 
        /// Since the struct does not have any constructor and all of its field private, this must be true if the struct was
        /// created at C# side.
        /// 
        /// If you are using <see cref="NativeTouch.WarmUp"> you must have an `if` to ignore the warm up touch in your callback.
        /// </summary>
        public bool WarmUpTouch { get { return nativelyGenerated == 0; } }

        /// <summary>
        /// [All platforms] 0 is the left most (same as Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// [iOS] Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// 
        /// [Android] It is possible to get fractions. The fraction is for sub-pixel precision, the whole pixel must move. There is no movement of only sub-pixel level (e.g. a series of movements from 255.04 to 255.94 can never happen) . See [the official google docs](https://developer.android.com/reference/android/view/MotionEvent#getX(int))
        /// </summary>
        public float X { get { return x; } }

        /// <summary>
        /// [All platforms] 0 is the left most (same as Unity)
        /// 
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// 
        /// [Android] Converted to `int` by rounding DOWN, discarding the sub-pixel precision.
        /// </summary>
        public int IntegerX { get { return (int)X; } }

        /// <summary>
        /// [All platforms] 0 is the top (DIFFERENT from Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// [iOS] Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// 
        /// [Android] It is possible to get fractions. The fraction is for sub-pixel precision, the whole pixel must move. There is no movement of only sub-pixel level (e.g. a series of movements from 255.04 to 255.94 can never happen) . See [the official google docs](https://developer.android.com/reference/android/view/MotionEvent#getX(int))
        /// </summary>
        public float Y { get { return y; } }

        /// <summary>
        /// [All platforms] 0 is the top (DIFFERENT from Unity)
        /// 
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// 
        /// [Android] Converted to `int` by rounding DOWN, discarding the sub-pixel precision.
        /// </summary>
        public int IntegerY { get { return (int)Y; } }

        /// <summary>
        /// Casted from `int` to <see cref="TouchPhase"> on getter of this property.
        /// 
        /// [iOS] Enum signature of Unity matches the native iOS side so it is the same. <see cref="TouchPhase.Stationary"> signifying this touch was staying still while the other touch moves. For explanation please see [Callback Details](http://exceed7.com/native-touch/callback-details.html)
        /// 
        /// [Android] You can never get  <see cref="TouchPhase.Stationary"> because Google Android API does not have one, at the same time  <see cref="TouchPhase.Moved"> might signify either stationary or really moved.
        /// For explanation please see [Callback Details](http://exceed7.com/native-touch/callback-details.html)
        /// </summary>
        /// <returns></returns>
        public TouchPhase Phase { get { return (TouchPhase)phase; } }

        /// <summary>
        /// Here's one thing Native Touch can give you more than Unity's. In Unity we don't have a timestamp indicating when the touch really happens and forced to use the in-frame time even though those touches likely occur out of the frame earlier, and it unnecessary punish the late-pressing player in a game that need timing. (Or vise-versa help players that likes to push early) But this number is a time since phone system start up and cannot be compare to <see cref="Time.realTimeSinceStartUp"> of Unity. To convert to Unity time use <see cref="NativeTouch.GetNativeTouchTime()"> static method to ask for respective native time, then at that moment you remember the <see cref="Time.realTimeSinceStartUp"> in Unity side. You can then be able to relate both times.
        /// 
        /// [iOS] Based on `ProcessInfo.systemUptime` in iOS's API. The unit is SECONDS with sub-millisecond precision, already in `double`.
        /// 
        /// [Android] Based on `SystemClock.uptimeMillis();` in Android's API. The unit is MILLISECONDS converted to `double`. (Originally `long`)
        /// 
        /// Android can report multiple touches per `MotionEvent` and even touch that stayed still are reported as `ACTION_MOVE`. But the one staying still also have the same new timestamp copied from the moved/up/down ones (the "main pointer" that cause the action)
        /// </summary>
        public double Timestamp { get { return timestamp; } }

#if UNITY_IOS
        /// <summary>
        /// [iOS] 0 is the left most (same as Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// </summary>
        public float PreviousX { get { return previousX; } }

        /// <summary>
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// </summary>
        public int PreviousIntegerX { get { return (int)PreviousX; } }

        /// <summary>
        /// [iOS] 0 is the top (DIFFERENT from Unity)
        /// 
        /// You get the point in the range of <see cref="Screen.width">/<see cref="Screen.height"> as long as you didn't use resolution scaling. If you use (dynamic) resolution scaling, the <see cref="Screen"> API is scaled down but native point stays in the range as if it hadn't scaled. Use <see cref="NativeTouch.RealScreenResolution()"> to get the original unscaled <see cref="Screen"> bounds.
        /// 
        /// Natively reported as [`UIKit` size](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2). Then it is multiplied by Native Scale factor so that it matches the coordinate system of Unity. 
        /// </summary>
        public float PreviousY { get { return previousY; } }

        /// <summary>
        /// [iOS] A simple convenience cast which likely does not change its value from `float` version at all.
        /// </summary>
        public int PreviousIntegerY { get { return (int)PreviousY; } }
        
        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618132-tapcount)
        /// </summary>
        public int TapCount { get { return tapCount; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/touchtype)
        /// </summary>
        public int Type { get { return type; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618110-force)
        /// </summary>
        public float Force { get { return force_pressure; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618121-maximumpossibleforce)
        /// </summary>
        public float MaximumPossibleForce { get { return maximumPossibleForce; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618106-majorradius)
        /// </summary>
        public float MajorRadius { get { return majorRadius_touchMajor; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618120-majorradiustolerance)
        /// </summary>
        public float MajorRadiusTolerance { get { return majorRadiusTolerance_touchMinor; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618118-altitudeangle)
        /// </summary>
        public float AltitudeAngle { get { return altitudeAngle_size; } }

        /// <summary>
        /// [iOS] [Read the official documentation](https://developer.apple.com/documentation/uikit/uitouch/1618131-azimuthangle)
        /// </summary>
        public float AzimuthAngle { get { return azimuthAngle_orientation; } }

#endif

#if UNITY_ANDROID
        /// <summary>
        /// [Android] Android does not have previous position for each touch, therefore you must use pointer ID to relate the touch movement.
        /// </summary>
        public int PointerId { get { return pointerId; } }

        /// <summary>
        /// [Android] [Read the official documentation](https://developer.android.com/reference/android/view/MotionEvent.html#getOrientation(int))
        /// </summary>
        public float Orientation { get { return azimuthAngle_orientation; } }

        /// <summary>
        /// [Android] [Read the official documentation](https://developer.android.com/reference/android/view/MotionEvent.html#getPressure(int))
        /// </summary>
        public float Pressure { get { return force_pressure; } }

        /// <summary>
        /// [Android] [Read the official documentation](https://developer.android.com/reference/android/view/MotionEvent.html#getSize(int))
        /// </summary>
        public float Size { get { return altitudeAngle_size; } }

        /// <summary>
        /// [Android] [Read the official documentation](https://developer.android.com/reference/android/view/MotionEvent.html#getTouchMajor(int))
        /// </summary>
        public float TouchMajor { get { return majorRadius_touchMajor; } }

        /// <summary>
        /// [Android] [Read the official documentation](https://developer.android.com/reference/android/view/MotionEvent.html#getTouchMinor(int))
        /// </summary>
        public float TouchMinor { get { return majorRadiusTolerance_touchMinor; } }

#endif
    }
}