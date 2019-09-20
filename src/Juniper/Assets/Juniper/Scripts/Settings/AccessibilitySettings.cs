using Juniper.Input;
using Juniper.Speech;
using UnityEngine;

using Clickable = Juniper.Widgets.Clickable;
using Selectable = UnityEngine.UI.Selectable;

namespace Juniper.Settings
{
    public class AccessibilitySettings : MonoBehaviour
    {
        private UnifiedInputModule input;

        public GameObject EnableVoiceButton;
        public GameObject EnableGazeButton;
        public GameObject EnableMouseButton;
        public GameObject EnableTouchButton;
        public GameObject EnableHandsButton;
        public GameObject EnableControllersButton;

        private bool? lastVoiceAvailable;
        private bool? lastVoiceEnabled;
        private bool? lastGazeAvailable;
        private bool? lastGazeEnabled;
        private bool? lastMouseAvailable;
        private bool? lastMouseEnabled;
        private bool? lastTouchAvailable;
        private bool? lastTouchEnabled;
        private bool? lastHandsAvailable;
        private bool? lastHandsEnabled;
        private bool? lastControllerAvailable;
        private bool? lastControllerEnabled;

        public void Awake()
        {
            input = ComponentExt.FindAny<UnifiedInputModule>();
        }

        public void Update()
        {
            DisableButton("Voice", EnableVoiceButton, input.VoiceAvailable, input.VoiceEnabled, ref lastVoiceAvailable, ref lastVoiceEnabled);
            DisableButton("Gaze pointer", EnableGazeButton, input.GazeAvailable, input.GazeEnabled, ref lastGazeAvailable, ref lastGazeEnabled);
            DisableButton("Mouse pointer", EnableMouseButton, input.MouseAvailable, input.MouseEnabled, ref lastMouseAvailable, ref lastMouseEnabled);
            DisableButton("Touch screen", EnableTouchButton, input.TouchAvailable, input.TouchEnabled, ref lastTouchAvailable, ref lastTouchEnabled);
            DisableButton("Hand tracking", EnableHandsButton, input.HandsAvailable, input.HandsEnabled, ref lastHandsAvailable, ref lastHandsEnabled);
            DisableButton("Motion controllers", EnableControllersButton, input.ControllersAvailable, input.ControllersEnabled, ref lastControllerAvailable, ref lastControllerEnabled);
        }

        private static void DisableButton(string name, GameObject button, bool available, bool enabled, ref bool? lastAvailable, ref bool? lastEnabled)
        {
            if (button != null && (available != lastAvailable || enabled != lastEnabled))
            {
                lastAvailable = available;
                lastEnabled = enabled;

                var sel = button.GetComponent<Selectable>();
                if (sel != null)
                {
                    sel.interactable = available;
                }

                var click = button.GetComponent<Clickable>();
                if (click != null)
                {
                    click.disabled = !available;
                }

                var text = !available
                    ? name + " not available"
                    : enabled
                        ? "Disable " + name.ToLowerInvariant()
                        : "Enable " + name.ToLowerInvariant();

#if UNITY_TEXTMESHPRO
                {
                    var tmp = button.GetComponentInChildren<TMPro.TextMeshPro>();
                    if (tmp != null)
                    {
                        tmp.text = text;
                    }
                }
                {
                    var tmp = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = text;
                    }
                }
#endif

#if UNITY_MODULES_UI
                {
                    var tmp = button.GetComponentInChildren<TextMesh>();
                    if (tmp != null)
                    {
                        tmp.text = text;
                    }
                }
#endif

                var keywordable = button.GetComponent<Keywordable>();
                if (keywordable != null)
                {
                    keywordable.SetTooltips();
                }
            }
        }

        private void SetInputMode(InputMode mode, bool enableAlternate, InputMode alternate)
        {
            if (enableAlternate)
            {
                mode |= alternate;
            }

            if (input.VoiceEnabled)
            {
                mode |= InputMode.Voice;
            }

            input.mode = mode;
        }

        public void EnableGaze()
        {
            SetInputMode(InputMode.Gaze, false, InputMode.None);
        }

        public void EnableMouse()
        {
            SetInputMode(InputMode.Mouse, input.TouchEnabled, InputMode.Touch);
        }

        public void EnableTouch()
        {
            SetInputMode(InputMode.Touch, input.MouseEnabled, InputMode.Mouse);
        }

        public void EnableHands()
        {
            SetInputMode(InputMode.Hands, input.ControllersEnabled, InputMode.Motion);
        }

        public void EnableControllers()
        {
            SetInputMode(InputMode.Motion, input.HandsEnabled, InputMode.Hands);
        }

        public void ToggleVoice()
        {
            if (input.VoiceEnabled)
            {
                input.mode &= ~InputMode.Voice;
            }
            else
            {
                input.mode |= InputMode.Voice;
            }
        }
    }
}
