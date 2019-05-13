namespace Juniper.HTTP
{
    public struct BodyInfo
    {
        public readonly string MIMEType;
        public readonly long Length;

        public BodyInfo(string mime, long length)
        {
            MIMEType = mime;
            Length = length;
        }
    }
}
