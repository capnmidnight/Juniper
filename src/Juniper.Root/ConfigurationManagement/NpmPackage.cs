using System;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class NpmPackage : ISerializable
    {
        public string Name { get; }

        public string DisplayName { get; }

        public string Version { get; }

        private NpmPackage(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("name");
            foreach (var field in info)
            {
                if (field.Name.Equals(nameof(DisplayName), StringComparison.OrdinalIgnoreCase))
                {
                    DisplayName = info.GetString(field.Name);
                }
                else if (field.Name.Equals(nameof(Version), StringComparison.OrdinalIgnoreCase))
                {
                    Version = info.GetString(field.Name);
                }
            }

            DisplayName ??= Name;
            Version ??= "unityPackage";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}


/*
 *
C:\Users\smcbeth.DLS-INC\AppData\Local\Unity\cache\packages\packages.unity.com\com.unity.xr.windowsmr@3.0.0

C:\Users\smcbeth.DLS-INC\AppData\Local\Unity\cache\npm\packages.unity.com\com.unity.xr.windowsmr
C:\Users\smcbeth.DLS-INC\AppData\Local\Unity\cache\npm\packages.unity.com\com.unity.xr.windowsmr\3.0.0
C:\Users\smcbeth.DLS-INC\AppData\Local\Unity\cache\npm\packages.unity.com\com.unity.xr.windowsmr\3.0.0\package

C:\Users\smcbeth.DLS-INC\AppData\Roaming\Unity\Asset Store-5.x

 */
