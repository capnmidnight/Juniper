using System;
using System.Collections;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.Widgets;

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

        public bool playOnAwake;
        private bool needsPlay;

        private AudioClip clip;

        [SerializeField]
        [HideInNormalInspector]
        private TextComponentWrapper textElement;

        public void Awake()
        {
            Find.Any(out interaction);
            needsPlay = playOnAwake;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (textElement == null)
            {
                textElement = GetComponent<TextComponentWrapper>();
            }

            if (textElement != null && string.IsNullOrEmpty(text))
            {
                text = textElement.Text;
            }
        }
#endif

        private void Update()
        {
            Preload();
        }

        private void Preload()
        {
            if (textElement != null && text != textElement.Text)
            {
                text = textElement.Text;
            }

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
                clip = null;
                this.Run(PreloadCoroutine());
            }
            else if (needsPlay && clip != null)
            {
                needsPlay = false;
                Play();
            }
        }

        private IEnumerator PreloadCoroutine()
        {
            var clipTask = interaction.PreloadSpeech(text, voiceName, speakingRate - 1, pitch - 1);
            yield return clipTask.AsCoroutine();
            clip = clipTask.Result;
        }

        private Coroutine lastCoroutine;
        public void Play()
        {
            if (IsAvailable && clip != null)
            {
                if (lastCoroutine != null)
                {
                    StopCoroutine(lastCoroutine);
                }

                lastCoroutine = (Coroutine)this.Run(PlayCoroutine());
            }
        }

        private IEnumerator PlayCoroutine()
        {
            KeywordRecognizer.Pause();
            var time = interaction.PlayAudioClip(clip, transform);
            var end = DateTime.Now.AddSeconds(time);
            while (DateTime.Now < end)
            {
                yield return null;
            }
            KeywordRecognizer.Resume();
        }
#endif
    }
}
