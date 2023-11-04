namespace Juniper.Collections
{
    public static class Tree
    {
        public static Tree<ValueT> ToTree<KeyT, ValueT>(this IEnumerable<ValueT> items,
            Func<ValueT, KeyT> getKey,
            Func<ValueT, KeyT> getParentKey,
            Func<ValueT, int>? getOrder = null)
            where KeyT : notnull
            where ValueT : notnull
        {
            return items.ToTree(getKey, getParentKey, Always.Identity, getOrder);
        }

        public static Tree<ValueT> ToTree<NodeT, KeyT, ValueT>(this IEnumerable<NodeT> items,
            Func<NodeT, KeyT> getKey,
            Func<NodeT, KeyT> getParentKey,
            Func<NodeT, ValueT> getValue,
            Func<NodeT, int>? getOrder = null)
            where KeyT : notnull
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
                    && nodes.ContainsKey(parentNodeID)
                        ? nodes[parentNodeID]
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
            Tree<T>? here = node;
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

        public Tree<T>? Find(T value)
        {
            return FindByKey(value, Always.Identity);
        }

        public Tree<T>? FindByKey<K>(K key1, Func<T, K> getKey) where K : notnull
        {
            var isClass = typeof(K).IsClass;
            foreach (var node in NodesBreadthFirst())
            {
                if (node.Value is not null)
                {
                    var key2 = getKey(node.Value);
                    if (isClass && ReferenceEquals(key1, key2)
                        || !isClass && key1.Equals(key2))
                    {
                        return node;
                    }
                }
            }

            return null;
        }
    }
}