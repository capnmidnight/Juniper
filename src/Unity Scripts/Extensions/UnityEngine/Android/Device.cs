namespace UnityEngine.Android
{
    /// <summary>
    /// A grab-bag of features specific to Android devices.
    /// </summary>
    public static class Device
    {
        private static int cachedAPILevel;
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
                else if (cachedAPILevel == 0)
                {
                    using (var androidVersion = new AndroidJavaClass("android.os.Build$VERSION"))
                    {
                        cachedAPILevel = androidVersion.GetStatic<int>("SDK_INT");
                    }
                }

                return cachedAPILevel;
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
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (unityActivity != null)
                {
                    using (unityActivity)
                    using (var toastClass = new AndroidJavaClass("android.widget.Toast"))
                    {
                        var args = new AndroidJavaRunnable(() =>
                        {
                            var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                            if (toastObject != null)
                            {
                                using (toastObject)
                                {
                                    toastObject.Call("show");
                                }
                            }
                        });

                        unityActivity.Call("runOnUiThread", args);
                    }
                }
            }
#endif
        }
    }
}
