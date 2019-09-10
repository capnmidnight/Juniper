using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Speech
{
    public class KeywordRecognizer :
#if UNITY_WSA || UNITY_STANDALONE_WIN
        WindowsKeywordRecognizer
#elif UNITY_ANDROID && AZURE_SPEECHSDK
        AzureKeywordRecognizer
#elif UNITY_EDITOR_WIN
        WindowsKeywordRecognizer
#else
        NoKeywordRecognizer
#endif
    {
    }

    /// <summary>
    /// A class that implements basic functionality for systems that manage speech recognition and
    /// route out the events associated with recognizing keywords.
    /// </summary>
    public abstract class AbstractKeywordRecognizer : MonoBehaviour, IKeywordRecognizer
    {
        /// <summary>
        /// The keywords for which to listen.
        /// </summary>
        protected string[] keywords;

        /// <summary>
        /// Respond to the speech recognition system having detected a keyword. You should only use
        /// this version in the Unity Editor. If you are programmatically attaching event listeners,
        /// you should prefer <see cref="KeywordRecognized"/>.
        /// </summary>
        public StringEvent onKeywordRecognized = new StringEvent();

        /// <summary>
        /// Respond to the speech recognition system having detected a keyword. You should only use
        /// this version if you are programmatically attaching event listeners. If you are attaching
        /// events in the Unity Editor, you should prefer <see cref="onKeywordRecognized"/>.
        /// </summary>
        public event EventHandler<KeywordRecognizedEventArgs> KeywordRecognized;

        protected bool IsRunning;

        /// <summary>
        /// Invokes <see cref="onKeywordRecognized"/> and <see cref="KeywordRecognized"/>.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        protected void OnKeywordRecognized(string keyword)
        {
            Debug.Log($"<==== Juniper ====> KeywordRecognizer::OnKeywordRecognized({keyword})");
            onKeywordRecognized?.Invoke(keyword);
            KeywordRecognized?.Invoke(this, new KeywordRecognizedEventArgs(keyword));
        }

        /// <summary>
        /// Find all of the keyword-responding components that are currently active in the scene,
        /// collect up their keywords, and register them return them as a set-array to be registered
        /// with the speech recognition system.
        /// </summary>
        /// <returns>The active keywords.</returns>
        public void RefreshKeywords()
        {
            CancelInvoke(nameof(RefreshKeywordsInternal));
            Invoke(nameof(RefreshKeywordsInternal), 0.25f);
        }

        private void RefreshKeywordsInternal()
        {
            TearDown();
            keywords = (from trigger in ComponentExt.FindAll<IKeywordTriggered>()
                        where trigger.Keywords != null
                        let comp = trigger as MonoBehaviour
                        where comp?.isActiveAndEnabled != false
                        from keyword in trigger.Keywords
                        where !string.IsNullOrEmpty(keyword)
                        select keyword)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
        }

        public virtual void Update()
        {
            if(IsAvailable && !IsRunning && keywords != null && keywords.Length > 0)
            {
                Setup();
            }
        }

        /// <summary>
        /// Initialize the speech recognition system.
        /// </summary>
        public void OnEnable()
        {
            RefreshKeywords();
        }

        /// <summary>
        /// Tear down the speech recognition system.
        /// </summary>
        public void OnDisable()
        {
            TearDown();
        }

        public abstract bool IsAvailable { get; }

        protected abstract void Setup();

        protected abstract void TearDown();
    }
}