using System;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.IO;

namespace Juniper.Azure.CognitiveServices
{
    public class TextToSpeechClient : VoicesClient
    {
        private readonly string azureResourceName;
        private readonly OutputFormat outputFormat;
        private readonly IAudioDecoder audioDecoder;

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, ITextDecoder<Voice[]> voiceListDecoder, OutputFormat outputFormat, IAudioDecoder audioDecoder, CachingStrategy cache)
            : base(azureRegion, azureSubscriptionKey, voiceListDecoder, cache)
        {
            if (string.IsNullOrEmpty(azureResourceName))
            {
                throw new ArgumentException("Must provide a resource name that is tied to the subscription", nameof(azureResourceName));
            }

            if (outputFormat == null)
            {
                throw new ArgumentException("Must provide an audio output format", nameof(outputFormat));
            }

            if (audioDecoder == null)
            {
                throw new ArgumentException("Must provide an audio decoder", nameof(audioDecoder));
            }

            if (audioDecoder.ContentType != outputFormat.ContentType)
            {
                throw new ArgumentException($"Must provide a decoder that matches the output format type. Given {audioDecoder.ContentType.Value}. Expected {outputFormat.ContentType.Value}", nameof(audioDecoder));
            }

            this.azureResourceName = azureResourceName;
            this.outputFormat = outputFormat;
            this.audioDecoder = audioDecoder;
        }

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, ITextDecoder<Voice[]> voiceListDecoder, OutputFormat outputFormat, IAudioDecoder audioDecoder)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, audioDecoder, null)
        { }

        public async Task<AudioData> Speak(string text, string voiceName, float rateChange, float pitchChange)
        {
            try
            {
                if (voiceName == null)
                {
                    var voices = await GetVoices();
                    foreach (var voice in voices)
                    {
                        if (voice.Locale == "en-US" && voice.Gender == "Female")
                        {
                            voiceName = voice.ShortName;
                            break;
                        }
                    }
                }

                if (voiceName == null)
                {
                    throw new InvalidOperationException("Could not find a default voice for text to speech commands");
                }

                var ttsRequest = new TextToSpeechRequest(azureRegion, azureResourceName, outputFormat)
                {
                    Text = text,
                    VoiceName = voiceName,
                    RateChange = rateChange,
                    PitchChange = pitchChange
                };

                if (!cache.IsCached(ttsRequest))
                {
                    ttsRequest.AuthToken = await GetAuthToken();
                }

                return await cache.Decode(ttsRequest, audioDecoder);
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public Task<AudioData> Speak(string text, string voiceName, float rateChange)
        {
            return Speak(text, voiceName, rateChange, 0);
        }

        public Task<AudioData> Speak(string text, string voiceName)
        {
            return Speak(text, voiceName, 0, 0);
        }

        public Task<AudioData> Speak(string text)
        {
            return Speak(text, null, 0, 0);
        }
    }
}