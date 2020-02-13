#if UNITY_ANDROID && ANDROID_API_23_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Android;

namespace Juniper.Permissions
{
    public abstract class AndroidPermissionHandler : AbstractPermissionHandler
    {
        private static readonly Dictionary<string, bool> granted = new Dictionary<string, bool>();
        private static string[] keys;

        public void Start()
        {
            if (keys != null)
            {
                foreach (var permission in keys)
                {
                    granted[permission] = Permission.HasUserAuthorizedPermission(permission);
                    if (!granted[permission])
                    {
                        Permission.RequestUserPermission(permission);
                    }
                }
            }
        }

        public void Update()
        {
            var allGranted = true;
            if (keys != null)
            {
                foreach (var permission in keys)
                {
                    if (!granted[permission])
                    {
                        allGranted = false;
                        granted[permission] = Permission.HasUserAuthorizedPermission(permission);
                        if (granted[permission])
                        {
                            UnityEngine.Debug.Log($"The user granted permission {permission}");
                        }
                        else
                        {
                            UnityEngine.Debug.Log($"The user DID NOT grant permission {permission}");
                        }
                    }
                }
            }

            if (allGranted)
            {
                enabled = false;
            }
        }

        public static void Add(string permission)
        {
            granted[permission] = false;
            keys = granted.Keys.ToArray();
        }

        public static bool IsGranted(string permission)
        {
            return granted.Get(permission);
        }
    }
}
#endif
