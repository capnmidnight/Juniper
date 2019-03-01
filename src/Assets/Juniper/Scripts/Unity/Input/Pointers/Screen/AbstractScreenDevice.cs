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
        public override Vector3 WorldPoint =>
            WorldFromScreen(ScreenPoint);

        public override Vector2 ViewportPoint =>
            ViewportFromScreen(ScreenPoint);

        public virtual Vector3 CameraPositionOffset =>
            Vector3.zero;

        public virtual Quaternion CameraRotationOffset =>
            Quaternion.identity;

        protected override void InternalUpdate()
        {
            var camT = CameraExtensions.MainCamera.transform;
            var cameraRotation = Quaternion.LookRotation(InteractionDirection, camT.up) * CameraRotationOffset;
            transform.position = camT.position + (cameraRotation * CameraPositionOffset);
            transform.rotation = cameraRotation;
        }
    }
}
