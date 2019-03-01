#if UNITY_MODULES_VIDEO
using System;

using Juniper.Progress;

using UnityEngine.Video;

namespace Juniper.Video
{
    [Serializable]
    public class StreamableVideoClip : StreamableAsset<VideoClip>
    {
        public void Load(string cacheDirectory, TimeSpan ttl, Action<string> resolve, IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
            resolve(LoadPath);
            prog?.SetProgress(1);
        }
    }
}
#endif
