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

        private static Token ToNum(int i)
        {
            return new Token(i.ToString(), "numbers");
        }

        private static Token ToStr(string str)
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
                    { "DIM", DeclareVariable },
                    { "LET", Translate },
                    { "PRINT", Print },
                    { "GOTO", SetProgramCounter },
                    { "IF", CheckConditional },
                    { "INPUT", WaitForInput },
                    { "END", PauseBeforeComplete },
                    { "STOP", PauseBeforeComplete },
                    { "REM", NoOp },
                    { "'", NoOp },
                    { "CLS", OnClearScreen },
                    { "ON", OnStatement },
                    { "GOSUB", GoToSubroutine },
                    { "RETURN", StackReturn },
                    { "LOAD", LoadCodeFile },
                    { "DATA", LoadData },
                    { "READ", ReadData },
                    { "RESTORE", RestoreData },
                    { "REPEAT", SetRepeat },
                    { "UNTIL", UntilLoop },
                    { "DEF FN", DefineFunction },
                    { "WHILE", WhileLoop },
                    { "WEND", StackReturn },
                    { "FOR", ForLoop },
                    { "NEXT", StackReturn },
                    { "LABEL", LabelLine }
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
                        lineNumberToken = ToNum((lastLineNumber ?? -1) + 1);
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
        public event EventHandler<Action<string>> Input;
        public event EventHandler<RuntimeException> Error;
        public event EventHandler ClearScreen;
        public event EventHandler<Action<Func<string, byte[]>>> LoadFile;
        public event EventHandler Next;
        public event EventHandler Done;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnOutput(string msg)
        {
            Output?.Invoke(this, msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnError(Line line, string script, Exception exp)
        {
            Error?.Invoke(this, new RuntimeException(source, line, script, exp));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnClearScreen()
        {
            ClearScreen?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool OnClearScreen(Line line)
        {
            OnClearScreen();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] OnLoadFile(string fileName)
        {
            byte[] data = null;
            LoadFile?.Invoke(this, loader =>
                data = loader(fileName));
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNext()
        {
            Next?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                    var line = GetLine(counter);
                    goNext = Process(line);
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

        private string Evaluate(Line line)
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

        private bool Translate(Line line)
        {
            _ = Evaluate(line);
            return true;
        }

        private bool LoadData(Line line)
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

        private bool PauseBeforeComplete(Line line)
        {
            OnOutput("PROGRAM COMPLETE - PRESS RETURN TO FINISH.");
            OnInput(_ =>
            {
                isDone = true;
                OnDone();
            });
            return false;
        }

        private bool Process(Line line)
        {
            if (line?.Count > 0)
            {
                var op = line[0];
                line.RemoveAt(0);
                if (op != null)
                {
                    if (op.type == "lineNumbers")
                    {
                        return SetProgramCounter(new Line { op });
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
                        return Translate(line);
                    }
                    else
                    {
                        throw new Exception("Unknown command. >>> " + op.value);
                    }
                }
            }

            return PauseBeforeComplete(null);
        }

        private bool SetProgramCounter(Line line)
        {
            var lineNumber = int.Parse(Evaluate(line));
            counter = -1;
            while (counter < lineNumbers.Count - 1
                && lineNumbers[counter + 1] < lineNumber)
            {
                ++counter;
            }

            return true;
        }

        private void OnError(string msg)
        {
            throw new Exception($"At line {lineNumbers[counter]}: {msg}");
        }

        private Line GetLine(int i)
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

        private bool LabelLine(Line line)
        {
            line.Add(EQUAL_SIGN);
            line.Add(ToNum(lineNumbers[counter]));
            return Translate(line);
        }

        private bool NoOp(Line line)
        {
            return true;
        }

        private bool GoToSubroutine(Line line)
        {
            returnStack.Push(ToNum(lineNumbers[counter + 1]));
            return SetProgramCounter(line);
        }

        private bool SetRepeat(Line line)
        {
            returnStack.Push(ToNum(lineNumbers[counter]));
            return true;
        }

        private bool DeclareVariable(Line line)
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
                    OnError("Identifier expected: " + idToken.value);
                }
                else
                {
                    var id = idToken.value;
                    object val = null;
                    if (decl[0].value == "("
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

        private bool Print(Line line)
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

            var txt = Evaluate(line) ?? "";

            OnOutput(txt + endLine);

            return true;
        }

        private bool CheckConditional(Line line)
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
                OnError("Expected THEN clause.");
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

                Line elseClause = null;
                Line thenClause;
                if (elseIndex == -1)
                {
                    thenClause = line.GetRange(thenIndex + 1, line.Count - thenIndex - 1);
                }
                else
                {
                    thenClause = line.GetRange(thenIndex + 1, elseIndex - thenIndex - 1);
                    elseClause = line.GetRange(elseIndex + 1, line.Count - elseIndex - 1);
                }
                if (Evaluate(condition) == "True")
                {
                    return Process(thenClause);
                }
                else if (elseClause != null)
                {
                    return Process(elseClause);
                }
            }

            return true;
        }

        private bool WaitForInput(Line line)
        {
            var toVar = line[line.Count - 1];
            line.RemoveAt(line.Count - 1);

            if (line.Count > 0)
            {
                _ = Print(line);
            }

            OnInput(str =>
            {
                str = str.ToUpperInvariant();
                Token valueToken = null;
                if (int.TryParse(str, out var num))
                {
                    valueToken = ToNum(num);
                }
                else
                {
                    valueToken = ToStr(str);
                }

                _ = Evaluate(new Line { toVar, EQUAL_SIGN, valueToken });
                OnNext();
            });

            return false;
        }

        private bool OnStatement(Line line)
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

                var idx = int.Parse(Evaluate(idxExpr)) - 1;

                if (0 <= idx && idx < targets.Count)
                {
                    return SetProgramCounter(new Line { targets[idx] });
                }
            }
            return true;
        }

        public bool ConditionalReturn(bool cond)
        {
            var ret = true;
            if (cond && returnStack.Count > 0)
            {
                var val = returnStack.Pop();
                ret = SetProgramCounter(new Line { val });
            }
            return ret;
        }

        private bool UntilLoop(Line line)
        {
            var cond = Evaluate(line) == "True";
            return ConditionalReturn(cond);
        }

        private int FindNext(string str)
        {
            for (var i = counter + 1; i < lineNumbers.Count; ++i)
            {
                var l = GetLine(i);
                if (l[0].value == str)
                {
                    return i;
                }
            }

            return lineNumbers.Count;
        }

        private bool WhileLoop(Line line)
        {
            var cond = Evaluate(line) == "True";
            if (!cond)
            {
                counter = FindNext("WEND");
            }
            else
            {
                returnStack.Push(ToNum(lineNumbers[counter]));
            }
            return true;
        }

        private bool ForLoop(Line line)
        {
            var n = lineNumbers[counter];
            var varExpr = new Line();
            var fromExpr = new Line();
            var toExpr = new Line();
            var skipExpr = new Line();
            var arrs = new[] { varExpr, fromExpr, toExpr, skipExpr };
            var a = 0;
            for (var i = 0; i < line.Count; ++i)
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
                skip = int.Parse(Evaluate(skipExpr));
            }

            if (!forLoopCounters.ContainsKey(n))
            {
                forLoopCounters[n] = int.Parse(Evaluate(fromExpr));
            }

            var end = int.Parse(Evaluate(toExpr));
            var cond = forLoopCounters[n] <= end;
            if (!cond)
            {
                _ = forLoopCounters.Remove(n);
                counter = FindNext("NEXT");
            }
            else
            {
                varExpr.Add(ToNum(forLoopCounters[n]));
                _ = Process(varExpr);
                forLoopCounters[n] += skip;
                returnStack.Push(ToNum(lineNumbers[counter]));
            }
            return true;
        }

        private bool StackReturn(Line line)
        {
            return ConditionalReturn(true);
        }

        private bool LoadCodeFile(Line line)
        {
            var fileName = Evaluate(line);
            var data = OnLoadFile(fileName);
            _ = Encoding.UTF8.GetString(data);

            OnError("Don't know what to do with loading files yet");

            return false;
        }

        private bool ReadData(Line line)
        {
            if (data.Count == 0)
            {
                var dataLine = FindNext("DATA");
                _ = Process(GetLine(dataLine));
            }

            var value = data[dataCounter];
            ++dataCounter;
            line.Add(EQUAL_SIGN);
            line.Add(ToNum(int.Parse(value)));
            return Translate(line);
        }

        private bool RestoreData(Line line)
        {
            dataCounter = 0;
            return true;
        }

        private bool DefineFunction(Line line)
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

            state[name] = Eval(script); // jshint ignore:line

            return true;
        }

        private object Eval(string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                throw new ArgumentException(nameof(script));
            }

            throw new NotImplementedException();
        }
    }
}