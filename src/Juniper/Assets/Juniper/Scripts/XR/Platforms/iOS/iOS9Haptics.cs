#if UNITY_IOS
using System;
using System.Collections;
using UnityEngine;

namespace Juniper.Unity.Haptics
{
    /// <summary>
    /// https://gist.github.com/kenshin03/6303582
    /// </summary>
    public class iOS9Haptics : AbstractHapticExpressor
    {
        /// <summary>
        /// Playing arbitrary haptic patterns is not supported on iOS.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException"></exception>
        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            throw new NotImplementedException("Playing arbitrary haptic patterns is not supported on iOS.");
        }
    }
}
#endif
