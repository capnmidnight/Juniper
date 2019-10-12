namespace Juniper
{
    public partial class MediaType
    {
        public sealed class Unknown : MediaType
        {
            private static string MakeExtension(string value)
            {
                var slashIndex = value.IndexOf('/');
                if(0 <= slashIndex)
                {
                    value = value.Substring(slashIndex + 1);
                }

                return value;
            }

            public Unknown(string value)
                : base(value, new string[] { MakeExtension(value) })
            { }
        }
    }
}
