namespace Juniper.Collections
{
    public static class Graph
    {
        public static Graph<KeyT, ValueT> ToGraph<KeyT, ValueT>(this IEnumerable<ValueT> items, Func<ValueT, KeyT> getFromKey, Func<ValueT, KeyT> getToKey)
        {
            return items.ToGraph(getFromKey, getToKey, Always.Identity);
        }

        public static Graph<KeyT, ValueT> ToGraph<NodeT, KeyT, ValueT>(this IEnumerable<NodeT> items, Func<NodeT, KeyT> getFromKey, Func<NodeT, KeyT> getToKey, Func<NodeT, ValueT> getValue)
        {
            var graph = new Graph<KeyT, ValueT>();
            foreach (var item in items)
            {
                graph.Add(new Edge<KeyT, ValueT>(getFromKey(item), getToKey(item), getValue(item)));
            }

            return graph;
        }
    }

    public class Graph<KeyT, ValueT>
    {
        private SparseMatrix<KeyT, List<Edge<KeyT, ValueT>>> matrix = new();

        public void Add(Edge<KeyT, ValueT> edge)
        {
            if (!matrix.Contains(edge.From, edge.To))
            {
                matrix.Add(edge.From, edge.To, new List<Edge<KeyT, ValueT>>());
            }

            matrix[edge.From, edge.To].Add(edge);
        }

        public IEnumerable<Edge<KeyT, ValueT>> Flood(KeyT from)
        {
            var visited = new HashSet<KeyT>();
            var queue = new Queue<KeyT> { from };
            while (queue.Count > 0)
            {
                var here = queue.Dequeue();
                visited.Add(here);

                if (matrix.ContainsColumn(here))
                {
                    var cells = matrix.ColumnCells(here);
                    queue.AddRange(cells.Where(v => !visited.Contains(v)));

                    foreach (var cell in cells)
                    {
                        var edges = matrix[here, cell];
                        foreach (var edge in edges)
                        {
                            yield return edge;
                        }
                    }
                }

                if (matrix.ContainsRow(here))
                {
                    var cells = matrix.RowCells(here);
                    queue.AddRange(cells.Where(v => !visited.Contains(v)));

                    foreach (var cell in cells)
                    {
                        var edges = matrix[cell, here];
                        foreach (var edge in edges)
                        {
                            yield return edge;
                        }
                    }
                }
            }
        }
    }
}