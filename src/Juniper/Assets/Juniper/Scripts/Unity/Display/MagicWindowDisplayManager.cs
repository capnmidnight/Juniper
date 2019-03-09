using Juniper.Unity.Input;

using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.Display
{
    public class MagicWindowDisplayManager : AbstractDisplayManager
    {
        public override bool Install(bool reset)
        {
            var baseInstall = base.Install(reset);

            UnityInput.gyro.enabled = true;
            UnityInput.compensateSensors = true;
            cameraCtrl.mode = CameraControl.Mode.MagicWindow;

            return baseInstall;
        }
    }
}
