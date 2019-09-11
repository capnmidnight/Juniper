using System.Text;

namespace System
{
    /// <summary>
    /// Extension methods for <c>System.Exception</c>.
    /// </summary>
    public static class ExceptionExt
    {
        /// <summary>
        /// Serialize the exception in a format suitable for printing in the console.
        /// </summary>
        /// <param name="e">The exception to serialize.</param>
        /// <param name="featureName">An arbitrary label to assist in tracking down bugs.</param>
        /// <returns>A single-line string describing the high-level details of the error.</returns>
        public static string ToShortString(this Exception e, string featureName)
        {
            return $"ERRR[{featureName} ({e.GetType().FullName})]: {e.Message}";
        }
    }
}