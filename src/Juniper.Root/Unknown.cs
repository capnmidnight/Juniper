namespace Juniper
{
    public partial class MediaType
    {
        public class Unknown : MediaType
        {
            internal Unknown(string value)
                : base("unknown", value, value)
            { }
        }
    }
}
