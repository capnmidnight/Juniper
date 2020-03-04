#if UNITY_XR_MAGICLEAP

using Juniper.Display;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Ground
{
    public class MagicLeapGround : AbstractGround
    {
        private static readonly Dictionary<Level, MLSpatialMapper.LevelOfDetail> DETAIL_LEVELS = new Dictionary<Level, MLSpatialMapper.LevelOfDetail>(3)
        {
            { Level.Low, MLSpatialMapper.LevelOfDetail.Minimum },
            { Level.Medium, MLSpatialMapper.LevelOfDetail.Medium },
            { Level.High, MLSpatialMapper.LevelOfDetail.Maximum }
        };

        private MLSpatialMapper mapper;

        public override void Install(bool reset)
        {
            base.Install(reset);

            mapper = DisplayManager.MainCamera.Ensure<MLSpatialMapper>().Value;
            mapper.meshParent = transform;
            if (mapper.meshPrefab == null)
            {
                var pre = mapper.Ensure<Transform>("Original")
                    .Value
                    .gameObject;
                pre.Ensure<MeshFilter>();
                pre.Ensure<MeshCollider>();
                pre.Ensure<MeshRenderer>((rend) =>
                    rend.SetMaterial(CurrentMaterial));

                pre.Deactivate();
                mapper.meshPrefab = pre;
            }
        }

        public override void Awake()
        {
            base.Awake();

            mapper.meshType = MLSpatialMapper.MeshType.Triangles;
            mapper.levelOfDetail = DETAIL_LEVELS[spatialMappingFidelity];
            mapper.computeNormals = true;
            mapper.removeMeshSkirt = false;
            mapper.meshAdded += Mapper_meshAdded;
        }

        private void Mapper_meshAdded(UnityEngine.Experimental.XR.TrackableId obj)
        {
            var mesh = mapper.meshIdToGameObjectMap[obj];
            var rend = mesh.GetComponent<Renderer>();
            rend.SetMaterial(CurrentMaterial);
        }

        public override void Uninstall()
        {
            base.Uninstall();

            foreach (var mapper in Find.All<MLSpatialMapper>())
            {
                mapper?.transform
                    ?.Find("Original")
                    ?.gameObject
                    ?.Destroy();

                mapper?.Destroy();
            }
        }
    }
}

#endif