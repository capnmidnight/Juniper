#if UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Juniper.Unity.Haptics
{
    /// <summary>
    /// Implements haptic feedback for Android API levels less than 26. Vibration amplitude is not
    /// available as an option, only pulse length.
    /// </summary>
    public class AndroidAPI1Haptics : AbstractAndroidAPIHaptics
    {
        /// <summary>
        /// Play a single vibration of a set length of time.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <param name="amplitude">   Strength of vibration (ignored).</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            if (amplitude > 0.25f)
            {
                yield return VibrateCoroutine(milliseconds);
            }
            else
            {
                yield return new WaitForSeconds(milliseconds * 0.001f);
            }
        }

        /// <summary>
        /// Play a patterned vibration, where all of the vibrations are the same amplitude, the
        /// default amplitude.
        /// </summary>
        /// <param name="pattern">   Pattern.</param>
        /// <param name="amplitudes">Strength of vibrations (ignored).</param>
        protected override IEnumerator PlayCoroutine(long[] pattern, float[] amplitudes)
        {
            if (pattern.Length > 0)
            {
                var collapsedPattern = new List<long>();
                var lastAmp = false;
                for (var i = 0; i < pattern.Length; ++i)
                {
                    var curAmp = amplitudes[i] > 0.25f;
                    if (collapsedPattern.Count == 0 || curAmp != lastAmp)
                    {
                        collapsedPattern.Add(0);
                    }
                    collapsedPattern[collapsedPattern.Count - 1] += pattern[i];
                    lastAmp = curAmp;
                }

                yield return PlayCoroutine(collapsedPattern.ToArray());
            }
        }
    }
}

#endif