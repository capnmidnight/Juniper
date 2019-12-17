using System;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Parameters of compression.
    /// </summary>
    /// <remarks>Being used in <see cref="M:BitMiracle.LibJpeg.JpegImage.WriteJpeg(System.IO.Stream,BitMiracle.LibJpeg.CompressionParameters)"/></remarks>
    public class CompressionParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionParameters"/> class.
        /// </summary>
        public CompressionParameters()
        {
        }

        internal CompressionParameters(CompressionParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Quality = parameters.Quality;
            SmoothingFactor = parameters.SmoothingFactor;
            SimpleProgressive = parameters.SimpleProgressive;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is CompressionParameters parameters))
            {
                return false;
            }

            return Quality == parameters.Quality &&
                    SmoothingFactor == parameters.SmoothingFactor &&
                    SimpleProgressive == parameters.SimpleProgressive;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms 
        /// and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets or sets the quality of JPEG image.
        /// </summary>
        /// <remarks>Default value: 75<br/>
        /// The quality value is expressed on the 0..100 scale.
        /// </remarks>
        /// <value>The quality of JPEG image.</value>
        public int Quality { get; set; } = 75;

        /// <summary>
        /// Gets or sets the coefficient of image smoothing.
        /// </summary>
        /// <remarks>Default value: 0<br/>
        /// If non-zero, the input image is smoothed; the value should be 1 for
        /// minimal smoothing to 100 for maximum smoothing.
        /// </remarks>
        /// <value>The coefficient of image smoothing.</value>
        public int SmoothingFactor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to write a progressive-JPEG file.
        /// </summary>
        /// <value>
        /// <c>true</c> for writing a progressive-JPEG file; <c>false</c> 
        /// for non-progressive JPEG files.
        /// </value>
        public bool SimpleProgressive { get; set; }
    }
}