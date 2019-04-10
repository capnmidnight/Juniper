using UnityEngine;

namespace Juniper.Unity.Input
{
    public abstract class AbstractVelocityLocomotion : MonoBehaviour
    {
        /// <summary>
        /// Linear velocity for the user
        /// </summary>
        public float moveSpeed = 0.5f;

        public float jink = 0.5f;

        protected const float MOVEMENT_SCALE = 15;

        protected Avatar stage;
        protected UnifiedInputModule input;

        /// <summary>
        /// Retrieves configuration values for the default height of the avatar.
        /// </summary>
        public virtual void Awake()
        {
            stage = ComponentExt.FindAny<Avatar>();
            input = ComponentExt.FindAny<UnifiedInputModule>();
        }
    }
}
