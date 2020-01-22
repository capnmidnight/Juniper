namespace UnityEngine
{
    /// <summary>
    /// Extension methods for Unity's Collider classes.
    /// </summary>
    public static class ColliderExt
    {
        /// <summary>
        /// Set the physics material. If we are in the Editor and the editor is not playing, then we
        /// set the sharedMaterial. Otherwise, we set the material.
        /// </summary>
        /// <param name="collid">Collid.</param>
        /// <param name="mat">   Mat.</param>
        public static void SetMaterial(this Collider collid, PhysicMaterial mat)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                collid.sharedMaterial = mat;
            }
            else
            {
                collid.material = mat;
            }
        }

        /// <summary>
        /// Get the physics material. If we are in the Editor and the editor is not playing, then we
        /// get the sharedMaterial. Otherwise, we get the material.
        /// </summary>
        /// <param name="collid">Collid.</param>
        public static PhysicMaterial GetMaterial(this Collider collid)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                return collid.sharedMaterial;
            }
            else
            {
                return collid.material;
            }
        }
    }
}
