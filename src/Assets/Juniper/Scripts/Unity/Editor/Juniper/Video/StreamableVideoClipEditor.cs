#if UNITY_MODULES_VIDEO
using Juniper.CustomEditors;

using UnityEditor;
using UnityEngine.Video;

namespace Juniper.Video.CustomEditors
{
    [CustomPropertyDrawer(typeof(StreamableVideoClip))]
    public class StreamableVideoClipEditor : StreamableAssetEditor<VideoClip, StreamableVideoClip>
    {
    }
}
#endif
