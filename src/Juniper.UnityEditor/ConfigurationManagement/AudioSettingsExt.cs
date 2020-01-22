 using UnityEngine;

namespace Juniper
{
    /// <summary>
    /// A collection of extra settings that Unity doesn't expose natively.
    /// </summary>
    public static class AudioSettingsExt
    {
        /// <summary>
        /// Unity provides SetSpatializerPlugin, but it hides SetAmbisonicDecoderPlugin.
        /// </summary>
        /// <param name="name">Name.</param>
        public static void SetAmbisonicDecoderPluginName(string name)
        {
            HiddenSettingsAccessor.CallHiddenStaticMethod<AudioSettings>("SetAmbisonicDecoderPluginName", name);
        }
    }
}
