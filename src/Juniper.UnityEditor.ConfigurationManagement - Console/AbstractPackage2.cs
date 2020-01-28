using System;
using System.IO;

namespace Juniper.ConfigurationManagement
{
    public abstract class AbstractPackage2
    {
        private static string unityProjectDirectory;

        internal static string UnityProjectRoot
        {
            get
            {
                return unityProjectDirectory ?? Environment.CurrentDirectory;
            }
            set
            {
                unityProjectDirectory = value;
            }
        }



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
