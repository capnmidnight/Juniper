namespace Juniper.Unity.Ground
{
    public abstract class AbstractARGround : AbstractGround
    {
        /// <summary>
        /// On HoloLens, the level of detail to use when performing spatial mapping.
        /// </summary>
        public Level spatialMappingFidelity = Level.Medium;
    }
}