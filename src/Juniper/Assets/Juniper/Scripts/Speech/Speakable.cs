using Juniper.Audio;

using UnityEngine;

namespace Juniper.Speech
{
    public class Speakable : MonoBehaviour
    {
#if AZURE_SPEECHSDK

        public bool IsAvailable
        {
            get
            {
                return interaction != null
                    && interaction.IsTextToSpeechAvailable;
            }
        }

        private InteractionAudio interaction;

        public string voiceLanguage;
        public string voiceGender;
        public string voiceName;

        [Range(0.1f, 5f)]
        public float speakingRate = 1;

        [Range(0.1f, 3f)]
        public float pitch = 1;

        public void Awake()
        {
            Find.Any(out interaction);
        }

        public void Speak(string text)
        {
            if (IsAvailable)
            {

                interaction.Speak(transform, text, voiceName, speakingRate - 1, pitch - 1);
            }
        }
#endif
    }
}
