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
                    writer.WriteLine("    public static class {0}", className);
                    writer.WriteLine("    {");
                    foreach (var entry in entries.Values.OrderBy(e => e.fieldName))
                    {
                        entry.Write(writer);
                    }
                    writer.WriteLine("    }");
                }
                writer.WriteLine("}");
            }
        }
    }
}