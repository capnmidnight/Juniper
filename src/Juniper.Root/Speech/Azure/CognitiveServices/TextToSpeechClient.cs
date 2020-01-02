using System;
using System.Threading.Tasks;
using Juniper.IO;
using Juniper.Sound;

namespace Juniper.Speech.Azure.CognitiveServices
{
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
            get { return base.OutputFormat; }

            set
            {
                base.OutputFormat = value;

                if (audioDecoder is object)
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
                    throw new NotSupportedException($"The provided audio decoder does not support the given output. Decoder: {audioDecoder.Format.Name}. Expected: {OutputFormat.Name}");
                }
            }
        }

        public async Task<AudioData> GetDecodedAudioAsync(string text, string voiceName, float rateChange, float pitchChange)
        {
            var stream = await GetAudioDataStreamAsync(text, voiceName, rateChange, pitchChange)
                .ConfigureAwait(false);
            return audioDecoder.Deserialize(stream);
        }

        public Task<AudioData> GetDecodedAudioAsync(string text, Voice voice, float rateChange, float pitchChange)
        {
            return GetDecodedAudioAsync(text, voice.ShortName, rateChange, pitchChange);
        }

        public Task<AudioData> GetDecodedAudioAsync(string text, string voiceName, float rateChange)
        {
            return GetDecodedAudioAsync(text, voiceName, rateChange, 0);
        }

        public Task<AudioData> GetDecodedAudioAsync(string text, Voice voice, float rateChange)
        {
            return GetDecodedAudioAsync(text, voice, rateChange, 0);
        }

        public Task<AudioData> GetDecodedAudioAsync(string text, string voiceName)
        {
            return GetDecodedAudioAsync(text, voiceName, 0, 0);
        }

        public Task<AudioData> GetDecodedAudioAsync(string text, Voice voice)
        {
            return GetDecodedAudioAsync(text, voice, 0, 0);
        }
    }
}