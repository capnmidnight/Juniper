#if UNITY_STANDALONE_WIN
using System.Globalization;
using System.Speech.Recognition;

namespace Juniper.Unity.Input.Speech
{
    public abstract class NETFXKeywordRecognizer : AbstractKeywordRecognizer
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        public const bool IsAvailable = true;

        /// <summary>
        /// The real recognizer.
        /// </summary>
        private SpeechRecognitionEngine recognizer;

        /// <summary>
        /// When speech is recognized, forward it into the keyword recognizer.
        /// </summary>
        /// <param name="e">Arguments.</param>
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            OnKeywordRecognized(e.Result.Text);
        }

        protected override void Setup()
        {
            if (keywords != null && keywords.Length > 0)
            {
                recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
                var grammar = new GrammarBuilder(new Choices(keywords));
                recognizer.LoadGrammar(new Grammar(grammar));
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        protected override void TearDown()
        {
            if (recognizer != null)
            {
                recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;
                recognizer.RecognizeAsyncStop();
                recognizer.Dispose();
                recognizer = null;
            }
        }
    }
}
#endif