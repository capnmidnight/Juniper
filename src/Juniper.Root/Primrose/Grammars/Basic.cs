using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Line = System.Collections.Generic.List<Juniper.Primrose.Token>;

namespace Juniper.Primrose
{
    public partial class Grammar
    {
        /// <summary>
        /// A grammar and an interpreter for a BASIC-like language.
        /// </summary>
        public static readonly Grammar Basic = new Basic();
    }

    /// <summary>
    /// A grammar and an interpreter for a BASIC-like language.
    /// </summary>
    public class Basic : Grammar
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

        public override Line Tokenize(string source)
        {
            return base.Tokenize(source.ToUpperInvariant());
        }

        public IInterpreter Interpret(string source)
        {
            return new BasicInterpreter(source, Tokenize(source));
        }

        public class RuntimeException : Exception
        {
            public readonly string source;

            public readonly string evaluatedScript;

            public readonly Token[] line;

            public RuntimeException(string source, Line line, string evaluatedScript, Exception innerException)
                : base("Runtime Error", innerException)
            {
                this.source = source;
                this.evaluatedScript = evaluatedScript;
                this.line = (from t in line
                             select t.Clone())
                        .ToArray();
            }
        }

        public interface IInterpreter
        {
            event EventHandler<string> Output;
            event EventHandler<Action<string>> Input;
            event EventHandler<RuntimeException> Error;
            event EventHandler ClearScreen;
            event EventHandler<Action<Func<string, byte[]>>> LoadFile;
            event EventHandler Next;
            event EventHandler Done;

            void Interpret();
        }

        public class BasicInterpreter : IInterpreter
        {
            private static readonly string[] FOR_LOOP_DELIMS = new[] { "=", "TO", "STEP" };
            private static readonly Token EQUAL_SIGN = new Token("=", "operators");
            private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);
            private static readonly Dictionary<string, string> tokenMap = new Dictionary<string, string>
            {
                { "OR", "||" },
                { "AND", "&&" },
                { "NOT", "!" },
                { "MOD", "%" },
                { "<>", "!=" }
            };

            private static Token toNum(int i)
            {
                return new Token(i.ToString(), "numbers");
            }

            private static Token toStr(string str)
            {
                return new Token("\"" + str
                    .Replace("\n", "\\n")
                    .Replace("\"", "\\\"") + "\"", "strings");
            }

            private readonly Dictionary<string, Func<Line, bool>> commands;
            private readonly Dictionary<string, object> state;

            private readonly string source;

            private readonly Dictionary<int, Line> program = new Dictionary<int, Line>();
            private readonly List<int> lineNumbers = new List<int>();
            private readonly List<Line> lines = new List<Line>();
            private readonly Random random = new Random();
            private readonly Stack<Token> returnStack = new Stack<Token>();
            private readonly Dictionary<int, int> forLoopCounters = new Dictionary<int, int>();
            private readonly List<string> data = new List<string>();

            private int counter = 0;
            private bool isDone = false;
            private int dataCounter = 0;

