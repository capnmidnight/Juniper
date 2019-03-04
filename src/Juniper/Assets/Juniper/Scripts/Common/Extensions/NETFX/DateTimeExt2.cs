#if UNITY_ANDROID && !UNITY_EDITOR
#define NEEDS_OFFSET
#endif

namespace System
{
    /// <summary>
    /// Extension methods for <c>System.DateTime</c>.
    /// </summary>
    public static class DateTimeExt2
    {
#if NEEDS_OFFSET
        public static readonly int RAW_TIMEZONE_OFFSET = new UnityEngine.AndroidJavaObject("java.util.GregorianCalendar")
                    .Call<UnityEngine.AndroidJavaObject>("getTimeZone")
                    .Call<int>("getRawOffset");
#endif

        /// <summary>
        /// The latest versions of Unity running .NET rutime 4.6 on Android have a defect that local
        /// times get reported as UTC and no timezone information is available.
        /// </summary>
        private static int MillisecondsOffset =>
#if NEEDS_OFFSET
            RAW_TIMEZONE_OFFSET;
#else
            0;

#endif

        /// <summary>
        /// Cache the TimeSpan so we don't have to recalculate it every time.
        /// </summary>
        private static TimeSpan? cachedOffset;

        /// <summary>
        /// The latest versions of Unity running .NET rutime 4.6 on Android have a defect that local
        /// times get reported as UTC and no timezone information is available.
        /// </summary>
        private static TimeSpan Offset
        {
            get
            {
                if (cachedOffset == null)
                {
                    cachedOffset = new TimeSpan(0, 0, 0, 0, MillisecondsOffset);
                }
                return cachedOffset.Value;
            }
        }

        /// <summary>
        /// The latest versions of Unity running .NET rutime 4.6 on Android have a defect that local
        /// times get reported as UTC and no timezone information is available. /// This is mostly
        /// only necessary for displaying the time. Most other things work without knowing the local
        /// time zone. /// WARNING: do not call this function during app startup, such as
        /// initializing fields in classes.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime FixTime(this DateTime time)
        {
            if (time.Kind == DateTimeKind.Local)
            {
                return time + Offset;
            }
            else if (time.Kind == DateTimeKind.Utc)
            {
                return time - Offset;
            }
            else
            {
                return time;
            }
        }
    }
}
