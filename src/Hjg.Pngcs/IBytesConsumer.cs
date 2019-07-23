namespace Ar.Com.Hjg.Pngcs
{
    internal interface IBytesConsumer
    {
        int consume(byte[] buf, int offset, int tofeed);
    }
}