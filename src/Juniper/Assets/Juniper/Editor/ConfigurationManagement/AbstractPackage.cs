using Juniper.Progress;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    internal abstract class AbstractPackage
    {
        public string Name { get; set; }
        public virtual string CompilerDefine { get; set; }
        public abstract bool IsInstalled { get; }

        public void Install()
        {
            Install(null);
        }

        public void Install(IProgress prog)
        {
            prog?.Report(0);
            InstallInternal(prog);
            prog?.Report(1);
        }
        protected abstract void InstallInternal(IProgress prog);

        public void Activate(BuildTargetGroup targetGroup)
        {
            Activate(targetGroup, null);
        }

        public virtual void Activate(BuildTargetGroup targetGroup, IProgress prog)
        {
            prog?.Report(0);
        }

        public void Uninstall()
        {
            Uninstall(null);
        }

        public virtual void Uninstall(IProgress prog)
        {
            prog?.Report(0);
        }
    }
}
