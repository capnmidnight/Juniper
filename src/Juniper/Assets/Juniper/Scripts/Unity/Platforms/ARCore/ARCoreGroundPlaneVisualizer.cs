#if ARCORE
using UnityEngine;

using System.Collections.Generic;
using GoogleARCore;

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
        static int count = 0;

        /// <summary>
        /// Initializes the TrackedPlaneVisualizer with a TrackedPlane.
        /// </summary>
        /// <param name="plane">The plane to vizualize.</param>
        public static ARCoreGroundPlaneVisualizer Initialize(TrackedPlane plane)
        {
            var planeObject = new GameObject($"GroundPlane_{count++}");
            var gpv = planeObject.AddComponent<ARCoreGroundPlaneVisualizer>();
            gpv.trackedPlane = plane;
            //var planeVisualizer = planeObject.AddComponent<TrackedPlaneVisualizer>();
            //planeVisualizer.Initialize(plane);
            return gpv;
        }

        /// <summary>
        /// The ARCore tracked plane object.
        /// </summary>
        TrackedPlane trackedPlane;

        /// <summary>
        /// Keeps the previous frame's mesh polygon to avoid mesh update every frame.
        /// </summary>
        List<Vector3> previousFrameMeshVertices = new List<Vector3>();

        /// <summary>
        /// The mesh polygon for the current frame.
        /// </summary>
        List<Vector3> meshVertices = new List<Vector3>();

        /// <summary>
        /// The ordering of vertices in the polygon.
        /// </summary>
        int[] meshIndices;

        /// <summary>
        /// A mesh object for rendering the polygon.
        /// </summary>
        public Mesh mesh;

        /// <summary>
        /// The renderer for <see cref="mesh"/>.
        /// </summary>
        MeshRenderer meshRenderer;

        /// <summary>
        /// A collider for <see cref="mesh"/>.
        /// </summary>
        MeshCollider meshCollider;

        /// <summary>
        /// Creates the mesh, mesh renderer, and mesh collider.
        /// </summary>
        public void Awake() =>
            Install(false);

#if UNITY_EDITOR
        public void Reset() =>
            Install(true);
#endif

        public void Install(bool reset)
        {
            mesh = this.EnsureComponent<MeshFilter>().Value.mesh;
            meshRenderer = this.EnsureComponent<MeshRenderer>();
            meshCollider = this.EnsureComponent<MeshCollider>();
        }

        public void Uninstall() { }

        /// <summary>
        /// Looks for new tracked planes and creates meshes for them.
        /// </summary>
        void Update()
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
            get { return meshRenderer.GetMaterial(); }
            set { meshRenderer.SetMaterial(value); }
        }

        /// <summary>
        /// Update mesh with a list of Vector3 and plane's center position.
        /// </summary>
        void UpdateMeshIfNeeded()
        {
            trackedPlane.GetBoundaryPolygon(meshVertices);

            if (!previousFrameMeshVertices.Matches(meshVertices))
            {
                previousFrameMeshVertices.Clear();
                previousFrameMeshVertices.AddRange(meshVertices);

                // A_______B |\ | | \ | | \ | | \ | | \| D-------C

                int planePolygonCount = meshVertices.Count - 2;
                meshIndices = new int[planePolygonCount * 3];

                for (int i = 1; i < planePolygonCount - 1; ++i)
                {
                    int A = 0;
                    int B = i;
                    int C = i + 1;

                    int n = i * 3;
                    meshIndices[n] = A;
                    meshIndices[n + 1] = B;
                    meshIndices[n + 2] = C;
                }

                float minY = float.MaxValue,
                    minX = float.MaxValue,
                    maxX = float.MinValue,
                    minZ = float.MaxValue,
                    maxZ = float.MinValue;

                for (int i = 0; i < meshVertices.Count; ++i)
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

                for (int i = 0; i < meshVertices.Count; ++i)
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