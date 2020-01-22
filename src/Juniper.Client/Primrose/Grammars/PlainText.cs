using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public partial class Grammar
    {
        /// <summary>
        /// A grammar that makes displaying plain text work with the text editor designed for syntax highlighting.
        /// </summary>
        public static readonly Grammar PlainText = new Grammar(
            "PlainText",
            new Rule("newlines", new Regex("(?:\\r\\n|\\r|\\n)")));
    }
}