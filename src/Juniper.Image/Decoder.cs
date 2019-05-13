using System;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.Image
{
    public static class Decoder
    {
        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">     Png bytes.</param>
        public static async Task<RawImage> DecodePNG(Stream imageStream)
        {
            return await Task.Run(() =>
            {
                var png = new Hjg.Pngcs.PngReader(imageStream);
                png.SetUnpackedMode(true);
                var rows = png.ReadRowsByte();
                var scans = rows.ScanlinesB;
                var data = new byte[rows.Nrows * rows.elementsPerRow];
                for (var i = 0; i < rows.Nrows; ++i)
                {
                    var row = rows.ScanlinesB[rows.Nrows - i - 1];
                    Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
                }

                return new RawImage
                {
                    width = rows.elementsPerRow / rows.channels,
                    height = rows.Nrows,
                    data = data
                };
            });
        }
    }
}
