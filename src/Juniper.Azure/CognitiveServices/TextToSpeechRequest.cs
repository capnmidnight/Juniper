using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;

using Juniper.Progress;
using Juniper.Sound;
using Juniper.Xml;

using static System.Math;

namespace Juniper.Speech.Azure.CognitiveServices;

public class TextToSpeechRequest : AbstractAzureSpeechRequest<MediaType.Audio>
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

    private readonly string resourceName;

    public TextToSpeechRequest(HttpClient http, string region, string resourceName, AudioFormat outputFormat)
        : base(http, HttpMethod.Post, region, "cognitiveservices/v1", outputFormat?.ContentType ?? throw new ArgumentNullException(nameof(outputFormat)))
    {
        this.resourceName = resourceName;
        OutputFormat = outputFormat;
    }

    private readonly SsmlDocument doc = new();

    public AudioFormat OutputFormat { get; }

    public string Text
    {
        get => doc.Text;
        set => doc.Text = value;
    }

    public string VoiceName
    {
        get => doc.VoiceName;
        set => doc.VoiceName = value;
    }

    private SpeechStyle _style;
    public SpeechStyle Style
    {
        get => _style;
        set
        {
            _style = value;
            doc.Style = Style.ToString();
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

            if (doc.HasStyle)
            {
                sb.Append("style=")
                  .Append(doc.Style);
            }

            return PathExt.FixPath(sb.ToString());
        }
    }

    protected override void ModifyRequest(HttpRequestMessage request, IProgress prog = null)
    {
        base.ModifyRequest(request, prog);
        request.KeepAlive()
            .UserAgent(resourceName);

        request.Header("X-MICROSOFT-OutputFormat", OutputFormat.Name)
            .Body(new StringContent(doc.ToString(), Encoding.UTF8, (string)MediaType.Application_SsmlXml));
    }
}
