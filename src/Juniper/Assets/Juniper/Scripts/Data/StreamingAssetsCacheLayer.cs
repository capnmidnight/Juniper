using System.IO;

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
            : base(
#if UNITY_EDITOR
            Path.Combine(Application.dataPath, "StreamingAssets")
#elif UNITY_ANDROID
            Application.dataPath
#else
            Application.streamingAssetsPath
#endif
                  )
        { }


#if !UNITY_EDITOR && UNITY_ANDROID
        protected override string GetCacheFileName(string fileDescriptor, HTTP.MediaType contentType)
        {
            var name = Path.Combine("assets", fileDescriptor);
            return base.GetCacheFileName(name, contentType);
        }
#endif
    }
}