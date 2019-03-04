using UnityEngine;

namespace UnityEditor
{
    /// <summary>
    /// A collection of extra settings that Unity doesn't expose natively.
    /// </summary>
    public class AudioSettingsExt : HiddenSettingsAccessor
    {
        /// <summary>
        /// Unity provides SetSpatializerPlugin, but it hides SetAmbisonicDecoderPlugin.
        /// </summary>
        /// <param name="name">Name.</param>
        public static void SetAmbisonicDecoderPluginName(string name)
        {
            CallHiddenStaticMethod<AudioSettings>("SetAmbisonicDecoderPluginName", name);
        }
    }
}