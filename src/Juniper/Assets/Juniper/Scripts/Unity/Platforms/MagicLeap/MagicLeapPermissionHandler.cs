#if UNITY_XR_MAGICLEAP
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Unity
{
    public abstract class MagicLeapPermissionHandler : AbstractPermissionHandler
    {
        public override bool Install(bool reset)
        {
            var baseInstall = base.Install(reset);

            this.EnsureComponent<PrivilegeRequester>();

            return baseInstall;
        }

        public override bool Uninstall()
        {
            this.RemoveComponent<PrivilegeRequester>();
        }
    }
}
#endif