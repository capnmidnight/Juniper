using Juniper.Progress;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    internal abstract class AbstractPackage
    {
        public string Name;
        public string CompilerDefine;

        public virtual void Install(IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
        }

        public virtual void Activate(BuildTargetGroup targetGroup, IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
        }

        public virtual void Uninstall(IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
        }
    }
}