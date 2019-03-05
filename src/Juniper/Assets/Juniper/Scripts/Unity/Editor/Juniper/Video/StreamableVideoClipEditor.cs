#if UNITY_MODULES_VIDEO

using Juniper.Unity.CustomEditors;

using UnityEditor;

using UnityEngine.Video;

namespace Juniper.Unity.Video.CustomEditors
{
    [CustomPropertyDrawer(typeof(StreamableVideoClip))]
    public class StreamableVideoClipEditor : StreamableAssetEditor<VideoClip, StreamableVideoClip>
    {
    }
}

#endif