#if UNITY_MODULES_TERRAIN

using System.Collections.Generic;

using UnityEngine;

namespace Juniper.Unity.Ground
{
    public class StaticTerrain : AbstractGround
    {
        /// <summary>
        /// On VR and desktop systems, this is what is displayed in place of the ground.
        /// </summary>
        public TerrainData terrainData;

        /// <summary>
        /// The Unity Terrain object in which <see cref="TerrainData"/> is utilized.
        /// </summary>
        private Terrain terrain;

        /// <summary>
        /// Set to true when the Ground object had a Terrain object within it when the application
        /// first started. This is a signal that we should not change the material and just let the
        /// material be whatever it is already.
        /// </summary>
        private bool preexistingTerrain;

        protected override void Awake()
        {
            if (GroundVisualizers.Empty())
            {
                Collider collid;

                if (terrainData == null)
                {
                    var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.layer = GroundLayer;
                    plane.transform.SetParent(transform, false);
                    plane.transform.localScale = 10 * Vector3.one;

                    collid = plane.GetComponent<Collider>();
                }
                else
                {
                    terrain = GetComponentInChildren<Terrain>();
                    preexistingTerrain = terrain != null;
                    if (!preexistingTerrain)
                    {
                        var terrainT = this.Ensure<Transform>("Terrain");
                        terrain = terrainT.Ensure<Terrain>();
                        terrain.materialType = Terrain.MaterialType.Custom;
                        terrain.materialTemplate = CurrentMaterial;
                        terrain.terrainData = terrainData;
                    }

                    terrain.gameObject.layer = GroundLayer;

                    var t = terrain.Ensure<TerrainCollider>();
                    if (t.IsNew)
                    {
                        t.Value.terrainData = terrain.terrainData;
                        if (!preexistingTerrain)
                        {
                            terrain.transform.position = -t.Value.bounds.center;
                        }
                    }

                    collid = t;
                }

                if (collid.GetMaterial() == null)
                {
                    collid.SetMaterial(Roughness);
                }
            }
        }

        protected override void VisualizationChanged()
        {
            base.VisualizationChanged();

            if (terrain != null
                && !preexistingTerrain
                && terrain.materialType == Terrain.MaterialType.Custom)
            {
                terrain.materialTemplate = CurrentMaterial;
            }
        }
    }
}

#endif