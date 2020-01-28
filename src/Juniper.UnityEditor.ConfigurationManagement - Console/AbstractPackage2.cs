using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Juniper.IO;

namespace Juniper.ConfigurationManagement
{
    public abstract class AbstractPackage2
    {
        private static string unityProjectDirectory;

        internal static string UnityProjectRoot
        {
            get
            {
                if (unityProjectDirectory is null)
                {
                    unityProjectDirectory = Environment.CurrentDirectory;
                }

                return unityProjectDirectory;
            }
            set
            {
                unityProjectDirectory = value;
            }
        }

        public string Name { get; }

        public string Version { get; }

        public string ContentPath { get; }

        public string CompilerDefine { get; }

        public abstract PackageSource Source { get; }

        public abstract bool Available { get; }

        public abstract bool Cached { get; }

        public abstract float InstallPercentage { get; }

        public abstract bool IsInstalled { get; }

        public abstract bool CanUpdate { get; }

        public string PackageID { get; }

        protected AbstractPackage2(string packageID, string name, string version, string path, string compilerDefine)
        {
            PackageID = packageID;
            Name = name;
            Version = version;
            ContentPath = path;
            CompilerDefine = compilerDefine;
        }

        public abstract void Install();
    }
}
