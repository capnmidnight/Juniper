using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Juniper.Processes
{
    public class CopyJsonValueCommand : AbstractCommand
    {
        private readonly FileInfo readFile;
        private readonly FileInfo writeFile;
        private readonly string readField;
        private readonly string writeField;

        private static async Task<JsonNode?> ReadJsonAsync(FileInfo path)
        {
            if (path?.Exists != true)
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(path.FullName);

            return JsonNode.Parse(json);
        }

        public static async Task<string?> ReadJsonValueAsync(FileInfo file, string field)
        {
            var doc = await ReadJsonAsync(file);
            if(doc is null)
            {
                return null;
            }

            return doc[field]?.GetValue<string>();
        }

        public static async Task WriteJsonValueAsync(FileInfo file, string field, string value)
        {
            var doc = await ReadJsonAsync(file);
            doc ??= JsonNode.Parse("{}");
            if(doc is null)
            {
                throw new FileNotFoundException(file.FullName);
            }

            doc[field] = value;

            var json = doc.ToJsonString(new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            await File.WriteAllTextAsync(file.FullName, json);
        }

        public CopyJsonValueCommand(FileInfo readFile, string readField, FileInfo writeFile, string writeField)
            : base($"Json Copy [{readFile}:{readField} -> {writeFile}:{writeField}]")
        {
            this.readFile = readFile;
            this.readField = readField;
            this.writeFile = writeFile;
            this.writeField = writeField;
        }

        public override async Task RunAsync()
        {
            var fromJson = await ReadJsonAsync(readFile);

            if (fromJson is null)
            {
                OnError(new FileNotFoundException($"Couldn't find {readFile}"));
                return;
            }

            var toJson = await ReadJsonAsync(writeFile);

            if (toJson is null)
            {
                OnError(new FileNotFoundException($"Couldn't find {writeFile}"));
                return;
            }

            var value = fromJson[readField]?.GetValue<string>();
            if (value is null)
            {
                OnError(new FieldAccessException($"{readFile} had no {readField}"));
                return;
            }

            toJson[writeField] = value;
            var appsettingsJson = toJson.ToJsonString(new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            await File.WriteAllTextAsync(writeFile.FullName, appsettingsJson);

            OnInfo($"Wrote {writeField} = {value}");
        }
    }
}
