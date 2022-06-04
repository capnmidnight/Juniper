using Juniper.IO;

using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class SparseMatrix<IndexT, ValueT> : ISaveable<SparseMatrix<IndexT, ValueT>>
        where IndexT : IComparable<IndexT>
        where ValueT : IComparable<ValueT>

    {
        private readonly Dictionary<IndexT, Dictionary<IndexT, ValueT>> map;

        public SparseMatrix()
        {
            map = new();
        }

        protected SparseMatrix(SerializationInfo info, StreamingContext context)
            : this()
        {
            var entries = info.GetValue<(IndexT x, IndexT y, ValueT v)[]>(nameof(map));
            foreach ((var x, var y, var v) in entries)
            {
                if (!ContainsColumn(x))
                {
                    map.Add(x, new Dictionary<IndexT, ValueT>());
                }

                map[x].Add(y, v);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var entries = new List<(IndexT x, IndexT y, ValueT v)>();
            foreach (var (x, col) in map)
            {
                foreach (var (y, value) in col)
                {
                    entries.Add((x, y, value));
                }
            }
            info.AddValue(nameof(map), entries.ToArray());
        }

        public ValueT this[IndexT x, IndexT y]
        {

            get => Contains(x, y) ? map[x][y] : default;
            set
            {
                if (!map.ContainsKey(x))
                {
                    map.Add(x, new Dictionary<IndexT, ValueT>());
                }

                map[x][y] = value;
            }
        }

        public IEnumerable<ValueT> Values =>
            from col in map.Values
            from cell in col
            select cell.Value;

        public IEnumerable<(IndexT x, IndexT y, ValueT value)> Entries =>
            from col in map
            from cell in col.Value
            select (col.Key, cell.Key, cell.Value);

        public IEnumerable<IndexT> Columns => map.Keys;
        public IEnumerable<IndexT> Cells(IndexT x) => map[x].Keys;
        public IEnumerable<IndexT> Rows =>
            (from x in Columns
             from cell in Cells(x)
             select cell)
            .Distinct();

        public IEnumerable<IndexT> Keys =>
            Columns
                .Union(Rows)
                .Distinct();

        public int Count =>
            map.Values.Sum(col => col.Count);

        public void Add(IndexT x, IndexT y, ValueT value)
        {
            if (Contains(x, y))
            {
                throw new InvalidOperationException("Entry already exists");
            }

            this[x, y] = value;
        }

        public void Clear()
        {
            map.Clear();
        }

        public bool ContainsColumn(IndexT x)
        {
            return map.ContainsKey(x);
        }

        public bool ContainsRow(IndexT y)
        {
            return map.Any(col => col.Value.ContainsKey(y));
        }

        public bool Contains(IndexT x, IndexT y)
        {
            return map.ContainsKey(x)
                && map[x].ContainsKey(y);
        }

        public bool RemoveColumn(IndexT x)
        {
            if (!ContainsColumn(x))
            {
                return false;
            }

            map[x].Clear();
            map.Remove(x);

            return true;
        }

        public bool RemoveRow(IndexT y)
        {
            bool removed = false;
            var toRemove = new List<IndexT>();
            foreach (var (col, values) in map)
            {
                if (values.Remove(y))
                {
                    if (values.Count == 0)
                    {
                        toRemove.Add(col);
                    }
                    removed = true;
                }
            }

            foreach (var col in toRemove)
            {
                map.Remove(col);
            }

            return removed;
        }

        public bool Remove(IndexT x, IndexT y)
        {
            if (!Contains(x, y))
            {
                return false;
            }

            map[x].Remove(y);
            if (map[x].Count == 0)
            {
                map.Remove(x);
            }

            return true;
        }
    }
}
