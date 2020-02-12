#if UNITY_ANDROID

using System;
using System.Collections;
using System.Linq;

using UnityEngine;

namespace Juniper.Haptics
{
    /// <summary>
    /// Haptic system that implements the latest Android API version (26), which enables vibrations
    /// of different strengths to be played without pulse-width modulating the vibration manually.
    /// </summary>
    public class AndroidHaptics : AbstractHapticRetainedExpressor
    {
        /// <summary>
        /// Calls one of the static functions on VibrationEffect that creates a amplitude-modulated
        /// vibration pattern and executes it through the Android Vibration interface.
        /// </summary>
        /// <param name="function">Function.</param>
        /// <param name="args">    Arguments.</param>
        private static void CreateVibrationEffect(string function, object[] args)
        {
            var vibrationEffect = AndroidHaptics.vibrationEffect.CallStatic<AndroidJavaObject>(function, args);
            CreateVibrationEffect_params[0] = vibrationEffect;
            vibrator.Call("vibrate", CreateVibrationEffect_params);
        }

        private static readonly object[] CreateVibrationEffect_params = new object[1];

        private static readonly object[] CANCEL_PARAMS = Array.Empty<object>();

        /// <summary>
        /// The android.os.VibrationEffect class, which contains the API 26 features.
        /// </summary>
        private static AndroidJavaClass vibrationEffect;

        /// <summary>
        /// The vibrator service.
        /// </summary>
        private static AndroidJavaObject vibrator;

        /// <summary>
        /// A cached value for VibrationEffect's DEFAULT_AMPLITUDE field, so we don't have to incur
        /// the context switching cost of reading it repeatedly.
        /// </summary>
        private static int DefaultAmplitude;

        private static readonly object[] GET_SYSTEM_SERVICE_PARAMS = { "vibrator" };

        /// <summary>
        /// Creates the haptic interface specific to Android API 26+.
        /// </summary>
        public void Awake()
        {
            if (vibrator == null)
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", GET_SYSTEM_SERVICE_PARAMS);
                }
            }

            if (vibrationEffect == null)
            {
                vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                DefaultAmplitude = vibrationEffect.GetStatic<int>("DEFAULT_AMPLITUDE");
            }
        }

        /// <summary>
        /// Cancel the current vibration, whatever it is.
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
            vibrator.Call("cancel", CANCEL_PARAMS);
        }

        private void OnDestroy()
        {
            if(vibrationEffect != null)
            {
                vibrationEffect.Dispose();
            }

            if(vibrator != null)
            {
                vibrator.Dispose();
            }
        }

        /// <summary>
        /// Play a single vibration of a set length of time.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds)
        {
            return VibrateCoroutine(milliseconds, DefaultAmplitude);
        }

        /// <summary>
        /// Play a single vibration, of a set length of time, at a set strength.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <param name="amplitude">   Amplitude values should be on the range [0, 1].</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            var end = DateTime.Now.AddMilliseconds(milliseconds);
            VibrateCoroutine_params2[0] = milliseconds;
            VibrateCoroutine_params2[1] = (int)(amplitude * 255);
            CreateVibrationEffect("createOneShot", VibrateCoroutine_params2);
            while (DateTime.Now < end)
            {
                yield return null;
            }
        }
        private readonly object[] VibrateCoroutine_params2 = new object[2];

        /// <summary>
        /// Play a patterned vibration, where all of the vibrations are the same amplitude, the
        /// default amplitude.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        protected override IEnumerator PlayCoroutine(long[] pattern)
        {
            var end = DateTime.Now.AddMilliseconds(pattern.Sum());
            PlayCoroutine_params2[0] = pattern;
            CreateVibrationEffect("createWaveform", PlayCoroutine_params2);
            while (DateTime.Now < end)
            {
                yield return null;
            }
        }
        private readonly object[] PlayCoroutine_params2 = new object[2] { null, -1 };

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="pattern">   Pattern.</param>
        /// <param name="amplitudes">Amplitudes.</param>
        protected override IEnumerator PlayCoroutine(long[] pattern, float[] amplitudes)
        {
            var end = DateTime.Now.AddMilliseconds(pattern.Sum());
            var amps = new int[amplitudes.Length];
            for (var i = 0; i < amplitudes.Length; ++i)
            {
                amps[i] = (int)(amplitudes[i] * 255);
            }
            PlayCoroutine_params3[0] = pattern;
            PlayCoroutine_params3[1] = amps;
            CreateVibrationEffect("createWaveform", PlayCoroutine_params3);
            while (DateTime.Now < end)
            {
                yield return null;
            }
        }
        private readonly object[] PlayCoroutine_params3 = new object[3] { null, null, -1 };
    }
}

#endif