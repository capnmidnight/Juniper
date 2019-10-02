#if AZURE_SPEECHSDK

using System;
using System.IO;
using System.Threading.Tasks;
using Juniper.Security;

using Microsoft.CognitiveServices.Speech;

using UnityEngine;

namespace Juniper.Speech
{
    public partial class KeywordRecognizer
#if UNITY_EDITOR
        : ICredentialReceiver
#endif
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        private bool IsUnrecoverable;
        public bool IsAvailable
        {
            get
            {
                return !IsUnrecoverable;
            }
        }

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

#if UNITY_EDITOR
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
#endif

        public void Awake()
        {
#if UNITY_EDITOR
            this.ReceiveCredentials();
#endif
        }

        protected void Setup()
        {
            IsStarting = true;

            var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
            config.SetProfanity(ProfanityOption.Raw);
            config.SpeechRecognitionLanguage = "en-us";

            recognizer = new SpeechRecognizer(config);
            recognizer.SessionStarted += Recognizer_SessionStarted;
            recognizer.Recognizing += Recognizer_OnPhraseRecognizing;
            recognizer.Recognized += Recognizer_OnPhraseRecognized;
            recognizer.SessionStopped += Recognizer_SessionStopped;
            recognizer.Canceled += Recognizer_Canceled;
            ErrorTrap(recognizer.StartContinuousRecognitionAsync());
        }

        private void Recognizer_OnPhraseRecognizing(object sender, SpeechRecognitionEventArgs e)
        {
            OnRecognitionRecognizing();
        }

        private void ErrorTrap(Task task)
        {
            task.ContinueWith(OnError, TaskContinuationOptions.OnlyOnFaulted)
                .ConfigureAwait(false);
        }

        private void OnError(Task t)
        {
            Debug.LogException(t.Exception);
            OnError(CancellationErrorCode.NoError, t.Exception.Message);
        }

        private void Recognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            if (e.Reason == CancellationReason.Error)
            {
                var errorCode = e.ErrorCode;
                var errorMessage = e.ErrorDetails;
                OnError(errorCode, errorMessage);
            }
            else
            {
                OnRecognitionCanceled();
            }
        }

        private void OnError(CancellationErrorCode errorCode, string errorMessage)
        {
            ScreenDebugger.Print($"Recognition error: [{errorCode}] {errorMessage}");
            IsUnrecoverable = errorCode == CancellationErrorCode.AuthenticationFailure
                || errorCode == CancellationErrorCode.BadRequest
                || errorCode == CancellationErrorCode.ConnectionFailure
                || errorCode == CancellationErrorCode.Forbidden
                || errorCode == CancellationErrorCode.RuntimeError
                || errorCode == CancellationErrorCode.ServiceUnavailable;

            TearDown();

            OnRecognitionError();
        }

        private void Recognizer_SessionStarted(object sender, SessionEventArgs e)
        {
            recognizer.SessionStarted -= Recognizer_SessionStarted;
            IsStarting = false;
            IsRunning = true;
            OnRecognitionStarted();
        }

        /// <summary>
        /// When speech is recognized, forward it into the keyword recognizer.
        /// </summary>
        /// <param name="args">Arguments.</param>
        void Recognizer_OnPhraseRecognized(object sender, SpeechRecognitionEventArgs args)
        {
            var complete = args.Result.Reason == ResultReason.RecognizedIntent
                || args.Result.Reason == ResultReason.RecognizedSpeech
                || args.Result.Reason == ResultReason.RecognizedKeyword;
            if (complete)
            {
                ProcessText(args.Result.Text, complete);
                OnRecognitionComplete();
            }
        }

        protected void TearDown()
        {
            if (recognizer != null)
            {
                IsStopping = true;
                recognizer.Recognized -= Recognizer_OnPhraseRecognized;
                recognizer.Recognizing -= Recognizer_OnPhraseRecognizing;
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
            OnRecognitionStopped();
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