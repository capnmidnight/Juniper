using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Juniper.Processes;

public class CopyJsonValueCommand : AbstractCommand
{
    private readonly FileInfo readFile;
    private readonly FileInfo writeFile;
    private readonly string readField;
    private readonly string writeField;

    private static async Task<JsonNode?> ReadJsonAsync(FileInfo path, CancellationToken cancellationToken)
    {
        if (path?.Exists != true)
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(path.FullName, cancellationToken);

        return JsonNode.Parse(json);
    }

    public static async Task<string?> ReadJsonValueAsync(FileInfo file, string field, CancellationToken cancellationToken)
    {
        var doc = await ReadJsonAsync(file, cancellationToken);
        if (doc is null)
        {
            return null;
        }

        return doc[field]?.GetValue<string>();
    }

    public static async Task WriteJsonValueAsync(FileInfo file, string field, string value, CancellationToken cancellationToken)
    {
        var doc = await ReadJsonAsync(file, cancellationToken);
        doc ??= JsonNode.Parse("{}");
        if (doc is null)
        {
            throw new FileNotFoundException(file.FullName);
        }

        doc[field] = value;

        var json = doc.ToJsonString(new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        });

        await File.WriteAllTextAsync(file.FullName, json, cancellationToken);
    }

    public CopyJsonValueCommand(FileInfo readFile, string readField, FileInfo writeFile, string writeField)
        : base($"Json Copy [{readFile.GetShortName()}:{readField} -> {writeFile.GetShortName()}:{writeField}]")
    {
        this.readFile = readFile;
        this.readField = readField;
        this.writeFile = writeFile;
        this.writeField = writeField;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var fromJson = await ReadJsonAsync(readFile, cancellationToken);

        if (fromJson is null)
        {
            OnError(new FileNotFoundException($"Couldn't find {readFile}"));
            return;
        }

        var value = fromJson[readField]?.GetValue<string>();
        if (value is null)
        {
            OnError(new FieldAccessException($"{readFile} had no {readField}"));
            return;
        }

        var toJson = (await ReadJsonAsync(writeFile, cancellationToken))
            ?? JsonNode.Parse("{}");
        if (toJson is null)
        {
            OnError(new InvalidOperationException("How did this happen?"));
            return;
        }

        toJson[writeField] = value;
        var appsettingsJson = toJson.ToJsonString(new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        });

        await File.WriteAllTextAsync(writeFile.FullName, appsettingsJson, cancellationToken);

        OnInfo($"Wrote {writeField} = {value}");
    }
}
