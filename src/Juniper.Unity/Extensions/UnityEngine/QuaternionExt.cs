using Juniper.Units;

namespace UnityEngine
{
    public static class QuaternionExt
    {
        public static string Label(this Quaternion quat, int sigFigs)
        {
            return quat.eulerAngles.Label(UnitOfMeasure.Degrees, sigFigs);
        }

        public static string Label(this Quaternion quat)
        {
            return quat.eulerAngles.Label(UnitOfMeasure.Degrees);
        }
    }
}
