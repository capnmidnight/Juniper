using Juniper.Display;

using UnityEngine;

namespace Juniper.Input
{
    public class RocketPack : MonoBehaviour
    {
        public float force = 100000;

#if UNITY_MODULES_PHYSICS
        private Rigidbody body;
        private Transform camT;

        public void Start()
        {
            body = GetComponent<Rigidbody>();
            camT = DisplayManager.MainCamera.transform;
        }

        public void FixedUpdate()
        {
            if (body != null && UnityEngine.Input.GetButton("Jump"))
            {
                body.AddForce(camT.up * force * Time.fixedDeltaTime, ForceMode.Force);
            }
        }

#endif
    }
}
