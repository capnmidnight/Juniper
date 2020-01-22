using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public partial class Grammar
    {
        /// <summary>
        /// A grammar for the JavaScript programming language.
        /// </summary>
        public static readonly Grammar JavaScript = new Grammar(
            "JavaScript",
            new Rule("newlines", new Regex("(?:\\r\\n|\\r|\\n)")),
            new Rule("startBlockComments", new Regex("\\/\\*")),
            new Rule("endBlockComments", new Regex("\\*\\/")),
            new Rule("regexes", new Regex("(?:^|,|;|\\(|\\[|\\{)(?:\\s*)(\\/(?:\\\\\\/|[^\\n\\/])+\\/)")),
            new Rule("stringDelim", new Regex("(\"|')")),
            new Rule("startLineComments", new Regex("\\/\\/.*$", RegexOptions.Multiline)),
            new Rule("numbers", new Regex("-?(?:(?:\\b\\d*)?\\.)?\\b\\d+\\b")),
            new Rule("keywords", new Regex("\\b(?:break|case|catch|class|const|continue|debugger|default|delete|do|else|export|finally|for|function|if|import|in|instanceof|let|new|return|super|switch|this|throw|try|typeof|var|void|while|with)\\b")),
            new Rule("functions", new Regex("(\\w+)(?:\\s*\\()")),
            new Rule("members", new Regex("(\\w+)\\.")),
            new Rule("members", new Regex("((\\w+\\.)+)(\\w+)")));
    }
}