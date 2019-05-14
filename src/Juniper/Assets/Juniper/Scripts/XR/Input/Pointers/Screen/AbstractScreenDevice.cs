using UnityEngine;

namespace Juniper.Input.Pointers.Screen
{
    /// <summary>
    /// A ScreenDevice is a <see cref="AbstractPointerDevice{ButtonIDType, ConfigType}"/> that has
    /// its primary pointer interactions happening in screen space.
    /// </summary>
    public abstract class AbstractScreenDevice<ButtonIDType, ConfigType> :
        AbstractPointerDevice<ButtonIDType, ConfigType>,
        IScreenDevice
        where ButtonIDType : struct
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
    }
}