            public BasicInterpreter(string source, Line tokens)
            {
                commands = new Dictionary<string, Func<Line, bool>>()
                {
                    { "DIM", declareVariable },
                    { "LET", translate },
                    { "PRINT", print },
                    { "GOTO", setProgramCounter },
                    { "IF", checkConditional },
                    { "INPUT", waitForInput },
                    { "END", pauseBeforeComplete },
                    { "STOP", pauseBeforeComplete },
                    { "REM", noop },
                    { "'", noop },
                    { "CLS", clearScreen },
                    { "ON", onStatement },
                    { "GOSUB", gotoSubroutine },
                    { "RETURN", stackReturn },
                    { "LOAD", loadCodeFile },
                    { "DATA", loadData },
                    { "READ", readData },
                    { "RESTORE", restoreData },
                    { "REPEAT", setRepeat },
                    { "UNTIL", untilLoop },
                    { "DEF FN", defineFunction },
                    { "WHILE", whileLoop },
                    { "WEND", stackReturn },
                    { "FOR", forLoop },
                    { "NEXT", stackReturn },
                    { "LABEL", labelLine }
                };

                state = new Dictionary<string, object>
                {
                    { "INT", new Func<float, int>(v => (int) v) },
                    { "CLK", new Func<float>(() => (float)(DateTime.Now - EPOCH).TotalHours) },
                    { "LEN", new Func<string, int>(str => str.Length) },
                    { "POW", new Func<float, float, float>((b, e) => (float)Math.Pow(b, e)) },
                    { "LINE", new Func<int>(() => lineNumbers[counter]) },
                    { "RND", new Func<float>(() => (float)random.NextDouble()) },
                    { "TAB", new Func<int, string>(num =>
                    {
                        var str = "";
                        for(var i = 0; i < num; ++i)
                        {
                            str += '\t';
                        }
                        return str;
                    }) }
                };

                this.source = source;

                // Remove comments and whitespace, and do a light translation of
                // operator tokens.
                var currentLine = new Line();
                while (tokens.Count > 0)
                {
                    var token = tokens[0];
                    tokens.RemoveAt(0);
                    if (token.type == "newlines")
                    {
                        currentLine = new Line();
                        lines.Add(currentLine);
                    }
                    else if (token.type != "regular" && token.type != "comments")
                    {
                        token.value = tokenMap.Get(token.value, token.value);
                        currentLine.Add(token);
                    }
                }

                // Parse the line numbers for the program
                int? lastLineNumber = null;
                for (var i = 0; i < lines.Count; ++i)
                {
                    var line = lines[i];
                    if (line.Count > 0)
                    {
                        var lineNumberToken = line[0];
                        line.RemoveAt(0);

                        // If the first token of a line is not actually a line number,
                        // we will auto-generate a line number and re-insert the token
                        // to be processed as normal code.
                        if (lineNumberToken.type != "lineNumbers")
                        {
                            line.Insert(0, lineNumberToken);
                            lineNumberToken = toNum((lastLineNumber ?? -1) + 1);
                        }

                        var lineNumber = int.Parse(
                            lineNumberToken.value,
                            NumberStyles.AllowLeadingWhite
                                | NumberStyles.AllowTrailingWhite);

                        // Line numbers should be ordered correctly, or we throw a syntax error.
                        if (lastLineNumber.HasValue && lineNumber <= lastLineNumber)
                        {
                            throw new Exception($"expected line number greater than {lastLineNumber}, but received {lineNumberToken.value}.");
                        }
                        // deleting empty lines
                        else if (line.Count > 0)
                        {
                            lineNumberToken.value = lineNumber.ToString();
                            lineNumbers.Add(lineNumber);
                            program[lineNumber] = line;
                        }

                        lastLineNumber = lineNumber;
                    }
                }
            }

            public event EventHandler<string> Output;
            private void OnOutput(string msg)
            {
                Output?.Invoke(this, msg);
            }

            public event EventHandler<Action<string>> Input;
            private void OnInput(Action<string> resume)
            {
                if (Input != null)
                {
                    resume(null);
                }
                else
                {
                    Input(this, resume);
                }
            }

            public event EventHandler<RuntimeException> Error;
            private void OnError(Line line, string script, Exception exp)
            {
                Error?.Invoke(this, new RuntimeException(source, line, script, exp));
            }

            public event EventHandler ClearScreen;
            private void OnClearScreen()
            {
                ClearScreen?.Invoke(this, EventArgs.Empty);
            }

            private bool clearScreen(Line line)
            {
                OnClearScreen();
                return true;
            }

            public event EventHandler<Action<Func<string, byte[]>>> LoadFile;
            private byte[] OnLoadFile(string fileName)
            {
                byte[] data = null;
                LoadFile?.Invoke(this, loader =>
                    data = loader(fileName));
                return data;
            }

