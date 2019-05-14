#if UNITY_XR_WINDOWSMR_METRO
using System.Collections.Generic;
using System.Linq;

using Juniper.Unity.Ground;

using UnityEngine;
using UnityEngine.XR.WSA;

namespace Juniper.Unity.Ground
{
    public abstract class WindowsMRGround : AbstractGround
    {
        /// <summary>
        /// When running on HololLens, a mapping between Juniper's quality levels and WindowsMR's
        /// Spatial Mapping detail levels.
        /// </summary>
        private static readonly Dictionary<Level, SpatialMappingBase.LODType> LODTypeMapping = new Dictionary<Level, SpatialMappingBase.LODType>(3)
        {
            { Level.Low, SpatialMappingBase.LODType.Low },
            { Level.Medium, SpatialMappingBase.LODType.Medium },
            { Level.High, SpatialMappingBase.LODType.High }
        };

        /// <summary>
        /// When running on HoloLens, the component that manages the rendering of the spatial mesh.
        /// </summary>
        private SpatialMappingRenderer gRenderer;

        /// <summary> When running on HoloLens, the component that manages physics
        // collisions with the spatial mesh.
        /// </summary>
        private SpatialMappingCollider gCollider;

        public GameObject surfaceParent;

        /// <summary>
        /// The last value of surfaceParent, to detect changes.
        /// </summary>
        private GameObject lastSurfaceParent;

        /// <summary>
        /// On HoloLens, the time, in seconds, to wait before attempting another spatial update. This
        /// can be an expensive operation, so it needs to be scaled carefully.
        /// </summary>
        public float secondsBetweenSpatialMappingUpdates = 3;

        /// <summary>
        /// The last value of secondsBetweenSpatialMappingUpdates, to detect changes.
        /// </summary>
        private float lastUpdateTime;

        /// <summary>
        /// On HoloLens, the number of spatial updates (multiply by <see
        /// cref="secondsBetweenSpatialMappingUpdates"/> to get the total amount of time) before old
        /// spatial data gets thrown out.
        /// </summary>
        public int updateCountBeforeSpatialMappingCleanup = 10;

        /// <summary>
        /// The last value of updateCountBeforeSpatialMappingCleanup, to detect changes.
        /// </summary>
        private int lastUpdateCount;

        /// <summary>
        /// On HoloLens, the radius in meters out to which to keep spatial meshes alive.
        /// </summary>
        public float spatialMappingRange = 5;

        /// <summary>
        /// The range at which updates should be search for.
        /// </summary>
        private Vector3 HalfBoxExtents
        {
            get
            {
                return spatialMappingRange * Vector3.one;
            }
            set
            {
                spatialMappingRange = (value.x + value.y + value.z) / 3;
            }
        }

        private Vector3 lastHalfBox;

        /// <summary>
        /// Should updates to the spatial mapping be stopped?
        /// </summary>
        public bool freezeUpdates = false;

        /// <summary>
        /// The last value of freezeUpdates, to detect changes.
        /// </summary>
        private bool lastFreezeUpdates;

        private SpatialMappingBase.LODType LODType
        {
            get
            {
                return LODTypeMapping[spatialMappingFidelity];
            }
            set
            {
                spatialMappingFidelity = (from pairs in LODTypeMapping
                                         where pairs.Value == value
                                         select pairs.Key)
                                        .First();
            }
        }

        private SpatialMappingBase.LODType lastLODType;

        public SpatialMappingBase.VolumeType volumeType = SpatialMappingBase.VolumeType.AxisAlignedBox;

