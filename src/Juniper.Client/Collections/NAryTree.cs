using System;
using System.Collections.Generic;
using System.Text;

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

        /// <summary>
        /// All nodes below the current node.
        /// </summary>
        public List<NAryTree<T>> Children { get; } = new List<NAryTree<T>>();

        /// <summary>
        /// The next node above the current node.
        /// </summary>
        protected NAryTree<T> Parent { get; set; }

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

        private NAryTree(T value, NAryTree<T> parent)
            : this(value)
        {
            Parent = parent;
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
        public bool IsLeaf
        {
            get { return Children.Count == 0; }
        }

        /// <summary>
        /// Returns true if this node has no parent node.
        /// </summary>
        public bool IsRoot
        {
            get { return Parent is null; }
        }

        /// <summary>
        /// Add an element as a child of the node.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void Add(T node, Func<T, T, bool> isChildOf)
        {
            if (Value is null)
            {
                Value = node;
            }
            else
            {
                var q = new Queue<NAryTree<T>>
                {
                    this
                };

                NAryTree<T> lastParent = null;
                while (q.Count > 0)
                {
                    var here = q.Dequeue();
                    if (isChildOf(here.Value, node))
                    {
                        ++here.Count;
                        lastParent = here;
                        q.AddRange(here.Children);
                    }
                }

                if (lastParent != null)
                {
                    lastParent.Children.Add(new NAryTree<T>(node, lastParent));
                }
            }
        }

        public NAryTree<T> Remove(T node)
        {
            var q = new Queue<NAryTree<T>>
            {
                this
            };

            NAryTree<T> found = null;
            while (q.Count > 0 && found is null)
            {
                var here = q.Dequeue();
                if (here.Value.Equals(node))
                {
                    found = here;
                }
                else
                {
                    q.AddRange(here.Children);
                }
            }

            if (found != null)
            {
                found.Parent.Count -= found.Count;
                _ = found.Parent.Children.Remove(found);
                found.Parent = null;
            }

            return found;
        }

        public IEnumerable<NAryTree<T>> Where(Func<NAryTree<T>, bool> predicate, TreeTraversalOrder order = TreeTraversalOrder.BreadthFirst)
        {
            // how to do recursion without killing the function call stack
            var items = new List<NAryTree<T>>
            {
                this
            };
            while (items.Count > 0)
            {
                var index = (order == TreeTraversalOrder.BreadthFirst)
                    ? 0
                    : items.Count - 1;
                var here = items[index];
                items.RemoveAt(index);

                if (predicate(here))
                {
                    yield return here;
                    items.AddRange(here.Children);
                }
            }
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        public IEnumerable<NAryTree<T>> Flatten(TreeTraversalOrder order = TreeTraversalOrder.BreadthFirst)
        {
            return Where(_ => true, order);
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        /// <param name="act">Act.</param>
        public StateT Accumulate<StateT>(TreeTraversalOrder order, StateT state, Func<StateT, NAryTree<T>, StateT> act)
        {
            foreach (var child in Flatten(order))
            {
                state = act(state, child);
            }

            return state;
        }

        /// <summary>
        /// Print out a debugging string.
        /// </summary>
        /// <returns>A text representation of the tree.</returns>
        public override string ToString()
        {
            return Accumulate(TreeTraversalOrder.DepthFirst, new StringBuilder(), (sb, here) =>
            {
                for (var i = 0; i < here.Depth; ++i)
                {
                    _ = sb.Append("--");
                }

                _ = sb.AppendLine(here.Value.ToString());
                return sb;
            }).ToString();
        }
    }
}