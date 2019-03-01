using System;

namespace Juniper.Input.Speech
{
    /// <summary>
    /// Provides the keyword that was recognized in a speech recognition event.
    /// </summary>
    public class KeywordRecognizedEventArgs : EventArgs
    {
        /// <summary>
        /// The keyword that was recognized.
        /// </summary>
        public readonly string Keyword;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Juniper.Input.Speech.KeywordRecognizedEventArgs"/> class.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        public KeywordRecognizedEventArgs(string keyword)
        {
            Keyword = keyword;
        }
    }
}
