using UnityEngine;

namespace Juniper.World.LightEstimation
{
    /// <summary>
    /// Uses the AR subsystem's light estimation values to modify the current ambient and directional
    /// light settings to make for a more convincing indoor scene. Only one of these components may
    /// be included on a gameObject. This component requires a Unity Light component. This component
    /// runs in the editor during edit mode.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class IndoorLightEstimate : AbstractLightEstimate
    {
        /// <summary>
        /// Gets the estimated cloud cover.
        /// </summary>
        /// <value>The cloud cover.</value>
        protected override float CloudCover
        {
            get
            {
                return 0.25f;
            }
        }

        /// <summary>
        /// Defaults to false. Overriding classes can indicate they have a rotation value to make the
        /// base class update the sun light's rotation value. Otherwise, it will not be modified from
        /// whatever the user set in the Unity Editor.
        /// </summary>
        /// <value><c>true</c> if has sun rotation; otherwise, <c>false</c>.</value>
        protected override bool HasSunRotation
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a rotation quaternion that will put the "sun dot" of Unity's default skybox into
        /// the correct position in the sky.
        /// </summary>
        /// <value>The sun rotation.</value>
        protected override Vector3 SunRotation
        {
            get
            {
                return overhead;
            }
        }
    }
}
