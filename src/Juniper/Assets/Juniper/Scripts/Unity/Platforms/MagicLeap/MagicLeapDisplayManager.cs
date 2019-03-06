#if MAGIC_LEAP

using MSA;

using UnityEngine;

namespace Juniper.Unity.Display
{
    public class MagicLeapDisplayManager : AbstractDisplayManager
    {
        protected override float DEFAULT_FOV
        {
            get
            {
#if UNITY_EDITOR
                return 80;
#else
                return 30;
#endif
            }
        }

        public override void Install(bool reset)
        {
            reset &= Application.isEditor;

            base.Install(reset);

            listener.EnsureComponent<MSAListener>();
        }

        public override void Uninstall()
        {
            this.RemoveComponent<MSAListener>();

            base.Uninstall();
        }
    }
}

#endif
