using System;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public class PackageDefineSymbol : ISerializable
    {
        public string Name { get; }
        public string CompilerDefine { get; }

        public PackageDefineSymbol(string name, string compilerDefine)
        {
            Name = name;
            CompilerDefine = compilerDefine;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(CompilerDefine), CompilerDefine);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected PackageDefineSymbol(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Name = info.GetString(nameof(Name));
            CompilerDefine = info.GetString(nameof(CompilerDefine));
        }
    }
}
