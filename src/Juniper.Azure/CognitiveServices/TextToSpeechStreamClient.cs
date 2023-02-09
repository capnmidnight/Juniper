using Juniper.IO;
using Juniper.Sound;

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class TextToSpeechStreamClient : VoicesClient
    {
        public static readonly AudioFormat[] SupportedFormats = {
            new("raw-16khz-16bit-mono-pcm", MediaType.Audio_Raw, 16000, Units.Bits.PER_SHORT, 1),
            new("riff-16khz-16bit-mono-pcm", MediaType.Audio_Wave, 16000, Units.Bits.PER_SHORT, 1),
            new("raw-24khz-16bit-mono-pcm", MediaType.Audio_Raw, 24000, Units.Bits.PER_SHORT, 1),
            new("riff-24khz-16bit-mono-pcm", MediaType.Audio_Wave, 24000, Units.Bits.PER_SHORT, 1),
            new("raw-48khz-16bit-mono-pcm", MediaType.Audio_Raw, 48000, Units.Bits.PER_SHORT, 1),
            new("riff-48khz-16bit-mono-pcm", MediaType.Audio_Wave, 48000, Units.Bits.PER_SHORT, 1),
            new("raw-8khz-8bit-mono-mulaw", MediaType.Audio_Raw, 8000, Units.Bits.PER_BYTE, 1),
            new("riff-8khz-8bit-mono-mulaw", MediaType.Audio_PCMU, 8000, Units.Bits.PER_BYTE, 1),
            new("raw-8khz-8bit-mono-alaw", MediaType.Audio_Raw, 8000, Units.Bits.PER_BYTE, 1),
            new("riff-8khz-8bit-mono-alaw", MediaType.Audio_PCMA, 8000, Units.Bits.PER_BYTE, 1),
            new("audio-16khz-32kbitrate-mono-mp3", MediaType.Audio_Mpeg, 16000, 32, 1),
            new("audio-16khz-64kbitrate-mono-mp3", MediaType.Audio_Mpeg, 16000, 32, 1),
            new("audio-16khz-128kbitrate-mono-mp3", MediaType.Audio_Mpeg, 16000, 32, 1),
            new("audio-24khz-48kbitrate-mono-mp3", MediaType.Audio_Mpeg, 24000, 32, 1),
            new("audio-24khz-96kbitrate-mono-mp3", MediaType.Audio_Mpeg, 24000, 32, 1),
            new("audio-24khz-160kbitrate-mono-mp3", MediaType.Audio_Mpeg, 24000, 32, 1),
            new("audio-48khz-96kbitrate-mono-mp3", MediaType.Audio_Mpeg, 48000, 32, 1),
            new("audio-48khz-192kbitrate-mono-mp3", MediaType.Audio_Mpeg, 48000, 32, 1),
            new("raw-16khz-16bit-mono-truesilk", MediaType.Audio_Silk, 16000, Units.Bits.PER_SHORT, 1),
            new("raw-24khz-16bit-mono-truesilk", MediaType.Audio_Silk, 24000, Units.Bits.PER_SHORT, 1),
            new("webm-16khz-16bit-mono-opus", MediaType.Audio_WebMOpus, 16000, Units.Bits.PER_SHORT, 1),
            new("webm-24khz-16bit-mono-opus", MediaType.Audio_WebMOpus, 24000, Units.Bits.PER_SHORT, 1),
            new("ogg-16khz-16bit-mono-opus", MediaType.Audio_OggOpus, 16000, Units.Bits.PER_SHORT, 1),
            new("ogg-24khz-16bit-mono-opus", MediaType.Audio_OggOpus, 24000, Units.Bits.PER_SHORT, 1),
            new("ogg-48khz-16bit-mono-opus", MediaType.Audio_OggOpus, 48000, Units.Bits.PER_SHORT, 1)
        };


        private readonly string azureResourceName;

        public TextToSpeechStreamClient(HttpClient http, string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, CachingStrategy cache)
            : base(http, azureRegion, azureSubscriptionKey, voiceListDecoder, cache)
        {
            if (string.IsNullOrEmpty(azureResourceName))
            {
                throw new ArgumentException("Must provide a resource name that is tied to the subscription", nameof(azureResourceName));
            }

            this.azureResourceName = azureResourceName;
        }

        public TextToSpeechStreamClient(HttpClient http, string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder)
            : this(http, azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, null)
        { }

        public async Task<Stream> GetAudioDataStreamAsync(AudioFormat outputFormat, string text, string voiceName, float rateChange = 0, float pitchChange = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Must provide some text to generate speech.", nameof(text));
            }

            if (string.IsNullOrEmpty(voiceName))
            {
                throw new ArgumentException("Must provide a voice to generate speech.", nameof(voiceName));
            }

            try
            {
                var ttsRequest = new TextToSpeechRequest(Http, AzureRegion, azureResourceName, outputFormat)
                {
                    Text = text,
                    VoiceName = voiceName
                };

                if (!Cache.IsCached(ttsRequest))
                {
                    ttsRequest.AuthToken = await GetAuthTokenAsync()
                        .ConfigureAwait(false);
                }

                return await Cache.OpenAsync(ttsRequest)
                    .ConfigureAwait(false);
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public Task<Stream> GetAudioDataStreamAsync(AudioFormat outputFormat, string text, Voice voice, float rateChange = 0, float pitchChange = 0)
        {
            return GetAudioDataStreamAsync(outputFormat, text, voice?.ShortName, rateChange, pitchChange);
        }
    }
}