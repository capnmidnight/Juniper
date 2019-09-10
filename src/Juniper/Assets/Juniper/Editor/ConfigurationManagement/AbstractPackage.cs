using Juniper.Progress;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    internal abstract class AbstractPackage
    {
        public string Name;
        public string CompilerDefine;

        public void Install(IProgress prog = null)
        {
            prog?.Report(0);
            if (!IsInstalled)
            {
                InstallInternal(prog);
            }
            prog?.Report(1);
        }

        public virtual void Activate(BuildTargetGroup targetGroup, IProgress prog = null)
        {
            prog?.Report(0);
        }

        public virtual void Uninstall(IProgress prog = null)
        {
            prog?.Report(0);
        }

        public abstract bool IsInstalled
        {
            get;
        }

        protected abstract void InstallInternal(IProgress prog);
    }
}
