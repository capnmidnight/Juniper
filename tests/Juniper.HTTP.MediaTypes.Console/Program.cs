using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Juniper.HTTP.MediaTypes.Console
{
    public static partial class Program
    {
        private static XNamespace ns;

        private static void Main()
        {
            var groups = new Dictionary<string, Group>(StringComparer.InvariantCultureIgnoreCase);

            ParseApacheConf(groups).Wait();
            ParseIANAXml(groups).Wait();

            groups["Image"].entries["Raw"] = new Entry(groups["Image"], "Raw", "image/x-raw", null, new string[] { "raw" });
            groups["Image"].entries["Exr"] = new Entry(groups["Image"], "EXR", "image/x-exr", null, new string[] { "exr" });

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outDir = Path.Combine(home, "Projects", "Yarrow", "Juniper");
            outDir = Path.Combine(outDir, "src", "Juniper.Core", "HTTP");
            outDir = Path.Combine(outDir, "MediaType");

            WriteGroups(groups, outDir);
            WriteLookup(groups, outDir);
            WriteExtensionLookup(groups, outDir);
            WriteValues(groups, outDir);
        }

        private static void WriteGroups(Dictionary<string, Group> groups, string outDir)
        {
            foreach (var group in groups.Values)
            {
                group.Write(outDir);
            }
        }

        private static void WriteLookup(Dictionary<string, Group> groups, string outDir)
        {
            var byValue =
              from g in groups.Values
              from e in g.entries.Values
              where e.DeprecationMessage == null
              select e;
            "Lookup.cs".MakeFile(outDir, (writer) =>
            {
                writer.WriteLine("        private static readonly Dictionary<string, MediaType> byValue = new Dictionary<string, MediaType>() {");
                foreach (var ext in byValue)
                {
                    writer.WriteLine("            {{ \"{0}\", {1}.{2} }},", ext.Value, ext.Group.ClassName, ext.FieldName);
                }
                writer.WriteLine("        };");
            }, "using System.Collections.Generic;");
        }

        private static void WriteExtensionLookup(Dictionary<string, Group> groups, string outDir)
        {
            var byExtension =
              from grp in groups.Values
              from entry in grp.entries.Values
              where entry.Extensions != null
                && entry.DeprecationMessage == null
              from extension in entry.Extensions
              orderby entry.Value.IndexOf('+')
              group (entry, extension) by extension into L
              let first = L.First()
              orderby first.extension
              select first;
            "ExtensionLookup.cs".MakeFile(outDir, (writer) =>
            {
                writer.WriteLine("        private static readonly Dictionary<string, MediaType> byExtensions = new Dictionary<string, MediaType>() {");
                foreach (var ext in byExtension)
                {
                    writer.WriteLine("            {{ \"{0}\", {1}.{2} }},", ext.extension, ext.entry.Group.ClassName, ext.entry.FieldName);
                }
                writer.WriteLine("        };");
            }, "using System.Collections.Generic;");
        }

        private static void WriteValues(Dictionary<string, Group> groups, string outDir)
        {
            var allValues =
                from grp in groups.Values
                from entry in grp.entries.Values
                where entry.DeprecationMessage == null
                let value = $"{grp.ClassName}.{entry.FieldName}"
                orderby value
                select value;

            "Values.cs".MakeFile(outDir, (writer) =>
            {
                writer.WriteLine("        public static readonly MediaType[] Values = {");
                foreach(var value in allValues)
                {
                    writer.WriteLine("            {0},", value);
                }
                writer.WriteLine("        };");
            });
        }

        private static async Task ParseApacheConf(Dictionary<string, Group> groups)
        {
            using (var response = await HttpWebRequestExt
                .Create("http://svn.apache.org/viewvc/httpd/httpd/trunk/docs/conf/mime.types?view=co")
                .Accept(MediaType.Text.Plain)
                .Get())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var searching = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith("# "))
                    {
                        line = line.Substring(2);
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
                        var groupName = value.Substring(0, slashIndex);
                        var name = value.Substring(slashIndex + 1);

                        var group = groups.GetGroup(groupName);
                        name = name.CamelCase();

                        if (extensions == null)
                        {
                            var plusIndex = value.IndexOf('+');
                            if (0 <= plusIndex && plusIndex < value.Length - 1)
                            {
                                extensions = new string[] { value.Substring(plusIndex + 1) };
                            }
                        }

                        string deprecationMessage = null;
                        AddEntry(group, name, value, deprecationMessage, extensions);
                    }
                    else if (line.StartsWith("===================="))
                    {
                        searching = false;
                    }
                }
            }
        }

        private static async Task ParseIANAXml(Dictionary<string, Group> groups)
        {
            using (var response = await HttpWebRequestExt
                .Create("https://www.iana.org/assignments/media-types/media-types.xml")
                .UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36")
                .Accept(MediaType.Text.Xml)
                .Get())
            using (var stream = response.GetResponseStream())
            {
                var fullRegistry = XElement.Load(stream);
                ns = fullRegistry.GetDefaultNamespace();
                var files = fullRegistry.Descendants(ns + "file");
                foreach (var file in files)
                {
                    var groupName = file.Parent.Parent.Attribute("id").Value;
                    var nameAndDescription = file.Parent.Element(ns + "name").Value;
                    var groupAndName = file.Value;
                    var name = nameAndDescription;

                    var deprecationMessageIndex = nameAndDescription.IndexOf(" ");
                    var isDeprecated = deprecationMessageIndex >= 0;
                    string deprecationMessage = null;
                    if (isDeprecated)
                    {
                        deprecationMessage = nameAndDescription.Substring(deprecationMessageIndex + 1).Trim();
                        name = nameAndDescription.Substring(0, deprecationMessageIndex);
                        if (deprecationMessage.StartsWith("-"))
                        {
                            deprecationMessage = deprecationMessage.Substring(1).Trim();
                        }
                    }

                    var value = $"{groupName}/{name.ToLowerInvariant()}";
                    var group = groups.GetGroup(groupName);

                    var plusIndex = value.IndexOf('+');
                    string[] extensions = null;
                    if (0 <= plusIndex && plusIndex < value.Length - 1)
                    {
                        extensions = new string[] { value.Substring(plusIndex + 1) };
                    }

                    name = name.CamelCase();
                    AddEntry(group, name, value, deprecationMessage, extensions);
                }
            }
        }

        private static void AddEntry(Group group, string name, string value, string deprecationMessage, string[] extensions)
        {
            if (group.entries.ContainsKey(name))
            {
                var oldGroup = group.entries[name];
                deprecationMessage = deprecationMessage ?? oldGroup.DeprecationMessage;

                if(extensions == null)
                {
                    extensions = oldGroup.Extensions;
                }
                else if(oldGroup.Extensions != null)
                {
                    if(oldGroup.Extensions.Length > extensions.Length)
                    {
                        extensions = oldGroup.Extensions;
                    }
                    else if(oldGroup.Extensions.Length == extensions.Length)
                    {
                        var newExtensions = new string[extensions.Length];
                        for(int i = 0; i < extensions.Length; ++i)
                        {
                            if(extensions[i].Length > oldGroup.Extensions[i].Length
                                && !value.EndsWith("+" + extensions[i]))
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

            group.entries[name] = new Entry(group, name, value, deprecationMessage, extensions);
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
            var chars = s.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            return new string(chars);
        }

        public static string CamelCase(this string s, bool stripUnderscores = false)
        {
            if (!char.IsLetter(s[0]))
            {
                s = "vnd." + s;
            }

            if (s.StartsWith("vnd."))
            {
                s = "vendor." + s.Substring(4);
            }

            if (s.EndsWith("+"))
            {
                s = s.Substring(0, s.Length - 1) + ".plus";
            }

            var words = s.Split('+', '.');
            s = string.Join(string.Empty, words.Select(UpperFirst));
            words = s.Split('-');
            s = string.Join("_", words.Select(UpperFirst));

            if (stripUnderscores)
            {
                return s.Replace("_", "");
            }

            return s;
        }

        public static void MakeFile(this string fileName, string directoryName, Action<StreamWriter> act, string usingBlock = null)
        {
            var filePath = Path.Combine(directoryName, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                if (usingBlock != null)
                {
                    writer.WriteLine(usingBlock);
                    writer.WriteLine();
                }
                writer.WriteLine("namespace Juniper.HTTP");
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
        }
    }
}