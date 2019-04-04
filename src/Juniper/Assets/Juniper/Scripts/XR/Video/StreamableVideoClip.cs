#if UNITY_MODULES_VIDEO

using System;

using Juniper.Progress;

using UnityEngine.Video;

namespace Juniper.Unity.Video
{
    [Serializable]
    public class StreamableVideoClip : StreamableAsset<VideoClip>
    {
    }
}

#endif