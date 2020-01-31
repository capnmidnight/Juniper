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
                writer.WriteLine("            private {0}(string value, string[] extensions) : base(\"{1}/\" + value, extensions) {{ }}", ClassName, ClassName.ToLowerInvariant());
                writer.WriteLine();
                writer.WriteLine("            private {0}(string value) : this(value, null) {{ }}", ClassName);
                writer.WriteLine();
                writer.WriteLine("            public static readonly {0} Any{0} = new {0}(\"*\");", ClassName);
                writer.WriteLine();
                writer.WriteLine("            public override bool Matches(string fileName)");
                writer.WriteLine("            {");
                writer.WriteLine("                if (ReferenceEquals(this, Any{0}))", ClassName);
                writer.WriteLine("                {");
                writer.WriteLine("                    return Values.Any(x => x.Matches(fileName));");
                writer.WriteLine("                }");
                writer.WriteLine("                else");
                writer.WriteLine("                {");
                writer.WriteLine("                    return base.Matches(fileName);");
                writer.WriteLine("                }");
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
                writer.WriteLine("            public static new readonly {0}[] Values = {{", ClassName);
                var values = sortedEntries.Where(e => e.DeprecationMessage == null);
                var last = values.Last();
                var rest = values.SkipLast(1);
                foreach (var entry in rest)
                {
                    writer.WriteLine("                {0},", entry.FieldName);
                }
                writer.WriteLine("                {0}", last.FieldName);
                writer.WriteLine("            };");
                writer.WriteLine("        }");
            });
        }
    }
}