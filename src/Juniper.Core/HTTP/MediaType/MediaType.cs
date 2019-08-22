namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public readonly string Value;
        public readonly string[] Extensions;
        public readonly string PrimaryExtension;

        public MediaType(string value, string[] extensions = null)
        {
            Value = value;
            Extensions = extensions;
            if(extensions?.Length >= 1)
            {
                PrimaryExtension = extensions[0];
            }
        }

        public static implicit operator string(MediaType mediaType)
        {
            return mediaType.Value;
        }
    }
}
