#if OCULUS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.Unity.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            this.EnsureComponent<OVRManager>();
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.RemoveComponent<OVRManager>();
        }
    }
}
#endif