using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using Line = System.Collections.Generic.List<Juniper.Primrose.Token>;

namespace Juniper.Primrose
{
    /// <summary>
    /// A grammar and an interpreter for a BASIC-like language.
    /// </summary>
    public partial class Basic : Grammar
    {
        public Basic()
            : base(
                "BASIC",
                // Text needs at least the newlines token, or else every line will attempt to render as a single line and the line count won't work.
                new Rule("newlines", new Regex(" (?:\\r\\n|\\r|\\n)")),
                // BASIC programs used to require the programmer type in her own line numbers. The start at the beginning of the line.
                new Rule("lineNumbers", new Regex("^\\d+\\s+")),
                // Comments were lines that started with the keyword "REM" (for REMARK) and ran to the end of the line. They did not have to be numbered, because they were not executable and were stripped out by the interpreter.
                new Rule("startLineComments", new Regex("^REM\\s")),
                // Both double-quoted and single-quoted strings were not always supported, but in this case, I'm just demonstrating how it would be done for both.
                new Rule("strings", new Regex("\"(?:\\\\\"|[^\"])*\"")),
                new Rule("strings", new Regex("'(?:\\\\'|[^'])*'")),
                // Numbers are an optional dash, followed by a optional digits, followed by optional period, followed by 1 or more required digits. This allows us to match both integers and decimal numbers, both positive and negative, with or without leading zeroes for decimal numbers between (-1, 1).
                new Rule("numbers", new Regex("-?(?:(?:\\b\\d*)?\\.)?\\b\\d+\\b")),
                // Keywords are really just a list of different words we want to match, surrounded by the "word boundary" selector "\b".
                new Rule("keywords", new Regex("\\b(?:RESTORE|REPEAT|RETURN|LOAD|LABEL|DATA|READ|THEN|ELSE|FOR|DIM|LET|IF|TO|STEP|NEXT|WHILE|WEND|UNTIL|GOTO|GOSUB|ON|TAB|AT|END|STOP|PRINT|INPUT|RND|INT|CLS|CLK|LEN)\\b")),
                // Sometimes things we want to treat as keywords have different meanings in different locations. We can specify rules for tokens more than once.
                new Rule("keywords", new Regex("^DEF FN")),
                // These are all treated as mathematical operations.
                new Rule("operators", new Regex("(?:\\+|;|,|-|\\*\\*|\\*|\\/|>=|<=|=|<>|<|>|OR|AND|NOT|MOD|\\(|\\)|\\[|\\])")),
                // Once everything else has been matched, the left over blocks of words are treated as variable and function names.
                new Rule("identifiers", new Regex("\\w+\\$?")))
        { }

        public override Line Tokenize(string text)
        {
            return base.Tokenize(text.ToUpperInvariant());
        }

        public IInterpreter Interpret(string source)
        {
            return new BasicInterpreter(source, Tokenize(source));
        }
    }
}