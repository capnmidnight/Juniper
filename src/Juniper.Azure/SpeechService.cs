// Ignore Spelling: Viseme

using Juniper.IO;
using Juniper.Sound;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Azure;

public record RecognitionResult(string Culture, string Text);
public record Viseme(uint ID, float Offset);
public record SynthesisResult(TempFile File, Viseme[] Visemes);

public class SpeechServiceOptions
{
    public const string SpeechService = "Azure:Speech";
    public string SubscriptionKey { get; set; } = null;
    public string Region { get; set; } = null;
}

public interface ISpeechService
{
    public Task<RecognitionResult> RecognizeAsync(IFormFile fileIn, string speakerCulture, string targetCulture);
    public Task<SynthesisResult> SynthesizeAsync(string voice, string style, string text, EZFFMPEGFormat format = EZFFMPEGFormat.WebMOpus);
    public Task<SynthesisVoicesResult> GetVoicesAsync();
    public string SubscriptionKey { get; }
    public string Region { get; }
}

public static class SpeechServiceConfiguration
{
    public static WebApplicationBuilder ConfigureJuniperSpeechService(this WebApplicationBuilder appBuilder)
    {
        var configSection = appBuilder.Configuration.GetSection(SpeechServiceOptions.SpeechService);
        appBuilder.Services.Configure<SpeechServiceOptions>(configSection);

        appBuilder.Services
            // Give DI the class it needs to create
            .AddSingleton<SpeechService>()
            // Give DI an alias that other DI consumers can use to request the service without
            // knowing the specific type of `AppShellWindowFactorT`
            .AddSingleton<ISpeechService>(serviceProvider =>
                serviceProvider.GetRequiredService<SpeechService>());

        return appBuilder;
    }
}

public class SpeechService : ISpeechService
{
    private readonly SpeechConfig speechConfig;

    public SpeechService(IOptions<SpeechServiceOptions> options)
    {
        var subscriptionKey = options.Value.SubscriptionKey;
        var region = options.Value.Region;
        speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        speechConfig.SetProfanity(ProfanityOption.Raw);
    }

    public string SubscriptionKey => speechConfig.SubscriptionKey;
    public string Region => speechConfig.Region;

    public async Task<SynthesisVoicesResult> GetVoicesAsync()
    {
        using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
        return await speechSynthesizer.GetVoicesAsync();
    }

    public async Task<RecognitionResult> RecognizeAsync(IFormFile fileIn, string speakerCulture, string targetCulture)
    {
        if (fileIn is null)
        {
            throw new FileNotFoundException();
        }

        var mediaTypeIn = MediaType.Parse(fileIn.ContentType);
        using var streamIn = fileIn.OpenReadStream();
        using var fileOut = await EZFFMPEG.ConvertAsync(streamIn, mediaTypeIn, EZFFMPEGFormat.Wav);

        var languages = new string[] { "en-US", speakerCulture, targetCulture }
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToArray();
        var detectLangConfig = AutoDetectSourceLanguageConfig.FromLanguages(languages);
        using var audioConfig = AudioConfig.FromWavFileInput(fileOut.FilePath);
        using var recog = new SpeechRecognizer(speechConfig, detectLangConfig, audioConfig);

        var recogResult = await recog.RecognizeOnceAsync();
        var detectLangResult = AutoDetectSourceLanguageResult.FromResult(recogResult);
        return new RecognitionResult(detectLangResult.Language, recogResult.Text);
    }

    public async Task<SynthesisResult> SynthesizeAsync(string voice, string style, string text, EZFFMPEGFormat format = EZFFMPEGFormat.WebMOpus)
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

        var file = new TempFile(MediaType.Audio_WebMOpus);
        if (format != EZFFMPEGFormat.WebMOpus)
        {
            var newFile = await EZFFMPEG.ConvertAsync(file.FileInfo, format);
            file.Dispose();
            file = newFile;
        }

        var visemes = new List<Viseme>();
        using (var audioConfig = AudioConfig.FromWavFileOutput(file.FilePath))
        {
            using var synth = new SpeechSynthesizer(speechConfig, audioConfig);

            synth.VisemeReceived += (object sender, SpeechSynthesisVisemeEventArgs e) =>
                visemes.Add(new Viseme(e.VisemeId, e.AudioOffset / 10000000f));

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

        return new SynthesisResult(file, visemes.ToArray());
    }
}