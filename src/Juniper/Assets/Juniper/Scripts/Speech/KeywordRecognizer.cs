using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Speech
{
    /// <summary>
    /// A class that implements basic functionality for systems that manage speech recognition and
    /// route out the events associated with recognizing keywords.
    /// </summary>
    public partial class KeywordRecognizer : MonoBehaviour, IKeywordRecognizer
    {
#if UNITY_ANDROID && ANDROID_API_23_OR_GREATER && !UNITY_EDITOR
        [UnityEngine.Scripting.Preserve]
        private Microphone mic;

        static KeywordRecognizer()
        {
            Permissions.AndroidPermissionHandler.Add(UnityEngine.Android.Permission.Microphone);
        }

        private static bool IsPermitted
        {
            get
            {
                return Permissions.AndroidPermissionHandler.IsGranted(UnityEngine.Android.Permission.Microphone);
            }
        }

#else
        public const bool IsPermitted = true;
#endif

        private static bool IsWordChar(char c)
        {
            return char.IsLetter(c)
                || char.IsWhiteSpace(c)
                || char.IsNumber(c);
        }

        [Range(0, 1)]
        public float minimumIncompleteSimilarity = 0.85f;

        [Range(0, 1)]
        public float minimumCompleteSimilarity = 0.7f;

        public UnityEvent onRecognitionStarted;
        public UnityEvent onRecognitionStopped;
        public UnityEvent onRecognitionRecognizing;
        public UnityEvent onRecognitionComplete;
        public UnityEvent onRecognitionCanceled;
        public UnityEvent onRecognitionError;

        public event EventHandler RecognitionStarted;
        public event EventHandler RecognitionStopped;
        public event EventHandler RecognitionRecognizing;
        public event EventHandler RecognitionComplete;
        public event EventHandler RecognitionCanceled;
        public event EventHandler RecognitionError;

        protected void OnRecognitionStarted()
        {
            onRecognitionStarted?.Invoke();
            RecognitionStarted?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRecognitionStopped()
        {
            onRecognitionStopped?.Invoke();
            RecognitionStopped?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRecognitionRecognizing()
        {
            onRecognitionRecognizing?.Invoke();
            RecognitionRecognizing?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRecognitionComplete()
        {
            onRecognitionComplete?.Invoke();
            RecognitionComplete?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRecognitionCanceled()
        {
            onRecognitionCanceled?.Invoke();
            RecognitionCanceled?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRecognitionError()
        {
            onRecognitionError?.Invoke();
            RecognitionError?.Invoke(this, EventArgs.Empty);
        }

        protected bool IsRunning;
        protected bool IsStopping;
        protected bool IsStarting;
        protected bool IsPaused;

        private readonly List<Keywordable> keywordables = new List<Keywordable>();
        private DateTime lastMatchTime;
        private Keywordable lastReceiver;
        private Keywordable resultReceiver;
        private readonly object syncRoot = new object();

        public void AddKeywordable(Keywordable keywordable)
        {
            lock (syncRoot)
            {
                keywordables.MaybeAdd(keywordable);
            }
        }

        public void RemoveKeywordable(Keywordable keywordable)
        {
            lock (syncRoot)
            {
                keywordables.Remove(keywordable);
            }
        }

        protected void ProcessText(string text, bool isComplete)
        {
            var resultText = new string(text
                .ToLowerInvariant()
                .Where(IsWordChar)
                .ToArray());

            var maxSimilarity = 0f;
            Keywordable receiver = null;
            string match = null;
            foreach (var keywordable in keywordables)
            {
                foreach (var keyword in keywordable.keywords)
                {
                    var similarity = keyword.Similarity(resultText);
                    if (similarity > maxSimilarity)
                    {
                        match = keyword;
                        maxSimilarity = similarity;
                        receiver = keywordable;
                    }
                }
            }

            ScreenDebugger.Print($"{text} => {resultText} = {match} ({Units.Converter.Label(maxSimilarity, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent)})");

            if (isComplete && maxSimilarity < minimumCompleteSimilarity
                || !isComplete && maxSimilarity < minimumIncompleteSimilarity)
            {
                receiver = null;
            }

            lock (syncRoot)
            {
                resultReceiver = receiver;
            }
        }

        private void TearDownInternal()
        {
            if (IsRunning && !IsStopping)
            {
                TearDown();
            }
        }

        private static readonly TimeSpan DEBOUNCE_TIME = TimeSpan.FromMilliseconds(250);

        public void Update()
        {
            if (IsAvailable)
            {
                if (IsRunning && !IsPaused)
                {
                    Keywordable receiver = null;
                    lock (syncRoot)
                    {
                        if (resultReceiver != null
                            && (resultReceiver != lastReceiver
                                || (DateTime.Now - lastMatchTime) > DEBOUNCE_TIME))
                        {
                            lastReceiver = resultReceiver;
                            receiver = resultReceiver;
                            resultReceiver = null;
                        }
                        lastMatchTime = DateTime.Now;
                    }

                    if (receiver != null)
                    {
                        receiver.ActivateEvent();
                    }
                }
                else if (IsPermitted && !IsStarting)
                {
                    Setup();
                }
            }
        }

        /// <summary>
        /// Tear down the speech recognition system.
        /// </summary>
        public void OnDisable()
        {
            TearDownInternal();
        }
    }
}