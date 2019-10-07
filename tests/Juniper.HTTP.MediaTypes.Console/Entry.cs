using System.IO;
using System.Linq;

namespace Juniper.MediaTypes
{
    public class Entry
    {
        public readonly Group Group;
        public readonly string Value;
        public readonly string FieldName;
        public readonly string DeprecationMessage;
        public readonly string[] Extensions;
        public readonly string PrimaryExtension;

        public Entry(Group group, string fieldName, string value, string deprecationMessage, string[] extensions)
        {
            Group = group;
            Value = value;
            FieldName = fieldName;
            this.DeprecationMessage = deprecationMessage;
            this.Extensions = extensions;
            if(extensions?.Length >= 1)
            {
                PrimaryExtension = extensions[0];
            }
        }

        public void Write(StreamWriter writer)
        {
            if (!string.IsNullOrEmpty(DeprecationMessage))
            {
                writer.WriteLine();
                writer.WriteLine("            [System.Obsolete(\"{0}\")]", DeprecationMessage);
            }
            var shortName = Value.Substring(Group.ClassName.Length + 1);
            writer.Write("            public static readonly {0} {1} = new {0}(\"{2}\"", Group.ClassName, FieldName, shortName);
            if(Extensions != null)
            {
                writer.Write(", new string[] {{{0}}}", string.Join(", ", Extensions.Select(e => $"\"{e}\"")));
            }
            writer.WriteLine(");");


            if (DeprecationMessage != null)
            {
                writer.WriteLine();
            }
        }
    }
}