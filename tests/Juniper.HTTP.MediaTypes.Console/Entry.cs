using System.Collections.Generic;

namespace Juniper.HTTP.MediaTypes.Console
{
    public class Entry
    {
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

        public readonly string fieldName;
        public readonly string value;
        public readonly string deprecationMessage;

        private Entry(string name, string value, string deprecationMessage)
        {
            fieldName = name.CamelCase();
            this.value = value;
            this.deprecationMessage = deprecationMessage;
        }

        public override string ToString()
        {
            return value;
        }
    }
}