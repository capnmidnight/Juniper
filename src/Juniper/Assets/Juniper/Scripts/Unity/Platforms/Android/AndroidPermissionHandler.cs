#if UNITY_ANDROID
using UnityEngine.Android;

namespace Juniper.Unity
{
    public abstract class AndroidPermissionHandler : AbstractPermissionHandler
    {
        public void Start()
        {
#if UNITY_2018_1_OR_NEWER
            foreach (var permission in new string[]
            {
                Permission.Camera,
                Permission.ExternalStorageRead,
                Permission.FineLocation,
                Permission.Microphone
            })
            {
                print($"Checking permission {permission}");
                if (Permission.HasUserAuthorizedPermission(permission))
                {
                    print($"Already had permission {permission}");
                }
                else
                {
                    print($"Requesting permission {permission}");
                    Permission.RequestUserPermission(permission);

                    if (Permission.HasUserAuthorizedPermission(permission))
                    {
                        print($"The user granted permission {permission}");
                    }
                    else
                    {
                        print($"The user DID NOT grant permission {permission}");
                    }
                }
            }
#endif
        }
    }
}
#endif