#if !UNITY_WSA && !UNITY_STANDALONE_WIN && AZURE_SPEECHSDK

using System;
using System.IO;
using System.Linq;
using Juniper.Security;

using Microsoft.CognitiveServices.Speech;

using UnityEngine;
using UnityEngine.Scripting;

namespace Juniper.Speech
{
    public abstract class AzureKeywordRecognizer : AbstractKeywordRecognizer, ICredentialReceiver
    {
#if UNITY_ANDROID && ANDROID_API_23_OR_GREATER
        [Preserve]
        private Microphone mic;

        static AzureKeywordRecognizer()
        {
            Permissions.AndroidPermissionHandler.Add(UnityEngine.Android.Permission.Microphone);
        }
#endif

        public static bool Permitted
        {
            get
            {
#if UNITY_ANDROID && ANDROID_API_23_OR_GREATER
                return Permissions.AndroidPermissionHandler.IsGranted(UnityEngine.Android.Permission.Microphone);
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        public override bool IsAvailable { get { return true; } }

        protected override bool NeedsKeywords { get { return false; } }

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

        private string results;

        private object syncRoot = new object();

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

        /// <summary>
        /// When speech is recognized, forward it into the keyword recognizer.
        /// </summary>
        /// <param name="args">Arguments.</param>
        void Recognizer_OnPhraseRecognized(object sender, SpeechRecognitionEventArgs args)
        {
            if (args.Result.Reason == ResultReason.RecognizedKeyword
                || args.Result.Reason == ResultReason.RecognizedSpeech)
            {
                var text = args.Result.Text.ToLowerInvariant();
                text = new string(text.Where(IsWordChar).ToArray());
                lock (syncRoot)
                {
                    results = text;
                }
            }
        }

        public void Awake()
        {
#if UNITY_EDITOR
            this.ReceiveCredentials();
#endif
        }

        public override void Update()
        {
            base.Update();

            string text = null;
            lock (syncRoot)
            {
                if (!string.IsNullOrEmpty(results))
                {
                    text = results;
                    results = null;
                }
            }

            if (!string.IsNullOrEmpty(text))
            {
                OnKeywordRecognized(text);
            }
        }

        private static bool IsWordChar(char c)
        {
            return char.IsLetter(c)
                || char.IsWhiteSpace(c)
                || char.IsNumber(c);
        }

        protected override void Setup()
        {
            if (Permitted)
            {
                IsStarting = true;
                var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
                config.SetProfanity(ProfanityOption.Raw);
                config.SpeechRecognitionLanguage = "en-us";
                recognizer = new SpeechRecognizer(config);
                recognizer.Recognized += Recognizer_OnPhraseRecognized;
                recognizer.Recognizing += Recognizer_OnPhraseRecognized;
                recognizer.SessionStarted += Recognizer_SessionStarted;
                recognizer.SessionStopped += Recognizer_SessionStopped;
                recognizer.StartContinuousRecognitionAsync();
            }
        }

        private void Recognizer_SessionStarted(object sender, SessionEventArgs e)
        {
            recognizer.SessionStarted -= Recognizer_SessionStarted;
            IsStarting = false;
            IsRunning = true;
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

        private void Recognizer_SessionStopped(object sender, SessionEventArgs e)
        {
            recognizer.SessionStopped -= Recognizer_SessionStopped;
            recognizer.Dispose();
            recognizer = null;
            keywords = null;
            IsStopping = false;
            IsRunning = false;
        }
    }
}

#endif