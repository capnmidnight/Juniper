using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Sound;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class TextToSpeechClient : TextToSpeechStreamClient
    {
        private readonly IAudioDecoder audioDecoder;

        public TextToSpeechClient(HttpClient http, string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, IAudioDecoder audioDecoder, CachingStrategy cache)
            : base(http, azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, cache)
        {
            this.audioDecoder = audioDecoder
                ?? throw new ArgumentException("Must provide an audio decoder", nameof(audioDecoder));
        }

        public TextToSpeechClient(HttpClient http, string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, IAudioDecoder audioDecoder)
            : this(http, azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, audioDecoder, null)
        { }

        private void CheckAudioFormat(AudioFormat outputFormat)
        {
            if (audioDecoder.Format != outputFormat)
            {
                if (audioDecoder.SupportsFormat(outputFormat))
                {
                    audioDecoder.Format = outputFormat;
                }
                else
                {
                    throw new NotSupportedException($"The provided audio decoder does not support the given output. Decoder: {audioDecoder.Format.Name}. Expected: {outputFormat.Name}");
                }
            }
        }

        public async Task<AudioData> GetDecodedAudioAsync(AudioFormat outputFormat, string text, string voiceName, float rateChange, float pitchChange)
        {
            CheckAudioFormat(outputFormat);

            var stream = await GetAudioDataStreamAsync(outputFormat, text, voiceName, rateChange, pitchChange)
                .ConfigureAwait(false);

            return audioDecoder.Deserialize(stream);
        }

        public Task<AudioData> GetDecodedAudioAsync(AudioFormat outputFormat, string text, Voice voice, float rateChange, float pitchChange)
        {
            if (voice is null)
            {
                throw new ArgumentNullException(nameof(voice));
            }

            return GetDecodedAudioAsync(outputFormat, text, voice.ShortName, rateChange, pitchChange);
        }

        public Task<AudioData> GetDecodedAudioAsync(AudioFormat outputFormat, string text, string voiceName, float rateChange)
        {
            return GetDecodedAudioAsync(outputFormat, text, voiceName, rateChange, 0);
        }

        public Task<AudioData> GetDecodedAudioAsync(AudioFormat outputFormat, string text, Voice voice, float rateChange)
        {
            return GetDecodedAudioAsync(outputFormat, text, voice, rateChange, 0);
        }

        public Task<AudioData> GetDecodedAudioAsync(AudioFormat outputFormat, string text, string voiceName)
        {
            return GetDecodedAudioAsync(outputFormat, text, voiceName, 0, 0);
        }

        public Task<AudioData> GetDecodedAudioAsync(AudioFormat outputFormat, string text, Voice voice)
        {
            return GetDecodedAudioAsync(outputFormat, text, voice, 0, 0);
        }

        public async Task<Stream> GetWaveAudioAsync(AudioFormat outputFormat, string text, string voiceName, float rateChange, float pitchChange)
        {
            CheckAudioFormat(outputFormat);

            var stream = await GetAudioDataStreamAsync(outputFormat, text, voiceName, rateChange, pitchChange)
                .ConfigureAwait(false);

            return audioDecoder.ToWave(stream);
        }

        public Task<Stream> GetWaveAudioAsync(AudioFormat outputFormat, string text, Voice voice, float rateChange, float pitchChange)
        {
            if (voice is null)
            {
                throw new ArgumentNullException(nameof(voice));
            }

            return GetWaveAudioAsync(outputFormat, text, voice.ShortName, rateChange, pitchChange);
        }

        public Task<Stream> GetWaveAudioAsync(AudioFormat outputFormat, string text, string voiceName, float rateChange)
        {
            return GetWaveAudioAsync(outputFormat, text, voiceName, rateChange, 0);
        }

        public Task<Stream> GetWaveAudioAsync(AudioFormat outputFormat, string text, Voice voice, float rateChange)
        {
            return GetWaveAudioAsync(outputFormat, text, voice, rateChange, 0);
        }

        public Task<Stream> GetWaveAudioAsync(AudioFormat outputFormat, string text, string voiceName)
        {
            return GetWaveAudioAsync(outputFormat, text, voiceName, 0, 0);
        }

        public Task<Stream> GetWaveAudioAsync(AudioFormat outputFormat, string text, Voice voice)
        {
            return GetWaveAudioAsync(outputFormat, text, voice, 0, 0);
        }
    }
}