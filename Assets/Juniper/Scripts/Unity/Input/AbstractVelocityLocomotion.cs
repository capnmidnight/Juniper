using UnityEngine;

namespace Juniper.Input
{
    public abstract class AbstractVelocityLocomotion : MonoBehaviour
    {
        /// <summary>
        /// Linear velocity for the user
        /// </summary>
        public float moveSpeed = 0.5f;

        public float jink = 0.5f;

        protected const float MOVEMENT_SCALE = 15;
        protected Vector3 velocity;

        private StageExtensions stage;

        /// <summary>
        /// Retrieves configuration values for the default height of the avatar.
        /// </summary>
        public virtual void Awake() =>
            stage = ComponentExt.FindAny<StageExtensions>();

        public virtual void Update() =>
            stage.SetVelocity(velocity);
    }
}
