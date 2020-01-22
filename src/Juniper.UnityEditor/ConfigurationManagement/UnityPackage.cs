using System;
using System.Linq;

using Juniper.IO;

using Newtonsoft.Json.Linq;

namespace Juniper.ConfigurationManagement
{
    public sealed class UnityPackage : AbstractPackage
    {
        public string version { get; set; }
        public UnityPackage(string name)
        {
            var parts = name.Split('@');
            Name = parts[0];
            version = parts[1];

            parts = Name.ToUpperInvariant()
                .SplitX('.')
                .Skip(1)
                .ToArray();
            CompilerDefine = string.Join("_", parts).Replace('-', '_');
        }

        public static JObject Dependencies;

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
            Dependencies[Name] = version;
        }

        public override void Uninstall(IProgress prog)
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