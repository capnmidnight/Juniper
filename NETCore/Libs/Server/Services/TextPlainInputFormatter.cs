using Microsoft.AspNetCore.Mvc.Formatters;

namespace Juniper.Services;

public class TextPlainInputFormatter : InputFormatter
{
    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add(MediaType.Text_Plain.ToString()!);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        using var reader = new StreamReader(request.Body);
        var content = await reader.ReadToEndAsync();
        return await InputFormatterResult.SuccessAsync(content);
    }

    public override bool CanRead(InputFormatterContext context) => 
        context.HttpContext.Request.ContentType?.StartsWith(MediaType.Text_Plain.ToString()!) == true;
}