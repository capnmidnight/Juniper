using Juniper;

namespace UnityEngine
{
    public static class QuaternionExt
    {
        public static string Label(this Quaternion quat, int? sigFigs = null)
        {
            return quat.eulerAngles.Label(UnitOfMeasure.Degrees, sigFigs);
        }
    }
}
