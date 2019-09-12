using System;
using System.Collections;

namespace Juniper.Haptics
{
    public abstract class AbstractHapticImmediateExpressor : AbstractHapticRetainedExpressor
    {
        public override void Cancel()
        {
            base.Cancel();
            SetVibration(0, 0);
        }

        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            var start = DateTime.Now;
            var seconds = Units.Milliseconds.Seconds(milliseconds);
            var ts = TimeSpan.FromSeconds(seconds);
            SetVibration(milliseconds, amplitude);
            while((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }

        protected abstract void SetVibration(long milliseconds, float amplitude);
    }
}