        private SpatialMappingBase.VolumeType lastVolumeType;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (surfaceParent == null)
            {
                surfaceParent = gameObject;
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();


#if !UNITY_EDITOR
            if (Windows.Graphics.Holographic.HolographicDisplay.GetDefault()?.IsOpaque == false)
            {
                transform.ClearChildren();
            }
#endif
            var gr = this.Ensure<SpatialMappingRenderer>();
            gr.Value.occlusionMaterial = OcclusionMaterial;
            gr.Value.visualMaterial = VisualizationMaterial;

            var gc = this.Ensure<SpatialMappingCollider>();

            gCollider = gc;
            gRenderer = gr;

            if(gc.IsNew)
            {
                gc.Value.enableCollisions = true;
                gc.Value.layer = GroundLayer;
                gc.Value.material = Roughness;

                if (gr.IsNew)
                {
                    gc.Value.surfaceParent = surfaceParent;
                    gc.Value.freezeUpdates = freezeUpdates;
                    gc.Value.secondsBetweenUpdates = secondsBetweenSpatialMappingUpdates;
                    gc.Value.numUpdatesBeforeRemoval = updateCountBeforeSpatialMappingCleanup;
                    gc.Value.lodType = LODType;
                    gc.Value.volumeType = volumeType;
                    gc.Value.halfBoxExtents = HalfBoxExtents;
                }
                else
                {
                    gc.Value.surfaceParent = gr.Value.surfaceParent;
                    gc.Value.freezeUpdates = gr.Value.freezeUpdates;
                    gc.Value.secondsBetweenUpdates = gr.Value.secondsBetweenUpdates;
                    gc.Value.numUpdatesBeforeRemoval = gr.Value.numUpdatesBeforeRemoval;
                    gc.Value.lodType = gr.Value.lodType;
                    gc.Value.volumeType = gr.Value.volumeType;
                    gc.Value.halfBoxExtents = gr.Value.halfBoxExtents;
                }
            }

            gr.Value.surfaceParent = gc.Value.surfaceParent;
            gr.Value.freezeUpdates = gc.Value.freezeUpdates;
            gr.Value.secondsBetweenUpdates = gc.Value.secondsBetweenUpdates;
            gr.Value.numUpdatesBeforeRemoval = gc.Value.numUpdatesBeforeRemoval;
            gr.Value.lodType = gc.Value.lodType;
            gr.Value.volumeType = gc.Value.volumeType;
            gr.Value.halfBoxExtents = gc.Value.halfBoxExtents;

            lastSurfaceParent = surfaceParent;
            lastUpdateCount = updateCountBeforeSpatialMappingCleanup;
            lastUpdateTime = secondsBetweenSpatialMappingUpdates;
            lastFreezeUpdates = freezeUpdates;
            lastHalfBox = HalfBoxExtents;
            lastLODType = LODType;
            lastVolumeType = volumeType;
        }

        private bool Check<T>(T value, T gcValue, T grValue, ref T lastValue)
        {
            if (!gcValue.Equals(lastValue))
            {
                lastValue = gcValue;
                return true;
            }
            else if (!grValue.Equals(lastValue))
            {
                lastValue = grValue;
                return true;
            }
            else if (!value.Equals(lastValue))
            {
                lastValue = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Update()
        {
            if (gRenderer != null)
            {
                gRenderer.enabled = MeshVisualization != GroundMeshVisualization.None;

                if (gRenderer.enabled)
                {
                    if (MeshVisualization == GroundMeshVisualization.Debug)
                    {
                        gRenderer.renderState = SpatialMappingRenderer.RenderState.Visualization;
                    }
                    else
                    {
                        gRenderer.renderState = SpatialMappingRenderer.RenderState.Occlusion;
                    }
                }

                if (gCollider != null)
                {
                    if (Check(surfaceParent, gCollider.surfaceParent, gRenderer.surfaceParent, ref lastSurfaceParent))
                    {
                        surfaceParent = gCollider.surfaceParent = gRenderer.surfaceParent = lastSurfaceParent;
                    }

                    if (Check(updateCountBeforeSpatialMappingCleanup, gCollider.numUpdatesBeforeRemoval, gRenderer.numUpdatesBeforeRemoval, ref lastUpdateCount))
                    {
                        updateCountBeforeSpatialMappingCleanup = gCollider.numUpdatesBeforeRemoval = gRenderer.numUpdatesBeforeRemoval = lastUpdateCount;
                    }

                    if (Check(secondsBetweenSpatialMappingUpdates, gCollider.secondsBetweenUpdates, gRenderer.secondsBetweenUpdates, ref lastUpdateTime))
                    {
                        secondsBetweenSpatialMappingUpdates = gCollider.secondsBetweenUpdates = gRenderer.secondsBetweenUpdates = lastUpdateTime;
                    }

                    if (Check(freezeUpdates, gCollider.freezeUpdates, gRenderer.freezeUpdates, ref lastFreezeUpdates))
                    {
                        freezeUpdates = gCollider.freezeUpdates = gRenderer.freezeUpdates = lastFreezeUpdates;
                    }

                    if (Check(HalfBoxExtents, gCollider.halfBoxExtents, gRenderer.halfBoxExtents, ref lastHalfBox))
                    {
                        HalfBoxExtents = gCollider.halfBoxExtents = gRenderer.halfBoxExtents = lastHalfBox;
                    }

                    if (Check(LODType, gCollider.lodType, gRenderer.lodType, ref lastLODType))
                    {
                        LODType = gCollider.lodType = gRenderer.lodType = lastLODType;
                    }

                    if (Check(volumeType, gCollider.volumeType, gRenderer.volumeType, ref lastVolumeType))
                    {
                        volumeType = gCollider.volumeType = gRenderer.volumeType = lastVolumeType;
                    }
                }
            }

            base.Update();
        }
    }
}
#endif
