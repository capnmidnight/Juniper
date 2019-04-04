#if UNITY_MODULES_VIDEO

using Juniper.Unity.Audio;

using UnityEditor;

using UnityEngine;

namespace Juniper.UnityEditor.Audio
{
    [CustomPropertyDrawer(typeof(StreamableAudioClip))]
    public class StreamableAudioClipEditor : StreamableAssetEditor<AudioClip, StreamableAudioClip>
    {
    }
}

#endif