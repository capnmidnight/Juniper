using System.IO;
using Juniper.IO;
using Juniper.HTTP;
using UnityEngine;

namespace Juniper.Data
{
    public class StreamingAssetsCacheLayer :
#if !UNITY_EDITOR && UNITY_ANDROID
        Juniper.Compression.Zip.ZipFileCacheLayer
#else
        Juniper.IO.FileCacheLayer
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
        protected override string GetCacheFileName<MediaTypeT>(IContentReference<MediaTypeT> source)
        {
            var newFileName = Path.Combine(
                "assets",
                PathExt.FixPath(source.CacheID));
            var newRef = new ContentReference<MediaTypeT>(
                newFileName,
                source.ContentType);
            return base.GetCacheFileName(newRef);
        }
#endif
    }
}