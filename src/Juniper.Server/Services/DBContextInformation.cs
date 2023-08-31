// Ignore Spelling: env Configurator

namespace Juniper.Services
{
    internal class DBContextInformation : IDBContextInformation
    {
        public DBContextInformation(bool useIdentity)
        {
            UseIdentity = useIdentity;
        }

        public bool UseIdentity { get; }
    }
}
