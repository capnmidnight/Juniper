#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;

namespace E7.Native
{
    internal partial class NativeTouchInterface
    {
        internal const string AndroidPackageName = "com.Exceed7.NativeTouch";
        internal const string AndroidClassName = "NativeTouchListener";
        internal const string AndroidInterfaceName = "TouchDelegate";
        internal const string AndroidTouchTimeMethodName = "AndroidTouchTime";
        internal const string AndroidElapsedRealtimeNanos = "ElapsedRealtimeNanos";
        internal const string AndroidStart = "StartNativeTouch";
        internal const string AndroidStop = "StopNativeTouch";
        internal const string AndroidRealHeight = "RealScreenHeight";
        internal const string AndroidRealWidth = "RealScreenWidth";

        internal const string AndroidPathToClass = AndroidPackageName + "." + AndroidClassName;
        internal const string AndroidPathToInterface = AndroidPathToClass + "$" + AndroidInterfaceName;

#pragma warning disable 0649
        //This is a preprocessor stunt so that in-editor (but Android build) it does not try to use JNI...
        internal static AndroidJavaClass androidClassBridge 
#if !UNITY_EDITOR
            = new AndroidJavaClass(AndroidPathToClass);
#else
        ;
#endif
#pragma warning restore 0649

        [DllImport("nativetouche7")]
        internal static extern void registerCallbacksCheckRingBuffer(
                NativeTouch.CheckRingBufferDelegate fullDelegate, 
                NativeTouch.CheckRingBufferDelegate minimalDelegate, 
                IntPtr fullRingBuffer, 
                IntPtr minimalRingBuffer, 
                IntPtr finalCursorHandle,
                IntPtr dekkerHandle,
                int ringBufferSize
            );
    }
}
#endif