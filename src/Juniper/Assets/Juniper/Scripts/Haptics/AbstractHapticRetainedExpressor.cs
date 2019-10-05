using System.Collections;

using UnityEngine;

namespace Juniper.Haptics
{
    /// <summary>
    /// Non-iOS systems do not have built-in haptic patterns. This class replicates the functionality
    /// of the Taptic Engine as best it can on those platforms.
    /// </summary>
    public abstract class AbstractHapticRetainedExpressor : AbstractHapticDevice
    {
        /// <summary>
        /// Cancel vibrations
        /// </summary>
        public override void Cancel()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Play a canned haptic pattern.
        /// </summary>
        /// <param name="expr"></param>
        public override void Play(HapticExpression expr)
        {
            HapticPatternElement.Play(this, expr);
        }

        /// <summary>
        /// Play a haptic pattern.
        /// </summary>
        /// <param name="points"></param>
        public override void Play(params HapticPatternElement[] points)
        {
            this.Run(PlayCoroutine(points));
        }

        /// <summary>
        /// Play a single element of a haptic pattern.
        /// </summary>
        /// <param name="point"></param>
        public override void Vibrate(HapticPatternElement point)
        {
            this.Run(VibrateCoroutine(point));
        }

        /// <summary>
        /// Play a single vibration for a set period of time at max amplitude.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        public override void Vibrate(long milliseconds)
        {
            this.Run(VibrateCoroutine(milliseconds));
        }

        /// <summary>
        /// Play a patterned vibration with alternating amplitude modulation.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        public override void Play(long[] pattern)
        {
            this.Run(PlayCoroutine(pattern));
        }

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="pattern">   Pattern.</param>
        /// <param name="amplitudes">Amplitudes.</param>
        public override void Play(long[] pattern, float[] amplitudes)
        {
            this.Run(PlayCoroutine(pattern, amplitudes));
        }

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="milliseconds">Pattern.</param>
        /// <param name="amplitude">   Amplitudes.</param>
        public override void Vibrate(long milliseconds, float amplitude)
        {
            this.Run(VibrateCoroutine(milliseconds, amplitude));
        }

        /// <summary>
        /// Play a single vibration for a set period of time at max amplitude.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        protected virtual IEnumerator VibrateCoroutine(long milliseconds)
        {
            yield return VibrateCoroutine(milliseconds, 1);
        }

        /// <summary>
        /// Play a patterned vibration with alternating amplitude modulation.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        protected virtual IEnumerator PlayCoroutine(long[] pattern)
        {
            float amplitude = 1;
            for (var i = 0; i < pattern.Length; ++i)
            {
                yield return VibrateCoroutine(pattern[i], amplitude);
                amplitude = 1 - amplitude;
            }
        }

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="pattern">   Pattern.</param>
        /// <param name="amplitudes">Amplitudes.</param>
        protected virtual IEnumerator PlayCoroutine(long[] pattern, float[] amplitudes)
        {
            for (var i = 0; i < pattern.Length; ++i)
            {
                yield return VibrateCoroutine(pattern[i], amplitudes[i]);
            }
        }

        /// <summary>
        /// Play a haptic pattern.
        /// </summary>
        /// <param name="points"></param>
        private IEnumerator PlayCoroutine(HapticPatternElement[] points)
        {
            foreach (var point in points)
            {
                yield return VibrateCoroutine(point);
            }
            Cancel();
        }

        /// <summary>
        /// Play a single element of a haptic pattern.
        /// </summary>
        /// <param name="point"></param>
        private IEnumerator VibrateCoroutine(HapticPatternElement point)
        {
            yield return VibrateCoroutine(point.Length, point.Amplitude);
        }

        /// <summary>
        /// Play a patterned vibration with amplitude modulation.
        /// </summary>
        /// <param name="milliseconds">Pattern.</param>
        /// <param name="amplitude">   Amplitudes.</param>
        protected abstract IEnumerator VibrateCoroutine(long milliseconds, float amplitude);
    }
}
