using static System.Console;
using Juniper;
using System.Reflection;
using System.Text;

var baseType = typeof(MediaType);
var fields = baseType.GetFields(BindingFlags.Static | BindingFlags.Public)
    .GroupBy(f => f.FieldType)
    .ToDictionary(g => g.Key.Name.ToLowerInvariant(), g => g.ToArray());

var root = new DirectoryInfo(Environment.CurrentDirectory).CD("..", "..", "JS", "mediatypes", "src");

var template = @"import { specialize } from ""./util"";

const [name] = /*@__PURE__*/ (function() { return specialize(""[type_name]""); })();";

var valueTemplate = @"export const [value_name] = /*@__PURE__*/ (function() { return [type_name](""[sub_type]"", ""[extensions]"")[obsolete]; })();";
var sb = new StringBuilder();

foreach (var kv in fields)
{
    var name = kv.Key;
    if (name != "mediatype")
    {
        var typeName = name;
        if (name[0] == 'x')
        {
            if (name != "xconference")
            {
                typeName = name[0] + "-" + name[1..];
            }
            name = name[0] + name[1..2].ToUpper() + name[2..];
        }
        var file = root.Touch(name).AddExtension(MediaType.Application_Vendor_TypeScript);
        WriteLine("{0}: {1} = {2}", file.FullName, file.Exists, kv.Value.Length);
        sb.Clear();
        sb.AppendLine(template
            .Replace("[name]", name)
            .Replace("[type_name]", typeName)
        );
        sb.AppendLine();
        foreach (var field in kv.Value)
        {
            var value = (MediaType)field.GetValue(null)!;
            var attr = field.GetCustomAttribute<ObsoleteAttribute>();
            var obsMsg = attr?.Message is null
                ? ""
                : $@".deprecate(""{attr.Message}"")";
            sb.AppendLine(valueTemplate
                .Replace("[value_name]", field.Name)
                .Replace("[type_name]", name)
                .Replace("[sub_type]", value.FullSubType)
                .Replace("[extensions]", string.Join("\", \"", value.Extensions))
                .Replace("[obsolete]", obsMsg)
                .Replace(", \"\"", "")
            );
        }

        File.WriteAllText(file.FullName, sb.ToString());
    }
}