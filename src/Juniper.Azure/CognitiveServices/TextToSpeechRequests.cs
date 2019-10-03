using System;
using System.IO;
using System.Net;
using System.Text;
using Juniper.Audio;
using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public class TextToSpeechRequest : AbstractAzureSpeechRequest<MediaType.Audio>
    {
        private const string STYLE_SUPPORTED_VOICE = "en-US-Jessa24kRUS";

        private static void AddPercentField(StringBuilder sb, string fieldName, float fieldValue, bool addQuotes)
        {
            sb.Append(' ');
            sb.Append(fieldName);
            sb.Append('=');
            if (addQuotes)
            {
                sb.Append('\'');
            }
            if (fieldValue > 0)
            {
                sb.Append('+');
            }
            var precent = Units.Proportion.Percent(fieldValue);
            var rounded = Math.Round(precent, 2);
            var value = rounded.ToString("0.00");
            sb.Append(value);
            sb.Append("%");
            if (addQuotes)
            {
                sb.Append('\'');
            }
        }

        private readonly string resourceName;

        private readonly AudioFormat outputFormat;

        private string ssmlText;
        private int ssmlTextLength;

        public TextToSpeechRequest(string region, string resourceName, AudioFormat outputFormat)
            : base(region, "cognitiveservices/v1", outputFormat.ContentType)
        {
            this.resourceName = resourceName;
            this.outputFormat = outputFormat;
        }

        public string Text { get; set; }
        public string VoiceName { get; set; }
        public SpeechStyle Style { get; set; }
        private string StyleString
        {
            get
            {
                return Style.ToString().ToLowerInvariant();
            }
        }
        public float PitchChange { get; set; }
        public float RateChange { get; set; }
        public float VolumeChange { get; set; }

        private bool UseStyle
        {
            get
            {
                return VoiceName == STYLE_SUPPORTED_VOICE && Style != SpeechStyle.None;
            }
        }

        private bool HasPitchChange
        {
            get
            {
                return Math.Abs(PitchChange) > 0;
            }
        }

        private bool HasRateChange
        {
            get
            {
                return Math.Abs(RateChange) > 0;
            }
        }

        private bool HasVolumeChange
        {
            get
            {
                return Math.Abs(VolumeChange) > 0;
            }
        }

        private bool UseProsody
        {
            get
            {
                return HasPitchChange || HasRateChange || HasVolumeChange;
            }
        }

        public override string CacheID
        {
            get
            {
                var sb = new StringBuilder(base.CacheID);

                sb.Append(VoiceName);
                sb.Append(Text.GetHashCode());
                sb.Append(outputFormat.Name);

                if (UseStyle)
                {
                    sb.Append("style=");
                    sb.Append(StyleString);
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

                return sb.ToString();
            }
        }

        protected override BodyInfo GetBodyInfo()
        {
            var sb = new StringBuilder(300);
            sb.Append("<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>");
            sb.Append($"<voice name='{VoiceName}'>");

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
                sb.Append(">");
            }

            if (UseStyle)
            {
                sb.Append($"<mstts:express-as type='{StyleString}'>");
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
            sb.Append("</voice>");
            sb.Append("</speak>");

            ssmlText = sb.ToString();
            ssmlTextLength = System.Text.Encoding.UTF8.GetByteCount(ssmlText);
            return new BodyInfo(MediaType.Application.SsmlXml, ssmlTextLength);
        }

        protected override void WriteBody(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(ssmlText);
            }
        }

        protected override void ModifyRequest(HttpWebRequest request)
        {
            base.ModifyRequest(request);
            request.KeepAlive()
                .UserAgent(resourceName)
                .Header("X-Microsoft-OutputFormat", outputFormat.Name);
        }

        protected override ActionDelegate Action
        {
            get
            {
                return Post;
            }
        }
    }
}
