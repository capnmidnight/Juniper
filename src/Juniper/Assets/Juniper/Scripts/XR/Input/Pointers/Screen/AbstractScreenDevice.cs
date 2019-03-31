using Juniper.Unity.Display;
using Juniper.Unity.Haptics;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Screen
{
    /// <summary>
    /// A ScreenDevice is a <see cref="AbstractPointerDevice{ButtonIDType, HapticsType}"/> that has
    /// its primary pointer interactions happening in screen space.
    /// </summary>
    public abstract class AbstractScreenDevice<ButtonIDType, HapticsType, ConfigType> :
        AbstractPointerDevice<ButtonIDType, HapticsType, ConfigType>,
        IScreenDevice
        where ButtonIDType : struct
        where HapticsType : AbstractHapticDevice
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        public virtual Vector3 CameraPositionOffset
        {
            get
            {
                return Vector3.zero;
            }
        }

        public virtual Quaternion CameraRotationOffset
        {
            get
            {
                return Quaternion.identity;
            }
        }

        protected override void InternalUpdate()
        {
            var camT = DisplayManager.MainCamera.transform;
            var cameraRotation = Quaternion.LookRotation(Direction, camT.up) * CameraRotationOffset;
            transform.position = camT.position + (cameraRotation * CameraPositionOffset);
            transform.rotation = cameraRotation;
        }
    }
}
