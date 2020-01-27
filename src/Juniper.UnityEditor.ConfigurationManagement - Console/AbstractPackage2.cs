namespace Juniper.ConfigurationManagement
{
    public abstract class AbstractPackage2
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "<Pending>")]
        internal static string UnityProjectRoot;

        public string Name { get; }

        public string Version { get; }

        public string ContentPath { get; }

        public abstract PackageSource Source { get; }

        public abstract bool Available { get; }

        public abstract bool Cached { get; }

        public abstract bool IsInstalled { get; }

        public abstract bool CanUpdate { get; }

        protected AbstractPackage2(string name, string version, string path)
        {
            Name = name;
            Version = version;
            ContentPath = path;
        }

        public abstract void Install();
    }
}
