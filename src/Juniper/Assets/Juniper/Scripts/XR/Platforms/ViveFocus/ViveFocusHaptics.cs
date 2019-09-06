#if WAVEVR
using System.Collections;

using UnityEngine;

using wvr;

namespace Juniper.Haptics
{
    public class ViveFocusHaptics : AbstractHapticExpressor
    {
        public WaveVR_Controller.Device Controller { get; set; }

        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            var start = DatTime.Now;
            var seconds = Units.Milliseconds.Seconds(milliseconds);
            var nanoseconds = Units.Milliseconds.Nanoseconds(milliseconds);
            var ts = TimeSpan.FromSeconds(seconds);
            var amp = Mathf.Clamp(Mathf.RoundToInt(amplitude * 5), 0, 5);
            if (amp > 0)
            {
                Controller?.TriggerHapticPulse(
                    (ushort)nanoseconds,
                    WVR_InputId.WVR_InputId_Alias1_Trigger);
            }
            while((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}

#endif