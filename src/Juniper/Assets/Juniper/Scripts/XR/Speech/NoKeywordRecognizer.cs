using UnityEngine;

namespace Juniper.Speech
{
    public abstract class NoKeywordRecognizer : AbstractKeywordRecognizer
    {
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        public const bool IsAvailable = false;

        protected override void Setup()
        {
            Debug.LogWarning("No speech recognition available");
        }

        protected override void TearDown()
        {
        }
    }
}
