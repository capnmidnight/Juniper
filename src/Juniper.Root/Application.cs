using System.Collections.Generic;

namespace Juniper
{
    public partial class MediaType
    {

        public partial class Application : MediaType
        {
            private static List<Application> _allApp;
            private static List<Application> AllApp => _allApp ??= new();
            public static IReadOnlyCollection<Application> AllApplication => AllApp;
            public static readonly Application AnyApplication = new("*");

            public Application(string value, params string[] extensions) : base("application", value, extensions)
            {
                if (SubType != "*")
                {
                    AllApp.Add(this);
                }
            }
        }
    }
}
