using Juniper.IO;
using Juniper.Sound;

using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.Azure
{
    public interface ISpeechService
    {
        public Task<string> RecognizeAsync(IFormFile fileIn);
        public Task<string> RecognizeAsync(IFormFile fileIn, string language);
        public Task<TempFile> SynthesizeAsync(string voice, string style, string text);
        public Task<SynthesisVoicesResult> GetVoicesAsync();
    }

    public class SpeechService : ISpeechService
    {
        private readonly SpeechConfig speechConfig;

        public SpeechService(string subscriptionKey, string region)
        {
            speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
            speechConfig.SetProfanity(ProfanityOption.Raw);
        }

        public async Task<SynthesisVoicesResult> GetVoicesAsync()
        {
            using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
            return await speechSynthesizer.GetVoicesAsync();
        }

        public Task<string> RecognizeAsync(IFormFile fileIn) =>
            RecognizeAsync(fileIn, null);

        public async Task<string> RecognizeAsync(IFormFile fileIn, string language)
        {
            if (fileIn is null)
            {
                throw new FileNotFoundException();
            }

            var mediaTypeIn = MediaType.Parse(fileIn.ContentType);
            using var streamIn = fileIn.OpenReadStream();
            using var fileOut = await EZFFMPEG.ConvertAsync(streamIn, mediaTypeIn, EZFFMPEGFormat.Wav);

            speechConfig.SpeechRecognitionLanguage = language;
            using var audioConfig = AudioConfig.FromWavFileInput(fileOut.FilePath);
            using var recog = new SpeechRecognizer(speechConfig, audioConfig);

            var recogResult = await recog.RecognizeOnceAsync();
            var text = recogResult.Text;
            return text;
        }

        public async Task<TempFile> SynthesizeAsync(string voice, string style, string text)
        {
            if (string.IsNullOrEmpty(voice))
            {
                throw new ArgumentException($"'{nameof(voice)}' cannot be null or empty.", nameof(voice));
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz160KBitRateMonoMp3);
            var file = new TempFile(MediaType.Audio_Mpeg);

            using (var audioConfig = AudioConfig.FromWavFileOutput(file.FilePath))
            {
                using var synth = new SpeechSynthesizer(speechConfig, audioConfig);
                using var synthResult = await synth.SpeakSsmlAsync(new SsmlDocument
                {
                    VoiceName = voice,
                    Style = style,
                    Text = text
                }.ToString());
                if (synthResult.Reason != ResultReason.SynthesizingAudioCompleted)
                {
                    throw new InvalidOperationException("Couldn't synthesize speech");
                }
            }

            return file;
        }
    }
}