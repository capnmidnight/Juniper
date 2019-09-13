using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.HTTP.MediaTypes.Console
{
    public class Group
    {
        public readonly string fileName;
        public readonly string ClassName;
        public readonly Dictionary<string, Entry> entries = new Dictionary<string, Entry>(StringComparer.InvariantCultureIgnoreCase);

        public Group(string className)
        {
            ClassName = className;
            fileName = className + ".cs";
        }

        public void Write(string directoryName)
        {
            fileName.MakeFile(directoryName, (writer) =>
            {
                writer.WriteLine("        public sealed class {0} : MediaType", ClassName);
                writer.WriteLine("        {");
                writer.WriteLine("            public {0}(string value, string[] extensions) : base(\"{1}/\" + value, extensions) {{}}", ClassName, ClassName.ToLowerInvariant());
                writer.WriteLine();
                writer.WriteLine("            public {0}(string value) : this(value, null) {{}}", ClassName);
                writer.WriteLine();
                var sortedEntries = entries.Values.OrderBy(e => e.FieldName);
                foreach (var entry in sortedEntries)
                {
                    entry.Write(writer);
                }
                writer.WriteLine();
                writer.WriteLine("            public static readonly new {0}[] Values = {{", ClassName);
                foreach (var entry in sortedEntries.Where(e => e.DeprecationMessage == null))
                {
                    writer.WriteLine("                {0},", entry.FieldName);
                }
                writer.WriteLine("            };");
                writer.WriteLine("        }");
            });
        }
    }
}