using System;
using System.Runtime.Serialization;

namespace Juniper.Speech.Azure.CognitiveServices;

[Serializable]
public sealed class Voice : ISerializable
{
    public string Name { get; }

    public string ShortName { get; }

    public string Gender { get; }

    public string Locale { get; }

    private Voice(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        Name = info.GetString(nameof(Name));
        ShortName = info.GetString(nameof(ShortName));
        Gender = info.GetString(nameof(Gender));
        Locale = info.GetString(nameof(Locale));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(ShortName), ShortName);
        info.AddValue(nameof(Gender), Gender);
        info.AddValue(nameof(Locale), Locale);
    }
}
