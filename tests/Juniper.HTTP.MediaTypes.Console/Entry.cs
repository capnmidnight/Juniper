using System.IO;

namespace Juniper.HTTP.MediaTypes.Console
{
    public class Entry
    {
        public readonly string fieldName;
        public readonly string value;
        public readonly string deprecationMessage;

        public Entry(string name, string value, string deprecationMessage)
        {
            fieldName = name.CamelCase();
            this.value = value;
            this.deprecationMessage = deprecationMessage;
        }

        public void Write(StreamWriter writer)
        {
            if (deprecationMessage != null)
            {
                writer.WriteLine();
                writer.WriteLine("        [System.Obsolete(\"{0}\")]", deprecationMessage);
            }
            writer.WriteLine("        public const string {0} = \"{1}\";", fieldName, value);
            if (deprecationMessage != null)
            {
                writer.WriteLine();
            }
        }
    }
}