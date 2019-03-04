#if VUFORIA
using UnityEngine;
using Vuforia;

namespace Juniper.Display
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
            reset &= Application.isEditor;

            base.Install(reset);

            this.WithLock(() =>
            {
                var vuforia = this.EnsureComponent<VuforiaBehaviour>().Value;
                vuforia.enabled = false;
            });
        }

        public override void Uninstall()
        {
            this.RemoveComponent<VuforiaBehaviour>();

            base.Uninstall();
        }
    }
}
#endif
