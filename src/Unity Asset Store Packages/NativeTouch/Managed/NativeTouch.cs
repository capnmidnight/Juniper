using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace E7.Native
{
    /// <summary>
    /// This class contains various `static` methods as an entry point to use Native Touch.
    /// Please read HowToUse > HowToUse.zip (unzip outside of your project) or http://exceed7.com/native-touch
    /// </summary>
    public static partial class NativeTouch
    {
        /// <summary>
        /// This is only used internally. It is a type of delegate that native side calls to Unity to check out the ring buffer that contains touch data.
        /// </summary>
        internal delegate void CheckRingBufferDelegate(int start, int count);

        /// <summary>
        /// For callback-based API, you are handed each touch inside this delegate.
        /// </summary>
        public delegate void FullDelegate(NativeTouchDataFull ntdf);

        /// <summary>
        /// For callback-based API, you are handed each touch inside this delegate.
        /// </summary>
        public delegate void MinimalDelegate(NativeTouchData ntd);

        //The ring buffer that C# owns but native writes to.
        internal static NativeTouchData[] ntdRingBuffer;
        internal static NativeTouchDataFull[] ntdFullRingBuffer;

        //This is also given to native side so that we could do a basic mutex lock based on Dekker's Algorithm.
        //It is of length 3, enter intent flag x2 + turn variable
        internal static int[] dekker;

        //Content of this is incremented by native side every time a new touch is written.
        private static int[] finalCursorStorage;

        //At managed side it should be read only from this property.
        internal static int finalCursor
        {
            get
            {
                return finalCursorStorage[0];
            }
        }

        private static GCHandle ntdRingBufferHandle;
        private static GCHandle ntdFullRingBufferHandle;
        private static GCHandle dekkerHandle;
        private static GCHandle finalCursorHandle;

        private static NativeTouchRingBuffer nativeTouchDataQueue;

        //This is so that native side do not have to tell back manage side how much it is allocated, to do modulo math.
        internal static int activeRingBufferSize = -1;

        private static bool allocated = false;

        /// <summary>
        /// C# is holding several important memory area for native side to write to.
        /// </summary>
        private static void AllocateIfNotYet(StartOption startOption)
        {
            if (!allocated)
            {
                ntdRingBuffer = new NativeTouchData[startOption.ringBufferSize];
                ntdFullRingBuffer = new NativeTouchDataFull[startOption.ringBufferSize];
                dekker = new int[3];
                finalCursorStorage = new int[1];

                ntdRingBufferHandle = GCHandle.Alloc(ntdRingBuffer, GCHandleType.Pinned);
                ntdFullRingBufferHandle = GCHandle.Alloc(ntdFullRingBuffer, GCHandleType.Pinned);
                finalCursorHandle = GCHandle.Alloc(finalCursorStorage, GCHandleType.Pinned);
                dekkerHandle = GCHandle.Alloc(dekker, GCHandleType.Pinned);

                nativeTouchDataQueue = new NativeTouchRingBuffer();
                activeRingBufferSize = startOption.ringBufferSize;

                allocated = true;
            }
        }

        /// <summary>
        /// Let GC collect the memory area
        /// </summary>
        private static void Deallocate()
        {
            if (allocated)
            {
                ntdRingBufferHandle.Free();
                ntdFullRingBufferHandle.Free();
                finalCursorHandle.Free();

                ntdRingBuffer = null;
                ntdFullRingBuffer = null;
                nativeTouchDataQueue = null;

                allocated = false;
            }
        }

        /// <summary>
        /// Returns true when not in Editor and on iOS or Android.
        /// </summary>
        public static bool OnSupportedPlatform()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// An entry point for ring buffer iteration method of handling touches.
        /// The use is similar to Unity's <see cref="Input.touches">, however you need to "use it up" by iterate the touches manually in this.
        /// The way to do so is by `while` looping on <see cref="NativeTouchRingBuffer.TryGetAndMoveNext(out NativeTouchData)">.
        /// 
        /// For example, <see cref="Input.touches"> in the same frame, multiple <see cref="MonoBehaviour">'s `Update()` could check on it and see the same thing.
        /// But its content suddenly change to another set in the next frame. It is possible to miss the touch, but it is impossible to "use up" the touch in the same frame.
        /// 
        /// On the other hand, <see cref="NativeTouch.touches"> will continuously adds new touch events from native side.
        /// When you iterate over them with <see cref="NativeTouchRingBuffer.TryGetAndMoveNext(out NativeTouchData)">, you are "using the touch" and the pointer moves forward.
        /// You cannot do it again. So decide the central place where you want to handle touches in your main thread code and maybe share the get result.
        /// 
        /// If you didn't iterate through them for some reason, they will still remain there even to the next frame. Continuously accumulated with new touches.
        /// If you didn't iterate over a certain limit specifiable on <see cref="StartOption.ringBufferSize">, it will start discarding the early ones.
        /// If you start iterating again after that happens, it starts from the earliest available in the ring.
        /// Or you could also skip to the latest instantly with <see cref="NativeTouchRingBuffer.DiscardAllTouches">.
        /// 
        /// Also the data inside this is still the same as callback-style <see cref="NativeTouchData">. That is for example, in <see cref="Input.touches"> you could check for
        /// held-down touch and see it available continuously in every frame as long as the finger is down. For <see cref="NativeTouch.touches">, you will see just one
        /// DOWN event when you try to <see cref="NativeTouchRingBuffer.TryGetAndMoveNext(out NativeTouchData)"> it. It's how the touch natively works.
        /// 
        /// So an another way could think about this API : you are still doing the callback way but the code in callback is just 
        /// putting those touches in the central queue (kinda queue, but it is actually not), waiting for your <see cref="MonoBehaviour"> code to come and use them.
        /// </summary>
        public static NativeTouchRingBuffer touches
        {
            get
            {
                if(Started == false)
                {
                    throw new InvalidOperationException("You cannot get touches while Native Touch is not in Started state.");
                }
                return nativeTouchDataQueue;
            }
        }

        private static bool started;
        public static bool Started { get { return started; } }

        private static bool isFullMode;
        public static bool IsFullMode { get { return isFullMode; } }

        /// <summary>
        /// After starting it will call the static callback method you have registered with timing explained in the [Callback Details](http://exceed7.com/native-touch/callback-details) page. It won't allow you to start without registering any callbacks, unless you have <see cref="StartOption.noCallback"> in the <paramref name="startOption">.
        /// 
        /// [Editor] This method is a stub that does nothing.
        /// </summary>
        /// <param name="startOption">See the <see cref="StartOption"> class's member for definition of each options.</param>
        /// <exception cref="InvalidOperationException">Thrown when you start while already in <see cref="Started"> state, start without any callback while not in <see cref="StartOption.noCallback"> mode, or start in an unsupported platform.</exception>
        public static void Start(StartOption startOption = default(StartOption))
        {
            if (Application.isEditor) return;

            if(started)
            {
                throw new InvalidOperationException("You cannot start again (with or without different start option) while you are still in Started state. Call NativeTouch.Stop() first then you can start again with new options if you want.");
            }

            UnsupportedCheck();

            if (startOption == null)
            {
                startOption = new StartOption();
            }

            if (startOption.noCallback == false && NativeTouchInterface.AreSomeCallbacksNull(startOption.fullMode))
            {
                throw new InvalidOperationException(string.Format("Native Touch Start() aborted because you start while not registered any callback of the mode : {0}. Native Touch does not use null-check on event invoke for performance reason, and when the native side call you will get a null reference exception if Native Touch allows you to start like this. If you are intended to use ring buffer iteration based API, use `noCallback` in your `StartOption` to bypass this check.",
                startOption.fullMode ? "Full" : "Minimal")
                );
            }

            AllocateIfNotYet(startOption);


#if UNITY_IOS
            //On iOS everything goes to statically linked C
            NativeTouchInterface._StartNativeTouch(
                fullMode         : startOption.fullMode ? 1 : 0,
                disableUnityTouch: startOption.disableUnityTouch ? 1 : 0,
                noCallback       : startOption.noCallback ? 1 : 0,
                fullDelegate     : NativeTouchInterface.NativeTouchFullCallbackRingBuffer,
                minimalDelegate  : NativeTouchInterface.NativeTouchMinimalCallbackRingBuffer,
                fullRingBuffer   : ntdFullRingBufferHandle.AddrOfPinnedObject(),
                minimalRingBuffer: ntdRingBufferHandle.AddrOfPinnedObject(),
                finalCursorHandle: finalCursorHandle.AddrOfPinnedObject(),
                dekkerHandle     : dekkerHandle.AddrOfPinnedObject(),
                ringBufferSize   : startOption.ringBufferSize
            );
#elif UNITY_ANDROID
            //This goes to Java
            NativeTouchInterface.androidClassBridge.CallStatic(NativeTouchInterface.AndroidStart, new object[] { startOption.fullMode, startOption.disableUnityTouch, startOption.noCallback });

            //This goes to C directly, but dynamically linked.
            //(If UT allow statically linked lib on Android callback performance could be much better since IL2CPP could touch it.)
            NativeTouchInterface.registerCallbacksCheckRingBuffer(
                fullDelegate     :  NativeTouchInterface.NativeTouchFullCallbackRingBuffer,
                minimalDelegate  :  NativeTouchInterface.NativeTouchMinimalCallbackRingBuffer,
                fullRingBuffer   :  ntdFullRingBufferHandle.AddrOfPinnedObject(),
                minimalRingBuffer:  ntdRingBufferHandle.AddrOfPinnedObject(),
                finalCursorHandle:  finalCursorHandle.AddrOfPinnedObject(),
                dekkerHandle     :  dekkerHandle.AddrOfPinnedObject(),
                ringBufferSize   :  startOption.ringBufferSize
            );
#endif
            started = true;
            isFullMode = startOption.fullMode;
        }



        /// <summary>
        /// Advanced function. From what I observed,
        /// The static callback will be slow only on the first time unfortunately. Maybe because of how C# event invoking works.
        /// And also maybe caused by JIT compilation on platform like Android Mono is JITing codes for the first time.
        /// 
        /// Moreover there's no way to "pre-warm" this because the callback has to be triggered by player. If player did not
        /// touch the screen after <see cref="Start(StartOption)"> until the crucial moment the lag might be detrimental to the game.
        /// (e.g. In music games if you start Native Touch every time the game start, it would be difficult to perfect the first note.)
        /// 
        /// It might irritates your player that the first touch is always slow, so calling this will force invoke on every registered 
        /// static callbacks with a fake touch struct once. (all default values) Also can be used to warm-up your other things
        /// that you have put in the static callback as well.
        /// 
        /// Check and ignore this fake touch in your callback with if on <see cref="NativeTouchData.WarmUpTouch"> property.
        /// Real touch created from native side always has this property as `false`.
        /// </summary>
        public static void WarmUp()
        {
            if(IsFullMode)
            {
                NativeTouchInterface.NativeTouchFullCallback(new NativeTouchDataFull());
            }
            else
            {
                NativeTouchInterface.NativeTouchMinimalCallback(new NativeTouchData());
            }
        }

        /// <summary>
        /// Stop calling the static callback on touch.
        /// 
        /// [Android] It removes the touch listener from the view.
        /// 
        /// [iOS] The touch recognizer is not actually removed, since there is a strange bug that replays all the touches on remove.
        /// Instead I just temporartly disable it and returned to only normal Unity path.
        /// 
        /// [Editor] This method is a stub that does nothing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when you stop while not in <see cref="Started"> state.</exception>
        public static void Stop()
        {
            if (Application.isEditor) return;

            if(!started)
            {
                throw new InvalidOperationException("You can only Stop Native Touch while it is in Started state.");
            }

            UnsupportedCheck();

#if UNITY_IOS
            NativeTouchInterface._StopNativeTouch();
#elif UNITY_ANDROID
            NativeTouchInterface.androidClassBridge.CallStatic(NativeTouchInterface.AndroidStop);
#endif

            Deallocate();
            started = false;
        }

        /// <summary>
        /// Native Touch's coordinate is unscaled, if your game uses Resolution Scaling (Or dynamic resolution scaling) then <see cref="Screen.width">, <see cref="Screen.height">, <see cref="Screen.resolutions">, <see cref="Input.touches">, etc. will be scaled down too.
        /// 
        /// If you use that to calculate something with Native Touch's returned coordinate then it is wrong.
        /// Scaled down version of coordinate is not accessible in any ways from native side as far as I know. 
        /// 
        /// Since native side has no idea how small Unity is scaling things, the approach is to use this method to 
        /// return resolution of <see cref="Screen"> API as if it wasn't scaled. 
        /// 
        /// It got the same bounds as what coordinate that is returning from Native Touch.
        /// Then you could calculate the touch coordinate into the scaled coordinate.
        /// 
        /// [iOS] [See this table](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2), on the Native Resolution column, that's Unity's coordinate when resolution scaling is disabled. This method return values of that column by multiplying UIKit size with Native Scale Factor. Points from Native Touch has been processed in the same fashion.
        /// 
        /// However, "plus" device didn't match any table shown. For example :
        /// 
        /// iPhone 6
        /// Unity Bounds : {{0, 0}, {414, 736}} 
        /// Screen Bounds : {{0, 0}, {414, 736}} 
        /// Screen Native Bounds : {{0, 0}, {1242, 2208}} 
        /// Scale : 3.000000 Native Scale : 3.000000
        /// 
        /// When touching the bottom right coordinate, the coordinate returned is close to {414, 736}
        /// However the catch is that it could be in .000, .33332824707031, or .66665649414062.
        /// In all cases when multiplied by 3, you will get .9999999 which should be rounded to whole number.
        /// </summary>
        public static
#if UNITY_2017_2_OR_NEWER
    Vector2Int 
#else
    Vector2
#endif
        RealScreenResolution()
        {
            return new
#if UNITY_2017_2_OR_NEWER
    Vector2Int(
#else
    Vector2(
#endif

#if !UNITY_EDITOR
    #if UNITY_IOS
                NativeTouchInterface._RealScreenWidth(),
                NativeTouchInterface._RealScreenHeight()
    #elif UNITY_ANDROID
                NativeTouchInterface.androidClassBridge.CallStatic<int>(NativeTouchInterface.AndroidRealWidth),
                NativeTouchInterface.androidClassBridge.CallStatic<int>(NativeTouchInterface.AndroidRealHeight)
    #else
                0,0
    #endif
#else //if in editor
            0,0
#endif
    );
        }

        private static void UnsupportedCheck()
        {
#if !(UNITY_IOS || UNITY_ANDROID)
            throw new NotSupportedException("Platforms other than Android and iOS is not supported.");
#endif
        }

        /// <summary>
        /// Register as many callbacks as you want before calling <see cref="NativeTouch.Start(StartOption)">.
        /// Only callback with matching mode with what's in the <see cref="StartOption"> will be called.
        /// </summary>
        public static void RegisterCallback(NativeTouch.FullDelegate fullDelegate)
        {
            NativeTouchInterface.fullCallbacksBegan += fullDelegate;
            NativeTouchInterface.fullCallbacksEnded += fullDelegate;
            NativeTouchInterface.fullCallbacksCancelled += fullDelegate;
            NativeTouchInterface.fullCallbacksMoved += fullDelegate;
        }

        /// <summary>
        /// Register as many callbacks as you want before calling <see cref="Start(StartOption)">.
        /// Only callback with matching mode with what's in the <see cref="StartOption"> will be called.
        /// 
        /// This overload you can registered separate static callbacks for each callback type at native side.
        /// 
        /// [iOS] This closely mirrors the native side's 4 callbacks approach. 
        /// Each callback may receive multiple touches of differing phases but one of them will have 
        /// the phase as the same as type of callback. e.g. with 2 fingers holding and you down one more
        /// you will get the "Began" callback with 3 times invocation of began, stationary, stationary.
        /// 
        /// Do not misunderstand that Begin will be exclusively in Began callback, Moved will be exclusively in Moved callback, etc.
        /// This is how things work at native side too.
        /// 
        /// [Android] On the native side there is only one callback. However one `MotionEvent` contains one "main action"
        /// By this main action we will call appropriate callback type to mirror the iOS way.
        /// </summary>
        public static void RegisterCallback(
            NativeTouch.FullDelegate fullDelegateBegan,
            NativeTouch.FullDelegate fullDelegateEnded,
            NativeTouch.FullDelegate fullDelegateCancelled,
            NativeTouch.FullDelegate fullDelegateMoved
        )
        {
            NativeTouchInterface.fullCallbacksBegan += fullDelegateBegan;
            NativeTouchInterface.fullCallbacksEnded += fullDelegateEnded;
            NativeTouchInterface.fullCallbacksCancelled += fullDelegateCancelled;
            NativeTouchInterface.fullCallbacksMoved += fullDelegateMoved;
        }

        /// <summary>
        /// Register as many callbacks as you want before calling <see cref="Start(StartOption)">.
        /// Only callback with matching mode with what's in the <see cref="StartOption"> will be called.
        /// 
        /// This overload you can registered separate static callbacks for each callback type at native side.
        /// 
        /// [iOS] This closely mirrors the native side's 4 callbacks approach. 
        /// Each callback may receive multiple touches of differing phases but one of them will have 
        /// the phase as the same as type of callback. e.g. with 2 fingers holding and you down one more
        /// you will get the "Began" callback with 3 times invocation of began,stationary,stationary
        /// 
        /// [Android] On the native side there is only one callback. However one `MotionEvent` contains one "main action"
        /// By this main action we will call appropriate callback type to mirror the iOS way.
        /// </summary>
        public static void RegisterCallback(NativeTouch.MinimalDelegate minimalDelegate)
        {
            NativeTouchInterface.minimalCallbacksBegan += minimalDelegate;
            NativeTouchInterface.minimalCallbacksEnded += minimalDelegate;
            NativeTouchInterface.minimalCallbacksCancelled += minimalDelegate;
            NativeTouchInterface.minimalCallbacksMoved += minimalDelegate;
        }

        /// <summary>
        /// Register as many callbacks as you want before calling <see cref="Start(StartOption)">.
        /// Only callback with matching mode with what's in the <see cref="StartOption"> will be called.
        /// </summary>
        public static void RegisterCallback(
            NativeTouch.MinimalDelegate minimalDelegateBegan,
            NativeTouch.MinimalDelegate minimalDelegateEnded,
            NativeTouch.MinimalDelegate minimalDelegateCancelled,
            NativeTouch.MinimalDelegate minimalDelegateMoved
        )
        {
            NativeTouchInterface.minimalCallbacksBegan += minimalDelegateBegan;
            NativeTouchInterface.minimalCallbacksEnded += minimalDelegateEnded;
            NativeTouchInterface.minimalCallbacksCancelled += minimalDelegateCancelled;
            NativeTouchInterface.minimalCallbacksMoved += minimalDelegateMoved;
        }

        /// <summary>
        /// Clear all the registered callbacks. You cannot clear callbacks while still in <see cref="Start(StartOption)"> state.
        /// You have to <see cref="Stop"> first.
        /// </summary>
        public static void ClearCallbacks()
        {
            if(started)
            {
                throw new InvalidOperationException("Clearing or changing callback targets while Native Touch is running is not allowed. Please call NativeTouch.Stop() first.");
            }
            NativeTouchInterface.ClearCallbacks();
        }

        /// <summary>
        /// Use this to ask the time on the same timeline as the timestamp that comes together with your touch, which is not the same between devices.
        /// 
        /// Unity's input system does not provide you with the touch timestamp. The best you can do is to use the time at beginning of the frame which is
        /// still not the real performance of your players.
        /// 
        /// With this and an anchor <see cref="Time.realtimeSinceStartup"> remembered you should be able to convert those timestamp to meaningful time for your game.
        /// 
        /// [iOS] Based on `ProcessInfo.systemUptime` in iOS's API. The unit is **SECONDS**. [See documentation](https://developer.apple.com/documentation/foundation/nsprocessinfo/1414553-systemuptime)
        /// 
        /// The API returns `double` and it is retured as-is.
        /// 
        /// [Android] Based on `SystemClock.uptimeMillis();` in Android's API. The unit is **MILLISECONDS**.
        /// 
        /// The API actually returns `long`, but converted to `double` to be in line with iOS.
        /// </summary>
        /// <returns></returns>
        public static double GetNativeTouchTime()
        {
#if UNITY_IOS
            double doubleTime = NativeTouchInterface._GetNativeTouchTime();
            return doubleTime;
#elif UNITY_ANDROID
            return (double)NativeTouchInterface.androidClassBridge.CallStatic<long>(NativeTouchInterface.AndroidTouchTimeMethodName);
#else
            return -1;
#endif
        }
    }
}