using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Collections
{
    public static class NAryTree
    {
        public static NAryTree<V> ToTree<K, V>(this IEnumerable<V> items,
            Func<V, K> getKey,
            Func<V, K> getParentKey,
            Func<V, int> getOrder = null)
        {
            var rootNode = new NAryTree<V>(default);
            var nodes = new Dictionary<K, NAryTree<V>>();

            foreach (var item in items)
            {
                var nodeID = getKey(item);
                var node = new NAryTree<V>(item);
                nodes.Add(nodeID, node);
            }

            foreach (var node in nodes.Values)
            {
                var parentNodeID = getParentKey(node.Value);
                var isParentNodeDefined = parentNodeID != null;
                var hasParentNode = isParentNodeDefined && nodes.ContainsKey(parentNodeID);
                var parentNode = !isParentNodeDefined
                    ? rootNode
                    : hasParentNode
                        ? nodes[parentNodeID]
                        : null;

                if (parentNode is not null)
                {
                    var index = parentNode.Children.Count;
                    if (getOrder is not null)
                    {
                        index = getOrder(node.Value);
                    }
                    parentNode.Connect(node, index);
                }
            }

            return rootNode;
        }
    }

    /// <summary>
    /// A node in an N-ary tree.
    /// </summary>
    /// <typeparam name="T">Any type of object</typeparam>
    public partial class NAryTree<T>
    {
        /// <summary>
        /// The value stored in this node.
        /// </summary>
        public T Value { get; internal set; }

        protected List<NAryTree<T>> ChildNodes { get; } = new List<NAryTree<T>>();

        /// <summary>
        /// All nodes below the current node.
        /// </summary>
        public IReadOnlyList<NAryTree<T>> Children => ChildNodes;

        /// <summary>
        /// The next node above the current node.
        /// </summary>
        public NAryTree<T> Parent { get; private set; }

        public NAryTree()
        { }

        /// <summary>
        /// Creates a root node with no children.
        /// </summary>
        /// <param name="value">The value to store at the root.</param>
        public NAryTree(T value)
        {
            Value = value;
        }

        /// <summary>
        /// How deep into the tree is this branch
        /// </summary>
        protected int Depth
        {
            get
            {
                var depth = 0;
                var here = this;
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

        public void Add(T value)
        {
            Connect(new NAryTree<T>(value));
        }

        public void AddRange(IEnumerable<T> values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            AddRange(values.Select(v => new NAryTree<T>(v)));
        }

        public void AddRange(IEnumerable<NAryTree<T>> nodes)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            foreach (var node in nodes)
            {
                node.Parent = this;
                ChildNodes.Add(node);
            }
        }

        public void Connect(NAryTree<T> node)
        {
            Connect(node, ChildNodes.Count);
        }

        public void Connect(NAryTree<T> node, int index)
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
        }

        public IEnumerable<T> ValuesDepthFirst()
        {
            foreach (var node in NodesDepthFirst())
            {
                yield return node.Value;
            }
        }

        public IEnumerable<NAryTree<T>> NodesDepthFirst()
        {
            var toVisit = new Stack<NAryTree<T>>();
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
                yield return node.Value;
            }
        }

        public IEnumerable<NAryTree<T>> NodesBreadthFirst()
        {
            var toVisit = new Queue<NAryTree<T>>();
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

        public NAryTree<T> Trim(Func<NAryTree<T>, bool> filter)
        {
            var toVisit = new Queue<NAryTree<T>>();
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

        public bool Contains(NAryTree<T> node)
        {
            while (node is not null)
            {
                if (node == this)
                {
                    return true;
                }

                node = node.Parent;
            }

            return false;
        }

        public void Remove(NAryTree<T> child)
        {
            if (child.Parent == this)
            {
                child.Parent = null;
                ChildNodes.Remove(child);
            }
        }

        public void RemoveFromParent()
        {
            if (Parent is not null)
            {
                Parent.Remove(this);
            }
        }
    }
}