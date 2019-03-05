#if HOLOLENS
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.WSA;

namespace Juniper.Unity.Ground
{
    public abstract class HoloLensGround : AbstractARGround
    {
        /// <summary>
        /// On HoloLens, the time, in seconds, to wait before attempting another spatial update. This
        /// can be an expensive operation, so it needs to be scaled carefully.
        /// </summary>
        public float secondsBetweenSpatialMappingUpdates = 3;

        /// <summary>
        /// On HoloLens, the number of spatial updates (multiply by <see
        /// cref="secondsBetweenSpatialMappingUpdates"/> to get the total amount of time) before old
        /// spatial data gets thrown out.
        /// </summary>
        public int updateCountBeforeSpatialMappingCleanup = 10;

        /// <summary>
        /// On HoloLens, the radius in meters out to which to keep spatial meshes alive.
        /// </summary>
        public float spatialMappingRange = 5;

        /// <summary>
        /// When running on HololLens, a mapping between Juniper's quality levels and WindowsMR's
        /// Spatial Mapping detail levels.
        /// </summary>
        private static readonly Dictionary<Level, SpatialMappingBase.LODType> LODTypeMapping = new Dictionary<Level, SpatialMappingBase.LODType>
        {
            { Level.Low, SpatialMappingBase.LODType.Low },
            { Level.Medium, SpatialMappingBase.LODType.Medium },
            { Level.High, SpatialMappingBase.LODType.High }
        };

        /// <summary>
        /// When running on HoloLens, the component that manages the rendering of the spatial mesh.
        /// </summary>
        private SpatialMappingRenderer groundRenderer;

        protected override void InternalStart(XRSystem xr)
        {
            transform.ClearChildren();

            var gr = this.EnsureComponent<SpatialMappingRenderer>();
            gr.Value.occlusionMaterial = OcclusionMaterial;
            gr.Value.visualMaterial = VisualizationMaterial;

            var gc = this.EnsureComponent<SpatialMappingCollider>();

            if (gc.IsNew && gr.IsNew)
            {
                gc.Value.enableCollisions = true;
                gc.Value.layer = GroundLayer;
                gc.Value.material = Roughness;

                gc.Value.surfaceParent = gameObject;
                gc.Value.freezeUpdates = false;
                gc.Value.secondsBetweenUpdates = secondsBetweenSpatialMappingUpdates;
                gc.Value.numUpdatesBeforeRemoval = updateCountBeforeSpatialMappingCleanup;
                gc.Value.lodType = LODTypeMapping[spatialMappingFidelity];
                gc.Value.volumeType = SpatialMappingBase.VolumeType.AxisAlignedBox;
                gc.Value.halfBoxExtents = spatialMappingRange * Vector3.one;
            }
            else if (gr.IsNew)
            {
                gr.Value.surfaceParent = gc.Value.surfaceParent;
                gr.Value.freezeUpdates = gc.Value.freezeUpdates;
                gr.Value.secondsBetweenUpdates = gc.Value.secondsBetweenUpdates;
                gr.Value.numUpdatesBeforeRemoval = gc.Value.numUpdatesBeforeRemoval;
                gr.Value.lodType = gc.Value.lodType;
                gr.Value.volumeType = gc.Value.volumeType;
                gr.Value.halfBoxExtents = gc.Value.halfBoxExtents;
            }
            else if (gc.IsNew)
            {
                gc.Value.surfaceParent = groundRenderer.surfaceParent;
                gc.Value.freezeUpdates = groundRenderer.freezeUpdates;
                gc.Value.secondsBetweenUpdates = groundRenderer.secondsBetweenUpdates;
                gc.Value.numUpdatesBeforeRemoval = groundRenderer.numUpdatesBeforeRemoval;
                gc.Value.lodType = groundRenderer.lodType;
                gc.Value.volumeType = groundRenderer.volumeType;
                gc.Value.halfBoxExtents = groundRenderer.halfBoxExtents;
            }

            groundRenderer = gr;
        }

        public override void Update()
        {
            if (groundRenderer != null)
            {
                groundRenderer.enabled = MeshVisualization != GroundMeshVisualization.None;

                if (groundRenderer.enabled)
                {
                    if (MeshVisualization == GroundMeshVisualization.Debug)
                    {
                        groundRenderer.renderState = SpatialMappingRenderer.RenderState.Visualization;
                    }
                    else
                    {
                        groundRenderer.renderState = SpatialMappingRenderer.RenderState.Occlusion;
                    }
                }
            }

            base.Update();
        }
    }
}
#endif
