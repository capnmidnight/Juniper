#if UNITY_MODULES_VIDEO

using UnityEditor;

using UnityEngine;

namespace Juniper.Audio
{
    [CustomPropertyDrawer(typeof(StreamableAudioClip))]
    public class StreamableAudioClipEditor : StreamableAssetEditor<AudioClip, StreamableAudioClip>
    {
    }
}

#endif