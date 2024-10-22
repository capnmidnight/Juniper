namespace Juniper.Collections;

public static class Tree
{
    public static Tree<ValueT> ToTree<KeyT, ValueT>(this IEnumerable<ValueT> items,
        Func<ValueT, KeyT> getKey,
        Func<ValueT, KeyT?> getParentKey,
        Func<ValueT, int>? getOrder = null)
        where KeyT : class
        where ValueT : notnull
    {
        var rootNode = new Tree<ValueT>();
        var nodes = new Dictionary<KeyT, Tree<ValueT>>();

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var node = new Tree<ValueT>(item);
            nodes.Add(nodeID, node);
        }

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var node = nodes[nodeID];
            var parentNodeID = getParentKey(item);
            var parentNode = parentNodeID is not null
                && nodes.TryGetValue(parentNodeID, out var value)
                    ? value
                    : rootNode;
            var index = parentNode.Children.Count;
            if (getOrder is not null)
            {
                index = getOrder(item);
            }
            parentNode.Connect(node, index);
        }

        return rootNode;
    }

    public static Tree<ValueT> ToTree<ItemT, KeyT, ValueT>(this IEnumerable<ItemT> items,
        Func<ItemT, KeyT> getKey,
        Func<ItemT, KeyT?> getParentKey,
        Func<ItemT, ValueT> getValue,
        Func<ItemT, int>? getOrder = null)
        where ItemT : notnull
        where KeyT : class
        where ValueT : notnull
    {
        var rootNode = new Tree<ValueT>();
        var nodes = new Dictionary<KeyT, Tree<ValueT>>();

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var value = getValue(item);
            var node = new Tree<ValueT>(value);
            nodes.Add(nodeID, node);
        }

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var node = nodes[nodeID];
            var parentNodeID = getParentKey(item);
            var parentNode = parentNodeID is not null
                && nodes.TryGetValue(parentNodeID, out var value)
                    ? value
                    : rootNode;
            var index = parentNode.Children.Count;
            if (getOrder is not null)
            {
                index = getOrder(item);
            }
            parentNode.Connect(node, index);
        }

        return rootNode;
    }

    public static Tree<ValueT> ToTree<ItemT, KeyT, ValueT>(this IEnumerable<ItemT> items,
        Func<ItemT, KeyT> getKey,
        Func<ItemT, KeyT?> getParentKey,
        Func<ItemT, ValueT> getValue,
        Func<ItemT, int>? getOrder = null)
        where ItemT : notnull
        where KeyT : struct
        where ValueT : notnull
    {
        var rootNode = new Tree<ValueT>();
        var nodes = new Dictionary<KeyT, Tree<ValueT>>();

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var value = getValue(item);
            var node = new Tree<ValueT>(value);
            nodes.Add(nodeID, node);
        }

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var node = nodes[nodeID];
            var parentNodeID = getParentKey(item);
            var parentNode = parentNodeID.HasValue
                && nodes.TryGetValue(parentNodeID.Value, out var value) 
                    ? value 
                    : rootNode;
            var index = parentNode.Children.Count;
            if (getOrder is not null)
            {
                index = getOrder(item);
            }
            parentNode.Connect(node, index);
        }

        return rootNode;
    }

    public static Tree<ValueT> ToTree<KeyT, ValueT>(this IEnumerable<ValueT> items,
        Func<ValueT, KeyT> getKey,
        Func<ValueT, KeyT?> getParentKey,
        Func<ValueT, int>? getOrder = null)
        where KeyT : struct
        where ValueT : notnull
    {
        var rootNode = new Tree<ValueT>();
        var nodes = new Dictionary<KeyT, Tree<ValueT>>();

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var node = new Tree<ValueT>(item);
            nodes.Add(nodeID, node);
        }

        foreach (var item in items)
        {
            var nodeID = getKey(item);
            var node = nodes[nodeID];
            var parentNodeID = getParentKey(item);
            var parentNode = parentNodeID.HasValue
                && nodes.TryGetValue(parentNodeID.Value, out var value) 
                    ? value 
                    : rootNode;
            var index = parentNode.Children.Count;
            if (getOrder is not null)
            {
                index = getOrder(item);
            }
            parentNode.Connect(node, index);
        }

        return rootNode;
    }

    public static IEnumerable<ValueT> EnumerateBreadthFirst<ValueT>(this ValueT rootItem, Func<ValueT, IEnumerable<ValueT>> getChildren)
        where ValueT: notnull
    {
        var queue = new Queue<ValueT> { rootItem };
        while(queue.Count > 0)
        {
            var here = queue.Dequeue();
            foreach (var child in getChildren(here))
            {
                queue.Enqueue(child);
            }
            yield return here;
        }
    }

    public static Tree<ValueT> ToTree<KeyT, ValueT>(this ValueT rootItem,
        Func<ValueT, KeyT> getKey,
        Func<ValueT, KeyT?> getParentKey,
        Func<ValueT, IEnumerable<ValueT>> getChildren,
        Func<ValueT, int>? getOrder = null)
        where KeyT : struct
        where ValueT : notnull =>
        rootItem.EnumerateBreadthFirst(getChildren).ToTree(getKey, getParentKey, getOrder);
}


/// <summary>
/// A node in an N-ary tree.
/// </summary>
/// <typeparam name="T">Any type of object</typeparam>
public partial class Tree<T> where T : notnull
{
    /// <summary>
    /// The value stored in this node.
    /// </summary>
    public T? Value { get; internal set; }

