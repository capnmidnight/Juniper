#if (UNITY_IOS || UNITY_ANDROID) && !GOOGLEVR && !OCULUS
#define HAS_HAPTICS
#endif

using System.Collections;

using UnityEngine;

#if UNITY_STANDALONE || UNITY_EDITOR
using XInputDotNetPure;
#elif UNITY_WSA && !UNITY_EDITOR
using System.Linq;
using Windows.Gaming.Input;
#endif

namespace Juniper.Haptics
{
    /// <summary>
    /// When no specific haptic implementation is available, but we know the system supports haptics,
    /// we fallback to using Unity's built-in Vibrate function, which is pretty primitive.
    /// </summary>
    public class DefaultHaptics : AbstractHapticExpressor
    {
#if UNITY_WSA && !UNITY_EDITOR
        Gamepad gp;
        public void Awake()
        {
            gp = Gamepad.Gamepads.FirstOrDefault();
            if (gp == null)
            {
                Gamepad.GamepadAdded += Gamepad_GamepadAdded;
                Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
            }
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            gp = e;
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            if (gp == e)
            {
                gp = null;
            }
        }

#endif

        private void SetVibration(float v)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            GamePad.SetVibration(PlayerIndex.One, v, v);

#elif UNITY_WSA
            if (gp != null)
            {
                gp.Vibration = new GamepadVibration
                {
                    LeftMotor = v,
                    RightMotor = v
                };
            }
#endif
        }

        /// <summary>
        /// Cancel the current vibration, whatever it is.
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
            SetVibration(0);
        }

        /// <summary>
        /// Play a single vibration of a set length of time.
        /// </summary>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <param name="amplitude">The strenght of vibration (ignored).</param>
        protected override IEnumerator VibrateCoroutine(long milliseconds, float amplitude)
        {
            var seconds = Units.Milliseconds.Seconds(milliseconds);
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
            SetVibration(amplitude);
#elif HAS_HAPTICS
            if (amplitude > 0.25f)
            {
                var now = Time.time;
                while (Time.time - now < seconds)
                {
                    Handheld.Vibrate();
                    yield return null;
                }
            }
            else
            {
#endif
            yield return new WaitForSeconds(seconds);
#if !(UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA) && HAS_HAPTICS
            }
#endif
        }
    }
}
