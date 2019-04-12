#if UNITY_WSA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.Unity.Input.Speech
{
    public abstract class UWPKeywordRecognizer : AbstractKeywordRecognizer
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
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