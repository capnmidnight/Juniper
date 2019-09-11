using System;
using System.Collections.Generic;
using System.Text;

namespace Juniper.Collections
{
    /// <summary>
    /// A node in an N-ary tree.
    /// </summary>
    /// <typeparam name="T">Any type of object</typeparam>
    public class NAryTree<T, U>
        where U : NAryTree<T, U>
    {
        /// <summary>
        /// The value stored in this node.
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// All nodes below the current node.
        /// </summary>
        public readonly List<NAryTree<T, U>> children;

        /// <summary>
        /// The next node above the current node.
        /// </summary>
        protected NAryTree<T, U> parent;

        /// <summary>
        /// Creates a root node with no children.
        /// </summary>
        /// <param name="value">The value to store at the root.</param>
        public NAryTree(T value)
        {
            Value = value;
            children = new List<NAryTree<T, U>>(3);
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
        public void Add(NAryTree<T, U> node)
        {
            children.Add(node);
            node.parent = this;
        }

        /// <summary>
        /// Create a new node out of a value and add it to the tree.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public void Add(T value)
        {
            Add(new NAryTree<T, U>(value));
        }

        /// <summary>
        /// Print out a debugging string.
        /// </summary>
        /// <returns>A text representation of the tree.</returns>
        public override string ToString()
        {
            return Accumulate(new StringBuilder(), (sb, here) =>
            {
                for (var i = 0; i < here.Depth; ++i)
                {
                    sb.Append("--");
                }
                sb.AppendLine(here.Value.ToString());
            }).ToString();
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        /// <param name="act">Act.</param>
        public void Recurse(Action<U> act)
        {
            // how to do recursion without killing the function call stack
            var stack = new Stack<U>();
            stack.Push((U)this);
            while (stack.Count > 0)
            {
                var here = stack.Pop();
                act(here);
                foreach (var child in here.children)
                {
                    stack.Push((U)child);
                }
            }
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        /// <param name="act">Act.</param>
        public IEnumerable<ValueT> Select<ValueT>(Func<U, ValueT> act)
        {
            // how to do recursion without killing the function call stack
            var stack = new Stack<U>();
            stack.Push((U)this);
            while (stack.Count > 0)
            {
                var here = stack.Pop();
                yield return act(here);
                foreach (var child in here.children)
                {
                    stack.Push((U)child);
                }
            }
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        /// <param name="act">Act.</param>
        public StateT Accumulate<StateT>(StateT state, Action<StateT, U> act)
        {
            // how to do recursion without killing the function call stack
            var stack = new Stack<U>();
            stack.Push((U)this);
            while (stack.Count > 0)
            {
                var here = stack.Pop();
                act(state, here);
                foreach (var child in here.children)
                {
                    stack.Push((U)child);
                }
            }

            return state;
        }
    }
}