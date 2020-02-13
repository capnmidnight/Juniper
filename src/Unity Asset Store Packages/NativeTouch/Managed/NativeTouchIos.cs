using System;
using System.Runtime.InteropServices;

namespace E7.Native
{
    internal partial class NativeTouchInterface
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        internal static extern void _StartNativeTouch(
            int fullMode, 
            int disableUnityTouch,
            int noCallback,
            NativeTouch.CheckRingBufferDelegate fullDelegate, 
            NativeTouch.CheckRingBufferDelegate minimalDelegate, 
            IntPtr fullRingBuffer, 
            IntPtr minimalRingBuffer, 
            IntPtr finalCursorHandle,
            IntPtr dekkerHandle,
            int ringBufferSize
        );

        [DllImport("__Internal")]
        internal static extern void _StopNativeTouch();

        [DllImport("__Internal")]
        internal static extern double _GetNativeTouchTime();

        [DllImport("__Internal")]
        internal static extern int _RealScreenWidth();

        [DllImport("__Internal")]
        internal static extern int _RealScreenHeight();
#endif
    }
}