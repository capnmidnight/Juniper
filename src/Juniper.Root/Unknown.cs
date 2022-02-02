namespace Juniper
{
    public partial class MediaType
    {
        public class Unknown : MediaType
        {
            private static string MakeExtension(string value)
            {
                var slashIndex = value.IndexOf('/');
                if (0 <= slashIndex)
                {
                    value = value[(slashIndex + 1)..];
                }

                return value;
            }

            internal Unknown(string value)
                : base(value, new string[] { MakeExtension(value) })
            { }
        }
    }
}
