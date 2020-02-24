using UnityEngine;

namespace Juniper.Widgets
{
    public class FollowSystemUserInterface : MonoBehaviour
    {
        private Transform target;

        void LateUpdate()
        {
            if (target == null)
            {
                var master = Find.Any<MasterSceneController>();
                if (master != null)
                {
                    target = master.systemUserInterface;
                }
            }

            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }

}