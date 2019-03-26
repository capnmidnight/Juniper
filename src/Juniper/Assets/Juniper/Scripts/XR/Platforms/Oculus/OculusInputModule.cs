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
            var baseInstall = base.Install(reset);

            this.EnsureComponent<OVRManager>();

            return baseInstall;
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.RemoveComponent<OVRManager>();
        }
    }
}
#endif