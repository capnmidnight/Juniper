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
    {
        /// <summary>
        /// The value stored in this node.
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// Creates a root node with no children.
        /// </summary>
        /// <param name="value">The value to store at the root.</param>
        public NAryTree(T value)
        {
            Value = value;
            depth = 0;
            children = new List<NAryTree<T>>(3);
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
        public void Add(NAryTree<T> node)
        {
            children.Add(node);
            node.parent = this;
            node.IncreaseDepth();
        }

        /// <summary>
        /// Create a new node out of a value and add it to the tree.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public void Add(T value)
        {
            Add(new NAryTree<T>(value));
        }

        /// <summary>
        /// Print out a debugging string.
        /// </summary>
        /// <returns>A text representation of the tree.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            Recurse(here =>
            {
                for (var i = 0; i < here.depth; ++i)
                {
                    sb.Append("--");
                }
                sb.AppendLine(here.Value.ToString());
            });
            return sb.ToString();
        }

        /// <summary>
        /// Perform an operation over the trie, using a local stack instead of the function call
        /// stack frame.
        /// </summary>
        /// <param name="act">Act.</param>
        protected void Recurse(Action<NAryTree<T>> act)
        {
            // how to do recursion without killing the function call stack
            var stack = new Stack<NAryTree<T>>();
            stack.Push(this);
            while (stack.Count > 0)
            {
                var here = stack.Pop();
                here.children.ForEach(stack.Push);
                act(here);
            }
        }

        /// <summary>
        /// How deep into the tree is this branch
        /// </summary>
        private int depth;

        /// <summary>
        /// The next node above the current node.
        /// </summary>
        private NAryTree<T> parent;

        /// <summary>
        /// All nodes below the current node.
        /// </summary>
        private List<NAryTree<T>> children;

        /// <summary>
        /// Increase the depth of this and any child nodes.
        /// </summary>
        private void IncreaseDepth()
        {
            Recurse(here => ++here.depth);
        }
    }
}
