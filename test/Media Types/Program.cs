using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Juniper.MediaTypes
{
    public static partial class Program
    {
        private static XNamespace ns;
        private static readonly HttpClient http = new(new HttpClientHandler { UseCookies = false });

        private static void PromotePrimaryExtension(Dictionary<string, Group> groups, string group, string entry, string ext)
        {
            if (groups.ContainsKey(group) && groups[group].Entries.ContainsKey(entry))
            {
                var extensions = groups[group].Entries[entry].Extensions;
                var index = Array.IndexOf(extensions, ext);
                if (0 <= index && index < extensions.Length)
                {
                    var temp = extensions[0];
                    extensions[0] = ext;
                    extensions[index] = temp;
                }
            }
        }

        public static async Task Main(string[] args)
        {
            if (args?.Length != 1)
            {
                System.Console.WriteLine("Usage: <exe> C:\\path\\to\\output\\dir");
                return;
            }

            var outDirName = args[0];
            if (outDirName[0] == '"')
            {
                outDirName = outDirName[1..^1];
            }
            var outDir = new DirectoryInfo(outDirName);
            if (!outDir.Exists)
            {
                System.Console.WriteLine("Directory not found: " + outDir.FullName);
                return;
            }

            var groups = new Dictionary<string, Group>(StringComparer.InvariantCultureIgnoreCase);

            await ParseApacheConfAsync(groups)
                .ConfigureAwait(false);

            await ParseIANAXmlAsync(groups)
                .ConfigureAwait(false);

            groups["Image"].Entries["Raw"] = new Entry(groups["Image"], "Raw", "image/x-raw", null, new string[] { "raw" });
            groups["Image"].Entries["Exr"] = new Entry(groups["Image"], "EXR", "image/x-exr", null, new string[] { "exr" });
            PromotePrimaryExtension(groups, "Audio", "Mpeg", "mp3");
            PromotePrimaryExtension(groups, "Audio", "Ogg", "ogg");

            WriteGroups(groups, outDir);
            WriteValues(groups, outDir);
        }

        private static void WriteGroups(Dictionary<string, Group> groups, DirectoryInfo outDir)
        {
            foreach (var group in groups.Values)
            {
                group.Write(outDir);
            }
        }

        private static void WriteValues(Dictionary<string, Group> groups, DirectoryInfo outDir)
        {
            var allValues =
                from grp in groups.Values
                from entry in grp.Entries.Values
                where entry.DeprecationMessage == null
                let value = $"{grp.ClassName}.{entry.FieldName}"
                orderby value
                select value;

            "MediaType.Values.cs".MakeFile(outDir, (writer) =>
            {
                writer.WriteLine("        public static readonly ReadOnlyCollection<MediaType> Values = Array.AsReadOnly(new MediaType[]{");
                var last = allValues.Last();
                var values = allValues.SkipLast(1);
                foreach (var value in values)
                {
                    writer.WriteLine("            {0},", value);
                }
                writer.WriteLine("            {0}", last);
                writer.WriteLine("        });");
            }, "using System;\r\nusing System.Collections.ObjectModel;");
        }

        private static async Task ParseApacheConfAsync(Dictionary<string, Group> groups)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("http://svn.apache.org/viewvc/httpd/httpd/trunk/docs/conf/mime.types?view=co"))
                .Accept(MediaType.Text_Plain);
            using var response = await http.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var searching = true;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync()
                    .ConfigureAwait(false);
                if (line.StartsWith("# ", StringComparison.Ordinal))
                {
                    line = line[2..];
                }

                if (!searching)
                {
                    var parts = line.Split(' ', '\t')
                        .Where(p => p.Length > 0);

                    var value = parts.First();
                    var extensions = parts.Skip(1).ToArray();
                    if (extensions.Length == 0)
                    {
                        extensions = null;
                    }
                    var slashIndex = value.IndexOf('/');
                    var groupName = value[..slashIndex];
                    var name = value[(slashIndex + 1)..];

                    var group = groups.GetGroup(groupName);
                    name = name.CamelCase();

                    if (extensions == null)
                    {
                        var plusIndex = value.IndexOf('+');
                        if (0 <= plusIndex && plusIndex < value.Length - 1)
                        {
                            extensions = new string[] { value[(plusIndex + 1)..] };
                        }
                    }

                    AddEntry(group, name, value, null, extensions);
                }
                else if (line.StartsWith("====================", StringComparison.Ordinal))
                {
                    searching = false;
                }
            }
        }

        private static async Task ParseIANAXmlAsync(Dictionary<string, Group> groups)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://www.iana.org/assignments/media-types/media-types.xml"))
                .UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36")
                .Accept(MediaType.Text_Xml);
            using var response = await http.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();
            var fullRegistry = XElement.Load(stream);
            ns = fullRegistry.GetDefaultNamespace();
            var files = fullRegistry.Descendants(ns + "file");
            foreach (var file in files)
            {
                var groupName = file.Parent.Parent.Attribute("id").Value;
                var nameAndDescription = file.Parent.Element(ns + "name").Value;
                var name = nameAndDescription;

                var deprecationMessageIndex = nameAndDescription.IndexOf(" ", StringComparison.Ordinal);
                var isDeprecated = deprecationMessageIndex >= 0;
                string deprecationMessage = null;
                if (isDeprecated)
                {
                    deprecationMessage = nameAndDescription[(deprecationMessageIndex + 1)..].Trim();
                    name = nameAndDescription[..deprecationMessageIndex];
                    if (deprecationMessage.StartsWith("-", StringComparison.Ordinal))
                    {
                        deprecationMessage = deprecationMessage[1..].Trim();
                    }
                }

                var value = $"{groupName}/{name.ToLowerInvariant()}";
                var group = groups.GetGroup(groupName);

                var plusIndex = value.IndexOf('+');
                string[] extensions = null;
                if (0 <= plusIndex && plusIndex < value.Length - 1)
                {
                    extensions = new string[] { value[(plusIndex + 1)..] };
                }

                name = name.CamelCase();
                AddEntry(group, name, value, deprecationMessage, extensions);
            }
        }

        private static void AddEntry(Group group, string name, string value, string deprecationMessage, string[] extensions)
        {
            if (group.Entries.ContainsKey(name))
            {
                var oldGroup = group.Entries[name];
                deprecationMessage ??= oldGroup.DeprecationMessage;

                if (extensions == null)
                {
                    extensions = oldGroup.Extensions;
                }
                else if (oldGroup.Extensions != null)
                {
                    if (oldGroup.Extensions.Length > extensions.Length)
                    {
                        extensions = oldGroup.Extensions;
                    }
                    else if (oldGroup.Extensions.Length == extensions.Length)
                    {
                        var newExtensions = new string[extensions.Length];
                        for (var i = 0; i < extensions.Length; ++i)
                        {
                            if (extensions[i].Length > oldGroup.Extensions[i].Length
                                && !value.EndsWith("+" + extensions[i], StringComparison.Ordinal))
                            {
                                newExtensions[i] = extensions[i];
                            }
                            else
                            {
                                newExtensions[i] = oldGroup.Extensions[i];
                            }
                        }

                        extensions = newExtensions;
                    }
                }
            }

            group.Entries[name] = new Entry(group, name, value, deprecationMessage, extensions);
        }

        private static Group GetGroup(this Dictionary<string, Group> groups, string name)
        {
            var groupName = name.CamelCase(true);
            if (!groups.ContainsKey(groupName))
            {
                groups[groupName] = new Group(groupName);
            }

            var group = groups[groupName];
            return group;
        }

        public static string UpperFirst(this string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var chars = s.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            return new string(chars);
        }

        public static string CamelCase(this string s, bool stripUnderscores = false)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (!char.IsLetter(s[0]))
            {
                s = "vnd." + s;
            }

            if (s.StartsWith("vnd.", StringComparison.InvariantCultureIgnoreCase))
            {
                s = "vendor." + s[4..];
            }

            if (s.EndsWith("+", StringComparison.InvariantCultureIgnoreCase))
            {
                s = s[0..^1] + ".plus";
            }

            var words = s.Split('+', '.');
            s = string.Concat(words.Select(UpperFirst));
            words = s.Split('-');
            s = string.Join("_", words.Select(UpperFirst));

            if (stripUnderscores)
            {
                return s.Replace("_", "");
            }

            return s;
        }

        public static void MakeFile(this string fileName, DirectoryInfo directory, Action<StreamWriter> act, string usingBlock)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Must provide a file name", nameof(fileName));
            }

            if (directory is null)
            {
                throw new ArgumentException("Must provide an output directory", nameof(directory));
            }

            if (act is null)
            {
                throw new ArgumentNullException(nameof(act));
            }

            var filePath = Path.Combine(directory.FullName, fileName);
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream);

            if (usingBlock != null)
            {
                writer.WriteLine(usingBlock);
                writer.WriteLine();
            }

            writer.WriteLine("namespace Juniper");
            writer.WriteLine("{");
            {
                writer.WriteLine("    public partial class MediaType");
                writer.WriteLine("    {");
                {
                    act(writer);
                }
                writer.WriteLine("    }");
            }
            writer.WriteLine("}");
        }

        public static void MakeFile(this string fileName, DirectoryInfo directory, Action<StreamWriter> act)
        {
            MakeFile(fileName, directory, act, null);
        }
    }
}