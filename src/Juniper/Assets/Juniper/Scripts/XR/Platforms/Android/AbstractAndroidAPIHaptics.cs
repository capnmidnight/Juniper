#if UNITY_ANDROID

using System;
using System.Collections;
using System.Linq;

using UnityEngine;

namespace Juniper.Haptics
{
    /// <summary>
    /// Android has two APIs for haptic feedback, but they all pump through the Vibrator system
    /// service. This abstract class provides its child classes with a reference to the vibrator, so
    /// we don't have to incur the JNI context switching cost every time we want to make a vibration.
    /// </summary>
    public abstract class AbstractAndroidAPIHaptics : AbstractHapticExpressor
    {
        /// <summary>
        /// The vibrator service.
        /// </summary>
        protected static AndroidJavaObject vibrator;

        /// <summary>
        /// Get the vibrator service. This can't be done in a static constructor because it crashes
        /// the Unity player when JNI objects are accessed at startup.
        /// </summary>
        protected virtual void Awake()
        {
            if (vibrator == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
        }

        /// <summary>
        /// Cancel the current vibration, whatever it is.
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
            vibrator.Call("cancel");
        }

        /// <summary>
        /// Play a single vibration of a set length of time.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds)
        {
            var start = DateTime.Now;
            var seconds = Units.Milliseconds.Seconds(milliseconds);
            var ts = TimeSpan.FromSeconds(seconds);
            vibrator.Call("vibrate", milliseconds);
            while((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Play a patterned vibration with alternating amplitude modulation.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        protected override IEnumerator PlayCoroutine(long[] pattern)
        {
            var start = DateTime.Now;
            var milliseconds = pattern.Sum();
            var seconds = Units.Milliseconds.Seconds(milliseconds);
            var ts = TimeSpan.FromSeconds(seconds);
            vibrator.Call("vibrate", pattern, -1);
            while ((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}

#endif