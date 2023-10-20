#nullable enable

using System.Xml.Linq;

namespace Juniper.Xml
{
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

        public Worksheet[] Worksheets { get; }

        private XmlSpreadsheet2003(string name, XElement root)
        {
            Name = name;
            Worksheets = root.Elements(WORKSHEET)
                .Select(e => new Worksheet(e))
                .ToArray();
        }

        public void GlobalReplaceValue(string v1, string v2)
        {
            foreach(var worksheet in Worksheets)
            {
                worksheet.Table.GlobalReplaceValue(v1, v2);
            }
        }

        public class Worksheet
        {
            public string Name { get; }

            public Table Table { get; }

            internal Worksheet(XElement element)
            {
                Name = element.Attribute(NAME)!.Value;
                Table = new Table(this, element.Element(TABLE)!);
            }
        }

        public class Table
        {
            public HashSet<string> Headers { get; }
            public Row[] Rows { get; }

            public Worksheet Sheet { get; }

            public bool IsUnderused => Rows.Any(r => r.IsUnderused);

            public IEnumerable<Row> UnderusedRows => Rows.Where(r => r.IsUnderused);

            internal Table(Worksheet sheet, XElement element)
            {
                if (element is null)
                {
                    throw new Exception("Expected a Table element");
                }

                Sheet = sheet;

                var rows = new Queue<XElement>(element.Elements(ROW));
                var headers = (from cell in rows.Dequeue().Elements(CELL)
                               let header = cell.Element(DATA)
                               select header.Value)
                            .ToArray();

                Headers = new(headers);

                Rows = rows.Select(row =>
                {
                    var dict = new Dictionary<string, string>();
                    var index = 0;
                    foreach (var cell in row.Elements(CELL))
                    {
                        var indexStr = cell.Attribute(INDEX)?.Value;
                        if (indexStr is not null && int.TryParse(indexStr, out var skipIndex))
                        {
                            index = skipIndex - 1;
                        }

                        if (0 <= index && index < headers.Length)
                        {
                            var val = cell.Element(DATA)?.Value ?? "";
                            dict[headers[index]] = val;
                            ++index;
                        }
                    }

                    return new Row(this, dict);
                })
                    .ToArray();

            }

            public bool Has(params string[] headers) =>
                headers.All(Headers.Contains);

            public bool Matches(params string[] headers) =>
                headers.Length == Headers.Count
                && Has(headers);

            public void GlobalReplaceValue(string v1, string v2)
            {
                foreach(var row in Rows)
                {
                    row.GlobalReplaceValue(v1, v2);
                }
            }
        }

        public class Row
        {
            public Dictionary<string, Cell> Cells { get; }

            public Table Table { get; }

            internal Row(Table table, Dictionary<string, string> data)
            {
                Table = table;
                Cells = data.ToDictionary(kv => kv.Key, kv => new Cell(this, kv.Key, kv.Value));
            }

            public bool Has(params string[] headers) => headers.All(Cells.ContainsKey);

            public string? Peek(string header) => Cells.Get(header)?.Peek;

            public void GlobalReplaceValue(string v1, string v2)
            {
                foreach(var cell in Cells.Values)
                {
                    cell.ReplaceValue(v1, v2);
                }
            }

            public string? this[string header]
            {
                get
                {
                    if (!Has(header))
                    {
                        return null;
                    }

                    return Cells[header].Value;
                }
            }

            public bool IsUnderused => Cells.Values.Any(v => !v.Accessed);

            public string[] UnusedFields =>
                (from kv in Cells
                 where !kv.Value.Accessed
                 select kv.Key)
                .ToArray();
        }

        public class Cell
        {
            public Row Row { get; }

            public string Key { get; }

            private string value;

            public string Value
            {
                get
                {
                    Accessed = true;
                    return value;
                }
            }

            /// <summary>
            /// Get the value without modifying the Accessed flag
            /// </summary>
            public string Peek => value;

            /// <summary>
            /// A flag indicating that this cell's value has been accessed. Useful
            /// for importers that do advanced validation that want to make sure they
            /// use all of the cell values in a work sheet.
            /// </summary>
            public bool Accessed { get; private set; }

            internal Cell(Row row, string key, string value)
            {
                Row = row;
                Key = key;
                this.value = value;
            }

            public void ReplaceValue(string v1, string v2)
            {
                if (value.Trim().ToLower() == v1.ToLower())
                {
                    value = v2;
                }
            }
        }
    }
}