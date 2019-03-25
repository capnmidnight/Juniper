#if VUFORIA
using UnityEngine;
using Vuforia;

namespace Juniper.Unity.Display
{
    public class VuforiaDisplayManager : AbstractDisplayManager
    {
        protected override float DEFAULT_FOV
        {
            get
            {
                return 31;
            }
        }

        public override bool Install(bool reset)
        {
            var baseInstall = base.Install(reset);

            this.WithLock(() =>
            {
                var vuforia = this.EnsureComponent<VuforiaBehaviour>().Value;
                vuforia.enabled = false;
            });

            return baseInstall;
        }

        public override void Uninstall()
        {
            this.RemoveComponent<VuforiaBehaviour>();

            base.Uninstall();
        }
    }
}
#endif
