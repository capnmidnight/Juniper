#if UNITY_ANDROID

using System.Collections;
using System.Linq;

using UnityEngine;

namespace Juniper.Haptics
{
    /// <summary>
    /// Haptic system that implements the latest Android API version (26), which enables vibrations
    /// of different strengths to be played without pulse-width modulating the vibration manually.
    /// </summary>
    public class AndroidAPI26Haptics : AbstractAndroidAPIHaptics
    {
        /// <summary>
        /// The android.os.VibrationEffect class, which contains the API 26 features.
        /// </summary>
        private static AndroidJavaClass VibrationEffect;

        /// <summary>
        /// A cached value for VibrationEffect's DEFAULT_AMPLITUDE field, so we don't have to incur
        /// the context switching cost of reading it repeatedly.
        /// </summary>
        private static int DefaultAmplitude;

        /// <summary>
        /// Calls one of the static functions on VibrationEffect that creates a amplitude-modulated
        /// vibration pattern and executes it through the Android Vibration interface.
        /// </summary>
        /// <param name="function">Function.</param>
        /// <param name="args">Arguments.</param>
        private static void CreateVibrationEffect(string function, params object[] args)
        {
            var vibrationEffect = VibrationEffect.CallStatic<AndroidJavaObject>(function, args);
            vibrator.Call("vibrate", vibrationEffect);
        }

        /// <summary>
        /// Creates the haptic interface specific to Android API 26+.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (VibrationEffect == null)
            {
                VibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                DefaultAmplitude = VibrationEffect.GetStatic<int>("DEFAULT_AMPLITUDE");
            }
        }

        /// <summary>
        /// Play a single vibration of a set length of time.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds) =>
            VibrateCoroutine(milliseconds, DefaultAmplitude);

        /// <summary>
        /// Play a single vibration, of a set length of time, at a set strength.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <param name="amplitude">Amplitude values should be on the range [0, 1].</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            CreateVibrationEffect("createOneShot", milliseconds, (int)(amplitude * 255));
            yield return new WaitForSeconds(milliseconds * 0.001f);
        }

        /// <summary>
        /// Play a patterned vibration, where all of the vibrations are the same amplitude, the
        /// default amplitude.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        protected override IEnumerator PlayCoroutine(long[] pattern)
        {
            CreateVibrationEffect("createWaveform", pattern, -1);
            yield return new WaitForSeconds(pattern.Sum() * 0.001f);
        }

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        /// <param name="amplitudes">Amplitudes.</param>
        protected override IEnumerator PlayCoroutine(long[] pattern, float[] amplitudes)
        {
            var amps = new int[amplitudes.Length];
            for (var i = 0; i < amplitudes.Length; ++i)
            {
                amps[i] = (int)(amplitudes[i] * 255);
            }
            CreateVibrationEffect("createWaveform", pattern, amps, -1);
            yield return new WaitForSeconds(pattern.Sum() * 0.001f);
        }
    }
}

#endif
