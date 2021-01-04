using System.Collections.Generic;
using System.Linq;

using Juniper.Input;
using Juniper.Widgets;

using UnityEngine;

namespace Juniper.Speech
{
    /// <summary>
    /// An object that can be fires an event when a particular keyword is uttered.
    /// </summary>
    public class Keywordable : AbstractShortcutable, IKeywordTriggered
    {

        /// <summary>
        /// The keywords that activate this event.
        /// </summary>
        public string[] keywords;

        public override bool IsInteractable()
        {
            return base.IsInteractable()
                && input.VoiceEnabled;
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
            if (keywords != null)
            {
                keywords = (from keyword in keywords
                            let k = keyword.ToLowerInvariant()
                            select k)
                        .ToArray();

                SetTooltips(false);
            }
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
            if (tooltip != null
                && (force || string.IsNullOrEmpty(tooltip.Text)))
            {
                tooltip.Text = DefaultDescription;
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

        protected override void OnEnable()
        {
            if (input != null && input.Voice != null)
            {
                input.Voice.AddKeywordable(this);
            }
        }

        protected override void OnDisable()
        {
            if (input != null && input.Voice != null)
            {
                input.Voice.RemoveKeywordable(this);
            }
        }
    }
}
