using System;
using System.IO;

using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR

using UnityEditor;

#else
using Juniper.Unity.Data;
#endif

namespace Juniper.Unity
{
    [Serializable]
    public abstract class StreamableAsset
    {
        public string AssetPath;

        public string CopyPath
        {
            get
            {
                return AssetPath.Replace("Assets/", "Assets/StreamingAssets/");
            }
        }

        public string LoadPath =>
#if UNITY_EDITOR
            AssetPath;

#else
            StreamingAssets.FormatPath(Application.dataPath, AssetPath.Replace("Assets/", ""));
#endif

        public ExceptionEvent onLoadError;

        public event EventHandler<Exception> LoadError;

        protected void OnLoadError(Exception exp)
        {
            onLoadError?.Invoke(exp);
            LoadError?.Invoke(this, exp);
        }

#if UNITY_EDITOR

        public abstract void Validate();

        public abstract void Export();

        public abstract void Import();

#endif
    }

    [Serializable]
    public abstract class StreamableAsset<AssetType> : StreamableAsset
        where AssetType : UnityEngine.Object
    {
        [HideInInspector]
        [SerializeField]
        private bool exported;

        public AssetType Asset;

#if UNITY_EDITOR

        public static implicit operator AssetType(StreamableAsset<AssetType> obj)
        {
            return obj.Asset;
        }

        public override void Validate()
        {
            if (Asset == null && exported)
            {
                GetAsset();
            }
            else
            {
                var assetPath = PathExt.Abs2Rel(AssetDatabase.GetAssetPath(Asset));
                if (AssetPath != assetPath)
                {
                    AssetPath = assetPath;
                }
            }
            exported = false;
        }

        public override void Export()
        {
            var absSrc = PathExt.Rel2Abs(AssetPath);
            var absDest = PathExt.Rel2Abs(CopyPath);
            FileExt.Copy(absSrc, absDest, true);
            Asset = null;
            exported = true;
        }

        public override void Import()
        {
            Validate();
        }

        private void GetAsset()
        {
            Asset = ResourceExt.EditorLoadAsset<AssetType>(AssetPath);
        }

#endif
    }
}
