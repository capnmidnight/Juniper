using System.Xml.Linq;

namespace Juniper.Xml
{
    public static class XmlSpreadsheet2003Extensions
    {
        public static XmlSpreadsheet2003.Table Union(this IEnumerable<XmlSpreadsheet2003.Table> table)
        {
            var first = table.First();
            var rest = table.Skip(1).All(t => t.IsMatch(first.Headers));
            return new XmlSpreadsheet2003.Table(table.SelectMany(t => t.Rows));
        }
    }

    public class XmlSpreadsheet2003
    {
        private static readonly XName WORKSHEET = XName.Get("Worksheet", "urn:schemas-microsoft-com:office:spreadsheet");
        private static readonly XName TABLE = XName.Get("Table", "urn:schemas-microsoft-com:office:spreadsheet");
        private static readonly XName ROW = XName.Get("Row", "urn:schemas-microsoft-com:office:spreadsheet");
        private static readonly XName CELL = XName.Get("Cell", "urn:schemas-microsoft-com:office:spreadsheet");
        private static readonly XName DATA = XName.Get("Data", "urn:schemas-microsoft-com:office:spreadsheet");

        private static readonly XName NAME = XName.Get("Name", "urn:schemas-microsoft-com:office:spreadsheet");
        private static readonly XName INDEX = XName.Get("Index", "urn:schemas-microsoft-com:office:spreadsheet");

        public static XmlSpreadsheet2003 Load(FileInfo file)
        {

            using var stream = new StreamReader(file.FullName);
            var root = XElement.Load(stream);
            return new XmlSpreadsheet2003(file.Name, root);
        }

        public string Name { get; }

        public Dictionary<string, Worksheet> Worksheets { get; }

        private XmlSpreadsheet2003(string name, XElement root)
        {
            Name = name;
            Worksheets = root.Elements(WORKSHEET)
                .Select(e => new Worksheet(e))
                .ToDictionary(s => s.Name);
        }

        public class Worksheet
        {
            public string Name { get; }

            public Table Table { get; }

            internal Worksheet(XElement element)
            {
                Name = element.Attribute(NAME).Value;
                Table = new Table(element.Element(TABLE));
            }
        }

        public class Table
        {
            public string[] Headers { get; }
            public List<Row> Rows { get; }

            public string SortKey => Headers
                .ToArray()
                .Join(", ");

            public bool IsMatch(params string[] headerPattern)
            {
                var temp = headerPattern.ToArray();
                Array.Sort(temp);
                return temp.Length == Headers.Length
                    && temp
                        .Select((v, i) => v == Headers[i])
                        .All(v => v);
            }

            internal Table(IEnumerable<Row> rows)
            {
                Headers = rows.SelectMany(r => r.Headers).Distinct().ToArray();
                Array.Sort(Headers);
                Rows = rows.ToList();
            }

            public bool IsUnderused => Rows.Any(r => r.IsUnderused);

            public IEnumerable<Row> UnderusedRows => Rows.Where(r => r.IsUnderused);

            internal Table(XElement element)
            {
                if (element is null)
                {
                    throw new Exception("Excpected a Table element");
                }

                var rows = new Queue<XElement>(element.Elements(ROW));
                Headers = (from cell in rows.Dequeue().Elements(CELL)
                           let header = cell.Element(DATA)
                           select header.Value)
                            .ToArray();

                Rows = new List<Row>();
                foreach (var row in rows)
                {
                    var dict = new Dictionary<string, string>();
                    foreach (var header in Headers)
                    {
                        dict.Add(header, "");
                    }
                    Rows.Add(new Row(dict));
                    var index = 0;
                    foreach (var cell in row.Elements(CELL))
                    {
                        var indexStr = cell.Attribute(INDEX)?.Value;
                        if (indexStr is not null && int.TryParse(indexStr, out var skipIndex))
                        {
                            index = skipIndex - 1;
                        }

                        if (0 <= index && index < Headers.Length)
                        {
                            var val = cell.Element(DATA)?.Value ?? "";
                            dict[Headers[index]] = val;
                            ++index;
                        }
                    }
                }

                Array.Sort(Headers);
            }
        }

        public class Row
        {
            private Dictionary<string, string> data;
            private Dictionary<string, bool> accessed;

            internal Row(Dictionary<string, string> data)
            {
                this.data = data;
                accessed = data.Keys.ToDictionary(v => v, _ => false);
            }

            public IEnumerable<string> Headers => data.Keys;

            public bool Has(string header) => data.ContainsKey(header);

            public string Peek(string header) => data.Get(header);

            public string this[string header]
            {
                get
                {
                    if (!Has(header))
                    {
                        return null;
                    }

                    accessed[header] = true;
                    return data[header];
                }
            }

            public bool IsUnderused => accessed.Any(kv => !kv.Value);

            public string[] UnusedFields =>
                (from kv in accessed
                 where !kv.Value
                 select kv.Key)
                .ToArray();
        }
    }
}