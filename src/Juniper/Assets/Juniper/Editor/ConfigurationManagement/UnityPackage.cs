using System.Linq;

using Juniper.Progress;

using Json.Lite.Linq;

namespace Juniper.ConfigurationManagement
{
    internal sealed class UnityPackage : AbstractPackage
    {
        public string version;

        private const string UNITY_SUBMODULE_PREFIX = "com.unity.modules.";

        internal UnityPackage(string name)
        {
            var parts = name.Split('@');
            Name = parts[0];
            version = parts[1];

#if !UNITY_2018_2_OR_NEWER
            if (Name == "com.unity.package-manager-ui")
            {
                version = "1.8.8";
            }
#endif

            parts = Name.ToUpperInvariant()
                .Split('.')
                .Skip(1)
                .ToArray();
            CompilerDefine = string.Join("_", parts).Replace('-', '_');
        }

        internal static JObject Dependencies;

        public override bool IsInstalled
        {
            get
            {
                var pkg = (string)Dependencies[Name];
                return pkg == version;
            }
        }

        protected override void InstallInternal(IProgress prog)
        {
#if UNITY_2018_2_OR_NEWER
            Dependencies[Name] = version;
#else
            if (!Name.StartsWith(UNITY_SUBMODULE_PREFIX))
            {
                Dependencies[Name] = version;
            }
#endif
        }

        public override void Uninstall(IProgress prog = null)
        {
            base.Uninstall(prog);

            if (Dependencies[Name] != null)
            {
                Dependencies.Remove(Name);
            }

            prog?.Report(1);
        }
    }
}
