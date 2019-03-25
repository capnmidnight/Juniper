using Juniper.Progress;

using UnityEditor;

namespace Juniper.UnityEditor.ConfigurationManagement
{
    internal abstract class AbstractPackage
    {
        public string Name;
        public string CompilerDefine;

        public virtual void Install(IProgress prog = null)
        {
            prog?.Report(0);
        }

        public virtual void Activate(BuildTargetGroup targetGroup, IProgress prog = null)
        {
            prog?.Report(0);
        }

        public virtual void Uninstall(IProgress prog = null)
        {
            prog?.Report(0);
        }
    }
}
