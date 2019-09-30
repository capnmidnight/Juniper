using System.IO;

using Juniper.HTTP;

using UnityEngine;
namespace Juniper.Data
{
    public class StreamingAssetsCacheLayer :
#if !UNITY_EDITOR && UNITY_ANDROID
        Juniper.Compression.Zip.ZipFileCacheLayer
#else
        Juniper.Caching.FileCacheLayer
#endif
    {
        public StreamingAssetsCacheLayer()
            : base(Application.dataPath)
        { }

        protected override string GetCacheFileName(string fileDescriptor, MediaType contentType)
        {
            var name = base.GetCacheFileName(fileDescriptor, contentType);
#if !UNITY_EDITOR && UNITY_ANDROID
            name = Path.Combine("assets", name);
#elif !UNITY_EDITOR && UNITY_IOS
            name = Path.Combine("Raw", name);
#else
            name = Path.Combine("StreamingAssets", name);
#endif
            return name;
        }
    }
}