#if UNITY_MODULES_VIDEO

using UnityEditor;

using UnityEngine.Video;

namespace Juniper.Video
{
    [CustomPropertyDrawer(typeof(StreamableVideoClip))]
    public class StreamableVideoClipEditor : StreamableAssetEditor<VideoClip, StreamableVideoClip>
    {
    }
}

#endif