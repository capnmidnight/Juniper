#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_XR_GOOGLEVR_ANDROID && !UNITY_XR_OCULUS
#define HAS_HAPTICS
#endif

using System;
using System.Collections;

#if (UNITY_STANDALONE || UNITY_EDITOR) && XINPUTDOTNETPURE

using XInputDotNetPure;

#elif UNITY_XR_WINDOWSMR_METRO
using System.Linq;
using Windows.Gaming.Input;
#endif

namespace Juniper.Haptics
{
    /// <summary>
    /// When no specific haptic implementation is available, but we know the system supports haptics,
    /// we fallback to using Unity's built-in Vibrate function, which is pretty primitive.
    /// </summary>
    public class DefaultHaptics : AbstractHapticImmediateExpressor
    {
#if UNITY_XR_WINDOWSMR_METRO && !UNITY_EDITOR
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

        /// <summary>
        /// Set a haptic pulse playing at a set amplitude.
        /// </summary>
        /// <param name="amplitude">The strength of the vibration.</param>
        protected override void SetVibration(long milliseconds, float amplitude)
        {
#if (UNITY_STANDALONE || UNITY_EDITOR) && XINPUTDOTNETPURE
            GamePad.SetVibration(PlayerIndex.One, amplitude, amplitude);

#elif UNITY_XR_WINDOWSMR_METRO
            if (gp != null)
            {
                gp.Vibration = new GamepadVibration
                {
                    LeftMotor = amplitude,
                    RightMotor = amplitude
                };
            }
#elif HAS_HAPTICS
            if(amplitude > 0.25f)
            {
                Handheld.Vibrate();
            }
#endif
        }
    }
}
