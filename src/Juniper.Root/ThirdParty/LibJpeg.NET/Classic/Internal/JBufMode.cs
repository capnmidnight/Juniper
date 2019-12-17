namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Operating modes for buffer controllers
    /// </summary>
    internal enum JBufMode
    {
        PassThrough,         /* Plain stripwise operation */

        /* Remaining modes require a full-image buffer to have been created */

        SaveSource,       /* Run source subobject only, save output */
        CrankDest,        /* Run dest subobject only, using saved data */
        SaveAndPass      /* Run both subobjects, save output */
    }
}
