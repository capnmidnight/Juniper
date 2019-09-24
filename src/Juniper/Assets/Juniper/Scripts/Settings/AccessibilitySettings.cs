using System;
using System.Linq;
using Juniper.Input;
using Juniper.Speech;
using Juniper.Widgets;
using UnityEngine;

using Clickable = Juniper.Widgets.Clickable;
using Selectable = UnityEngine.UI.Selectable;

namespace Juniper.Settings
{
    public class AccessibilitySettings : MonoBehaviour
    {
        private UnifiedInputModule input;

        public GameObject EnableGazeButton;
        public GameObject EnableScreenButton;
        public GameObject EnableHandsButton;
        public GameObject EnableVoiceButton;

        private bool? lastGazeAvailable;
        private bool? lastGazeEnabled;
        private bool? lastScreenAvailable;
        private bool? lastScreenEnabled;
        private bool? lastHandsAvailable;
        private bool? lastHandsEnabled;
        private bool? lastVoiceAvailable;
        private bool? lastVoiceEnabled;

        public void Awake()
        {
            Find.Any(out input);
        }

        int disablerCount;
        GameObject onlyDisablerButton;
        public void Update()
        {
            disablerCount = 0;
            onlyDisablerButton = null;

            SetButtonView("Gaze pointer",
                EnableGazeButton,
                input.GazeAvailable,
                input.GazeEnabled,
                ref lastGazeAvailable, ref lastGazeEnabled);

            SetButtonView("Screen pointer",
                EnableScreenButton,
                input.MouseAvailable || input.TouchAvailable,
                input.MouseEnabled || input.TouchEnabled,
                ref lastScreenAvailable, ref lastScreenEnabled);

            SetButtonView(
                "Hand tracking",
                EnableHandsButton,
                input.HandsAvailable || input.ControllersAvailable,
                input.HandsEnabled || input.ControllersEnabled,
                ref lastHandsAvailable, ref lastHandsEnabled);

            SetButtonView("Voice",
                EnableVoiceButton,
                input.VoiceAvailable,
                input.VoiceEnabled,
                ref lastVoiceAvailable, ref lastVoiceEnabled);

            if (disablerCount == 1)
            {
                EnableButton(onlyDisablerButton, false);
            }
        }

        private void SetButtonView(string name, GameObject button, bool deviceAvailable, bool deviceEnabled, ref bool? lastAvailable, ref bool? lastEnabled)
        {
            if (button != null && (deviceAvailable != lastAvailable || deviceEnabled != lastEnabled))
            {
                lastAvailable = deviceAvailable;
                lastEnabled = deviceEnabled;

                EnableButton(button, deviceAvailable);

                if (deviceAvailable && deviceEnabled && button != EnableVoiceButton)
                {
                    ++disablerCount;
                    onlyDisablerButton = button;
                }

                var text = !deviceAvailable
                    ? name + " not available"
                    : deviceEnabled
                        ? "Disable " + name.ToLowerInvariant()
                        : "Enable " + name.ToLowerInvariant();


                var textElement = button.Ensure<TextComponentWrapper>().Value;
                textElement.text = text;

                var keywordable = button.GetComponent<Keywordable>();
                if (keywordable != null)
                {
                    keywordable.SetTooltips();
                }
            }
        }

        private static void EnableButton(GameObject button, bool deviceAvailable)
        {
            var sel = button.GetComponent<Selectable>();
            if (sel != null)
            {
                sel.interactable = deviceAvailable;
            }

            var click = button.GetComponent<Clickable>();
            if (click != null)
            {
                click.disabled = !deviceAvailable;
            }

            var keywordable = button.GetComponent<Keywordable>();
            if(keywordable != null)
            {
                var keywords = keywordable.keywords;
                for(int i = 0; i < keywords.Length; ++i)
                {
                    keywords[i] = keywords[i].Replace(
                        deviceAvailable ? "disable" : "enable",
                        deviceAvailable ? "enable" : "disable");

                    if (!deviceAvailable)
                    {
                        keywords[i] = keywords[i].Replace("use", "disable");
                    }
                }

                keywords = keywords.Distinct().ToArray();
                if (deviceAvailable)
                {
                    var newKeywords = keywords.Select(x => x.Replace("enable", "use"));
                    keywords = keywords.Union(newKeywords).ToArray();
                }

                Array.Sort(keywords);

                keywordable.keywords = keywords;
                keywordable.SetTooltips();
            }
        }

        private void SetInputMode(InputMode mode)
        {
            if (input.VoiceEnabled)
            {
                mode |= InputMode.Voice;
            }

            input.mode = mode;
        }

        public void EnableGaze()
        {
            SetInputMode(InputMode.Gaze);
        }

        public void EnableScreen()
        {
            SetInputMode(InputMode.Mouse | InputMode.Touch);
        }

        public void EnableHands()
        {
            SetInputMode(InputMode.Hands | InputMode.Motion);
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
