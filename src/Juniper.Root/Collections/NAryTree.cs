using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Collections
{
    /// <summary>
    /// A node in an N-ary tree.
    /// </summary>
    /// <typeparam name="T">Any type of object</typeparam>
    public partial class NAryTree<T>
        where T : IEquatable<T>
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

        public int Count { get; private set; }

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
                while (here is object)
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

        private void Recount()
        {
            SearchNodesDepthFirst(
                (item) => item.Count = 1,
                null,
                (item) => item.Count += item.Children.Sum(child => child.Count));
        }

        public void Add(T value)
        {
            Add(new NAryTree<T>(value));
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
            Recount();
        }

        public void Add(NAryTree<T> node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            node.Parent = this;
            ChildNodes.Add(node);
            Recount();
        }

        public IEnumerable<T> ValuesDepthFirst
        {
            get
            {
                foreach (var node in NodesDepthFirst)
                {
                    yield return node.Value;
                }
            }
        }

        public IEnumerable<NAryTree<T>> NodesDepthFirst
        {
            get
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
        }

        public IEnumerable<T> ValuesBreadthFirst
        {
            get
            {
                foreach (var node in NodesBreadthFirst)
                {
                    yield return node.Value;
                }
            }
        }

        public IEnumerable<NAryTree<T>> NodesBreadthFirst
        {
            get
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
        }

        public void SearchValuesDepthFirst(Action<T> preItem, Action<T> perItem, Action<T> postItem)
        {
            SearchNodesDepthFirst(
                null,
                null,
                (node) => preItem(node.Value),
                (node) => perItem(node.Value),
                (node) => postItem(node.Value),
                null,
                null);
        }

        public void SearchValuesDepthFirst(Action<T> preGroup, Action<T> preItem, Action<T> perItem, Action<T> postItem, Action<T> postGroup)
        {
            SearchNodesDepthFirst(
                null,
                (node) => preGroup(node.Value),
                (node) => preItem(node.Value),
                (node) => perItem(node.Value),
                (node) => postItem(node.Value),
                (node) => postGroup(node.Value),
                null);
        }

        public void SearchValuesDepthFirst(Action start, Action<T> preGroup, Action<T> preItem, Action<T> perItem, Action<T> postItem, Action<T> postGroup, Action end)
        {
            SearchNodesDepthFirst(
                start,
                (node) => preGroup(node.Value),
                (node) => preItem(node.Value),
                (node) => perItem(node.Value),
                (node) => postItem(node.Value),
                (node) => postGroup(node.Value),
                end);
        }

        public void SearchNodesDepthFirst(Action<NAryTree<T>> preItem, Action<NAryTree<T>> perItem, Action<NAryTree<T>> postItem)
        {
            SearchNodesDepthFirst(
                null,
                null,
                preItem,
                perItem,
                postItem,
                null,
                null);
        }

        public void SearchNodesDepthFirst(Action<NAryTree<T>> preGroup, Action<NAryTree<T>> preItem, Action<NAryTree<T>> perItem, Action<NAryTree<T>> postItem, Action<NAryTree<T>> postGroup)
        {
            SearchNodesDepthFirst(
                null,
                preGroup,
                preItem,
                perItem,
                postItem,
                postGroup,
                null);
        }

        public void SearchNodesDepthFirst(Action start, Action<NAryTree<T>> preGroup, Action<NAryTree<T>> preItem, Action<NAryTree<T>> perItem, Action<NAryTree<T>> postItem, Action<NAryTree<T>> postGroup, Action end)
        {
            NAryTree<T> last = null;

            var visited = new Stack<NAryTree<T>>();
            var toVisit = new Stack<NAryTree<T>>();
            toVisit.Push(this);

            start?.Invoke();
            while (toVisit.Count > 0)
            {
                var here = toVisit.Pop();
                while (visited.Count > 0
                    && visited.Peek() != here.Parent)
                {
                    last = visited.Pop();
                    postGroup?.Invoke(last);
                    postItem?.Invoke(last);
                }

                preItem?.Invoke(here);
                perItem?.Invoke(here);

                if (here.Children.Count == 0)
                {
                    postItem?.Invoke(here);
                }
                else
                {
                    visited.Push(here);
                    preGroup?.Invoke(here);
                    foreach (var child in here.Children.AsEnumerable().Reverse())
                    {
                        toVisit.Push(child);
                    }
                }

                last = here;
            }

            while (visited.Count > 0)
            {
                last = visited.Pop();
                postGroup?.Invoke(last);
                postItem?.Invoke(last);
            }
            end?.Invoke();
        }
    }
}