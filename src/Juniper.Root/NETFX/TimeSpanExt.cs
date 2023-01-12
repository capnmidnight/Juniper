using Juniper.Units;

namespace System
{
    public static class TimeSpanExt
    { 
        public static string ToSillyString(this TimeSpan time)
        {
            if(Math.Abs(time.TotalDays) > 1)
            {
                return time.TotalDays.Label(UnitOfMeasure.Days, 2);
            }
            else if (Math.Abs(time.TotalHours) > 1)
            {
                return time.TotalHours.Label(UnitOfMeasure.Hours, 2);
            }
            else if (Math.Abs(time.TotalMinutes) > 1)
            {
                return time.TotalMinutes.Label(UnitOfMeasure.Minutes, 2);
            }
            else if (Math.Abs(time.TotalSeconds) > 1)
            {
                return time.TotalSeconds.Label(UnitOfMeasure.Seconds, 2);
            }
            else if(Math.Abs(time.TotalMilliseconds) > 1)
            {
                return time.TotalMilliseconds.Label(UnitOfMeasure.Milliseconds, 2);
            }
            else
            {
                return "0";
            }
        }
    }
}