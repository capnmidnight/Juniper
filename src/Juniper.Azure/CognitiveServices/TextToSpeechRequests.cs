using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

using Juniper.HTTP;
using Juniper.Sound;

using static System.Math;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class TextToSpeechRequest : AbstractAzureSpeechRequest<MediaType.Audio>
    {
        private const string STYLE_SUPPORTED_VOICE = "en-US-Jessa24kRUS";

        private static void AddPercentField(StringBuilder sb, string fieldName, float fieldValue, bool addQuotes)
        {
            _ = sb.Append(' ')
              .Append(fieldName)
              .Append('=');

            if (addQuotes)
            {
                _ = sb.Append('\'');
            }

            if (fieldValue > 0)
            {
                _ = sb.Append('+');
            }

            var precent = Units.Proportion.Percent(fieldValue);
            var rounded = Round(precent, 2);
            var value = rounded.ToString("0.00", CultureInfo.InvariantCulture);
            _ = sb.Append(value)
                .Append('%');
            if (addQuotes)
            {
                _ = sb.Append('\'');
            }
        }

        private readonly string resourceName;
        private string ssmlText;
        private int ssmlTextLength;

        public TextToSpeechRequest(string region, string resourceName, AudioFormat outputFormat)
            : base(HttpMethods.POST, region, "cognitiveservices/v1", outputFormat?.ContentType ?? throw new ArgumentNullException(nameof(outputFormat)), true)
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

                _ = sb.Append(VoiceName)
                  .Append(Text.GetHashCode())
                  .Append(OutputFormat.Name);

                if (UseStyle)
                {
                    _ = sb.Append("style=")
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

        protected override BodyInfo GetBodyInfo()
        {
            var sb = new StringBuilder(300)
                .Append("<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>")
                .Append("<voice name='")
                .Append(VoiceName)
                .Append("'>");

            if (UseProsody)
            {
                _ = sb.Append("<prosody");
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

                _ = sb.Append('>');
            }

            if (UseStyle)
            {
                _ = sb.Append("<mstts:express-as type='")
                  .Append(StyleString)
                  .Append("'>");
            }

            _ = sb.Append(Text);

            if (UseStyle)
            {
                _ = sb.Append("</mstts:express-as>");
            }

            if (UseProsody)
            {
                _ = sb.Append("</prosody>");
            }

            _ = sb.Append("</voice>")
              .Append("</speak>");

            ssmlText = sb.ToString();
            ssmlTextLength = Encoding.UTF8.GetByteCount(ssmlText);
            return new BodyInfo(Juniper.MediaType.Application.SsmlXml, ssmlTextLength);
        }

        protected override void WriteBody(Stream stream)
        {
            using var writer = new StreamWriter(stream);
            writer.Write(ssmlText);
        }

        protected override void ModifyRequest(HttpWebRequest request)
        {
            base.ModifyRequest(request);
            _ = request.KeepAlive()
                .UserAgent(resourceName)
                .Header("X-Microsoft-OutputFormat", OutputFormat.Name);
        }
    }
}
