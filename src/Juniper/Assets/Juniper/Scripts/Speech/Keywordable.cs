using System;
using System.Collections.Generic;
using System.Linq;
using Juniper.Input;
using Juniper.Widgets;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Speech
{
    /// <summary>
    /// An object that can be fires an event when a particular keyword is uttered.
    /// </summary>
    public class Keywordable : MonoBehaviour, IKeywordTriggered
    {
        /// <summary>
        /// A default function for when the Keywordable is applied to something that
        /// does not have a parent control.
        /// </summary>
        /// <returns></returns>
        private static bool AlwaysEnabled() { return true; }

        /// <summary>
        /// The keywords that activate this event.
        /// </summary>
        public string[] keywords;

        /// <summary>
        /// A keyboard shortcut to make development easier (and quieter, RE: open-floor plans).
        /// </summary>
        public KeyCode shortcutKey = KeyCode.None;

        /// <summary>
        /// The event to trigger when the keyword is recognized. If you are programmatically wiring
        /// up events, prefer the <see cref="KeywordDetected"/> event instead.
        /// </summary>
        public UnityEvent onKeywordDetected = new UnityEvent();

        /// <summary>
        /// The event to trigger when the keyword is recognized. If you are programmatically wiring
        /// up events, prefer this event over the <see cref="onKeywordDetected"/> event.
        /// </summary>
        public event EventHandler KeywordDetected;

        private bool wasLocked;

        private Func<bool> isParentEnabled;

        private UnifiedInputModule input;

        private bool wasInteractable;

        public bool IsInteractable()
        {
            return enabled && isParentEnabled() && input.VoiceEnabled;
        }

        /// <summary>
        /// The keywords that activate this event.
        /// </summary>
        /// <value>The keywords.</value>
        public IEnumerable<string> Keywords
        {
            get
            {
                return keywords;
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            keywords = (from keyword in keywords
                        let k = keyword.ToLowerInvariant()
                        select k)
                    .ToArray();

            SetTooltips(false);
        }
#endif

        [ContextMenu("Set tooltips")]
        public void SetTooltips()
        {
            SetTooltips(true);
        }

        private void SetTooltips(bool force)
        {
            var tooltip = GetComponent<Tooltipable>();
            if (tooltip != null && tooltip.tooltip != null)
            {
                var textElement = tooltip.tooltip.Ensure<TextComponentWrapper>().Value;
                if(force || string.IsNullOrEmpty(textElement.text))
                {
                    textElement.text = DefaultDescription;
                }
            }
        }

        /// <summary>
        /// The default text to set to a tooltip for this set of keywords.
        /// </summary>
        private string DefaultDescription
        {
            get
            {
                if (keywords == null || keywords.Length == 0)
                {
                    return string.Empty;
                }
                else if (keywords.Length == 1)
                {
                    return $"Say \"{keywords[0]}\" to activate.";
                }
                else if (keywords.Length == 2)
                {
                    return $"Say \"{keywords[0]}\" or \"{keywords[1]}\" to activate.";
                }
                else
                {
                    var last = keywords.Last();
                    var list = keywords.Take(keywords.Length - 1);
                    var first = string.Join(", ", (from word in list
                                                   select '"' + word + '"'));

                    return $"Say {first} or \"{last}\" to activate.";
                }
            }
        }

        /// <summary>
        /// Get the speech recognition manager.
        /// </summary>
        public void Awake()
        {
            var parentControl = GetComponent<IPointerClickHandler>();
            if (parentControl is UnityEngine.UI.Selectable selectable)
            {
                isParentEnabled = selectable.IsInteractable;
            }
            else if (parentControl is AbstractTouchable touchable)
            {
                isParentEnabled = touchable.IsInteractable;
            }
            else
            {
                isParentEnabled = AlwaysEnabled;
            }

            input = ComponentExt.FindAny<UnifiedInputModule>();
        }

        private void OnEnable()
        {
            input?.Voice?.AddKeywordable(this);
        }

        private void OnDisable()
        {
            input?.Voice?.RemoveKeywordable(this);
        }

        public void OnKeywordRecognized()
        {
            OnKeyword();
        }

        /// <summary>
        /// Check to see if the keyboard shortcut has been triggered.
        /// </summary>
        public void Update()
        {
            if (shortcutKey != KeyCode.None && UnityEngine.Input.GetKeyDown(shortcutKey)
                && (shortcutKey != KeyCode.Escape || !wasLocked))
            {
                OnKeyword();
            }

            var isInteractable = IsInteractable();
            if(isInteractable != wasInteractable)
            {
                wasInteractable = isInteractable;
                if (isInteractable)
                {
                    input?.Voice?.AddKeywordable(this);
                }
                else
                {
                    input?.Voice?.RemoveKeywordable(this);
                }
            }

            wasLocked = Cursor.lockState != CursorLockMode.None;
        }

        /// <summary>
        /// When a keyword is found (or a keyboard shortcut is triggered), fire the event (if it's valid).
        /// </summary>
        protected virtual void OnKeyword()
        {
            if (IsInteractable())
            {
                onKeywordDetected?.Invoke();
                KeywordDetected?.Invoke(this, EventArgs.Empty);
                var pointerEvent = new PointerEventData(ComponentExt.FindAny<EventSystem>())
                {
                    button = PointerEventData.InputButton.Left,
                    eligibleForClick = true,
                    clickCount = 1,
                    clickTime = Time.unscaledTime,
                };
                foreach (var button in GetComponents<IPointerClickHandler>())
                {
                    if (button != null)
                    {
                        button.OnPointerClick(pointerEvent);
                    }
                }
            }
        }
    }
}
