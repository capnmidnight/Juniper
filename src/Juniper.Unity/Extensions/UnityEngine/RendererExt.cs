namespace UnityEngine
{
    /// <summary>
    /// Extension methods for Unity's Renderer class.
    /// </summary>
    public static class RendererExt
    {
        /// <summary>
        /// Returns true if we are running in the editor, but the player is not running. In these
        /// cases, we want to set sharedMaterials instead of materials on objects to avoid creating
        /// duplicate objects that will leak into memory.
        /// </summary>
        public static bool UseShared
        {
            get
            {
                return Application.isEditor && !Application.isPlaying;
            }
        }

        /// <summary>
        /// If <see cref="UseShared"/> is true, returns the sharedMaterial from the renderer.
        /// Otherwise, gets the material property from the renderer.
        /// </summary>
        /// <returns>The material.</returns>
        /// <param name="renderer">Renderer.</param>
        public static Material GetMaterial(this Renderer renderer)
        {
            if (UseShared)
            {
                return renderer.sharedMaterial;
            }
            else
            {
                return renderer.material;
            }
        }

        /// <summary>
        /// If <see cref="UseShared"/> is true, set the sharedMaterial on the renderer. Otherwise,
        /// set the material property on the renderer.
        /// </summary>
        /// <param name="renderer">Renderer.</param>
        /// <param name="material">Material.</param>
        public static void SetMaterial(this Renderer renderer, Material material)
        {
            renderer.enabled = material != null;
            if (renderer.enabled)
            {
                if (UseShared)
                {
                    renderer.sharedMaterial = material;
                }
                else
                {
                    renderer.material = material;
                }
            }
        }

        /// <summary>
        /// If <see cref="UseShared"/> is true, returns the sharedMaterials from the renderer.
        /// Otherwise, gets the materials property from the renderer.
        /// </summary>
        /// <returns>The material.</returns>
        /// <param name="renderer">Renderer.</param>
        public static Material[] GetMaterials(this Renderer renderer)
        {
            if (UseShared)
            {
                return renderer.sharedMaterials;
            }
            else
            {
                return renderer.materials;
            }
        }

        /// <summary>
        /// If <see cref="UseShared"/> is true, set the sharedMaterials on the renderer. Otherwise,
        /// set the materials property on the renderer.
        /// </summary>
        /// <param name="renderer"> Renderer.</param>
        /// <param name="materials">Material.</param>
        public static void SetMaterials(this Renderer renderer, Material[] materials)
        {
            if (UseShared)
            {
                renderer.sharedMaterials = materials;
            }
            else
            {
                renderer.materials = materials;
            }
        }

        /// <summary>
        /// If <see cref="UseShared"/> is true, returns the sharedMesh from the mesh filter.
        /// Otherwise, gets the mesh property from the mesh filter.
        /// </summary>
        /// <returns>The material.</returns>
        /// <param name="filter">a mesh filter.</param>
        public static Mesh GetMesh(this MeshFilter filter)
        {
            if (UseShared)
            {
                return filter.sharedMesh;
            }
            else
            {
                return filter.mesh;
            }
        }

        /// <summary>
        /// If <see cref="UseShared"/> is true, set the sharedMesh on the mesh filter. Otherwise, set
        /// the mesh property on the mesh filter.
        /// </summary>
        /// <param name="filter">a mesh filter.</param>
        /// <param name="mesh">  a mesh.</param>
        public static void SetMesh(this MeshFilter filter, Mesh mesh)
        {
            if (UseShared)
            {
                filter.sharedMesh = mesh;
            }
            else
            {
                filter.mesh = mesh;
            }
        }
    }
}
