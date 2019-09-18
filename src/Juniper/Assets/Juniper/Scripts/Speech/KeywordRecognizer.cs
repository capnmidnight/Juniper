using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Juniper.Speech
{
    public class KeywordRecognizer :
#if UNITY_WSA || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        WindowsKeywordRecognizer
#elif UNITY_ANDROID && AZURE_SPEECHSDK
        AzureKeywordRecognizer
#else
        NoKeywordRecognizer
#endif
    {
    }

    /// <summary>
    /// A class that implements basic functionality for systems that manage speech recognition and
    /// route out the events associated with recognizing keywords.
    /// </summary>
    public abstract class AbstractKeywordRecognizer : MonoBehaviour, IKeywordRecognizer
    {
#if UNITY_ANDROID && ANDROID_API_23_OR_GREATER && !UNITY_EDITOR
        [Preserve]
        private Microphone mic;

        static AbstractKeywordRecognizer()
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
        public float minimumSimilarity = 0.7f;

        protected bool IsRunning;
        protected bool IsStopping;
        protected bool IsStarting;
        protected bool IsUnrecoverable;

        private readonly List<Keywordable> keywordables = new List<Keywordable>();
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

        protected void ProcessText(string text)
        {
            lock (syncRoot)
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

                if (maxSimilarity < minimumSimilarity)
                {
                    receiver = null;
                }

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

        public void Update()
        {
            if (IsAvailable)
            {
                if (IsRunning)
                {
                    Keywordable receiver = null;
                    lock (syncRoot)
                    {
                        if (resultReceiver != null)
                        {
                            receiver = resultReceiver;
                            resultReceiver = null;
                        }
                    }

                    if (receiver != null)
                    {
                        receiver.OnKeywordRecognized();
                    }
                }
                else if (IsPermitted && !IsUnrecoverable && !IsStarting)
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

        public abstract bool IsAvailable { get; }

        protected abstract void Setup();

        protected abstract void TearDown();
    }
}