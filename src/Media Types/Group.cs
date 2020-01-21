using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.MediaTypes
{
    internal class Group
    {
        public string TypeFileName { get; }

        public string ValuesFileName { get; }

        public string ClassName { get; }

        public Dictionary<string, Entry> Entries { get; } = new Dictionary<string, Entry>(StringComparer.InvariantCultureIgnoreCase);

        public Group(string className)
        {
            ClassName = className;
            TypeFileName = className + ".cs";
            ValuesFileName = className + ".Values.cs";
        }

        public void Write(string directoryName)
        {
            TypeFileName.MakeFile(directoryName, (writer) =>
            {
                writer.WriteLine("        public sealed partial class {0} : MediaType", ClassName);
                writer.WriteLine("        {");
                writer.WriteLine("            private {0}(string value, string[] extensions) : base(\"{1}/\" + value, extensions) {{}}", ClassName, ClassName.ToLowerInvariant());
                writer.WriteLine();
                writer.WriteLine("            private {0}(string value) : this(value, null) {{}}", ClassName);
                writer.WriteLine();
                writer.WriteLine("            public static readonly {0} Any{0} = new {0}(\"*\");", ClassName);
                writer.WriteLine();
                writer.WriteLine("            public override bool Matches(string fileName)");
                writer.WriteLine("            {");
                writer.WriteLine("                return base.Matches(fileName)");
                writer.WriteLine("                        || this == Any{0}", ClassName);
                writer.WriteLine("                            && Values.Any(v => v.Matches(fileName));");
                writer.WriteLine("            }");
                writer.WriteLine("        }");
            }, "using System.Linq;");

            ValuesFileName.MakeFile(directoryName, (writer) =>
            {
                writer.WriteLine("        public sealed partial class {0} : MediaType", ClassName);
                writer.WriteLine("        {");
                var sortedEntries = Entries.Values.OrderBy(e => e.FieldName);
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