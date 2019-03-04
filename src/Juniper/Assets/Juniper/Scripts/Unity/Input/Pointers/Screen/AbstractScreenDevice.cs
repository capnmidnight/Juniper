using Juniper.Display;
using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers.Screen
{
    /// <summary>
    /// A ScreenDevice is a <see cref="AbstractPointerDevice{ButtonIDType, HapticsType}"/> that has its primary
    /// pointer interactions happening in screen space.
    /// </summary>
    public abstract class AbstractScreenDevice<ButtonIDType, HapticsType, ConfigType> :
        AbstractPointerDevice<ButtonIDType, HapticsType, ConfigType>,
        IScreenDevice
        where ButtonIDType : struct
        where HapticsType : AbstractHapticDevice
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        public override Vector3 WorldPoint
        {
            get
            {
                return WorldFromScreen(ScreenPoint);
            }
        }

        public override Vector2 ViewportPoint
        {
            get
            {
                return ViewportFromScreen(ScreenPoint);
            }
        }

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
            var cameraRotation = Quaternion.LookRotation(InteractionDirection, camT.up) * CameraRotationOffset;
            transform.position = camT.position + (cameraRotation * CameraPositionOffset);
            transform.rotation = cameraRotation;
        }
    }
}
