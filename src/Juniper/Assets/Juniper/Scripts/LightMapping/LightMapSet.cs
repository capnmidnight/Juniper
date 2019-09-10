using System;

using Juniper.Collections;

using UnityEngine;

namespace Juniper.LightMapping
{
    /// <summary>
    /// A set of Directional, Light, and Shadow maps, for being able to switch between light maps at
    /// runtime using <see cref="LightMapSwitcher"/>.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "lightMapSet", menuName = "LightMapBlender/LightMapSet")]
    public class LightMapSet : ScriptableObject
    {
        /// <summary>
        /// Used to sort the textures according to their name, so that they line up correctly,
        /// regardless of how they are defined in the editor.
        /// </summary>
        private static readonly TextureSortComparer COMPARER = new TextureSortComparer();

        /// <summary>
        /// Directional light data.
        /// </summary>
        public Texture2D[] direction;

        /// <summary>
        /// Light map data.
        /// </summary>
        public Texture2D[] light;

        /// <summary>
        /// Shadow mask map data.
        /// </summary>
        public Texture2D[] shadowMask;

        /// <summary>
        /// The orientation of the key scene light to use when this light map set is active. It will
        /// update the skymap to make a day/night transition.
        /// </summary>
        public Vector3 SunOrientation;

        /// <summary>
        /// All of the lightMaps, packed up for use with <see cref="LightMapSwitcher"/>
        /// </summary>
        public LightmapData[] LightMaps
        {
            get; private set;
        }

        /// <summary>
        /// Sort the light maps and pack them up into the <see cref="LightMaps"/> property.
        /// </summary>
        public void Sort()
        {
            if (direction.Length != light.Length || direction.Length != shadowMask.Length)
            {
                throw new InvalidOperationException("In order for LightMapSwitcher to work, the Direction, Light, and ShadowMask arrays must be of equal length");
            }

            Array.Sort(direction, COMPARER);
            Array.Sort(light, COMPARER);

            LightMaps = new LightmapData[direction.Length];
            for (var i = 0; i < direction.Length; i++)
            {
                LightMaps[i] = new LightmapData
                {
                    lightmapDir = direction[i],
                    lightmapColor = light[i],
                    shadowMask = shadowMask[i]
                };
            }
        }
    }
}
