#if UNITY_MODULES_VIDEO

using Juniper.Progress;

using System;

using UnityEngine.Video;

namespace Juniper.Unity.Video
{
    [Serializable]
    public class StreamableVideoClip : StreamableAsset<VideoClip>
    {
        public void Load(string cacheDirectory, TimeSpan ttl, Action<string> resolve, IProgress prog = null)
        {
            prog?.Report(0);
            resolve(LoadPath);
            prog?.Report(1);
        }
    }
}

#endif