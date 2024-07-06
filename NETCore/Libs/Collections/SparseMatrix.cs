using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper.Collections;

[Serializable]
public class SparseMatrix<KeyT, ValueT> : ISaveable<SparseMatrix<KeyT, ValueT>>
    where KeyT : notnull
    where ValueT : notnull
{
    private readonly Dictionary<KeyT, Dictionary<KeyT, ValueT>> map;

    public SparseMatrix()
    {
        map = new();
    }

    protected SparseMatrix(SerializationInfo info, StreamingContext context)
        : this()
    {
        var entries = info.GetValue<(KeyT x, KeyT y, ValueT v)[]>(nameof(map));
        if (entries is not null)
        {
            foreach ((var x, var y, var v) in entries)
            {
                if (!ContainsColumn(x))
                {
                    map.Add(x, new Dictionary<KeyT, ValueT>());
                }

                map[x].Add(y, v);
            }
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        var entries = new List<(KeyT x, KeyT y, ValueT v)>();
        foreach (var (x, col) in map)
        {
            foreach (var (y, value) in col)
            {
                entries.Add((x, y, value));
            }
        }
        info.AddValue(nameof(map), entries.ToArray());
    }

    public ValueT? this[KeyT x, KeyT y]
    {

        get => Contains(x, y) ? map[x][y] : default;
        set
        {
            if (value is not null)
            {
                if (!map.TryGetValue(x, out var row))
                {
                    row = new Dictionary<KeyT, ValueT>();
                    map.Add(x, row);
                }

                row[y] = value;
            }
            else
            {
                if (map.TryGetValue(x, out var row))
                {
                    row.Remove(y);
                    if (row.Count == 0)
                    {
                        map.Remove(x);
                    }
                }
            }
        }
    }

    public IEnumerable<ValueT> Values =>
        from col in map.Values
        from cell in col
        where cell.Value is not null
        select cell.Value;

    public IEnumerable<(KeyT x, KeyT y, ValueT value)> Entries =>
        from col in map
        from cell in col.Value
        select (col.Key, cell.Key, cell.Value);

    public IEnumerable<KeyT> Columns => map.Keys;

    public IEnumerable<KeyT> ColumnCells(KeyT x) => map[x].Keys;

    public IEnumerable<KeyT> Rows =>
        (from x in Columns
         from cell in ColumnCells(x)
         select cell)
        .Distinct();

    public IEnumerable<KeyT> RowCells(KeyT y)
    {
        foreach (var column in Columns)
        {
            if (map[column].ContainsKey(y))
            {
                yield return column;
            }
        }
    }

    public IEnumerable<KeyT> Keys =>
        Columns
            .Union(Rows)
            .Distinct();

    public int Count =>
        map.Values.Sum(col => col.Count);

    public void Add(KeyT x, KeyT y, ValueT value)
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

    public bool ContainsColumn(KeyT x)
    {
        return map.ContainsKey(x);
    }

    public bool ContainsRow(KeyT y)
    {
        return map.Any(col => col.Value.ContainsKey(y));
    }

    public bool Contains(KeyT x, KeyT y)
    {
        return map.ContainsKey(x)
            && map[x].ContainsKey(y);
    }

    public bool RemoveColumn(KeyT x)
    {
        if (!ContainsColumn(x))
        {
            return false;
        }

        map[x].Clear();
        map.Remove(x);

        return true;
    }

    public bool RemoveRow(KeyT y)
    {
        var removed = false;
        var toRemove = new List<KeyT>();
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

    public bool Remove(KeyT x, KeyT y)
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
