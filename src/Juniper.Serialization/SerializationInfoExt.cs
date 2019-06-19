using System.Runtime.Serialization;

namespace Juniper.Serialization
{
    public static class SerializationInfoExt
    {
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }
    }
}
