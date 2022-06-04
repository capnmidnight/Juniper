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
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (featureName is null)
            {
                featureName = string.Empty;
            }
            else if (featureName.Length > 0)
            {
                featureName += " ";
            }

            return $"ERRR[{featureName}({e.GetType().FullName})]: {e.Message}";
        }

        public static string Unroll(this Exception e)
        {
            var sb = new StringBuilder();
            var q = new Queue<Exception>
            {
                e
            };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                if (here is not null)
                {
                    sb.AppendLine(here.ToShortString(string.Empty));
                    sb.AppendLine(here.StackTrace);

                    if (here is AggregateException agg)
                    {
                        q.AddRange(agg.InnerExceptions);
                    }
                    else
                    {
                        q.Enqueue(here.InnerException);
                    }
                }
            }

            return sb.ToString();
        }
    }
}