using Juniper.Animation;

using UnityEngine;

namespace Juniper.Widgets
{
    public class TeleportPad : Clickable
    {
        protected override void OnClick()
        {
            if (!running)
            {
                running = true;
                base.OnClick();
                darth.Enter();
            }
        }

        private Transform userT;
#if UNITY_MODULES_PHYSICS
        private Rigidbody user;
#endif
        private FadeTransition darth;
        private bool running;

        private void Start()
        {
            running = false;
            if (ComponentExt.FindAny(out Avatar avatar))
            {
                userT = avatar.transform;
#if UNITY_MODULES_PHYSICS
                user = userT.GetComponent<Rigidbody>();
#endif
            }

            if (ComponentExt.FindAny(out darth))
            {
                darth.Entered += Darth_Entered;
            }
        }

        private void Darth_Entered(object sender, System.EventArgs e)
        {
            if (running)
            {
                running = false;

                if (userT != null)
                {
                    userT.position = transform.position + 2 * Vector3.up;

#if UNITY_MODULES_PHYSICS
                    if (user != null)
                    {
                        user.isKinematic = true;
                        user.position = userT.position;
                        user.velocity = Vector3.zero;
                        user.isKinematic = false;
                    }
#endif
                }

                darth.Exit();
            }
        }
    }
}
