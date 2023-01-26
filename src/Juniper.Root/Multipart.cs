namespace Juniper
{
    public partial class MediaType
    {
        public partial class Multipart : MediaType
        {
            private static List<Multipart> _allMultipart;
            private static List<Multipart> AllMulti => _allMultipart ??= new();
            public static IReadOnlyCollection<Multipart> AllMultipart => AllMulti;
            public static readonly Multipart AnyMultipart = new("*");

            internal Multipart(string value, params string[] extensions) : base("multipart", value, extensions)
            {
                if (SubType != "*")
                {
                    AllMulti.Add(this);
                }
            }

            internal Multipart(MediaType copy)
                : this(
                      copy.Type == "multipart"
                      ? copy.FullSubType
                      : throw new ArgumentException("Invalid media type", nameof(copy)),
                      copy.Extensions.ToArray())
            {

            }
        }
    }
}
