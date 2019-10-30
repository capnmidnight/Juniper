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
        private class Device
        {
            public readonly string name;
            public readonly GameObject button;

            private readonly UnifiedInputModule input;
            private readonly Func<UnifiedInputModule, bool> isAvailable;
            private readonly Func<UnifiedInputModule, bool> isEnabled;

            public bool wasAvailable;
            public bool wasEnabled;

            public Device(UnifiedInputModule input, string name, GameObject button, Func<UnifiedInputModule, bool> isAvailable, Func<UnifiedInputModule, bool> isEnabled)
            {
                this.input = input;
                this.name = name;
                this.button = button;
                this.isAvailable = isAvailable;
                this.isEnabled = isEnabled;
                wasAvailable = !isAvailable(input);
                wasEnabled = !isEnabled(input);
            }

            public void Update()
            {
                var deviceAvailable = isAvailable(input);
                var deviceEnabled = isEnabled(input);

                if (button != null
                    && (deviceAvailable != wasAvailable
                        || deviceEnabled != wasEnabled))
                {
                    wasAvailable = deviceAvailable;
                    wasEnabled = deviceEnabled;

                    EnableButton(deviceAvailable);

                    var text = !deviceAvailable
                        ? name + " not available"
                        : deviceEnabled
                            ? "Disable " + name.ToLowerInvariant()
                            : "Enable " + name.ToLowerInvariant();

                    var textElement = button.GetComponent<TextComponentWrapper>();
                    if (textElement != null)
                    {
                        textElement.Text = text;
                    }

                    var keywordable = button.GetComponent<Keywordable>();
                    if (keywordable != null)
                    {
                        var keywords = keywordable.keywords;
                        for (int i = 0; i < keywords.Length; ++i)
                        {
                            if (!deviceEnabled)
                            {
                                keywords[i] = keywords[i]
                                    .Replace("disable", "enable")
                                    .Replace("stop using", "use");
                            }
                            else
                            {
                                keywords[i] = keywords[i]
                                    .Replace("enable", "disable")
                                    .Replace("use", "stop using");
                            }
                        }

                        keywords = keywords.Distinct().ToArray();
                        Array.Sort(keywords);
                        keywordable.keywords = keywords;
                        keywordable.SetTooltips();
                    }
                }
            }

            private void EnableButton(bool deviceAvailable)
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
                if (keywordable != null)
                {
                    keywordable.enabled = deviceAvailable;
                }
            }

            public void DisableButton()
            {
                EnableButton(false);
                wasAvailable = false;
            }
        }

        private UnifiedInputModule input;

        public GameObject EnableGazeButton;
        public GameObject EnableScreenButton;
        public GameObject EnableHandsButton;
        public GameObject EnableVoiceButton;

        private Device[] devices;

        public void Awake()
        {
            Find.Any(out input);

            devices = new[]
            {
                new Device(input,
                    "Gaze pointer",
                    EnableGazeButton,
                    (i) => i.GazeAvailable,
                    (i) => i.GazeEnabled),

                new Device(input,
                    "Screen pointer",
                    EnableScreenButton,
                    (i) => i.MouseAvailable || i.TouchAvailable,
                    (i) => i.MouseEnabled || i.TouchEnabled),

                new Device(input,
                    "Hand tracking",
                    EnableHandsButton,
                    (i) => i.HandsAvailable || i.ControllersAvailable,
                    (i) => i.HandsEnabled || i.ControllersEnabled),

                new Device(input,
                    "Voice",
                    EnableVoiceButton,
                    (i) => i.VoiceAvailable,
                    (i) => i.VoiceEnabled)
            };
        }

        public void Update()
        {
            foreach (var device in devices)
            {
                device.Update();
            }

            var disablerCount = 0;
            Device onlyDisablerButton = default;
            foreach (var device in devices)
            {
                if (device.wasEnabled)
                {
                    ++disablerCount;
                    onlyDisablerButton = device;
                }
            }

            if (disablerCount == 1)
            {
                onlyDisablerButton.DisableButton();
            }
        }

        public void ToggleGaze()
        {
            input.ToggleModeRequested(InputMode.Gaze);
        }

        public void ToggleScreen()
        {
            input.ToggleModeRequested(InputMode.Mouse);
            input.ToggleModeRequested(InputMode.Touch);
        }

        public void ToggleHands()
        {
            input.ToggleModeRequested(InputMode.Hands);
            input.ToggleModeRequested(InputMode.Motion);
        }

        public void ToggleVoice()
        {
            input.ToggleModeRequested(InputMode.Voice);
        }
    }
}
