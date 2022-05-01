using System.Text;
using System.Text.RegularExpressions;

using static System.Console;

var dirName = Environment.CurrentDirectory;

if (args.Length == 1)
{
    dirName = args[0];
}
else if (args.Length != 0)
{
    Error.WriteLine(@"Usage: ts-gen-ct [<types project directory>]

The types project directory defaults to the current working directory.
The types project directory must contain a directory named `types`.");
    return 1;
}

var dir = new DirectoryInfo(Path.Combine(dirName, "types"));
if (!dir.Exists)
{
    Error.WriteLine("Directory does not exist: " + dir.FullName);
    return 1;
}

foreach (var file in dir.EnumerateFiles("*.d.ts", SearchOption.AllDirectories))
{
    var insertions = new List<(int, string)>();
    var code = File.ReadAllText(file.FullName);
    var index = 0;

    bool Sub(string sub)
    {
        return index + sub.Length < code.Length
            && code[index..(index + sub.Length)] == sub;
    }

    var revertNewLines = code.Contains("\r\n");
    if (revertNewLines)
    {
        code = code.Replace("\r\n", "\n");
    }

    var inInlineComment = false;
    var inMultilineComment = false;
    var inString1 = false;
    var inString2 = false;
    var inString3 = false;
    var escaping = false;
    var exportFound = false;
    var classNameStart = -1;
    var className = "";
    var constructorFound = false;
    var typeListStart = -1;
    var typeListDepth = 0;
    var typeParamList = "";
    var scopeLevel = 0;
    var paramListDepth = 0;
    var paramListStart = -1;
    var paramLists = new List<string>();

    void Reset()
    {
        if (paramLists.Count > 0)
        {
            var typeArgList = "";
            if(typeParamList.Length > 0)
            {
                var typeListParts = typeParamList[1..(typeParamList.Length - 2)]
                    .Split(',')
                    .Select(e =>
                        e.Trim()
                        .Split(' ')
                        .First()
                        .Trim())
                    .ToArray();
                typeArgList = "<" + string.Join(", ", typeListParts) + ">";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"\n\nexport interface {className}Constructor{typeParamList} {{");
            foreach (var paramList in paramLists)
            {
                sb.AppendLine($"    new {paramList}: {className}{typeArgList};");
            }

            sb.AppendLine($"    prototype: {className}{typeArgList};");
            sb.Append("}");
            var @interface = sb.Replace("\r\n", "\n").ToString();
            if (!code.Contains(@interface))
            {
                if (revertNewLines)
                {
                    @interface = @interface.Replace("\n", "\r\n");
                }
                insertions.Add((index + 1, @interface));
            }
        }

        inInlineComment = false;
        inMultilineComment = false;
        inString1 = false;
        inString2 = false;
        inString3 = false;
        escaping = false;
        exportFound = false;
        className = "";
        constructorFound = false;
        classNameStart = -1;
        typeListStart = -1;
        typeListDepth = 0;
        typeParamList = "";
        scopeLevel = 0;
        paramListDepth = 0;
        paramListStart = -1;
        paramLists.Clear();
    }

    while (index < code.Length)
    {
        var contextPre = code[Math.Max(index - 10, 0)..index];
        var contextPost = code[index..Math.Min(code.Length, index + 11)];
        var context = contextPre + "|" + contextPost;
        var isWhitespace = char.IsWhiteSpace(code[index]);
        var isString1Delim = code[index] == '\'';
        var isString2Delim = code[index] == '"';
        var isString3Delim = code[index] == '`';
        var isEscaper = code[index] == '\\';
        var isTypeListStart = code[index] == '<';
        var isTypeListEnd = code[index] == '>';
        var isScopeStart = code[index] == '{';
        var isScopeEnd = code[index] == '}';
        var isParamListStart = code[index] == '(';
        var isParamListEnd = code[index] == ')';
        var isInlineCommentStart = Sub("//");
        var isInlineCommentEnd = Sub("\n");
        var isMultilineCommentStart = Sub("/*");
        var isMultlineCommentEnd = Sub("*/");
        var isExport = Sub("export class ");
        var isConstructor = Sub("constructor");

        var inString = inString1 || inString2 || inString3;

        if (inInlineComment)
        {
            inInlineComment = !isInlineCommentEnd;
        }
        else if (inMultilineComment)
        {
            if (isMultlineCommentEnd)
            {
                inMultilineComment = false;
                index++;
            }
        }
        else if (inString)
        {
            escaping = !escaping && isEscaper;

            if (!escaping)
            {
                inString1 &= !isString1Delim;
                inString2 &= !isString2Delim;
                inString3 &= !isString3Delim;
            }
        }
        else if (isInlineCommentStart)
        {
            inInlineComment = true;
            index++;
        }
        else if (isMultilineCommentStart)
        {
            inMultilineComment = true;
            index++;
        }
        else if (isString1Delim)
        {
            inString1 = true;
        }
        else if (isString2Delim)
        {
            inString2 = true;
        }
        else if (isString3Delim)
        {
            inString3 = true;
        }
        else if (!exportFound)
        {
            if (isExport)
            {
                exportFound = true;
                index += "export class ".Length;
                classNameStart = index;
            }
        }
        else if (className.Length == 0)
        {
            if (isWhitespace || isTypeListStart || isScopeStart)
            {
                className = code[classNameStart..index];
                typeListStart = index;

                if (isTypeListStart)
                {
                    typeListDepth++;
                }
                else if (isScopeStart)
                {
                    scopeLevel++;
                }
            }
        }
        else if (typeListStart > 0)
        {
            if (isTypeListStart)
            {
                typeListDepth++;
            }
            else if (isTypeListEnd)
            {
                typeListDepth--;
                if (typeListDepth == 0)
                {
                    typeParamList = code[typeListStart..(index + 1)];
                    typeListStart = -1;
                }
            }
            else if (typeListDepth == 0 && !isWhitespace)
            {
                typeListStart = -1;
                if (isScopeStart)
                {
                    scopeLevel++;
                }
            }
        }
        else if (isScopeStart)
        {
            scopeLevel++;
        }
        else if (scopeLevel > 0 && isScopeEnd)
        {
            scopeLevel--;
            if (scopeLevel == 0)
            {
                Reset();
            }
        }
        else if (scopeLevel == 1)
        {
            if (!constructorFound)
            {
                if (isConstructor)
                {
                    constructorFound = true;
                    index += "constructor".Length - 1;
                }
            }
            else if (isParamListStart)
            {
                if (paramListDepth == 0)
                {
                    paramListStart = index;
                }
                paramListDepth++;
            }
            else if (paramListDepth > 0 && isParamListEnd)
            {
                paramListDepth--;
                if (paramListDepth == 0 && paramListStart > -1)
                {
                    paramLists.Add(code[paramListStart..(index + 1)]);
                    constructorFound = false;
                    paramListStart = -1;
                }
            }
        }

        index++;
    }

    if (insertions.Count > 0)
    {
        WriteLine(file.FullName);
        insertions.Reverse();

        var codeSB = new StringBuilder(code);
        foreach (var (insertionPoint, snippet) in insertions)
        {
            codeSB.Insert(insertionPoint, snippet);
        }

        code = codeSB.ToString();
        File.WriteAllText(file.FullName, code);
    }
}

return 0;