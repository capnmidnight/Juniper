using System.IO;
using System.Reflection;

namespace Juniper
{
    public static class Resources
    {
        private static readonly Assembly assembly = typeof(Resources).Assembly;

        public static Stream GetStream(string name)
        {
            return assembly.GetManifestResourceStream(name);
        }
    }
}
