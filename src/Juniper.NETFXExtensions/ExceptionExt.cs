using System.Text;

namespace System
{
    /// <summary>
    /// Extension methods for <c>System.Exception</c>.
    /// </summary>
    public static class ExceptionExt
    {
        /// <summary>
        /// Serialze the exception with its stack trace, for logging purposes.
        /// </summary>
        /// <param name="e">The exception to serialize.</param>
        /// <param name="featureName">An arbitrary label to assist in tracking down bugs.</param>
        /// <param name="userName">The name of the person running the code that errored.</param>
        /// <returns>A string of XML code that describes the error.</returns>
        public static string ToXMLLogString(this Exception e, string featureName, string userName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"<infoLog user=""{0}"" log=""{1}"" timestamp=""{2}"">
", userName ?? "none", featureName, DateTime.Now);
            var exp = e;
            for (var i = 0; exp != null; ++i)
            {
                sb.AppendFormat(
    @"<exception depth=""{0}"" type=""{1}"">
    <source>{2}</source>
    <message>{3}</message>
    <stackTrace>{4}</stackTrace>
</exception>
", i, exp.GetType().FullName, exp.Source, exp.Message, exp.StackTrace, featureName);
                exp = exp.InnerException;
            }
            sb.AppendLine("</infoLog>");
            return sb.ToString();
        }

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
