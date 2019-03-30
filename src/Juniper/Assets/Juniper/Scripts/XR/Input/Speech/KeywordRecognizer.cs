using System;
using System.Linq;

using Juniper.Input.Speech;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Unity.Input.Speech
{
    /// <summary>
    /// A class that implements basic functionality for systems that manage speech recognition and
    /// route out the events associated with recognizing keywords.
    /// </summary>
    public class KeywordRecognizer : MonoBehaviour, IKeywordRecognizer
    {
        /// <summary>
        /// The keywords for which to listen.
        /// </summary>
        protected string[] keywords;

        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
#if UNITY_WSA
        public const bool IsAvailable = true;

        /// <summary>
        /// The real recognizer.
        /// </summary>
        UnityEngine.Windows.Speech.KeywordRecognizer recognizer;

        /// <summary>
        /// When speech is recognized, forward it into the keyword recognizer.
        /// </summary>
        /// <param name="args">Arguments.</param>
        void Recognizer_OnPhraseRecognized(UnityEngine.Windows.Speech.PhraseRecognizedEventArgs args)
        {
            OnKeywordRecognized(args.text);
        }
#else
        public const bool IsAvailable = false;
#endif

        /// <summary>
        /// Respond to the speech recognition system having detected a keyword. You should only use
        /// this version in the Unity Editor. If you are programmatically attaching event listeners,
        /// you should preferr <see cref="KeywordRecognized"/>.
        /// </summary>
        public StringEvent onKeywordRecognized = new StringEvent();

        /// <summary>
        /// Respond to the speech recognition system having detected a keyword. You should only use
        /// this version if you are programmatically attaching event listeners. If you are attaching
        /// events in the Unity Editor, you should preferr <see cref="onKeywordRecognized"/>.
        /// </summary>
        public event EventHandler<KeywordRecognizedEventArgs> KeywordRecognized;

        /// <summary>
        /// Invokes <see cref="onKeywordRecognized"/> and <see cref="KeywordRecognized"/>.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        protected void OnKeywordRecognized(string keyword)
        {
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
            this.WithLock(() =>
            {
                keywords = (from comp in Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                            where (Application.isPlaying || comp.gameObject.scene.isLoaded)
                            && comp is IKeywordTriggered
                            let trigger = (IKeywordTriggered)comp
                            where trigger.Keywords != null
                            from keyword in trigger.Keywords
                            where !string.IsNullOrEmpty(keyword)
                            select keyword)
                .Distinct()
                .ToArray();

                Array.Sort(keywords);
            });
        }

        /// <summary>
        /// Initialize the speech recognition system.
        /// </summary>
        public void OnEnable()
        {
#if UNITY_WSA
            OnDisable();

            if (keywords != null && keywords.Length > 0)
            {
                recognizer = new UnityEngine.Windows.Speech.KeywordRecognizer(keywords);
                recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
                recognizer.Start();
            }
#endif
        }

        /// <summary>
        /// Tear down the speech recognition system.
        /// </summary>
        public void OnDisable()
        {
#if UNITY_WSA
            if (recognizer != null)
            {
                recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
                recognizer.Stop();
                recognizer.Dispose();
                recognizer = null;
            }
#endif
        }
    }
}