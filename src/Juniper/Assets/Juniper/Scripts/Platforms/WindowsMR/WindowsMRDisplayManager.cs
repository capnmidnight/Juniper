#if UNITY_XR_WINDOWSMR_METRO
using UnityEngine;

namespace Juniper.Display
{
    public class WindowsMRDisplayManager : AbstractDisplayManager
    {
#if UNITY_5_3_OR_NEWER && (!UNITY_2017_1_OR_NEWER || (UNITY_2017_2_OR_NEWER && !UNITY_2017_3_OR_NEWER))
        public override void Update()
        {
            base.Update();

            bool focusFound = false;
            var camT = MainCamera.transform;
            var origin = camT.position;
            var fwd = camT.forward;
            var arFocus = (from obj in GameObject.FindGameObjectsWithTag("AR Focal Point")
                           where obj.activeInHierarchy
                           select obj.transform)
                .FirstOrDefault();
            if (arFocus != null)
            {
                var center = arFocus.Center();
                focusFound = SetFocus(origin, fwd, center, true);
                if (focusFound)
                {
                    focusPoint = center;

                    directions[0] = arFocus.forward;
                    directions[1] = -arFocus.forward;
                    directions[2] = arFocus.up;
                    directions[3] = -arFocus.up;
                    directions[4] = arFocus.right;
                    directions[5] = -arFocus.right;

                    focusDir = directions
                        .OrderByDescending(dir =>
                            Vector3.Dot(dir, -fwd))
                        .FirstOrDefault();
                }
            }

            if (!focusFound)
            {
                var att = ComponentExt.FindAny<AttentionDirector>();
                if (att != null)
                {
                    focusFound = SetFocus(origin, fwd, att.Target, false);
                }
            }

            if (!focusFound)
            {
                var ray = new Ray(origin, fwd);
                focusDir = -fwd;
                RaycastHit hitinfo;
                if (Physics.Raycast(ray, out hitinfo))
                {
                    focusPoint = hitinfo.point;
                }
                else
                {
                    focusPoint = origin + 10 * fwd;
                }
            }

            // NOTE: the SetFocusPointForFrame feature is currently defective - STM 2018-03-22
            // REF: https://forum.unity.com/threads/holographicsettings-setfocuspointforframe-seems-to-make-things-worse-hololens.514103/
            focusPoint = Vector3.Lerp(lastFocus, focusPoint, 0.1f);
            var rot = Quaternion.FromToRotation(lastNormal, focusDir);
            rot = Quaternion.Slerp(Quaternion.identity, rot, 0.1f);
            focusDir = rot * lastNormal;
            if (focusDir.sqrMagnitude > 0)
            {
                focusDir.Normalize();
                HolographicSettings.SetFocusPointForFrame(focusPoint, focusDir);
            }
            lastFocus = focusPoint;
            lastNormal = focusDir;
        }

        Vector3 focusPoint,
            focusDir,
            lastFocus,
            lastNormal;
        Vector3[] directions = new Vector3[6];

        bool SetFocus(Vector3 origin)
        {
            bool focusFound = MainCamera.IsInView(viewPost);
            if (focusFound)
            {
                focusPoint = viewPos;
            }

            focusDir = MainCamera.transform.position - origin;

            return focusFound;
        }
#endif
    }
}
#endif
