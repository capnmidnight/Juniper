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

        public override void Install(bool reset)
        {
            base.Install(reset);

            this.WithLock(() =>
            {
                var vuforia = this.Ensure<VuforiaBehaviour>().Value;
                vuforia.enabled = false;
            });
        }

        public override void Uninstall()
        {
            this.Remove<VuforiaBehaviour>();

            base.Uninstall();
        }
    }
}
#endif
