using System;
using System.Runtime.Serialization;

namespace Juniper.Azure.CognitiveServices
{
    [Serializable]
    public sealed class Voice : ISerializable
    {
        public readonly string Name;
        public readonly string ShortName;
        public readonly string Gender;
        public readonly string Locale;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Voice(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString(nameof(Name));
            ShortName = info.GetString(nameof(ShortName));
            Gender = info.GetString(nameof(Gender));
            Locale = info.GetString(nameof(Locale));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(ShortName), ShortName);
            info.AddValue(nameof(Gender), Gender);
            info.AddValue(nameof(Locale), Locale);
        }
    }
}
