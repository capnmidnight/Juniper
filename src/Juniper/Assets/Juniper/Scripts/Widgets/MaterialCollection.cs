using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// A read-only collection of materials that can be applied to renderers.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Materials", menuName = "Materials/Material Collection")]
    public class MaterialCollection : ScriptableObject, IEnumerable<Material>
    {
        /// <summary>
        /// The materials collected together.
        /// </summary>
        public Material[] materials;

        /// <summary>
        /// The number of materials in the collection.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get
            {
                return materials.Length;
            }
        }

        /// <summary>
        /// Gets the Unity Material at the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public Material this[int index]
        {
            get
            {
                return materials[index];
            }
        }

        /// <summary>
        /// Gets an enumerator over the materials.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Material> GetEnumerator()
        {
            return materials.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator over the materials.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return materials.GetEnumerator();
        }
    }
}
