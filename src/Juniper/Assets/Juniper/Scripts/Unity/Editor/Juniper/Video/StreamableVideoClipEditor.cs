#if UNITY_MODULES_VIDEO

using Juniper.Unity.Video;

using UnityEditor;

using UnityEngine.Video;

namespace Juniper.UnityEditor.Video
{
    [CustomPropertyDrawer(typeof(StreamableVideoClip))]
    public class StreamableVideoClipEditor : StreamableAssetEditor<VideoClip, StreamableVideoClip>
    {
    }
}

#endif