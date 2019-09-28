using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.Audio.NAudio;
using Juniper.Azure.CognitiveServices;
using Juniper.HTTP;
using Juniper.Security;
using Juniper.Serialization;

using UnityEngine;

namespace Juniper.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public class SpeechOutput : MonoBehaviour, ICredentialReceiver
    {
#if AZURE_SPEECHSDK
        /// <summary>
        /// Reads as true if the current XR subsystem supports speech recognition.
        /// </summary>
        private readonly bool IsUnrecoverable;
        public bool IsAvailable
        {
            get
            {
                return !IsUnrecoverable;
            }
        }

        /// <summary>
        /// The real recognizer.
        /// </summary>
        private SpeechRequest requester;

        [SerializeField]
        [HideInNormalInspector]
        private string azureApiKey;

        [SerializeField]
        [HideInNormalInspector]
        private string azureRegion;

        [SerializeField]
        [HideInInspector]
        private AudioSource aud;

#if UNITY_EDITOR
        private void OnValidate()
        {
            aud = GetComponent<AudioSource>();
            if (aud != null)
            {
                aud.playOnAwake = false;
                aud.loop = false;
            }
        }
#endif

        public string voiceLanguage;
        public string voiceGender;
        public string voiceName;

        private IDeserializer<AudioData> decoder;

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

        public void Awake()
        {
#if UNITY_EDITOR
            this.ReceiveCredentials();
#endif

            if (aud == null)
            {
                aud = GetComponent<AudioSource>();
            }

            if (aud != null)
            {
                aud.playOnAwake = false;
                aud.loop = false;
                if (!string.IsNullOrEmpty(azureApiKey)
                    && !string.IsNullOrEmpty(azureRegion)
                    && !string.IsNullOrEmpty(voiceName))
                {
                    decoder = new NAudioAudioDataDecoder(MediaType.Audio.Mpeg);

#if UNITY_EDITOR
                    var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    var cacheDirName = Path.Combine(userProfile, "Projects");
#else
                    var cacheDirName = Application.persistentDataPath;
#endif
                    var cacheLocation = new DirectoryInfo(cacheDirName);
                    var plainText = new StreamStringDecoder();
                    var voiceJson = new JsonFactory<Voice[]>();
                    var authRequest = new Azure.AuthTokenRequest(azureRegion, azureApiKey);
                    authRequest.PostForDecoded(plainText).ContinueWith(async tT =>
                    {
                        var authToken = tT.Result;
                        var voiceRequest = new VoiceListRequest(azureRegion, authToken, cacheLocation);
                        var voices = await voiceRequest.GetDecoded(voiceJson);
                        var voice = voices.FirstOrDefault(v => v.ShortName == voiceName);

                        requester = new SpeechRequest(
                            azureRegion,
                            authToken,
                            "dls-dev-speech-recognition",
                            OutputFormat.Audio24KHz160KbitrateMonoMP3,
                            cacheLocation)
                        {
                            Voice = voice
                        };
                    }).ConfigureAwait(false);
                }
            }
        }

        private bool speechLocked;
        public void Speak(string text)
        {
            if (requester != null && !speechLocked)
            {
                speechLocked = true;
                StartCoroutine(SpeakCoroutine(text));
            }
        }

        public IEnumerator SpeakCoroutine(string text)
        {
            print("Getting speech");
            CleanupLastAudio();

            requester.Text = text;
            var audioTask = requester.PostForDecoded(decoder);
            yield return audioTask.AsCoroutine();
            var audioData = audioTask.Result;

            var clip = AudioClip.Create(
                text,
                audioData.samplesPerChannel,
                audioData.numChannels,
                audioData.frequency,
                false);

            clip.SetData(audioData.data, 0);
            clip.LoadAudioData();

            aud.playOnAwake = false;
            aud.loop = false;
            aud.clip = clip;
            aud.Play();
            speechLocked = false;
        }

        private void CleanupLastAudio()
        {
            if (aud.clip != null)
            {
                aud.Stop();
                aud.clip = null;
            }
        }

        private void OnDestroy()
        {
            CleanupLastAudio();
        }
#endif
    }
}
