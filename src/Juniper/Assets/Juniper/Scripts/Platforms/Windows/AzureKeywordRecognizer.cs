#if AZURE_SPEECHSDK

using System;
using System.IO;
using System.Threading.Tasks;
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
        private bool IsUnrecoverable;
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
            ErrorTrap(recognizer.StartContinuousRecognitionAsync());
        }

        private void ErrorTrap(Task task)
        {
            task.ContinueWith(OnError, TaskContinuationOptions.OnlyOnFaulted)
                .ConfigureAwait(false);
        }

        private void OnError(Task t)
        {
            Debug.LogException(t.Exception);
            OnError(CancellationErrorCode.NoError, t.Exception.Message, true);
        }

        private void Recognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            if (e.Reason == CancellationReason.Error)
            {
                var errorCode = e.ErrorCode;
                var errorMessage = e.ErrorDetails;
                OnError(errorCode, errorMessage, false);
            }
        }

        private void OnError(CancellationErrorCode errorCode, string errorMessage, bool tryAgain)
        {
            ScreenDebugger.Print($"Recognition error: [{errorCode}] {errorMessage}");
            IsUnrecoverable = !tryAgain;
            if (!IsRunning)
            {
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

        protected override void TearDown()
        {
            if (recognizer != null)
            {
                IsStopping = true;
                recognizer.Recognized -= Recognizer_OnPhraseRecognized;
                recognizer.Recognizing -= Recognizer_OnPhraseRecognized;
                if (IsRunning)
                {
                    ErrorTrap(recognizer.StopContinuousRecognitionAsync());
                }
                else
                {
                    FinishTeardown();
                }
            }
        }

        private void Recognizer_SessionStopped(object sender, SessionEventArgs e)
        {
            FinishTeardown();
        }

        private void FinishTeardown()
        {
            recognizer.SessionStopped -= Recognizer_SessionStopped;
            recognizer.Dispose();
            recognizer = null;
            IsStopping = false;
            IsRunning = false;
        }
    }
}

#endif