namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// Representation of special JPEG marker.
    /// </summary>
    /// <remarks>You can't create instance of this class manually.
    /// Concrete objects are instantiated by library and you can get them
    /// through <see cref="JpegDecompressStruct.Marker_list">Marker_list</see> property.
    /// </remarks>
    /// <seealso cref="JpegDecompressStruct.Marker_list"/>
    /// <seealso href="81c88818-a5d7-4550-9ce5-024a768f7b1e.htm" target="_self">Special markers</seealso>
    public class JpegMarkerStruct
    {
        internal JpegMarkerStruct(byte marker, int originalDataLength, int lengthLimit)
        {
            Marker = marker;
            OriginalLength = originalDataLength;
            Data = new byte[lengthLimit];
        }

        /// <summary>
        /// Gets the special marker.
        /// </summary>
        /// <value>The marker value.</value>
        public byte Marker { get; }

        /// <summary>
        /// Gets the full length of original data associated with the marker.
        /// </summary>
        /// <value>The length of original data associated with the marker.</value>
        /// <remarks>This length excludes the marker length word, whereas the stored representation 
        /// within the JPEG file includes it. (Hence the maximum data length is really only 65533.)
        /// </remarks>
        public int OriginalLength { get; }

        /// <summary>
        /// Gets the data associated with the marker.
        /// </summary>
        /// <value>The data associated with the marker.</value>
        /// <remarks>The length of this array doesn't exceed <c>length_limit</c> for the particular marker type.
        /// Note that this length excludes the marker length word, whereas the stored representation 
        /// within the JPEG file includes it. (Hence the maximum data length is really only 65533.)
        /// </remarks>
        public byte[] Data { get; }
    }
}
