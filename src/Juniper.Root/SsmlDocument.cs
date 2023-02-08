using System.Globalization;
using System.Text;

using static System.Math;

namespace Juniper
{
    public class SsmlDocument
    {
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

        public string Text { get; set; }
        public string VoiceName { get; set; }

        private string _style;
        public string Style
        {
            get => _style;
            set => _style = value
                ?.Trim()
                ?.ToLowerInvariant();
        }

        public float PitchChange { get; set; }
        public float RateChange { get; set; }
        public float VolumeChange { get; set; }

        public bool HasStyle =>
            !string.IsNullOrEmpty(Style)
            && Style != "none";

        public bool HasPitchChange =>
            Abs(PitchChange) > 0;

        public bool HasRateChange =>
            Abs(RateChange) > 0;

        public bool HasVolumeChange =>
            Abs(VolumeChange) > 0;

        public bool UseProsody =>
            HasPitchChange
            || HasRateChange
            || HasVolumeChange;

        public override string ToString()
        {
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

            if (HasStyle)
            {
                sb.Append("<mstts:express-as type='")
                  .Append(Style)
                  .Append("'>");
            }

            sb.Append(Text);

            if (HasStyle)
            {
                sb.Append("</mstts:express-as>");
            }

            if (UseProsody)
            {
                sb.Append("</prosody>");
            }

            sb.Append("</voice>")
              .Append("</speak>");

            return sb.ToString();
        }
    }
}
