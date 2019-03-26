#if UNITY_XR_OCULUS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.Unity.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                this.Ensure<OVRManager>();
                return true;
            }

            return false;
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.Remove<OVRManager>();
        }
    }
}
#endif