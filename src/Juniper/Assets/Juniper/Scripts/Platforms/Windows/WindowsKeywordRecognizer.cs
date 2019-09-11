#if UNITY_WSA || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

namespace Juniper.Speech
{
    public abstract class WindowsKeywordRecognizer : AbstractKeywordRecognizer
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        public override bool IsAvailable { get { return true; } }

        protected override bool NeedsKeywords { get { return true; } }

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

        protected override void Setup()
        {
            if (keywords != null && keywords.Length > 0)
            {
                recognizer = new UnityEngine.Windows.Speech.KeywordRecognizer(keywords);
                recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
                recognizer.Start();
            }
        }

        protected override void TearDown()
        {
            if (recognizer != null)
            {
                recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
                recognizer.Stop();
                recognizer.Dispose();
                recognizer = null;
            }
        }
    }
}

#endif