#if UNITY_XR_ARCORE
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

namespace Juniper.Ground
{
    /// <summary>
    /// Visualizes a TrackedPlane in the Unity scene.
    /// </summary>
    internal class ARCoreGroundPlaneVisualizer : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// The number of ground planes that are currently being tracked.
        /// </summary>
        private static int count = 0;

        /// <summary>
        /// Initializes the TrackedPlaneVisualizer with a TrackedPlane.
        /// </summary>
        /// <param name="plane">The plane to vizualize.</param>
        public static ARCoreGroundPlaneVisualizer Initialize(DetectedPlane plane)
        {
            var planeObject = new GameObject($"GroundPlane_{count++}");
            var gpv = planeObject.AddComponent<ARCoreGroundPlaneVisualizer>();
            gpv.trackedPlane = plane;
            return gpv;
        }

        /// <summary>
        /// The ARCore tracked plane object.
        /// </summary>
        private DetectedPlane trackedPlane;

        /// <summary>
        /// Keeps the previous frame's mesh polygon to avoid mesh update every frame.
        /// </summary>
        private readonly List<Vector3> previousFrameMeshVertices = new List<Vector3>(100);

        /// <summary>
        /// The mesh polygon for the current frame.
        /// </summary>
        private readonly List<Vector3> meshVertices = new List<Vector3>(100);

        /// <summary>
        /// The ordering of vertices in the polygon.
        /// </summary>
        private int[] meshIndices;

        /// <summary>
        /// A mesh object for rendering the polygon.
        /// </summary>
        public Mesh mesh;

        /// <summary>
        /// The renderer for <see cref="mesh"/>.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// A collider for <see cref="mesh"/>.
        /// </summary>
        private MeshCollider meshCollider;

        /// <summary>
        /// Creates the mesh, mesh renderer, and mesh collider.
        /// </summary>
        public void Awake()
        {
            Install(false);
        }

        public void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        public void Install(bool reset)
        {
            mesh = this.Ensure<MeshFilter>().Value.mesh;
            meshRenderer = this.Ensure<MeshRenderer>();
            meshCollider = this.Ensure<MeshCollider>();
        }

        public void Uninstall()
        {
        }

        /// <summary>
        /// Looks for new tracked planes and creates meshes for them.
        /// </summary>
        private void Update()
        {
            if (trackedPlane != null)
            {
                if (trackedPlane.SubsumedBy != null)
                {
                    Destroy(gameObject);
                }
                else if (Session.Status != SessionStatus.Tracking)
                {
                    meshCollider.enabled = meshRenderer.enabled = false;
                }
                else
                {
                    meshCollider.enabled = meshRenderer.enabled = true;
                    UpdateMeshIfNeeded();
                }
            }
        }

        public Material CurrentGroundMeshMaterial
        {
            get
            {
                return meshRenderer.GetMaterial();
            }
            set
            {
                meshRenderer.SetMaterial(value);
            }
        }

        /// <summary>
        /// Update mesh with a list of Vector3 and plane's center position.
        /// </summary>
        private void UpdateMeshIfNeeded()
        {
            trackedPlane.GetBoundaryPolygon(meshVertices);

            if (!previousFrameMeshVertices.Matches(meshVertices))
            {
                previousFrameMeshVertices.Clear();
                previousFrameMeshVertices.AddRange(meshVertices);

                // A_______B |\ | | \ | | \ | | \ | | \| D-------C

                var planePolygonCount = meshVertices.Count - 2;
                meshIndices = new int[planePolygonCount * 3];

                for (var i = 1; i < planePolygonCount - 1; ++i)
                {
                    var A = 0;
                    var B = i;
                    var C = i + 1;

                    var n = i * 3;
                    meshIndices[n] = A;
                    meshIndices[n + 1] = B;
                    meshIndices[n + 2] = C;
                }

                float minY = float.MaxValue,
                    minX = float.MaxValue,
                    maxX = float.MinValue,
                    minZ = float.MaxValue,
                    maxZ = float.MinValue;

                for (var i = 0; i < meshVertices.Count; ++i)
                {
                    var v = meshVertices[i];
                    minY = Mathf.Min(minY, v.y);
                    minX = Mathf.Min(minX, v.x);
                    maxX = Mathf.Max(maxX, v.x);
                    minZ = Mathf.Min(minZ, v.z);
                    maxZ = Mathf.Max(maxZ, v.z);
                }

                float midX = 0.5f * (minX + maxX),
                    midZ = 0.5f * (minZ + maxZ);

                var loc = new Vector3(midX, minY, midZ);

                for (var i = 0; i < meshVertices.Count; ++i)
                {
                    meshVertices[i] -= loc;
                }

                transform.position = loc;

                mesh.Clear();
                mesh.SetVertices(meshVertices);
                mesh.SetIndices(meshIndices, MeshTopology.Triangles, 0);
            }
        }
    }
}
#endif
