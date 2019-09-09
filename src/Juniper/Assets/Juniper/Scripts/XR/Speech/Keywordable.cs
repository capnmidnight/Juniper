using System;
using System.Collections.Generic;
using System.Linq;

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
        /// The keywords that activate this event.
        /// </summary>
        public string[] keywords;

        /// <summary>
        /// A keyboard shortcut to make development easier (and quieter, RE: open-floor plans).
        /// </summary>
        public KeyCode shortcutKey = KeyCode.None;

        /// <summary>
        /// The event to trigger when the keyword is recognized. If you are programmatically wiring
        /// up events, prefer the <see cref="Keyword"/> event instead.
        /// </summary>
        public UnityEvent onKeyword = new UnityEvent();

        /// <summary>
        /// The event to trigger when the keyword is recognized. If you are programmatically wiring
        /// up events, prefer this event over the <see cref="onKeyword"/> event.
        /// </summary>
        public event EventHandler Keyword;

        private bool wasLocked;

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

        /// <summary>
        /// Get the speech recognition manager.
        /// </summary>
        public void Awake()
        {
            keyer = ComponentExt.FindAny<KeywordRecognizer>();
        }

        /// <summary>
        /// Trigger updating the current recognizable keyword set.
        /// </summary>
        public void OnEnable()
        {
            if (HasKeywords)
            {
                keyer.RefreshKeywords();
            }
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
            wasLocked = Cursor.lockState != CursorLockMode.None;
        }

        /// <summary>
        /// When a keyword is found (or a keyboard shortcut is triggered), fire the event (if it's valid).
        /// </summary>
        protected virtual void OnKeyword()
        {
            onKeyword?.Invoke();
            Keyword?.Invoke(this, EventArgs.Empty);
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

        /// <summary>
        /// The main speech recognition manager.
        /// </summary>
        private KeywordRecognizer keyer;

        /// <summary>
        /// Returns true when this component has more than one keyword defined and we were able to
        /// find the speech recognition manager.
        /// </summary>
        /// <value><c>true</c> if has keywords; otherwise, <c>false</c>.</value>
        private bool HasKeywords
        {
            get
            {
                return keywords != null && keywords.Length > 0 && keyer != null;
            }
        }

        /// <summary>
        /// Respond to a keyword detected event. If the keyword was one that we care about in this
        /// component, then trigger the event.
        /// </summary>
        /// <param name="word">Word.</param>
        private void OnKeyword(string word)
        {
            if (Array.BinarySearch(keywords, word) >= 0)
            {
                OnKeyword();
            }
        }

        /// <summary>
        /// On startup, register an event handler on the speech reconition manager for whenever
        /// keywords are detected.
        /// </summary>
        private void Start()
        {
            if (HasKeywords)
            {
                keywords = (from keyword in keywords
                            let k = keyword.ToLowerInvariant()
                            orderby k
                            select k)
                        .Distinct()
                        .ToArray();

                keyer.onKeywordRecognized.AddListener(OnKeyword);
            }
        }
    }
}
