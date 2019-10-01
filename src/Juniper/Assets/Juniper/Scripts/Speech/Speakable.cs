using System;
using System.Collections;
using System.Threading.Tasks;
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

        public string text;
        private string lastText;

        public string voiceLanguage;
        public string voiceGender;
        public string voiceName;
        private string lastVoiceName;

        [Range(0.1f, 5f)]
        public float speakingRate = 1;
        private float lastSpeakingRate;

        [Range(0.1f, 3f)]
        public float pitch = 1;
        private float lastPitch;

        private AudioClip clip;

        private KeywordRecognizer recognizer;

        public void Awake()
        {
            Find.Any(out interaction);
            Find.Any(out recognizer);
        }

        private void Update()
        {
            Preload();
        }

        private void Preload()
        {
            if (text != lastText
                || voiceName != lastVoiceName
                || speakingRate != lastSpeakingRate
                || pitch != lastPitch)
            {
                lastText = text;
                lastVoiceName = voiceName;
                lastSpeakingRate = speakingRate;
                lastPitch = pitch;
                print("Loading speech " + text);
                StartCoroutine(PreloadCoroutine());
            }
        }

        private IEnumerator PreloadCoroutine()
        {
            var clipTask = interaction.PreloadSpeech(text, voiceName, speakingRate - 1, pitch - 1);
            yield return clipTask.AsCoroutine();
            clip = clipTask.Result;
        }

        private Coroutine lastCoroutine;
        public void Speak()
        {
            if (IsAvailable && clip != null)
            {
                if (lastCoroutine != null) {
                    StopCoroutine(lastCoroutine);
                }

                lastCoroutine = StartCoroutine(SpeakCoroutine());

            }
        }

        private IEnumerator SpeakCoroutine()
        {
            var recognizerEnabled = recognizer.isActiveAndEnabled;
            recognizer.enabled = false;
            var time = interaction.PlayAudioClip(clip, transform);
            var end = DateTime.Now.AddSeconds(time);
            while(DateTime.Now < end)
            {
                yield return null;
            }
            recognizer.enabled = recognizerEnabled;
        }
#endif
    }
}
