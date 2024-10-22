using System;

namespace Juniper.Speech
{
    /// <summary>
    /// Provides the keyword that was recognized in a speech recognition event.
    /// </summary>
    public class KeywordRecognizedEventArgs : EventArgs
    {
        /// <summary>
        /// The keyword that was recognized.
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="Juniper.Input.Speech.KeywordRecognizedEventArgs"/> class.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        public KeywordRecognizedEventArgs(string keyword)
        {
            Keyword = keyword;
        }
    }
}