            public event EventHandler Next;
            private void OnNext()
            {
                Next?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler Done;
            private void OnDone()
            {
                Done?.Invoke(this, EventArgs.Empty);
            }

            public void Interpret()
            {
                if (!isDone)
                {
                    var goNext = true;
                    while (goNext)
                    {
                        var line = getLine(counter);
                        goNext = process(line);
                        ++counter;
                    }
                }
            }

            private bool IsFunction(Token t)
            {
                if (t != null
                    && state.ContainsKey(t.value))
                {
                    return state[t.value] is Delegate;
                }
                return false;
            }

            private string evaluate(Line line)
            {
                var script = "";
                for (var i = 0; i < line.Count; ++i)
                {
                    var t = line[i];
                    var nest = 0;
                    if (t.type == "identifiers"
                        && !IsFunction(t)
                        && i < line.Count - 1
                        && line[i + 1].value == "(")
                    {
                        for (var j = i + 1; j < line.Count; ++j)
                        {
                            var t2 = line[j];
                            if (t2.value == "(")
                            {
                                if (nest == 0)
                                {
                                    t2.value = "[";
                                }
                                ++nest;
                            }
                            else if (t2.value == ")")
                            {
                                --nest;
                                if (nest == 0)
                                {
                                    t2.value = "]";
                                }
                            }
                            else if (t2.value == "," && nest == 1)
                            {
                                t2.value = "][";
                            }

                            if (nest == 0)
                            {
                                break;
                            }
                        }
                    }
                    script += t.value;
                }
                //with ( state ) { // jshint ignore:line
                try
                {
                    return null;
                    //return eval2(script); // jshint ignore:line
                }
                catch (Exception exp)
                {
                    OnError(line, script, exp);
                    return null;
                }
                //}
            }

            private bool translate(Line line)
            {
                evaluate(line);
                return true;
            }

            private bool loadData(Line line)
            {
                while (line.Count > 0)
                {
                    var t = line[0];
                    line.RemoveAt(0);
                    if (t.type != "operators")
                    {
                        data.Add(t.value);
                    }
                }
                return true;
            }

            private bool pauseBeforeComplete(Line line)
            {
                OnOutput("PROGRAM COMPLETE - PRESS RETURN TO FINISH.");
                OnInput(_ =>
                {
                    isDone = true;
                    OnDone();
                });
                return false;
            }

            private bool process(Line line)
            {
                if (line != null && line.Count > 0)
                {
                    var op = line[0];
                    line.RemoveAt(0);
                    if (op != null)
                    {
                        if (op.type == "lineNumbers")
                        {
                            return setProgramCounter(new Line { op });
                        }
                        else if (commands.ContainsKey(op.value))
                        {
                            return commands[op.value](line);
                        }
                        else if (state.ContainsKey(op.value)
                            || (line.Count > 0
                                && line[0].type == "operators"
                                && line[0].value == "="))
                        {
                            line.Insert(0, op);
                            return translate(line);
                        }
                        else
                        {
                            throw new Exception("Unknown command. >>> " + op.value);
                        }
                    }
                }

                return pauseBeforeComplete(null);
            }

            private bool setProgramCounter(Line line)
            {
                var lineNumber = int.Parse(evaluate(line));
                counter = -1;
                while (counter < lineNumbers.Count - 1
                    && lineNumbers[counter + 1] < lineNumber)
                {
                    ++counter;
                }

                return true;
            }

            private void error(string msg)
            {
                throw new Exception($"At line {lineNumbers[counter]}: {msg}");
            }

            private Line getLine(int i)
            {
                if (0 <= i && i < lineNumbers.Count)
                {
                    var lineNumber = lineNumbers[i];
                    if (program.ContainsKey(lineNumber))
                    {
                        var line = program[lineNumber];
                        if (line != null)
                        {
                            return line.Clone();
                        }
                    }
                }

                return null;
            }

            private bool labelLine(Line line)
            {
                line.Add(EQUAL_SIGN);
                line.Add(toNum(lineNumbers[counter]));
                return translate(line);
            }

            private bool noop(Line line)
            {
                return true;
            }

            private bool gotoSubroutine(Line line)
            {
                returnStack.Push(toNum(lineNumbers[counter + 1]));
                return setProgramCounter(line);
            }

            private bool setRepeat(Line line)
            {
                returnStack.Push(toNum(lineNumbers[counter]));
                return true;
            }

            private bool declareVariable(Line line)
            {
                var decl = new Line();
                var decls = new List<Line> { decl };
                var nest = 0;
                for (var i = 0; i < line.Count; ++i)
                {
                    var t = line[i];
                    if (t.value == "(")
                    {
                        ++nest;
                    }
                    else if (t.value == ")")
                    {
                        --nest;
                    }

                    if (nest == 0 && t.value == ",")
                    {
                        decl = new Line();
                        decls.Add(decl);
                    }
                    else
                    {
                        decl.Add(t);
                    }
                }

                for (var i = 0; i < decls.Count; ++i)
                {
                    decl = decls[i];
                    var idToken = decl[0];
                    decl.RemoveAt(0);

                    if (idToken.type != "identifiers")
                    {
                        error("Identifier expected: " + idToken.value);
                    }
                    else
                    {
                        var id = idToken.value;
                        object val = null;
                        if (decl.First().value == "("
                            && decl.Last().value == ")")
                        {
                            var sizes = new List<int>();
                            for (var j = 1; j < decl.Count - 1; ++j)
                            {
                                if (decl[j].type == "numbers")
                                {
                                    sizes.Add(int.Parse(decl[j].value));
                                }
                            }
                            if (sizes.Count == 0)
                            {
                                val = new List<object>();
                            }
                            else
                            {
                                val = new object[sizes[0]];
                                var queue = new Queue<object[]> { (object[])val };
                                for (var j = 1; j < sizes.Count; ++j)
                                {
                                    var size = sizes[j];
                                    for (int k = 0, l = queue.Count; k < l; ++k)
                                    {
                                        var arr = queue.Dequeue();
                                        for (var m = 0; m < arr.Length; ++m)
                                        {
                                            var subArr = new object[size];
                                            arr[m] = subArr;
                                            if (j < sizes.Count - 1)
                                            {
                                                queue.Enqueue(subArr);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        state[id] = val;
                    }
                }

                return true;
            }

            private bool print(Line line)
            {
                var endLine = "\n";
                var nest = 0;
                line = line.Select((t, i) =>
                {
                    t = t.Clone();
                    if (t.type == "operators")
                    {
                        if (t.value == ",")
                        {
                            if (nest == 0)
                            {
                                t.value = "+ \", \" + ";
                            }
                        }
                        else if (t.value == ";")
                        {
                            t.value = "+ \" \"";
                            if (i < line.Count - 1)
                            {
                                t.value += " + ";
                            }
                            else
                            {
                                endLine = "";
                            }
                        }
                        else if (t.value == "(")
                        {
                            ++nest;
                        }
                        else if (t.value == ")")
                        {
                            --nest;
                        }
                    }
                    return t;
                })
                    .ToList();

                var txt = evaluate(line);
                if (txt == null)
                {
                    txt = "";
                }

                OnOutput(txt + endLine);

                return true;
            }

            private bool checkConditional(Line line)
            {
                int thenIndex = -1,
                  elseIndex = -1;

                for (var i = 0; i < line.Count; ++i)
                {
                    if (line[i].type == "keywords" && line[i].value == "THEN")
                    {
                        thenIndex = i;
                    }
                    else if (line[i].type == "keywords" && line[i].value == "ELSE")
                    {
                        elseIndex = i;
                    }
                }

                if (thenIndex == -1)
                {
                    error("Expected THEN clause.");
                }
                else
                {
                    var condition = line.GetRange(0, thenIndex);
                    for (var i = 0; i < condition.Count; ++i)
                    {
                        var t = condition[i];
                        if (t.type == "operators" && t.value == "=")
                        {
                            t.value = "==";
                        }
                    }

                    Line thenClause = null,
                      elseClause = null;
                    if (elseIndex == -1)
                    {
                        thenClause = line.GetRange(thenIndex + 1, line.Count - thenIndex - 1);
                    }
                    else
                    {
                        thenClause = line.GetRange(thenIndex + 1, elseIndex - thenIndex - 1);
                        elseClause = line.GetRange(elseIndex + 1, line.Count - elseIndex - 1);
                    }
                    if (evaluate(condition) == "True")
                    {
                        return process(thenClause);
                    }
                    else if (elseClause != null)
                    {
                        return process(elseClause);
                    }
                }

                return true;
            }

            private bool waitForInput(Line line)
            {
                var toVar = line[line.Count - 1];
                line.RemoveAt(line.Count - 1);

                if (line.Count > 0)
                {
                    print(line);
                }

                OnInput(str =>
                {
                    str = str.ToUpperInvariant();
                    Token valueToken = null;
                    if (int.TryParse(str, out var num))
                    {
                        valueToken = toNum(num);
                    }
                    else
                    {
                        valueToken = toStr(str);
                    }

                    evaluate(new Line { toVar, EQUAL_SIGN, valueToken });
                    OnNext();
                });

                return false;
            }

            private bool onStatement(Line line)
            {
                var idxExpr = new Line();
                var targets = new Line();
                while (line.Count > 0
                    && (line[0].type != "keywords"
                        || line[0].value != "GOTO"))
                {
                    var idx = line[0];
                    line.RemoveAt(0);
                    idxExpr.Add(idx);
                }

                if (line.Count > 0)
                {
                    line.RemoveAt(0); // burn the goto;

                    for (var i = 0; i < line.Count; ++i)
                    {
                        var t = line[i];
                        if (t.type != "operators"
                            || t.value != ",")
                        {
                            targets.Add(t);
                        }
                    }

                    int idx = int.Parse(evaluate(idxExpr)) - 1;

                    if (0 <= idx && idx < targets.Count)
                    {
                        return setProgramCounter(new Line { targets[idx] });
                    }
                }
                return true;
            }

            public bool conditionalReturn(bool cond)
            {
                var ret = true;
                if (cond && returnStack.Count > 0)
                {
                    var val = returnStack.Pop();
                    ret = setProgramCounter(new Line { val });
                }
                return ret;
            }

            private bool untilLoop(Line line)
            {
                var cond = evaluate(line) == "True";
                return conditionalReturn(cond);
            }

            private int findNext(string str)
            {
                for (var i = counter + 1; i < lineNumbers.Count; ++i)
                {
                    var l = getLine(i);
                    if (l[0].value == str)
                    {
                        return i;
                    }
                }

                return lineNumbers.Count;
            }

            private bool whileLoop(Line line)
            {
                var cond = evaluate(line) == "True";
                if (!cond)
                {
                    counter = findNext("WEND");
                }
                else
                {
                    returnStack.Push(toNum(lineNumbers[counter]));
                }
                return true;
            }

            private bool forLoop(Line line)
            {
                var n = lineNumbers[counter];
                var varExpr = new Line();
                var fromExpr = new Line();
                var toExpr = new Line();
                var skipExpr = new Line();
                var arrs = new[] { varExpr, fromExpr, toExpr, skipExpr };
                var a = 0;
                var i = 0;
                for (i = 0; i < line.Count; ++i)
                {
                    var t = line[i];
                    if (t.value == FOR_LOOP_DELIMS[a])
                    {
                        if (a == 0)
                        {
                            varExpr.Add(t);
                        }
                        ++a;
                    }
                    else
                    {
                        arrs[a].Add(t);
                    }
                }

                var skip = 1;
                if (skipExpr.Count > 0)
                {
                    skip = int.Parse(evaluate(skipExpr));
                }

                if (!forLoopCounters.ContainsKey(n))
                {
                    forLoopCounters[n] = int.Parse(evaluate(fromExpr));
                }

                var end = int.Parse(evaluate(toExpr));
                var cond = forLoopCounters[n] <= end;
                if (!cond)
                {
                    forLoopCounters.Remove(n);
                    counter = findNext("NEXT");
                }
                else
                {
                    varExpr.Add(toNum(forLoopCounters[n]));
                    process(varExpr);
                    forLoopCounters[n] += skip;
                    returnStack.Push(toNum(lineNumbers[counter]));
                }
                return true;
            }

            private bool stackReturn(Line line)
            {
                return conditionalReturn(true);
            }

            private bool loadCodeFile(Line line)
            {
                var fileName = evaluate(line);
                var data = OnLoadFile(fileName);
                var source = Encoding.UTF8.GetString(data);

                error("Don't know what to do with loading files yet");

                return false;
            }

            private bool readData(Line line)
            {
                if (data.Count == 0)
                {
                    var dataLine = findNext("DATA");
                    process(getLine(dataLine));
                }

                var value = data[dataCounter];
                ++dataCounter;
                line.Add(EQUAL_SIGN);
                line.Add(toNum(int.Parse(value)));
                return translate(line);
            }

            private bool restoreData(Line line)
            {
                dataCounter = 0;
                return true;
            }

            private bool defineFunction(Line line)
            {
                var nameToken = line[0];
                line.RemoveAt(0);
                var name = nameToken.value;
                var signature = "";
                var body = "";
                var fillSig = true;
                for (var i = 0; i < line.Count; ++i)
                {
                    var t = line[i];
                    if (t.type == "operators" && t.value == "=")
                    {
                        fillSig = false;
                    }
                    else if (fillSig)
                    {
                        signature += t.value;
                    }
                    else
                    {
                        body += t.value;
                    }
                }

                name = "FN" + name;

                var script = "(function " + name + signature + "{ return " + body +
                  "; })";

                // state[name] = eval2(script); // jshint ignore:line

                return true;
            }
        }
    }
}