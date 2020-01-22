using System.IO;

using UnityEditor;

namespace Juniper
{
    public static class PluginImporterExt
    {
        public static void SetPlatformData(string file, BuildTarget target, string property, string value)
        {
            if (File.Exists(file))
            {
                var plugin = (PluginImporter)AssetImporter.GetAtPath(file);
                if (plugin != null)
                {
                    plugin.SetPlatformData(target, property, value);
                }
            }
        }
    }
}
