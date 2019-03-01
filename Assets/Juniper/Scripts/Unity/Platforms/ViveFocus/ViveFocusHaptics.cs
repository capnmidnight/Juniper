#if WAVEVR
using System;
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
            var amp = Mathf.Clamp(Mathf.RoundToInt(amplitude * 5), 0, 5);
            if (amp > 0)
            {
                Controller?.TriggerHapticPulse(
                    (ushort)(milliseconds * 1000),
                    WVR_InputId.WVR_InputId_Alias1_Trigger,
                    (WVR_Intensity)amp);
            }
            yield return new WaitForSeconds(0.001f * milliseconds);
        }
    }
}

#endif
