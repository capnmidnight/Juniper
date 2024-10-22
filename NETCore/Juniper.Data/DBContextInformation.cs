namespace Juniper.Data;

public interface IDBContextInformation
{
    bool UseIdentity { get; }
}

public class DBContextInformation : IDBContextInformation
{
    public DBContextInformation(bool useIdentity)
    {
        UseIdentity = useIdentity;
    }

    public bool UseIdentity { get; }
}
