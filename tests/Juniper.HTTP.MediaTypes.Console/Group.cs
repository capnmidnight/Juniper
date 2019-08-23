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
            this.ClassName = className;
            fileName = className + ".cs";
        }

        public void Write(string directoryName)
        {
            fileName.MakeFile(directoryName, (writer) =>
            {
                writer.WriteLine("        public sealed class {0} : MediaType", ClassName);
                writer.WriteLine("        {");
                writer.Write("            public {0}(string value, string[] extensions = null)", ClassName);
                writer.WriteLine(" : base(\"{0}/\" + value, extensions) {{}}", ClassName.ToLowerInvariant());
                writer.WriteLine();
                foreach (var entry in entries.Values.OrderBy(e => e.FieldName))
                {
                    entry.Write(writer);
                }
                writer.WriteLine("        }");
            });
        }
    }
}