using System.Collections.Generic;
using System.Linq;
using Juniper.Units;
using Juniper.XR;
using UnityEngine;

namespace Juniper.Ground
{
    /// <summary>
    /// Manages references to the ground, either 3D terrain in VR apps or detected planes and meshes
    /// in AR apps.
    /// </summary>
    [ExecuteInEditMode]
    public class TerrainManager :
#if UNITY_XR_ARKIT
        ARKitGround
#elif UNITY_XR_MAGICLEAP
        MagicLeapGround
#elif UNITY_XR_ARCORE
        ARCoreGround
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRGround
#elif UNITY_MODULES_TERRAIN
        StaticTerrain
#else
        NoTerrain
#endif
    {
    }

    public abstract class AbstractGround :
        MonoBehaviour,
        IInstallable
    {
#if UNITY_MODULES_PHYSICS

        /// <summary>
        /// A friction material for the ground to interact with the player's shoes.
        /// </summary>
        [Header("Ground")]
        public PhysicMaterial Roughness;

#endif

        /// <summary>
        /// On HoloLens, the level of detail to use when performing spatial mapping.
        /// </summary>
        public Level spatialMappingFidelity = Level.Medium;

        /// <summary>
        /// The current type of visualization to use for displaying the terrain.
        /// </summary>
        public GroundMeshVisualization MeshVisualization;

        /// <summary>
        /// The material to use when rendering in the <see cref="GroundMeshVisualization.Occluded"/> mode.
        /// </summary>
        public Material OcclusionMaterial;

        /// <summary>
        /// The material to use when rendering in the editor in the <see
        /// cref="GroundMeshVisualization.Occluded"/> mode.
        /// </summary>
        public Material OcclusionMaterialInEditor;

        /// <summary>
        /// The material to use when rendering in the <see cref="GroundMeshVisualization.Debug"/> mode.
        /// </summary>
        public Material VisualizationMaterial;

        /// <summary>
        /// The material to use to render the terrain, according to <see cref="MeshVisualization"/>.
        /// </summary>
        public Material CurrentMaterial
        {
            get
            {
                if (MeshVisualization == GroundMeshVisualization.None)
                {
                    return null;
                }
                else if (MeshVisualization == GroundMeshVisualization.Debug)
                {
                    return VisualizationMaterial;
                }
                else if (Application.isEditor)
                {
                    return OcclusionMaterialInEditor;
                }
                else
                {
                    return OcclusionMaterial;
                }
            }
        }

        public void ToggleVisualization()
        {
            if (MeshVisualization != GroundMeshVisualization.Debug)
            {
                MeshVisualization = GroundMeshVisualization.Debug;
            }
            else
            {
                MeshVisualization = GroundMeshVisualization.Occluded;
            }
        }

        /// <summary>
        /// The visualization used in the last frame, used to detect changes in the visualization type.
        /// </summary>
        private GroundMeshVisualization? lastMeshVisualization;

        /// <summary>
        /// The numeric layer identifier in which the Ground object exists.
        /// </summary>
        /// <value>The ground layer.</value>
        public int GroundLayer
        {
            get
            {
                return gameObject.layer;
            }
        }

        /// <summary>
        /// Renderers that exist in the direct ancestry of the Ground object, including itself.
        /// </summary>
        /// <value>The ground visualizers.</value>
        public IEnumerable<Renderer> GroundVisualizers
        {
            get
            {
                return from child in transform.Family()
                       let rend = child.GetComponent<Renderer>()
                       where rend != null
                       select rend;
            }
        }

        protected virtual void Awake()
        {
            Install(false);
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        /// <summary>
        /// Updates plane visualizers for ARCore, or changes the ground rendering material on HoloLens.
        /// </summary>
        public virtual void Update()
        {
            if (MeshVisualization != lastMeshVisualization)
            {
                VisualizationChanged();
                lastMeshVisualization = MeshVisualization;
            }
        }

        protected virtual void VisualizationChanged()
        {
            foreach (var gr in GroundVisualizers)
            {
                gr.SetMaterial(CurrentMaterial);
            }
        }

        public virtual void Install(bool reset)
        {
            if (reset)
            {
                gameObject.layer = LayerMask.NameToLayer("Terrain");
            }
        }

        public virtual void Uninstall()
        {
        }
    }
}