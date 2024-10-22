namespace BitMiracle.LibJpeg
{
    internal interface IRawImage
    {
        int Width
        { get; }

        int Height
        { get; }

        Colorspace Colorspace
        { get; }

        int ComponentsPerPixel
        { get; }

        void BeginRead();
        byte[] GetPixelRow();
        void EndRead();
    }
}
