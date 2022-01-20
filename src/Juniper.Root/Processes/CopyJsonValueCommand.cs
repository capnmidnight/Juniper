using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly string from;
        private readonly string to;
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

        public CopyJsonValueCommand(DirectoryInfo root, string from, string to, string readField, string writeField)
        {
            this.root = root;
            this.readField = readField;
            this.writeField = writeField;
            this.from = PathExt.FixPath(from);
            this.to = PathExt.FixPath(to);
        }

        public override async Task RunAsync(CancellationToken? token = null)
        {
            var fromPath = Path.Combine(
                root.FullName,
                from);

            var toPath = Path.Combine(
                root.FullName,
                to);

            var fromJson = await ReadJsonAsync(fromPath, token);
            if(token?.IsCancellationRequested == true)
            {
                OnWarning("Operation cancelled");
                return;
            }

            if (fromJson is null)
            {
                OnError(new FileNotFoundException($"Couldn't find {from}"));
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
                OnError(new FileNotFoundException($"Couldn't find {to}"));
                return;
            }

            var value = fromJson[readField]?.GetValue<string>();
            if (value is null)
            {
                OnError(new FieldAccessException($"{from} had no {readField}"));
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
