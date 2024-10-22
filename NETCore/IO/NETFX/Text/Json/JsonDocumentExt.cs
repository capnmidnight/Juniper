namespace System.Text.Json;

public static class JsonDocumentExt
{
    public static string ToJsonString(this JsonDocument jdoc)
    {
        using var stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        jdoc.WriteTo(writer);
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
