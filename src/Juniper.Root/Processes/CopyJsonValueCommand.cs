using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public class CopyJsonValueCommand : AbstractCommand
    {
        private readonly FileInfo readFile;
        private readonly FileInfo writeFile;
        private readonly string readField;
        private readonly string writeField;

        private static async Task<JsonNode> ReadJsonAsync(FileInfo path, CancellationToken? token)
        {
            if (path?.Exists != true)
            {
                return null;
            }

            var json = token.HasValue
                ? await File.ReadAllTextAsync(path.FullName, token.Value)
                : await File.ReadAllTextAsync(path.FullName);

            return JsonNode.Parse(json);
        }

        public static async Task<string> ReadJsonValueAsync(FileInfo file, string field)
        {
            var doc = await ReadJsonAsync(file, null);
            return doc[field]?.GetValue<string>();
        }

        public static async Task WriteJsonValueAsync(FileInfo file, string field, string value)
        {
            var doc = await ReadJsonAsync(file, null);
            if(doc is null)
            {
                doc = JsonNode.Parse("{}");
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
        {
            this.readFile = readFile;
            this.readField = readField;
            this.writeFile = writeFile;
            this.writeField = writeField;
            CommandName = $"Json Copy [{readFile}:{readField} -> {writeFile}:{writeField}]";
        }

        public override async Task RunAsync(CancellationToken? token = null)
        {
            var fromJson = await ReadJsonAsync(readFile, token);
            if (token?.IsCancellationRequested == true)
            {
                OnWarning("Operation cancelled");
                return;
            }

            if (fromJson is null)
            {
                OnError(new FileNotFoundException($"Couldn't find {readFile}"));
                return;
            }

            var toJson = await ReadJsonAsync(writeFile, token);
            if (token?.IsCancellationRequested == true)
            {
                OnWarning("Operation cancelled");
                return;
            }

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

            if (token.HasValue)
            {
                await File.WriteAllTextAsync(writeFile.FullName, appsettingsJson, token.Value);
            }
            else
            {
                await File.WriteAllTextAsync(writeFile.FullName, appsettingsJson);
            }

            if (token?.IsCancellationRequested == true)
            {
                OnWarning("Operation cancelled");
                return;
            }

            OnInfo($"Wrote {writeField} = {value}");
        }
    }
}
