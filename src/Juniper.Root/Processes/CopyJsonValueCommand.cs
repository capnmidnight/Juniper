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
        private readonly DirectoryInfo root;
        private readonly string readFile;
        private readonly string writeFile;
        private readonly string readField;
        private readonly string writeField;

        private static async Task<JsonNode> ReadJsonAsync(string path, CancellationToken? token)
        {
            if (path is null
                || !File.Exists(path))
            {
                return null;
            }

            var json = token.HasValue
                ? await File.ReadAllTextAsync(path, token.Value)
                : await File.ReadAllTextAsync(path);

            return JsonObject.Parse(json);
        }

        public CopyJsonValueCommand(DirectoryInfo root, string readFile, string readField, string writeFile, string writeField)
        {
            this.root = root;
            this.readFile = PathExt.FixPath(readFile);
            this.readField = readField;
            this.writeFile = PathExt.FixPath(writeFile);
            this.writeField = writeField;
            CommandName = $"Json Copy [{readFile}:{readField} -> {writeFile}:{writeField}]";
        }

        public override async Task RunAsync(CancellationToken? token = null)
        {
            var fromPath = Path.Combine(
                root.FullName,
                readFile);

            var toPath = Path.Combine(
                root.FullName,
                writeFile);

            var fromJson = await ReadJsonAsync(fromPath, token);
            if(token?.IsCancellationRequested == true)
            {
                OnWarning("Operation cancelled");
                return;
            }

            if (fromJson is null)
            {
                OnError(new FileNotFoundException($"Couldn't find {readFile}"));
                return;
            }

            var toJson = await ReadJsonAsync(toPath, token);
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
                await File.WriteAllTextAsync(toPath, appsettingsJson, token.Value);
            }
            else
            {
                await File.WriteAllTextAsync(toPath, appsettingsJson);
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
