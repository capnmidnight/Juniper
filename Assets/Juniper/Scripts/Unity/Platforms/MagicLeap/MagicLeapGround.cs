#if MAGIC_LEAP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Ground
{
    public class MagicLeapGround :
        AbstractARGround
    {
        private static readonly Dictionary<Level, MLSpatialMapper.LevelOfDetail> DETAIL_LEVELS = new Dictionary<Level, MLSpatialMapper.LevelOfDetail>
        {
            { Level.Low, MLSpatialMapper.LevelOfDetail.Minimum },
            { Level.Medium, MLSpatialMapper.LevelOfDetail.Medium },
            { Level.High, MLSpatialMapper.LevelOfDetail.Maximum }
        };

        public override void Install(bool reset)
        {
            var stage = ComponentExt.FindAny<StageExtensions>();
            var mapper = stage.EnsureComponent<MLSpatialMapper>().Value;
            mapper.meshParent = transform;
            if (mapper.meshPrefab == null)
            {
                var pre = mapper.EnsureTransform("Original")
                    .Value
                    .gameObject;
                pre.EnsureComponent<MeshFilter>();
                pre.EnsureComponent<MeshCollider>();
                pre.EnsureComponent<MeshRenderer>((rend) =>
                    rend.SetMaterial(CurrentMaterial));

                pre.Deactivate();
                mapper.meshPrefab = pre;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var stage = ComponentExt.FindAny<StageExtensions>();
            var mapper = stage.GetComponent<MLSpatialMapper>();
            mapper.meshType = MLSpatialMapper.MeshType.Triangles;
            mapper.levelOfDetail = DETAIL_LEVELS[spatialMappingFidelity];
            mapper.computeNormals = true;
            mapper.removeMeshSkirt = false;
            mapper.meshAdded += Mapper_meshAdded;
        }

        private void Mapper_meshAdded(UnityEngine.Experimental.XR.TrackableId obj)
        {
            var stage = ComponentExt.FindAny<StageExtensions>();
            var mapper = stage.GetComponent<MLSpatialMapper>();
            var mesh = mapper.meshIdToGameObjectMap[obj];
            var rend = mesh.GetComponent<Renderer>();
            rend.SetMaterial(CurrentMaterial);
        }

        public override void Uninstall()
        {
            base.Uninstall();

            var stage = ComponentExt.FindAny<StageExtensions>();
            var mapper = stage.GetComponent<MLSpatialMapper>();
            mapper.transform.Find("Original")
                ?.gameObject
                ?.Destroy();

            mapper.Destroy();
        }
    }
}

#endif
