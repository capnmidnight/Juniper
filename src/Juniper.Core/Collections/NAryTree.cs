using System;
using System.Collections.Generic;
using System.Text;

namespace Juniper.Collections
{
    /// <summary>
    /// A node in an N-ary tree.
    /// </summary>
    /// <typeparam name="T">Any type of object</typeparam>
    public class NAryTree<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// The value stored in this node.
        /// </summary>
        public T Value { get; internal set; }

        /// <summary>
        /// All nodes below the current node.
        /// </summary>
        public readonly List<NAryTree<T>> children = new List<NAryTree<T>>();

        /// <summary>
        /// The next node above the current node.
        /// </summary>
        protected NAryTree<T> parent;

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
            this.parent = parent;
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
                while (here != null)
                {
                    ++depth;
                    here = here.parent;
                }

                return depth;
            }
        }

        /// <summary>
        /// Returns true if there are no child nodes.
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return children.Count == 0;
            }
        }

        /// <summary>
        /// Returns true if this node has no parent node.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return parent == null;
            }
        }

        /// <summary>
        /// Add an element as a child of the node.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void Add(T node, Func<T, T, bool> isChildOf)
        {
            if (Value == null)
            {
                Value = node;
            }
            else
            {
                var q = new Queue<NAryTree<T>>();
                q.Add(this);

                NAryTree<T> lastParent = null;
                while (q.Count > 0)
                {
                    var here = q.Dequeue();
                    if (isChildOf(here.Value, node))
                    {
                        ++here.Count;
                        lastParent = here;
                        q.AddRange(here.children);
                    }
                }

                if (lastParent != null)
                {
                    lastParent.children.Add(new NAryTree<T>(node, lastParent));
                }
            }
        }

        public NAryTree<T> Remove(T node)
        {
            var q = new Queue<NAryTree<T>>();
            q.Add(this);

            NAryTree<T> found = null;
            while (q.Count > 0 && found == null)
            {
                var here = q.Dequeue();
                if (here.Value.Equals(node))
                {
                    found = here;
                }
                else
                {
                    q.AddRange(here.children);
                }
            }

            if (found != null)
            {
                found.parent.Count -= found.Count;
                found.parent.children.Remove(found);
                found.parent = null;
            }

            return found;
        }

        public enum Order
        {
            BreadthFirst,
            DepthFirst
        }

        public IEnumerable<NAryTree<T>> Where(Func<NAryTree<T>, bool> predicate, Order order = Order.BreadthFirst)
        {
            // how to do recursion without killing the function call stack
            var items = new List<NAryTree<T>>();
            items.Add(this);
            while (items.Count > 0)
            {
                var index = order == Order.BreadthFirst ? 0 : items.Count - 1;
                var here = items[index];
                items.RemoveAt(index);

                if (predicate(here))
                {
                    yield return here;
                    items.AddRange(here.children);
                }
            }
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        public IEnumerable<NAryTree<T>> Flatten(Order order = Order.BreadthFirst)
        {
            return Where(_ => true, Order.BreadthFirst);
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        /// <param name="act">Act.</param>
        public StateT Accumulate<StateT>(Order order, StateT state, Func<StateT, NAryTree<T>, StateT> act)
        {
            foreach(var child in Flatten(order))
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
            return Accumulate(Order.DepthFirst, new StringBuilder(), (sb, here) =>
            {
                for (var i = 0; i < here.Depth; ++i)
                {
                    sb.Append("--");
                }
                sb.AppendLine(here.Value.ToString());
                return sb;
            }).ToString();
        }
    }
}