using UnityEngine;

namespace Juniper.LightMapping
{
    /// <summary>
    /// The LightMapSwitcher takes different sets of static light map sets and switches between them
    /// at runtime. You have to disable auto static light mapping in Unity's Lighting settings and
    /// run the light mapper manually. This will make Unity dump the light map into a directory that
    /// you can then archive. Create a <see cref="LightMapSet"/> out of the files in that archive and
    /// then combine all of your LightMapSets into the LightMapSwitcher. In this way, you can have
    /// high quality lighting of different lighting conditions that can be switched between on the fly.
    /// </summary>
    [ExecuteInEditMode]
    public class LightMapSwitcher : MonoBehaviour
    {
        /// <summary>
        /// The current light map set to use.
        /// </summary>
        public int Index;

        /// <summary>
        /// The key light source.
        /// </summary>
        public Light Sun;

        /// <summary>
        /// The collection of light maps that we will be switching between.
        /// </summary>
        public LightMapSet[] LightMapSets;

        /// <summary>
        /// What for the <see cref="Index"/> value to be changed, then update the <see
        /// cref="LightmapSettings.lightmaps"/> property.
        /// </summary>
        public void Update()
        {
            if (LightMapSets != null)
            {
                Index = Mathf.Max(0, Mathf.Min(LightMapSets.Length - 1, Index));

                if (Index != prevIndex && 0 <= Index && Index < LightMapSets.Length)
                {
                    var map = LightMapSets[Index];
                    LightmapSettings.lightmaps = map.LightMaps;
                    if (Sun != null)
                    {
                        Sun.transform.rotation = Quaternion.Euler(map.SunOrientation);
                    }
                    prevIndex = Index;
                }
            }
        }

        /// <summary>
        /// Switch to the next light map in the series, looping back to the first when we reach the
        /// end of the collection. This can also be executed in the editor as a context menu on the
        /// component, "Set Next Lightmap".
        /// </summary>
        [ContextMenu("Set Next Lightmap")]
        public void Next()
        {
            Index = (Index + 1) % LightMapSets.Length;
            Update();
        }

        /// <summary>
        /// The light map set that was used in the last frame.
        /// </summary>
        private int? prevIndex;
    }
}
