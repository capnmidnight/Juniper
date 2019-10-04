using System.IO;

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
        protected override string GetCacheFileName<MediaTypeT>(IO.IContentReference<MediaTypeT> fileRef)
        {
            var newFileName = Path.Combine(
                "assets",
                PathExt.FixPath(fileRef.CacheID));
            var newRef = new IO.ContentReference<MediaTypeT>(
                newFileName,
                fileRef.ContentType);
            return base.GetCacheFileName(newRef);
        }
#endif
    }
}