namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class XConference : MediaType
        {
            public static readonly XConference X_Cooltalk = new("x-cooltalk", new string[] { "ice" });

            public static new readonly XConference[] Values = {
                X_Cooltalk
            };
        }
    }
}
