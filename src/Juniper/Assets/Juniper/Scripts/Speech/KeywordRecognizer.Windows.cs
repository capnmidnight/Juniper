#if !AZURE_SPEECHSDK && (UNITY_WSA || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)


using UnityEngine.Windows.Speech;

namespace Juniper.Speech
{
    public partial class KeywordRecognizer
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The real recognizer.
        /// </summary>
        DictationRecognizer recognizer;

        protected void Setup()
        {
            IsStarting = true;
            recognizer = new DictationRecognizer();
            recognizer.DictationHypothesis += Recognizer_DictationHypothesis;
            recognizer.DictationResult += Recognizer_DictationResult;
            recognizer.DictationComplete += Recognizer_DictationComplete;
            recognizer.Start();
            IsStarting = false;
            IsRunning = true;
        }

        private void Recognizer_DictationHypothesis(string text)
        {
            ProcessText(text, false);
        }

        /// <summary>
        /// When speech is recognized, forward it into the keyword recognizer.
        /// </summary>
        /// <param name="text">The text recognized.</param>
        /// <param name="confidence">How confident the system is in the recognized text being correct.</param>
        private void Recognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            ProcessText(text, true);
        }

        private void Recognizer_DictationComplete(DictationCompletionCause cause)
        {
            TearDown();
        }

        protected void TearDown()
        {
            if (recognizer != null)
            {
                IsStopping = true;
                recognizer.DictationHypothesis -= Recognizer_DictationHypothesis;
                recognizer.DictationResult -= Recognizer_DictationResult;
                recognizer.DictationComplete -= Recognizer_DictationComplete;
                recognizer.Stop();
                recognizer.Dispose();
                recognizer = null;
                IsStopping = false;
                IsRunning = false;
            }
        }
    }
}

#endif