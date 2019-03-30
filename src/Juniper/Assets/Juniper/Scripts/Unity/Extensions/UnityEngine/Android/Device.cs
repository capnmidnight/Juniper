namespace UnityEngine.Android
{
    /// <summary>
    /// A grab-bag of features specific to Android devices.
    /// </summary>
    public static class Device
    {
        /// <summary>
        /// Get the current Android SDK API version.
        /// </summary>
        /// <value>The <c>SDK_INT</c> value out of the OS build version number.</value>
        /// <example>
        /// if(UnityEngine.Android.Device.APILevel &gt;= 26) { // do Android 8.0 Oreo-specific stuff. }
        /// </example>
        public static int APILevel
        {
            get
            {
#if UNITY_ANDROID
                if (Application.isEditor)
                {
#endif
                    return 0;
#if UNITY_ANDROID
                }
                else
                {
                    var androidVersion = new AndroidJavaClass("android.os.Build$VERSION");
                    if (androidVersion == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return androidVersion.GetStatic<int>("SDK_INT");
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        public static void ShowToastMessage(string message)
        {
#if UNITY_ANDROID
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                var toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
#endif
        }
    }
}