using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

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

        public override List<Token> Tokenize(string code)
        {
            return base.Tokenize(code.ToUpperInvariant());
        }

        private static readonly Token EQUAL_SIGN = new Token("=", "operators");
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);

        public void Interpret(string code,
            Stream input, Stream output, Stream errorOut,
            Action clearScreen, Func<string, string> loadFile,
            Action next, Action done)
        {
            var program = new Dictionary<int, List<Token>>();
            var lineNumbers = new List<int>();
            var currentLine = new List<Token>();
            var lines = new List<List<Token>>();
            var data = new List<byte>();
            var returnStack = new List<int>();
            var forLoopCounters = new Dictionary<int, int>();
            var random = new Random();
            int counter = 0;
            var isDone = false;
            var dataCounter = 0;

            var state = new Dictionary<string, object>
            {
                { "INT", new Func<float, int>(v => (int) v) },
                { "RND", new Func<float>(() => (float)random.NextDouble()) },
                { "CLK", new Func<float>(() => (float)(DateTime.Now - EPOCH).TotalHours) },
                { "LEN", new Func<string, int>(str => str.Length) },
                { "LINE", new Func<int>(() => lineNumbers[counter]) },
                { "POW", new Func<float, float, float>((b, e) => (float)Math.Pow(b, e)) },
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

            Func<int, Token> toNum = i => new Token(i.ToString(), "numbers");
            Func<string, Token> toStr = str => new Token("\"" + str
                .Replace("\n", "\\n")
                .Replace("\"", "\\\"") + "\"", "strings");

            var tokenMap = new Dictionary<string, string>
            {
                { "OR", "||" },
                { "AND", "&&" },
                { "NOT", "!" },
                { "MOD", "%" },
                { "<>", "!=" }
            };

            var tokens = Tokenize(code);

            // Remove comments and whitespace, and do a light translation of
            // operator tokens.
            while (tokens.Count > 0)
            {
                var token = tokens[0];
                tokens.RemoveAt(0);
                if (token.type == "newlines")
                {
                    currentLine = new List<Token>();
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
        /*

  function process(line)
{
    if (line && line.length > 0)
    {
        var op = line.shift();
        if (op)
        {
            if (commands.hasOwnProperty(op.value))
            {
                return commands[op.value](line);
            }
            else if (!isNaN(op.value))
            {
                return setProgramCounter([op]);
            }
            else if (state[op.value] ||
              (line.length > 0 && line[0].type === "operators" &&
                line[0].value === "="))
            {
                line.unshift(op);
                return translate(line);
            }
            else
            {
                error("Unknown command. >>> " + op.value);
            }
        }
    }
    return pauseBeforeComplete();
}

function error(msg)
{
    errorOut("At line " + lineNumbers[counter] + ": " + msg);
}

function getLine(i)
{
    var lineNumber = lineNumbers[i];
    var line = program[lineNumber];
    return line && line.slice();
}

function evaluate(line)
{
    var script = "";
    for (var i = 0; i < line.length; ++i)
    {
        var t = line[i];
        var nest = 0;
        if (t.type === "identifiers" &&
          typeof state[t.value] !== "function" &&
          i < line.length - 1 &&
          line[i + 1].value === "(")
        {
            for (var j = i + 1; j < line.length; ++j)
            {
                var t2 = line[j];
                if (t2.value === "(")
                {
                    if (nest === 0)
                    {
                        t2.value = "[";
                    }
                    ++nest;
                }
                else if (t2.value === ")")
                {
                    --nest;
                    if (nest === 0)
                    {
                        t2.value = "]";
                    }
                }
                else if (t2.value === "," && nest === 1)
                {
                    t2.value = "][";
                }

                if (nest === 0)
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
        return eval2(script); // jshint ignore:line
    }
    catch (exp)
    {
        console.error(exp);
        console.debug(line.join(", "));
        console.error(script);
        error(exp.message + ": " + script);
    }
    //}
}

function declareVariable(line)
{
    var decl = [],
      decls = [decl],
      nest = 0,
      i;
    for (i = 0; i < line.length; ++i)
    {
        var t = line[i];
        if (t.value === "(")
        {
            ++nest;
        }
        else if (t.value === ")")
        {
            --nest;
        }
        if (nest === 0 && t.value === ",")
        {
            decl = [];
            decls.push(decl);
        }
        else
        {
            decl.push(t);
        }
    }
    for (i = 0; i < decls.length; ++i)
    {
        decl = decls[i];
        var id = decl.shift();
        if (id.type !== "identifiers")
        {
            error("Identifier expected: " + id.value);
        }
        else
        {
            var val = null,
              j;
            id = id.value;
            if (decl[0].value === "(" && decl[decl.length - 1].value === ")")
            {
                var sizes = [];
                for (j = 1; j < decl.length - 1; ++j)
                {
                    if (decl[j].type === "numbers")
                    {
                        sizes.push(decl[j].value | 0);
                    }
                }
                if (sizes.length === 0)
                {
                    val = [];
                }
                else
                {
                    val = new Array(sizes[0]);
                    var queue = [val];
                    for (j = 1; j < sizes.length; ++j)
                    {
                        var size = sizes[j];
                        for (var k = 0,
                            l = queue.length; k < l; ++k)
                        {
                            var arr = queue.shift();
                            for (var m = 0; m < arr.length; ++m)
                            {
                                arr[m] = new Array(size);
                                if (j < sizes.length - 1)
                                {
                                    queue.push(arr[m]);
                                }
                            }
                        }
                    }
                }
            }
            state[id] = val;
            return true;
        }
    }
}

function print(line)
{
    var endLine = "\n";
    var nest = 0;
    line = line.map(function(t, i) {
        t = t.clone();
        if (t.type === "operators")
        {
            if (t.value === ",")
            {
                if (nest === 0)
                {
                    t.value = "+ \", \" + ";
                }
            }
            else if (t.value === ";")
            {
                t.value = "+ \" \"";
                if (i < line.length - 1)
                {
                    t.value += " + ";
                }
                else
                {
                    endLine = "";
                }
            }
            else if (t.value === "(")
            {
                ++nest;
            }
            else if (t.value === ")")
            {
                --nest;
            }
        }
        return t;
    });
    var txt = evaluate(line);
    if (txt === undefined)
    {
        txt = "";
    }
    output(txt + endLine);
    return true;
}

function setProgramCounter(line)
{
    var lineNumber = parseFloat(evaluate(line));
    counter = -1;
    while (counter < lineNumbers.length - 1 &&
      lineNumbers[counter + 1] < lineNumber)
    {
        ++counter;
    }

    return true;
}

function checkConditional(line)
{
    var thenIndex = -1,
      elseIndex = -1,
      i;
    for (i = 0; i < line.length; ++i)
    {
        if (line[i].type === "keywords" && line[i].value === "THEN")
        {
            thenIndex = i;
        }
        else if (line[i].type === "keywords" && line[i].value === "ELSE")
        {
            elseIndex = i;
        }
    }
    if (thenIndex === -1)
    {
        error("Expected THEN clause.");
    }
    else
    {
        var condition = line.slice(0, thenIndex);
        for (i = 0; i < condition.length; ++i)
        {
            var t = condition[i];
            if (t.type === "operators" && t.value === "=")
            {
                t.value = "==";
            }
        }
        var thenClause,
          elseClause;
        if (elseIndex === -1)
        {
            thenClause = line.slice(thenIndex + 1);
        }
        else
        {
            thenClause = line.slice(thenIndex + 1, elseIndex);
            elseClause = line.slice(elseIndex + 1);
        }
        if (evaluate(condition))
        {
            return process(thenClause);
        }
        else if (elseClause)
        {
            return process(elseClause);
        }
    }

    return true;
}

function pauseBeforeComplete()
{
    output("PROGRAM COMPLETE - PRESS RETURN TO FINISH.");
    input(function() {
        isDone = true;
        if (done)
        {
            done();
        }
    });
    return false;
}

function labelLine(line)
{
    line.push(EQUAL_SIGN);
    line.push(toNum(lineNumbers[counter]));
    return translate(line);
}

function waitForInput(line)
{
    var toVar = line.pop();
    if (line.length > 0)
    {
        print(line);
    }
    input(function(str) {
        str = str.toUpperCase();
        var valueToken = null;
        if (!isNaN(str))
        {
            valueToken = toNum(str);
        }
        else
        {
            valueToken = toStr(str);
        }
        evaluate([toVar, EQUAL_SIGN, valueToken]);
        if (next)
        {
            next();
        }
    });
    return false;
}

function onStatement(line)
{
    var idxExpr = [],
      idx = null,
      targets = [];
    try
    {
        while (line.length > 0 &&
          (line[0].type !== "keywords" ||
            line[0].value !== "GOTO"))
        {
            idxExpr.push(line.shift());
        }

        if (line.length > 0)
        {
            line.shift(); // burn the goto;

            for (var i = 0; i < line.length; ++i)
            {
                var t = line[i];
                if (t.type !== "operators" ||
                  t.value !== ",")
                {
                    targets.push(t);
                }
            }

            idx = evaluate(idxExpr) - 1;

            if (0 <= idx && idx < targets.length)
            {
                return setProgramCounter([targets[idx]]);
            }
        }
    }
    catch (exp)
    {
        console.error(exp);
    }
    return true;
}

function gotoSubroutine(line)
{
    returnStack.push(toNum(lineNumbers[counter + 1]));
    return setProgramCounter(line);
}

function setRepeat()
{
    returnStack.push(toNum(lineNumbers[counter]));
    return true;
}

function conditionalReturn(cond)
{
    var ret = true;
    var val = returnStack.pop();
    if (val && cond)
    {
        ret = setProgramCounter([val]);
    }
    return ret;
}

function untilLoop(line)
{
    var cond = !evaluate(line);
    return conditionalReturn(cond);
}

function findNext(str)
{
    for (i = counter + 1; i < lineNumbers.length; ++i)
    {
        var l = getLine(i);
        if (l[0].value === str)
        {
            return i;
        }
    }
    return lineNumbers.length;
}

function whileLoop(line)
{
    var cond = evaluate(line);
    if (!cond)
    {
        counter = findNext("WEND");
    }
    else
    {
        returnStack.push(toNum(lineNumbers[counter]));
    }
    return true;
}

var FOR_LOOP_DELIMS = ["=", "TO", "STEP"];

function forLoop(line)
{
    var n = lineNumbers[counter];
    var varExpr = [];
    var fromExpr = [];
    var toExpr = [];
    var skipExpr = [];
    var arrs = [varExpr, fromExpr, toExpr, skipExpr];
    var a = 0;
    var i = 0;
    for (i = 0; i < line.length; ++i)
    {
        var t = line[i];
        if (t.value === FOR_LOOP_DELIMS[a])
        {
            if (a === 0)
            {
                varExpr.push(t);
            }
            ++a;
        }
        else
        {
            arrs[a].push(t);
        }
    }

    var skip = 1;
    if (skipExpr.length > 0)
    {
        skip = evaluate(skipExpr);
    }

    if (forLoopCounters[n] === undefined)
    {
        forLoopCounters[n] = evaluate(fromExpr);
    }

    var end = evaluate(toExpr);
    var cond = forLoopCounters[n] <= end;
    if (!cond)
    {
        delete forLoopCounters[n];
        counter = findNext("NEXT");
    }
    else
    {
        varExpr.push(toNum(forLoopCounters[n]));
        process(varExpr);
        forLoopCounters[n] += skip;
        returnStack.push(toNum(lineNumbers[counter]));
    }
    return true;
}

function stackReturn()
{
    return conditionalReturn(true);
}

function loadCodeFile(line)
{
    loadFile(evaluate(line))
      .then(next);
    return false;
}

function noop()
{
    return true;
}

function loadData(line)
{
    while (line.length > 0)
    {
        var t = line.shift();
        if (t.type !== "operators")
        {
            data.push(t.value);
        }
    }
    return true;
}

function readData(line)
{
    if (data.length === 0)
    {
        var dataLine = findNext("DATA");
        process(getLine(dataLine));
    }
    var value = data[dataCounter];
    ++dataCounter;
    line.push(EQUAL_SIGN);
    line.push(toNum(value));
    return translate(line);
}

function restoreData()
{
    dataCounter = 0;
    return true;
}

function defineFunction(line)
{
    var name = line.shift()
      .value;
    var signature = "";
    var body = "";
    var fillSig = true;
    for (var i = 0; i < line.length; ++i)
    {
        var t = line[i];
        if (t.type === "operators" && t.value === "=")
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
    state[name] = eval2(script); // jshint ignore:line
    return true;
}

function translate(line)
{
    evaluate(line);
    return true;
}

var commands = {
    DIM: declareVariable,
    LET: translate,
    PRINT: print,
    GOTO: setProgramCounter,
    IF: checkConditional,
    INPUT: waitForInput,
    END: pauseBeforeComplete,
    STOP: pauseBeforeComplete,
    REM: noop,
    "'": noop,
    CLS: clearScreen,
    ON: onStatement,
    GOSUB: gotoSubroutine,
    RETURN: stackReturn,
    LOAD: loadCodeFile,
    DATA: loadData,
    READ: readData,
    RESTORE: restoreData,
    REPEAT: setRepeat,
    UNTIL: untilLoop,
    "DEF FN": defineFunction,
    WHILE: whileLoop,
    WEND: stackReturn,
    FOR: forLoop,
    NEXT: stackReturn,
    LABEL: labelLine
  };

  return function()
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
};
};
*/
    }
}