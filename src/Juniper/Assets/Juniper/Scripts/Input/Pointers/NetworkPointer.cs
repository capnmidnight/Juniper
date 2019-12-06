using System;

using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers
{
    public class NetworkPointer
        : AbstractPointerDevice<Unary, UnaryPointerConfiguration, NoHaptics>
    {
        private static readonly TimeSpan CONNECTION_TIMEOUT = TimeSpan.FromSeconds(5);

        public DateTime lastClickTime;
        public Vector3 offset = Vector3.zero;

        private DateTime lastUpdate = DateTime.MinValue;
        private bool pressed, wasPressed;

        private Ray ray;

        public Ray Ray
        {
            get
            {
                return ray;
            }
            set
            {
                ray = value;
                lastUpdate = DateTime.Now;
            }
        }

        public override bool IsConnected
        {
            get
            {
                return (DateTime.Now - lastUpdate) < CONNECTION_TIMEOUT;
            }
        }

        public override bool IsButtonPressed(Unary button)
        {
            return pressed;
        }

        public override bool IsButtonDown(Unary button)
        {
            return pressed && !wasPressed;
        }

        public override bool IsButtonUp(Unary button)
        {
            return !pressed && wasPressed;
        }

        protected override void InternalUpdate()
        {
            var delta = DateTime.UtcNow - lastClickTime;
            wasPressed = pressed;

            pressed = delta.TotalMilliseconds <= 0.25;
            transform.position = ray.origin + offset;
            transform.rotation = Quaternion.LookRotation(ray.direction);
        }

        public override Vector3 WorldPoint
        {
            get
            {
                return transform.position
                    + transform.forward * MinimumPointerDistance;
            }
        }
    }
}
