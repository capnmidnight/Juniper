#if UNITY_XR_MAGICLEAP
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Unity
{
    public abstract class MagicLeapPermissionHandler : AbstractPermissionHandler
    {
        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                this.Ensure<PrivilegeRequester>();
                return true;
            }

            return false;
        }

        public override bool Uninstall()
        {
            this.Remove<PrivilegeRequester>();
        }
    }
}
#endif
