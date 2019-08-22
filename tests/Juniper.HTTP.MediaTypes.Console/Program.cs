using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Juniper.HTTP.MediaTypes.Console
{
    public static partial class Program
    {
        private static XNamespace ns;

        private static void Main()
        {
            var groups = new Dictionary<string, Group>();
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var mediaTypesFileName = Path.Combine(myDocs, "media-types.xml");
            var fullRegistry = XElement.Load(mediaTypesFileName);
            ns = fullRegistry.GetDefaultNamespace();
            var files = fullRegistry.Descendants(ns + "file");
            foreach (var file in files)
            {
                var groupName = file.Parent.Parent.Attribute("id").Value;
                var nameAndDescription = file.Parent.Element(ns + "name").Value;
                var groupAndName = file.Value;
                Parse(groups, groupName, nameAndDescription, groupAndName);
            }

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var outDir = Path.Combine(home, "Projects", "Yarrow", "Juniper");
            outDir = Path.Combine(outDir, "src", "Juniper.Core", "HTTP");
            outDir = Path.Combine(outDir, "MediaTypes");
            foreach (var group in groups.Values)
            {
                group.Write(outDir);
            }
        }

        public static void Parse(Dictionary<string, Group> groups, string groupName, string nameAndDescription, string groupAndName)
        {
            var name = groupAndName.Substring(groupName.Length + 1);
            var value = $"{groupName}/{name}";

            var isDeprecated = nameAndDescription.Length > name.Length;
            string deprecationMessage = null;
            if (isDeprecated)
            {
                deprecationMessage = nameAndDescription.Substring(name.Length + 2).Trim();
                if (deprecationMessage.StartsWith("-"))
                {
                    deprecationMessage = deprecationMessage.Substring(1).Trim();
                }
            }

            if (!groups.ContainsKey(groupName))
            {
                groups[groupName] = new Group(groupName);
            }

            var group = groups[groupName];

            if (!group.entries.ContainsKey(name) || deprecationMessage != null)
            {
                group.entries[name] = new Entry(name, value, deprecationMessage);
            }
        }

        public static string UpperFirst(this string s)
        {
            var chars = s.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            return new string(chars);
        }

        public static string CamelCase(this string s)
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

            var words = s.Split('+', '-', '.');
            return string.Join(string.Empty, words.Select(UpperFirst));
        }
    }
}