    protected List<Tree<T>> ChildNodes { get; } = new List<Tree<T>>();

    /// <summary>
    /// All nodes below the current node.
    /// </summary>
    public IReadOnlyList<Tree<T>> Children => ChildNodes;

    /// <summary>
    /// The next node above the current node.
    /// </summary>
    public Tree<T>? Parent { get; private set; }

    public Tree()
    { }

    /// <summary>
    /// Creates a root node with no children.
    /// </summary>
    /// <param name="value">The value to store at the root.</param>
    public Tree(T value)
    {
        Value = value;
    }

    /// <summary>
    /// How deep into the tree is this branch
    /// </summary>
    public int Depth
    {
        get
        {
            var depth = 0;
            var here = Parent;
            while (here is not null)
            {
                ++depth;
                here = here.Parent;
            }

            return depth;
        }
    }

    /// <summary>
    /// Returns true if there are no child nodes.
    /// </summary>
    public bool IsLeaf => ChildNodes.Count == 0;

    /// <summary>
    /// Returns true if this node has no parent node.
    /// </summary>
    public bool IsRoot => Parent is null;

    public Tree<T> Add(T value)
    {
        return Connect(new Tree<T>(value));
    }

    public IEnumerable<Tree<T>> AddRange(IEnumerable<T> values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        return ConnectRange(values.Select(v => new Tree<T>(v)));
    }

    public IEnumerable<Tree<T>> ConnectRange(IEnumerable<Tree<T>> nodes)
    {
        if (nodes is null)
        {
            throw new ArgumentNullException(nameof(nodes));
        }

        foreach (var node in nodes)
        {
            node.Parent = this;
            ChildNodes.Add(node);
            yield return node;
        }
    }

    public Tree<T> Connect(Tree<T> node)
    {
        return Connect(node, ChildNodes.Count);
    }

    public Tree<T> Connect(Tree<T> node, int index)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (index < 0 || ChildNodes.Count < index)
        {
            throw new IndexOutOfRangeException();
        }

        node.Parent = this;
        ChildNodes.Insert(index, node);
        return node;
    }

    public IEnumerable<T> ValuesDepthFirst()
    {
        foreach (var node in NodesDepthFirst())
        {
            if (node.Value is not null)
            {
                yield return node.Value;
            }
        }
    }

    public IEnumerable<Tree<T>> NodesDepthFirst()
    {
        var toVisit = new Stack<Tree<T>>();
        toVisit.Push(this);
        while (toVisit.Count > 0)
        {
            var here = toVisit.Pop();
            yield return here;

            if (here.Children.Count > 0)
            {
                foreach (var child in here.Children.AsEnumerable().Reverse())
                {
                    toVisit.Push(child);
                }
            }
        }
    }

    public IEnumerable<T> ValuesBreadthFirst()
    {
        foreach (var node in NodesBreadthFirst())
        {
            if (node.Value is not null)
            {
                yield return node.Value;
            }
        }
    }

    public IEnumerable<Tree<T>> NodesBreadthFirst()
    {
        var toVisit = new Queue<Tree<T>>();
        toVisit.Enqueue(this);
        while (toVisit.Count > 0)
        {
            var here = toVisit.Dequeue();
            yield return here;

            if (here.Children.Count > 0)
            {
                foreach (var child in here.Children.AsEnumerable().Reverse())
                {
                    toVisit.Enqueue(child);
                }
            }
        }
    }

    public Tree<T> Trim(Func<Tree<T>, bool> filter)
    {
        var toVisit = new Queue<Tree<T>>();
        toVisit.Enqueue(this);
        while (toVisit.Count > 0)
        {
            var here = toVisit.Dequeue();
            if (filter(here))
            {
                here.RemoveFromParent();
            }
            else if (here.Children.Count > 0)
            {
                foreach (var child in here.Children.AsEnumerable().Reverse())
                {
                    toVisit.Enqueue(child);
                }
            }
        }

        return this;
    }

    public bool Contains(Tree<T> node)
    {
        var here = node;
        while (here is not null)
        {
            if (here == this)
            {
                return true;
            }

            here = here.Parent;
        }

        return false;
    }

    public void Remove(Tree<T> child)
    {
        if (child.Parent == this)
        {
            child.Parent = null;
            ChildNodes.Remove(child);
        }
    }

    public void RemoveFromParent()
    {
        Parent?.Remove(this);
    }

    public Tree<T>? Find(T? value)
    {
        foreach (var node in NodesBreadthFirst())
        {
            if (Equals(value, node.Value))
            {
                return node;
            }
        }

        return null;
    }

    public Tree<T>? FindByKey<K>(K? key1, Func<T, K?> getKey)
        where K : class
    {
        if (key1 is null)
        {
            if (IsRoot && Value is null)
            {
                return this;
            }

            return null;
        }

        foreach (var node in NodesBreadthFirst())
        {
            if (node.Value is not null)
            {
                var key2 = getKey(node.Value);
                if (Equals(key1, key2))
                {
                    return node;
                }
            }
        }

        return null;
    }

    public Tree<T>? FindByKey<K>(K? key1, Func<T, K?> getKey)
        where K : struct
    {
        if (key1 is null)
        {
            if (IsRoot && Value is null)
            {
                return this;
            }

            return null;
        }

        foreach (var node in NodesBreadthFirst())
        {
            if (node.Value is not null)
            {
                var key2 = getKey(node.Value);
                if (Equals(key1, key2))
                {
                    return node;
                }
            }
        }

        return null;
    }
}