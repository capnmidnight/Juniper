#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Windows.Devices.Haptics;
using Windows.Globalization;
using Windows.Perception;
using Windows.UI.Input.Spatial;

namespace Juniper.Unity.Haptics
{
    public class WindowsMRHaptics : AbstractHapticExpressor
    {
        private SimpleHapticsController controller;
        private Dictionary<ushort, SimpleHapticsControllerFeedback> expressions;
        private SimpleHapticsControllerFeedback buzz;
        private SimpleHapticsControllerFeedback click;
        private SimpleHapticsControllerFeedback press;
        private SimpleHapticsControllerFeedback release;

        private uint _controllerID;
        public uint ControllerID
        {
            get { return _controllerID; }
            set
            {
                _controllerID = value;
                GetController();
            }
        }

        private async void GetController()
        {
            var access = await VibrationDevice.RequestAccessAsync();
            if (access == VibrationAccessStatus.Allowed)
            {
                var mgr = SpatialInteractionManager.GetForCurrentView();
                var calendar = new Calendar();
                var timestamp = PerceptionTimestampHelper.FromHistoricalTargetTime(calendar.GetDateTime());
                controller = (from s in mgr.GetDetectedSourcesAtTimestamp(timestamp)
                              where s.Source.Id == ControllerID
                              select s.Source.Controller.SimpleHapticsController)
                            .FirstOrDefault();
                expressions = new Dictionary<ushort, SimpleHapticsControllerFeedback>();
                if (controller != null)
                {
                    foreach (var fb in controller.SupportedFeedback)
                    {
                        if (fb.Waveform == KnownSimpleHapticsControllerWaveforms.BuzzContinuous)
                        {
                            buzz = fb;
                        }
                        else if (fb.Waveform == KnownSimpleHapticsControllerWaveforms.Click)
                        {
                            click = fb;
                        }
                        else if (fb.Waveform == KnownSimpleHapticsControllerWaveforms.Press)
                        {
                            press = fb;
                        }
                        else if (fb.Waveform == KnownSimpleHapticsControllerWaveforms.Release)
                        {
                            release = fb;
                        }
                        expressions[fb.Waveform] = fb;
                    }
                }
            }
        }

        /// <summary>
        /// Play a canned haptic pattern.
        /// </summary>
        /// <param name="expr"></param>
        public override void Play(HapticExpression expr)
        {
            if (controller != null)
            {
                if (expr == HapticExpression.Click)
                {
                    controller.SendHapticFeedback(click);
                }
                else if (expr == HapticExpression.Press)
                {
                    controller.SendHapticFeedback(press);
                }
                else if (expr == HapticExpression.Release)
                {
                    controller.SendHapticFeedback(release);
                }
                else
                {
                    base.Play(expr);
                }
            }
        }

        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            var seconds = Units.Milliseconds.Seconds(milliseconds);
            if (expressions.ContainsKey(KnownSimpleHapticsControllerWaveforms.BuzzContinuous))
            {
                controller?.SendHapticFeedbackForDuration(
                    expressions[KnownSimpleHapticsControllerWaveforms.BuzzContinuous],
                    amplitude,
                    TimeSpan.FromSeconds(seconds));
            }
            yield return new WaitForSeconds(seconds);
        }

        public override void Cancel()
        {
            base.Cancel();
            controller?.StopFeedback();
        }
    }
}
#endif