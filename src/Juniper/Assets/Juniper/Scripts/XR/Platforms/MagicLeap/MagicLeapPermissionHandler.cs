#if UNITY_XR_MAGICLEAP
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper
{
    public abstract class MagicLeapPermissionHandler : AbstractPermissionHandler
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            this.Ensure<PrivilegeRequester>();
        }

        public override void Uninstall()
        {
            this.Remove<PrivilegeRequester>();
        }
    }
}
#endif
