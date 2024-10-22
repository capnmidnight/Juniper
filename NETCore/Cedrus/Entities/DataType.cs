namespace Juniper.Cedrus.Entities;

/// <summary>
/// Make sure this stays in sync with the DataType enum in 
/// <see cref="../../JS/cedrus/src/models/DataType.ts">TypeScript DataType enum</see>
/// </summary>
public enum DataType
{
    Unknown,
    Boolean,
    Currency,
    Date,
    Decimal,
    Enumeration,
    File,
    Integer,
    Link,
    LongText,
    String
}
