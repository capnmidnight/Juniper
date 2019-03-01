namespace System.Text
{
    /// <summary>
    /// Extension methods for <c>System.Text.StringBuilder</c>.
    /// </summary>
    public static class StringBuilderExt
    {
        /// <summary>
        /// Find a character in a StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder to search.</param>
        /// <param name="c">The character to search for.</param>
        /// <returns>
        /// The index of the first occurence of the character <paramref name="c"/>, otherwise returns -1.
        /// </returns>
        /// <remarks>
        /// If <paramref name="c"/> occurs multiple times in the string, this function always returns
        /// the index of the first occurence.
        /// </remarks>
        public static int IndexOf(this StringBuilder sb, char c)
        {
            for (var i = 0; i < sb.Length; ++i)
            {
                if (sb[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
