using System.Reflection;

namespace Juniper.IO
{
    public class AssemblyResourceDataSource : IDataSource
    {
        private readonly Assembly assembly;

        public AssemblyResourceDataSource(Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public Stream? GetStream(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return assembly.GetManifestResourceStream(fileName);
        }
    }
}