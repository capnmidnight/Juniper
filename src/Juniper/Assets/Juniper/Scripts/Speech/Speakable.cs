using System;
using System.Collections;
using System.Threading.Tasks;
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

        private AudioClip clip;

        [SerializeField]
        [HideInNormalInspector]
        private TextComponentWrapper textElement;

        public void Awake()
        {
            Find.Any(out interaction);
        }

        public void Start()
        {
            if (playOnAwake)
            {
                Play();
            }
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

                var clipTask = LoadTask();
                clipTask.ConfigureAwait(false);
            }
        }

        private async Task LoadTask()
        {
            clip = await interaction.PreloadSpeech(text, voiceName, speakingRate - 1, pitch - 1);
        }

        public void Play()
        {
            if (IsAvailable && clip != null)
            {
                var playTask = PlayTask();
                playTask.ConfigureAwait(false);
            }
        }

        private async Task PlayTask()
        {
            KeywordRecognizer.Pause();
            var time = interaction.PlayAudioClip(clip, transform);
            await Task.Delay((int)Units.Seconds.Milliseconds(time));
            OnEnd?.Invoke();
            KeywordRecognizer.Resume();
        }
#endif
    }
}
