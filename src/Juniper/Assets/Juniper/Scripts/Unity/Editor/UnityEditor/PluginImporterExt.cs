using System.IO;

namespace UnityEditor
{
    public static class PluginImporterExt
    {
        public static void SetPlatformData(string file, BuildTarget target, string property, string value)
        {
            if (File.Exists(file))
            {
                var plugin = (PluginImporter)AssetImporter.GetAtPath(file);
                plugin?.SetPlatformData(target, property, value);
            }
        }

        public static class iOS
        {
            public static class ARKit
            {
                private static readonly string PATH = PathExt.FixPath("Assets/Plugins/iOS/UnityARKit/NativeInterface/ARSessionNative.mm");

                private static void SetCompileFlags(string flags)
                {
                    SetPlatformData(PATH, BuildTarget.iOS, "CompileFlags", flags);
                }

                public static void Enable()
                {
                    SetCompileFlags("-DARKIT");
                }

                public static void Disable()
                {
                    SetCompileFlags("");
                }
            }
        }
    }
}
