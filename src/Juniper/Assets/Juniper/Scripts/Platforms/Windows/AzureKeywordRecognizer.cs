#if !UNITY_WSA && !UNITY_STANDALONE_WIN && AZURE_SPEECHSDK

using System;
using System.IO;

using Juniper.Security;

using Microsoft.CognitiveServices.Speech;

using UnityEngine;

namespace Juniper.Speech
{
    public abstract class AzureKeywordRecognizer : AbstractKeywordRecognizer, ICredentialReceiver
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        public override bool IsAvailable { get { return !IsUnrecoverable; } }

        /// <summary>
        /// The real recognizer.
        /// </summary>
        private SpeechRecognizer recognizer;

        [SerializeField]
        [HideInNormalInspector]
        private string azureApiKey;

        [SerializeField]
        [HideInNormalInspector]
        private string azureRegion;

        public string CredentialFile
        {
            get
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
                return keyFile;
            }
        }

        public void ReceiveCredentials(string[] args)
        {
            if (args == null)
            {
                azureApiKey = null;
                azureRegion = null;
            }
            else
            {
                azureApiKey = args[0];
                azureRegion = args[1];
            }
        }

        public void Awake()
        {
#if UNITY_EDITOR
            this.ReceiveCredentials();
#endif
        }

        protected override void Setup()
        {
            IsStarting = true;
            var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
            config.SetProfanity(ProfanityOption.Raw);
            config.SpeechRecognitionLanguage = "en-us";
            recognizer = new SpeechRecognizer(config);
            recognizer.SessionStarted += Recognizer_SessionStarted;
            recognizer.Recognizing += Recognizer_OnPhraseRecognized;
            recognizer.Recognized += Recognizer_OnPhraseRecognized;
            recognizer.SessionStopped += Recognizer_SessionStopped;
            recognizer.Canceled += Recognizer_Canceled;
            recognizer.StartContinuousRecognitionAsync();
        }

        private void Recognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            if(e.Reason == CancellationReason.Error)
            {
                ScreenDebugger.Print($"Recognition error: [{e.ErrorCode}] {e.ErrorDetails}");
                IsUnrecoverable = true;
                TearDown();
            }
        }

        private void Recognizer_SessionStarted(object sender, SessionEventArgs e)
        {
            recognizer.SessionStarted -= Recognizer_SessionStarted;
            IsStarting = false;
            IsRunning = true;
        }

        /// <summary>
        /// When speech is recognized, forward it into the keyword recognizer.
        /// </summary>
        /// <param name="args">Arguments.</param>
        void Recognizer_OnPhraseRecognized(object sender, SpeechRecognitionEventArgs args)
        {
            if (args.Result.Reason == ResultReason.RecognizedIntent
                || args.Result.Reason == ResultReason.RecognizingIntent
                || args.Result.Reason == ResultReason.RecognizedKeyword
                || args.Result.Reason == ResultReason.RecognizingKeyword
                || args.Result.Reason == ResultReason.RecognizedSpeech
                || args.Result.Reason == ResultReason.RecognizingSpeech)
            {
                ProcessText(args.Result.Text);
            }
        }

        private void Recognizer_SessionStopped(object sender, SessionEventArgs e)
        {
            recognizer.SessionStopped -= Recognizer_SessionStopped;
            recognizer.Dispose();
            recognizer = null;
            IsStopping = false;
            IsRunning = false;
        }

        protected override void TearDown()
        {
            if (recognizer != null)
            {
                IsStopping = true;
                recognizer.Recognized -= Recognizer_OnPhraseRecognized;
                recognizer.Recognizing -= Recognizer_OnPhraseRecognized;
                recognizer.StopContinuousRecognitionAsync();
            }
        }
    }
}

#endif