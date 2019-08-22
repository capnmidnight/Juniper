using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juniper.HTTP.MediaTypes.Console
{
    public class Group
    {
        public readonly string name;
        public readonly string fileName;
        public readonly string className;
        public readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>();

        public Group(string name)
        {
            this.name = name;

            className = name.CamelCase();
            fileName = className + ".cs";
        }

        public void Write(string directoryName)
        {
            var filePath = Path.Combine(directoryName, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("namespace Juniper.HTTP.MediaTypes");
                writer.WriteLine("{");
                {
                    WriteClass(writer, string.Empty);
                }
                writer.WriteLine("}");
            }
        }

        private void WriteClass(StreamWriter writer, string prefix)
        {
            prefix += "    ";
            writer.Write(prefix);
            writer.WriteLine("public static class {0}", className);
            writer.Write(prefix);
            writer.WriteLine("{");
            WriteFields(writer, prefix);
            writer.Write(prefix);
            writer.WriteLine("}");
        }

        private void WriteFields(StreamWriter writer, string prefix)
        {
            prefix += "    ";
            foreach (var entry in entries.Values.OrderBy(e => e.fieldName))
            {
                if (entry.deprecationMessage != null)
                {
                    writer.WriteLine();
                    writer.Write(prefix);
                    writer.WriteLine("[System.Obsolete(\"{0}\")]", entry.deprecationMessage);
                }
                var fieldName = entry.fieldName;
                writer.Write(prefix);
                writer.WriteLine("public const string {0} = \"{1}\";", fieldName, entry.value);
                if (entry.deprecationMessage != null)
                {
                    writer.WriteLine();
                }
            }
        }
    }
}