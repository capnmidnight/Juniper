using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public partial class Grammar
    {
        /// <summary>
        /// A grammar for displaying the results of Unit Tests.
        /// </summary>
        public static readonly Grammar TestResults = new Grammar(
            "TestResults",
            new Rule("newlines", new Regex("(?:\\r\\n|\\r|\\n)")),
            new Rule("numbers", new Regex("(\\[)(o+)")),
            new Rule("numbers", new Regex("(\\d+ succeeded), 0 failed")),
            new Rule("numbers", new Regex("^    Successes:")),
            new Rule("functions", new Regex("(x+)\\]")),
            new Rule("functions", new Regex("[1-9]\\d* failed")),
            new Rule("functions", new Regex("^Failures:")),
            new Rule("comments", new Regex("(\\d + ms:)(.*) ")),
            new Rule("keywords", new Regex("(Test results for )(\\w +):")),
            new Rule("strings", new Regex("\\w +")));
    }
}