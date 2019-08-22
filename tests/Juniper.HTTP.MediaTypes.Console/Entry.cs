using System.IO;
using System.Linq;

namespace Juniper.HTTP.MediaTypes.Console
{
    public class Entry
    {
        private static readonly string[] EMPTY_STRING_ARRAY = new string[0];

        public readonly string FieldName;
        public readonly string PrimaryExtension;

        private readonly string value;
        private readonly string deprecationMessage;
        private readonly string[] extensions;

        public Entry(string name, string value, string deprecationMessage)
        {
            FieldName = name.CamelCase();
            this.value = value;
            this.deprecationMessage = deprecationMessage;
            extensions = null;
        }

        public Entry(string name, string value, string[] extensions)
        {
            FieldName = name.CamelCase();
            this.value = value;
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

            writer.Write("            public static readonly MediaType {0} = new MediaType(\"{1}\"", FieldName, value);
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