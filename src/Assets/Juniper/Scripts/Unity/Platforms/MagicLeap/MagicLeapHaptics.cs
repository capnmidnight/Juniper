#if MAGIC_LEAP
using System.Collections;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Haptics
{
    public class MagicLeapHaptics : AbstractHapticExpressor
    {
        private MLInputController controller;

        public void SetController(MLInputController ctrl) =>
            controller = ctrl;

        public override void Cancel() =>
            controller?.StopFeedbackPatternVibe();

        private static MLInputControllerFeedbackPatternVibe MillisecondsToPattern(long milliseconds)
        {
            if (milliseconds < 75)
            {
                return MLInputControllerFeedbackPatternVibe.Tick;
            }
            else
            {
                return MLInputControllerFeedbackPatternVibe.Buzz;
            }
        }

        private static MLInputControllerFeedbackIntensity AmplitudeToIntensity(float amplitude)
        {
            if (amplitude <= 0.333f)
            {
                return MLInputControllerFeedbackIntensity.Low;
            }
            else if (amplitude <= 0.667f)
            {
                return MLInputControllerFeedbackIntensity.Medium;
            }
            else
            {
                return MLInputControllerFeedbackIntensity.High;
            }
        }

        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            var seconds = Units.Milliseconds.Seconds(milliseconds);
            controller?.StartFeedbackPatternVibe(MillisecondsToPattern(milliseconds), AmplitudeToIntensity(amplitude));
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Play a canned haptic pattern.
        /// </summary>
        /// <param name="expr"></param>
        public override void Play(HapticExpression expr)
        {
            if (expr == HapticExpression.Click)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Click, MLInputControllerFeedbackIntensity.Medium);
            }
            else if (expr == HapticExpression.Light)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.Low);
            }
            else if (expr == HapticExpression.Medium)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.Medium);
            }
            else if (expr == HapticExpression.Heavy)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.High);
            }
            else if (expr == HapticExpression.Press)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceDown, MLInputControllerFeedbackIntensity.Medium);
            }
            else if (expr == HapticExpression.Release)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.Medium);
            }
            else if (expr == HapticExpression.SelectionChange)
            {
                controller?.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Tick, MLInputControllerFeedbackIntensity.Low);
            }
            else
            {
                base.Play(expr);
            }
        }
    }
}
#endif
