using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.IO;

namespace Juniper.Azure.CognitiveServices
{
    public class TextToSpeechStreamClient : VoicesClient
    {
        private readonly string azureResourceName;

        public TextToSpeechStreamClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, AudioFormat outputFormat, CachingStrategy cache)
            : base(azureRegion, azureSubscriptionKey, voiceListDecoder, cache)
        {
            if (string.IsNullOrEmpty(azureResourceName))
            {
                throw new ArgumentException("Must provide a resource name that is tied to the subscription", nameof(azureResourceName));
            }

            this.azureResourceName = azureResourceName;

            OutputFormat = outputFormat
                ?? throw new ArgumentException("Must provide an audio output format", nameof(outputFormat));
        }

        public virtual AudioFormat OutputFormat
        {
            get;
            set;
        }

        public TextToSpeechStreamClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, AudioFormat outputFormat)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, null)
        { }

        public async Task<Stream> GetAudioDataStream(string text, string voiceName, float rateChange, float pitchChange)
        {
            try
            {
                var ttsRequest = new TextToSpeechRequest(azureRegion, azureResourceName, OutputFormat)
                {
                    Text = text,
                    VoiceName = voiceName,
                    RateChange = rateChange,
                    PitchChange = pitchChange
                };

                if (!cache.IsCached(ttsRequest))
                {
                    ttsRequest.AuthToken = await GetAuthToken()
                        .ConfigureAwait(false);
                }

                return await cache.Open(ttsRequest)
                    .ConfigureAwait(false);
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public Task<Stream> GetAudioDataStream(string text, Voice voice, float rateChange, float pitchChange)
        {
            return GetAudioDataStream(text, voice.ShortName, rateChange, pitchChange);
        }

        public Task<Stream> GetAudioDataStream(string text, string voiceName, float rateChange)
        {
            return GetAudioDataStream(text, voiceName, rateChange, 0);
        }

        public Task<Stream> GetAudioDataStream(string text, Voice voice, float rateChange)
        {
            return GetAudioDataStream(text, voice, rateChange, 0);
        }

        public Task<Stream> GetAudioDataStream(string text, string voiceName)
        {
            return GetAudioDataStream(text, voiceName, 0, 0);
        }

        public Task<Stream> GetAudioDataStream(string text, Voice voice)
        {
            return GetAudioDataStream(text, voice, 0, 0);
        }
    }

    public class TextToSpeechClient : TextToSpeechStreamClient
    {
        private readonly IAudioDecoder audioDecoder;

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, AudioFormat outputFormat, IAudioDecoder audioDecoder, CachingStrategy cache)
            : base(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, cache)
        {
            this.audioDecoder = audioDecoder
                ?? throw new ArgumentException("Must provide an audio decoder", nameof(audioDecoder));

            CheckDecoderFormat();
        }

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, AudioFormat outputFormat, IAudioDecoder audioDecoder)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, audioDecoder, null)
        { }

        public override AudioFormat OutputFormat
        {
            get
            {
                return base.OutputFormat;
            }

            set
            {
                base.OutputFormat = value;

                if (audioDecoder != null)
                {
                    CheckDecoderFormat();
                }
            }
        }

        private void CheckDecoderFormat()
        {
            if (audioDecoder.Format != OutputFormat)
            {
                if (audioDecoder.SupportsFormat(OutputFormat))
                {
                    audioDecoder.Format = OutputFormat;
                }
                else
                {
                    throw new ArgumentException($"The provided audio decoder does not support the given output. Decoder: {audioDecoder.Format.Name}. Expected: {OutputFormat.Name}", nameof(audioDecoder));
                }
            }
        }

        public async Task<AudioData> GetDecodedAudio(string text, string voiceName, float rateChange, float pitchChange)
        {
            var stream = await GetAudioDataStream(text, voiceName, rateChange, pitchChange)
                .ConfigureAwait(false);
            return audioDecoder.Deserialize(stream);
        }

        public Task<AudioData> GetDecodedAudio(string text, Voice voice, float rateChange, float pitchChange)
        {
            return GetDecodedAudio(text, voice.ShortName, rateChange, pitchChange);
        }

        public Task<AudioData> GetDecodedAudio(string text, string voiceName, float rateChange)
        {
            return GetDecodedAudio(text, voiceName, rateChange, 0);
        }

        public Task<AudioData> GetDecodedAudio(string text, Voice voice, float rateChange)
        {
            return GetDecodedAudio(text, voice, rateChange, 0);
        }

        public Task<AudioData> GetDecodedAudio(string text, string voiceName)
        {
            return GetDecodedAudio(text, voiceName, 0, 0);
        }

        public Task<AudioData> GetDecodedAudio(string text, Voice voice)
        {
            return GetDecodedAudio(text, voice, 0, 0);
        }
    }
}