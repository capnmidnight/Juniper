namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AlternateKeyAttribute : Attribute
{
    public string[] PropertyNames { get; }
    public AlternateKeyAttribute(string firstPropertyName, params string[] additionalPropertyNames)
    {
        PropertyNames = additionalPropertyNames
            .Prepend(firstPropertyName)
            .ToArray();
    }
}
