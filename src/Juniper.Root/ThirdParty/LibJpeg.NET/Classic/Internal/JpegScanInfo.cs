namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// The script for encoding a multiple-scan file is an array of these:
    /// </summary>
    internal class JpegScanInfo
    {
        public int comps_in_scan { get; set; }      /* number of components encoded in this scan */
        public int[] component_index = new int[JpegConstants.MAX_COMPS_IN_SCAN]; /* their SOF/comp_info[] indexes */
        public int Ss { get; set; }
        public int Se { get; set; }         /* progressive JPEG spectral selection parms */
        public int Ah { get; set; }
        public int Al { get; set; }         /* progressive JPEG successive approx. parms */
    }
}
