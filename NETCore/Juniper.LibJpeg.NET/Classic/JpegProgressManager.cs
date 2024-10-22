using System;

namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// The progress monitor object.
    /// </summary>
    /// <seealso href="febdc6af-ca72-4f3b-8cfe-3473ce6a7c7f.htm" target="_self">Progress monitoring</seealso>
    public class JpegProgressManager
    {
        /// <summary>
        /// Occurs when progress is changed.
        /// </summary>
        /// <seealso href="febdc6af-ca72-4f3b-8cfe-3473ce6a7c7f.htm" target="_self">Progress monitoring</seealso>
        public event EventHandler OnProgress;

        /// <summary>
        /// Gets or sets the number of work units completed in this pass.
        /// </summary>
        /// <value>The number of work units completed in this pass.</value>
        /// <seealso href="febdc6af-ca72-4f3b-8cfe-3473ce6a7c7f.htm" target="_self">Progress monitoring</seealso>
        public int PassCounter { get; set; }

        /// <summary>
        /// Gets or sets the total number of work units in this pass.
        /// </summary>
        /// <value>The total number of work units in this pass.</value>
        /// <seealso href="febdc6af-ca72-4f3b-8cfe-3473ce6a7c7f.htm" target="_self">Progress monitoring</seealso>
        public int PassLimit { get; set; }

        /// <summary>
        /// Gets or sets the number of passes completed so far.
        /// </summary>
        /// <value>The number of passes completed so far.</value>
        /// <seealso href="febdc6af-ca72-4f3b-8cfe-3473ce6a7c7f.htm" target="_self">Progress monitoring</seealso>
        public int CompletedPasses { get; set; }

        /// <summary>
        /// Gets or sets the total number of passes expected.
        /// </summary>
        /// <value>The total number of passes expected.</value>
        /// <seealso href="febdc6af-ca72-4f3b-8cfe-3473ce6a7c7f.htm" target="_self">Progress monitoring</seealso>
        public int TotalPasses { get; set; }

        /// <summary>
        /// Indicates that progress was changed.
        /// </summary>
        /// <remarks>Call this method if you change some progress parameters manually.
        /// This method ensures happening of the <see cref="JpegProgressManager.OnProgress">OnProgress</see> event.</remarks>
        public void Updated()
        {
            OnProgress?.Invoke(this, EventArgs.Empty);
        }
    }
}