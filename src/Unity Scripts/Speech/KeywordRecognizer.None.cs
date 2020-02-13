#if !AZURE_SPEECHSDK && !UNITY_WSA && !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN

using UnityEngine;

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
                return false;
            }
        }

        protected void Setup()
        {
            Debug.LogWarning("No speech recognition available");
        }

        protected void TearDown()
        {
        }
    }
}

#endif