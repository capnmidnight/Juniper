using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public class Rule
    {
        public readonly string name;

        public readonly Regex test;

        public Rule(string name, Regex test)
        {
            this.name = name;
            this.test = test;
        }

        public void CarveOutMatchedToken(List<Token> tokens, int j)
        {
            var token = tokens[j];
            if (token.type == "regular")
            {
                var res = test.Match(token.value);
                if (res.Success)
                {
                    // Only use the last group that matches the regex, to allow for more
                    // complex regexes that can match in special contexts, but not make
                    // the context part of the token.
                    var midx = res.Captures[res.Captures.Count - 1].Value;
                    var start = res.Value.IndexOf(midx);
                    var end = start + midx.Length;
                    if (start == 0)
                    {
                        // the rule matches the start of the token
                        token.type = name;
                        if (end < token.value.Length)
                        {
                            // but not the end
                            var next = token.SplitAt(end);
                            next.type = "regular";
                            tokens.Splice(j + 1, 0, next);
                        }
                    }
                    else
                    {
                        // the rule matches from the middle of the token
                        var mid = token.SplitAt(start);
                        if (midx.Length < mid.value.Length)
                        {
                            // but not the end
                            var right = mid.SplitAt(midx.Length);
                            tokens.Splice(j + 1, 0, right);
                        }

                        mid.type = name;
                        tokens.Splice(j + 1, 0, mid);
                    }
                }
            }
        }
    }
}