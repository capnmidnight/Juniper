using System.Collections.Generic;
using UnityEngine;

namespace Juniper.Collections
{
    /// <summary>
    /// Sorts textures according to their names.
    /// </summary>
    public class TextureSortComparer : IComparer<Texture2D>
    {
        /// <summary>
        /// The comparer to use on the names of the textures.
        /// </summary>
        private readonly NaturalSortComparer nameComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnityEngine.TextureSortComparer"/> class.
        /// </summary>
        public TextureSortComparer() : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnityEngine.TextureSortComparer"/> class.
        /// </summary>
        /// <param name="inAscendingOrder">If set to <c>true</c> in ascending order.</param>
        public TextureSortComparer(bool inAscendingOrder)
        {
            nameComparer = new NaturalSortComparer(inAscendingOrder);
        }

        /// <summary>
        /// Compare two textures by name.
        /// </summary>
        /// <returns>The compare.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public int Compare(Texture2D x, Texture2D y) =>
            nameComparer.Compare(x.name, y.name);

        /// <summary>
        /// Compare two textures by name.
        /// </summary>
        /// <returns>The compare.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        int IComparer<Texture2D>.Compare(Texture2D x, Texture2D y) =>
            nameComparer.Compare(x.name, y.name);
    }
}
