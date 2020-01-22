using System;
using System.Collections;

namespace Juniper
{
    public static class Wait
    {
        public static IEnumerator Until(DateTime end)
        {
            while(DateTime.Now < end)
            {
                yield return null;
            }
        }

        public static IEnumerator Minutes(float minutes)
        {
            return Until(DateTime.Now.AddMinutes(minutes));
        }

        public static IEnumerator Seconds(float seconds)
        {
            return Until(DateTime.Now.AddSeconds(seconds));
        }

        public static IEnumerator Milliseconds(float milliseconds)
        {
            return Until(DateTime.Now.AddMilliseconds(milliseconds));
        }
    }
}