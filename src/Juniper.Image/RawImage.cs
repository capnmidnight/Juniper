namespace Juniper.Image
{
    /// <summary>
    /// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
    /// </summary>
    public struct RawImage
    {
        public byte[] data;
        public int width, height;
    }
}
