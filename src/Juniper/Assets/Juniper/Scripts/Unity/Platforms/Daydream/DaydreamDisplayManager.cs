#if GOOGLEVR
using UnityEngine;

namespace Juniper.Unity.Display
{
    public class DaydreamDisplayManager : AbstractDisplayManager
    {
        public override bool Install(bool reset)
        {
            reset &= Application.isEditor;

            var baseInstall = base.Install(reset);

            this.EnsureComponent<GvrHeadset>();
        }

        public override void Uninstall()
        {
            this.RemoveComponent<GvrHeadset>();

            base.Uninstall();
        }
    }
}
#endif
