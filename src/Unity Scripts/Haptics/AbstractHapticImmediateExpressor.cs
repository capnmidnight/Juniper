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
            var end = DateTime.Now.AddMilliseconds(milliseconds);
            SetVibration(milliseconds, amplitude);
            while(DateTime.Now < end)
            {
                yield return null;
            }
        }

        protected abstract void SetVibration(long milliseconds, float amplitude);
    }
}
