using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Juniper.Ground
{
    public abstract class AbstractGround :
        MonoBehaviour,
        IInstallable
    {
        /// <summary>
        /// A friction material for the ground to interact with the player's shoes.
        /// </summary>
        [Header("Ground")]
        public PhysicMaterial Roughness;

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
        /// The material to use to render the terrain, occording to <see cref="MeshVisualization"/>.
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
        public int GroundLayer =>
            gameObject.layer;

        /// <summary>
        /// Renderers that exist in the direct ancestery of the Ground object, including itself.
        /// </summary>
        /// <value>The ground visualizers.</value>
        public IEnumerable<Renderer> GroundVisualizers =>
            from child in transform.Family()
            let rend = child.GetComponent<Renderer>()
            where rend != null
            select rend;

        public virtual void Awake() =>
            Install(false);

        public virtual void Reinstall() =>
            Install(true);

#if UNITY_EDITOR
        public void Reset() =>
            Reinstall();
#endif

        public void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ground");

            var xr = ComponentExt.FindAny<JuniperPlatform>();

            if (xr.ARMode > 0)
            {
                this.WithLock(() =>
                    InternalStart(xr));
            }
        }

        /// <summary>
        /// Configures the right plane-finding system for any current AR subsystem, or configures
        /// terrain rendering for any current VR subsystem.
        /// </summary>
        protected virtual void InternalStart(JuniperPlatform xr) { }

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

        public virtual void Install(bool reset) { }

        public virtual void Uninstall() { }
    }
}
