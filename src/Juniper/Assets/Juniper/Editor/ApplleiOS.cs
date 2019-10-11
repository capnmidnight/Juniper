using System;
using System.IO;

using UnityEditor;

namespace Juniper
{
    public static class ApplleiOS
    {
        private static readonly string PATH = PathExt.FixPath("Assets/Plugins/iOS/UnityARKit/NativeInterface/ARSessionNative.mm");

        private static void SetCompileFlags(string flags)
        {
            PluginImporterExt.SetPlatformData(PATH, BuildTarget.iOS, "CompileFlags", flags);
        }

        public static void EnableARKit()
        {
            SetCompileFlags("-DARKIT");
        }

        public static void DisableARKit()
        {
            SetCompileFlags("");
        }
        /// <summary>
        /// The PlayerSettings.iOS.targetOSVersionString value, exposed as a System.Version object.
        /// </summary>
        /// <value>The target OS Version.</value>
        public static Version TargetOSVersion
        {
            get
            {
                return new Version(PlayerSettings.iOS.targetOSVersionString);
            }

            set
            {
                PlayerSettings.iOS.targetOSVersionString = value.ToString(2);
            }
        }

        /// <summary>
        /// The name of the hidden field within Unity's PlayerSettings for the ARKit Support checkbox.
        /// </summary>
        /// <remarks>We can see the existance of this property here: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/PlayerSettingsIOS.bindings.cs#L580-L587</remarks>
        private const string REQUIRES_ARKIT_SUPPORT_KEY = "requiresARKitSupport";

        /// <summary>
        /// Gets are sets a value indicating ARKit support is required.
        /// </summary>
        /// <value><c>true</c> if requires ARK it support; otherwise, <c>false</c>.</value>
        public static bool RequiresARKitSupport
        {
            get
            {
                return HiddenSettingsAccessor.GetHiddenStaticProperty<PlayerSettings.iOS, bool>(REQUIRES_ARKIT_SUPPORT_KEY);
            }

            set
            {
                HiddenSettingsAccessor.SetHiddenStaticProperty<PlayerSettings.iOS>(REQUIRES_ARKIT_SUPPORT_KEY, value);
            }
        }
    }
}
