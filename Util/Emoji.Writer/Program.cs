using Juniper;

using System.Reflection;
using System.Text;

using static System.Console;

var root = new DirectoryInfo(Environment.CurrentDirectory).GoUpUntil(dir => dir.Name.Equals("juniper", StringComparison.CurrentCultureIgnoreCase));
if (root is null)
{
    Error.WriteLine("Couldn't find root directory");
    return;
}

var tsDir = root.CD("JS", "emoji", "src");
var tsEmojisFile = tsDir.Touch("index").AddExtension(MediaType.Text_X_TypeScript);

var emojis = typeof(Emoji)
    .GetProperties(BindingFlags.Public | BindingFlags.Static)
    .Where(f => f.PropertyType == typeof(Emoji))
    .Select(f => (f.Name, (Emoji)f.GetValue(null)!))
    .ToDictionary(f => f.Item2, f => f.Name);

var groups = typeof(Emoji)
    .GetFields(BindingFlags.Public | BindingFlags.Static)
    .Where(f => f.FieldType.IsAssignableTo(typeof(IEnumerable<Emoji>)))
    .Select(f => (f.Name, (IEnumerable<Emoji>)f.GetValue(null)!))
    .ToDictionary(f => f.Item2, f => f.Name);

foreach (var file in tsDir.GetFiles("*.ts"))
{
    if (file.Name != "Emoji.ts"
        && file.Name != "isSurfer.ts")
    {
        file.Delete();
    }
}

await Task.WhenAll(
    WriteTSEmojisFile(tsEmojisFile, emojis),
    WriteTSGroupsFiles(tsDir, emojis, groups)
);

async Task WriteTSEmojisFile(FileInfo tsEmojisFile, Dictionary<Emoji, string> emojis)
{
    using var fileStream = tsEmojisFile.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None);
    using var writer = new StreamWriter(fileStream);
    await writer.WriteLineAsync(@"import { Emoji } from ""./Emoji"";");
    await writer.WriteLineAsync();

    foreach (var emoji in emojis)
    {
        await writer.WriteLineAsync($@"export const {emoji.Value} = new Emoji(""{Encode(emoji.Key.Value)}"", ""{emoji.Key.Desc}"");");
    }

    await writer.FlushAsync();
    writer.Close();

}

string Encode(string value)
{
    var bytes = Encoding.Unicode.GetBytes(value);
    var utf16 = "";
    for (int i = 0; i < bytes.Length; ++i)
    {
        if (i % 2 == 0)
        {
            utf16 += "\\u";
        }

        var offset = 1 - (2 * (i % 2));
        utf16 += bytes[i + offset].ToString("X2");
    }

    return utf16;
}

async Task WriteTSGroupsFiles(DirectoryInfo tsDir, Dictionary<Emoji, string> emojis, Dictionary<IEnumerable<Emoji>, string> groups)
{
    foreach (var group in groups)
    {
        var file = tsDir.Touch(group.Value).AddExtension(MediaType.Text_X_TypeScript);
        using var fileStream = file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        using var writer = new StreamWriter(fileStream);

        await writer.WriteAsync("import ");
        var sep = "{";
        foreach (var emoji in group.Key)
        {
            if (emoji is not null)
            {
                await writer.WriteAsync($"{sep} {emojis[emoji]}");
                sep = ",";
            }
        }
        await writer.WriteLineAsync(@" } from ""./index"";");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("export default [");
        foreach (var emoji in group.Key)
        {
            if (emoji is not null)
            {
                await writer.WriteLineAsync($"    {emojis[emoji]},");
            }
        }
        await writer.WriteLineAsync("];");

        await writer.FlushAsync();
        writer.Close();
    }
}