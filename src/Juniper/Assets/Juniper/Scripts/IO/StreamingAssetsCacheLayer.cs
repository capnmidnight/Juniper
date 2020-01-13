#if !UNITY_EDITOR && UNITY_ANDROID
#define UNPACK_APK
#endif

using System.Threading.Tasks;

using Juniper.Compression.Zip;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.IO
{
    public class StreamingAssetsCacheLayer : FileCacheLayer
    {
        public static bool NeedsUnpack
        {
            get
            {
#if UNPACK_APK
                return true;
#else
                return false;
#endif
            }
        }

        public static Task UnpackAPK(IProgress prog)
        {
            var apk = Application.dataPath;
            var appData = Application.persistentDataPath;
            return Task.Run(() =>
                Decompressor.Decompress(apk, appData, "assets/", prog));
        }

        public StreamingAssetsCacheLayer()
            : base(
#if UNPACK_APK
            Application.persistentDataPath
#else
            Application.streamingAssetsPath
#endif
                  )
        {
        }
    }
}