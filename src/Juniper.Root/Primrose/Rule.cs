using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Juniper.Primrose
{
    public class Rule
    {
        public string Name { get; }

        public Regex Test { get; }

        public Rule(string name, Regex test)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A name for the rule is required.", nameof(name));
            }

            Name = name;
            Test = test ?? throw new ArgumentNullException(nameof(test));
        }

        public void CarveOutMatchedToken(List<Token> tokens, int j)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (j < 0 || tokens.Count <= j)
            {
                throw new ArgumentOutOfRangeException(nameof(j));
            }

            var token = tokens[j];
            if (token.Type == "regular")
            {
                var res = Test.Match(token.Value);
                if (res.Success)
                {
                    // Only use the last group that matches the regex, to allow for more
                    // complex regexes that can match in special contexts, but not make
                    // the context part of the token.
                    var midx = res.Captures[res.Captures.Count - 1].Value;
                    var start = res.Value.IndexOf(midx, StringComparison.InvariantCulture);
                    var end = start + midx.Length;
                    if (start == 0)
                    {
                        // the rule matches the start of the token
                        token.Type = Name;
                        if (end < token.Value.Length)
                        {
                            // but not the end
                            var next = token.SplitAt(end);
                            next.Type = "regular";
                            _ = tokens.Splice(j + 1, 0, next);
                        }
                    }
                    else
                    {
                        // the rule matches from the middle of the token
                        var mid = token.SplitAt(start);
                        if (midx.Length < mid.Value.Length)
                        {
                            // but not the end
                            var right = mid.SplitAt(midx.Length);
                            _ = tokens.Splice(j + 1, 0, right);
                        }

                        mid.Type = Name;
                        _ = tokens.Splice(j + 1, 0, mid);
                    }
                }
            }
        }
    }
}