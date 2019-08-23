using System.IO;
using System.Linq;

namespace Juniper.HTTP.MediaTypes.Console
{
    public class Entry
    {
        public readonly Group Group;
        public readonly string Value;
        public readonly string FieldName;
        public readonly string PrimaryExtension;

        private readonly string deprecationMessage;
        private readonly string[] extensions;

        public Entry(Group group, string fieldName, string value, string deprecationMessage)
        {
            Group = group;
            Value = value;
            FieldName = fieldName;
            this.deprecationMessage = deprecationMessage;
            extensions = null;
        }

        public Entry(Group group, string fieldName, string value, string[] extensions)
        {
            Group = group;
            Value = value;
            FieldName = fieldName;
            deprecationMessage = null;
            this.extensions = extensions;
            if(extensions?.Length >= 1)
            {
                PrimaryExtension = extensions[0];
            }
        }

        public void Write(StreamWriter writer)
        {
            if (!string.IsNullOrEmpty(deprecationMessage))
            {
                writer.WriteLine();
                writer.WriteLine("            [System.Obsolete(\"{0}\")]", deprecationMessage);
            }
            var shortName = Value.Substring(Group.ClassName.Length + 1);
            writer.Write("            public static readonly {0} {1} = new {0}(\"{2}\"", Group.ClassName, FieldName, shortName);
            if(extensions != null)
            {
                writer.Write(", new string[] {{{0}}}", string.Join(", ", extensions.Select(e => $"\"{e}\"")));
            }
            writer.WriteLine(");");


            if (deprecationMessage != null)
            {
                writer.WriteLine();
            }
        }
    }
}