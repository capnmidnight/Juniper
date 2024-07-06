namespace Juniper.Data;

public interface IDBContextInformation
{
    bool UseIdentity { get; }
}

public class DbContextInformation : IDBContextInformation
{
    public bool UseIdentity { get; }

    public DbContextInformation(bool useIdentity)
    {
        UseIdentity = useIdentity;
    }
}
