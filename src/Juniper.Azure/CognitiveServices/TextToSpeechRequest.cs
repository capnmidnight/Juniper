using Juniper.Progress;
using Juniper.Sound;

using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;

using static System.Math;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class TextToSpeechRequest : AbstractAzureSpeechRequest<MediaType.Audio>
    {
        private const string STYLE_SUPPORTED_VOICE = "en-US-Jessa24kRUS";

        public static readonly AudioFormat[] SupportedFormats = {
            new("raw-16khz-16bit-mono-pcm", MediaType.Audio.Raw, 16000, Units.Bits.PER_SHORT, 1),
            new("riff-16khz-16bit-mono-pcm", MediaType.Audio.Wave, 16000, Units.Bits.PER_SHORT, 1),
            new("raw-24khz-16bit-mono-pcm", MediaType.Audio.Raw, 24000, Units.Bits.PER_SHORT, 1),
            new("riff-24khz-16bit-mono-pcm", MediaType.Audio.Wave, 24000, Units.Bits.PER_SHORT, 1),
            new("raw-48khz-16bit-mono-pcm", MediaType.Audio.Raw, 48000, Units.Bits.PER_SHORT, 1),
            new("riff-48khz-16bit-mono-pcm", MediaType.Audio.Wave, 48000, Units.Bits.PER_SHORT, 1),
            new("raw-8khz-8bit-mono-mulaw", MediaType.Audio.Raw, 8000, Units.Bits.PER_BYTE, 1),
            new("riff-8khz-8bit-mono-mulaw", MediaType.Audio.PCMU, 8000, Units.Bits.PER_BYTE, 1),
            new("raw-8khz-8bit-mono-alaw", MediaType.Audio.Raw, 8000, Units.Bits.PER_BYTE, 1),
            new("riff-8khz-8bit-mono-alaw", MediaType.Audio.PCMA, 8000, Units.Bits.PER_BYTE, 1),
            new("audio-16khz-32kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, 32, 1),
            new("audio-16khz-64kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, 32, 1),
            new("audio-16khz-128kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, 32, 1),
            new("audio-24khz-48kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, 32, 1),
            new("audio-24khz-96kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, 32, 1),
            new("audio-24khz-160kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, 32, 1),
            new("audio-48khz-96kbitrate-mono-mp3", MediaType.Audio.Mpeg, 48000, 32, 1),
            new("audio-48khz-192kbitrate-mono-mp3", MediaType.Audio.Mpeg, 48000, 32, 1),
            new("raw-16khz-16bit-mono-truesilk", MediaType.Audio.Silk, 16000, Units.Bits.PER_SHORT, 1),
            new("raw-24khz-16bit-mono-truesilk", MediaType.Audio.Silk, 24000, Units.Bits.PER_SHORT, 1),
            new("webm-16khz-16bit-mono-opus", MediaType.Audio.WebMOpus, 16000, Units.Bits.PER_SHORT, 1),
            new("webm-24khz-16bit-mono-opus", MediaType.Audio.WebMOpus, 24000, Units.Bits.PER_SHORT, 1),
            new("ogg-16khz-16bit-mono-opus", MediaType.Audio.OggOpus, 16000, Units.Bits.PER_SHORT, 1),
            new("ogg-24khz-16bit-mono-opus", MediaType.Audio.OggOpus, 24000, Units.Bits.PER_SHORT, 1),
            new("ogg-48khz-16bit-mono-opus", MediaType.Audio.OggOpus, 48000, Units.Bits.PER_SHORT, 1)
        };

        private static void AddPercentField(StringBuilder sb, string fieldName, float fieldValue, bool addQuotes)
        {
            sb.Append(' ')
              .Append(fieldName)
              .Append('=');

            if (addQuotes)
            {
                sb.Append('\'');
            }

            if (fieldValue > 0)
            {
                sb.Append('+');
            }

            var precent = Units.Proportion.Percent(fieldValue);
            var rounded = Round(precent, 2);
            var value = rounded.ToString("0.00", CultureInfo.InvariantCulture);
            sb.Append(value)
                .Append('%');
            if (addQuotes)
            {
                sb.Append('\'');
            }
        }

        private readonly string resourceName;

        public TextToSpeechRequest(string region, string resourceName, AudioFormat outputFormat)
            : base(HttpMethod.Post, region, "cognitiveservices/v1", outputFormat?.ContentType ?? throw new ArgumentNullException(nameof(outputFormat)))
        {
            this.resourceName = resourceName;
            OutputFormat = outputFormat;
        }

        public AudioFormat OutputFormat { get; }

        public string Text { get; set; }
        public string VoiceName { get; set; }
        public SpeechStyle Style { get; set; }

        private string StyleString =>
            Style.ToString().ToLowerInvariant();

        public float PitchChange { get; set; }
        public float RateChange { get; set; }
        public float VolumeChange { get; set; }

        private bool UseStyle
        {
            get
            {
                return VoiceName == STYLE_SUPPORTED_VOICE
                  && Style != SpeechStyle.None;
            }
        }

        private bool HasPitchChange =>
            Abs(PitchChange) > 0;

        private bool HasRateChange =>
            Abs(RateChange) > 0;

        private bool HasVolumeChange =>
            Abs(VolumeChange) > 0;

        private bool UseProsody
        {
            get
            {
                return HasPitchChange
                    || HasRateChange
                    || HasVolumeChange;
            }
        }

        protected override string InternalCacheID
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(VoiceName)
                  .Append(Text.GetHashCode())
                  .Append(OutputFormat.Name);

                if (UseStyle)
                {
                    sb.Append("style=")
                      .Append(StyleString);
                }

                if (HasPitchChange)
                {
                    AddPercentField(sb, "pitch", PitchChange, false);
                }

                if (HasRateChange)
                {
                    AddPercentField(sb, "rate", RateChange, false);
                }

                if (HasVolumeChange)
                {
                    AddPercentField(sb, "volume", VolumeChange, false);
                }

                return PathExt.FixPath(sb.ToString());
            }
        }

        protected override void ModifyRequest(HttpRequestMessage request, IProgress prog = null)
        {
            base.ModifyRequest(request, prog);
            request.KeepAlive()
                .UserAgent(resourceName);

            var sb = new StringBuilder(300)
                .Append("<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>")
                .Append("<voice name='")
                .Append(VoiceName)
                .Append("'>");

            if (UseProsody)
            {
                sb.Append("<prosody");
                if (HasPitchChange)
                {
                    AddPercentField(sb, "pitch", PitchChange, true);
                }

                if (HasRateChange)
                {
                    AddPercentField(sb, "rate", RateChange, true);
                }

                if (HasVolumeChange)
                {
                    AddPercentField(sb, "volume", VolumeChange, true);
                }

                sb.Append('>');
            }

            if (UseStyle)
            {
                sb.Append("<mstts:express-as type='")
                  .Append(StyleString)
                  .Append("'>");
            }

            sb.Append(Text);

            if (UseStyle)
            {
                sb.Append("</mstts:express-as>");
            }

            if (UseProsody)
            {
                sb.Append("</prosody>");
            }

            sb.Append("</voice>")
              .Append("</speak>");

            request.Header("X-MICROSOFT-OutputFormat", OutputFormat.Name)
                .Body(new StringContent(sb.ToString(), Encoding.UTF8, MediaType.Application.SsmlXml));
        }
    }
}