using System;
using System.Collections;

using Juniper.Audio;
using Juniper.Widgets;

using UnityEngine;
using UnityEngine.Events;

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

        public bool playOnAwake;
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

        public UnityEvent OnEnd;

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

        public void Preload()
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
                if (clip != null)
                {
                    clip.DestroyImmediate();
                    clip = null;
                }
                var clipTask = interaction.PreloadSpeech(text, voiceName, speakingRate - 1, pitch - 1)
                    .ContinueWith(ct => clip = ct.Result);
                clipTask.ConfigureAwait(false);
            }
            else if (needsPlay && clip != null)
            {
                needsPlay = false;
                PlayInternal();
            }
        }

        public void Play()
        {
            if(clip == null)
            {
                needsPlay = true;
            }
            else
            {
                PlayInternal();
            }
        }

        private Coroutine lastCoroutine;

        private void PlayInternal()
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
            OnEnd?.Invoke();
            KeywordRecognizer.Resume();
        }
#endif
    }
}
