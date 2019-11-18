using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public partial class Grammar
    {
        /// <summary>
        /// A grammar for HyperText Markup Language.
        /// </summary>
        public static readonly Grammar HTML = new Grammar(
            "HTML",
            new Rule("newlines", new Regex("(?:\\r\\n|\\r|\\n)")),
            new Rule("startBlockComments", new Regex("(?:<| &lt;)!--")),
            new Rule("endBlockComments", new Regex("--(?:>|&gt;)")),
            new Rule("stringDelim", new Regex("(\"|')")),
            new Rule("numbers", new Regex("-?(?:(?:\\b\\d*)?\\.)?\\b\\d+\\b")),
            new Rule("keywords", new Regex("(?:<|&lt;)\\/?(html|base|head|link|meta|style|title|address|article|aside|footer|header|h1|h2|h3|h4|h5|h6|hgroup|nav|section|dd|div|dl|dt|figcaption|figure|hr|li|main|ol|p|pre|ul|a|abbr|b|bdi|bdo|br|cite|code|data|dfn|em|i|kbd|mark|q|rp|rt|rtc|ruby|s|samp|small|span|strong|sub|sup|time|u|var|wbr|area|audio|img|map|track|video|embed|object|param|source|canvas|noscript|script|del|ins|caption|col|colgroup|table|tbody|td|tfoot|th|thead|tr|button|datalist|fieldset|form|input|label|legend|meter|optgroup|option|output|progress|select|textarea|details|dialog|menu|menuitem|summary|content|element|shadow|template|acronym|applet|basefont|big|blink|center|command|content|dir|font|frame|frameset|isindex|keygen|listing|marquee|multicol|nextid|noembed|plaintext|spacer|strike|tt|xmp)\\b")),
            new Rule("members", new Regex("(\\w+)=")));
    }
}