using System.Text.Json;
using System.Text.Json.Nodes;

var projects = new DirectoryInfo(Directory.GetCurrentDirectory())
    .CD("@juniper-lib")
    .GetDirectories()
    .Where(d => d.Name != "esbuild")
    .Select(d => (
        Config: d.Touch("tsconfig.json"), 
        Package: d.Touch("package.json")
    ))
    .Where(p => p.Config.Exists && p.Package.Exists)
    .Select(p => (
        CfgFile: p.Config, 
        Config: Read(p.Config), 
        PkgFile: p.Package, 
        Package: Read(p.Package)
    ));

var packageLookup = projects
    .ToDictionary(p => p.Package!["name"]!.ToString(), p => p.PkgFile);
var opts = new JsonSerializerOptions
{
    WriteIndented = true
};

foreach (var p in projects)
{
    var references = p.Package
        ?["dependencies"]
        ?.AsObject()
        ?.Select(o => o.Key)
        ?.Where(k => k.StartsWith("@juniper-lib") && k != "@juniper-lib/esbuild")
        ?.Select(name => packageLookup[name])
        ?.Select(file => file.Directory?.Abs2Rel(p.CfgFile.Directory))
        ?.Select(path => new Dictionary<string, JsonNode?>{ ["path"] = path })
        ?.Select(dict => new JsonObject(dict))
        ?.ToArray();

    var extends = p.Config!["extends"];
    var include = p.Config["include"];
    p.Config.Clear();
    p.Config["extends"] = extends;
    p.Config["include"] = include;
    p.Config["references"] = JsonValue.Create(references);
    var cfgJson = p.Config.ToJsonString(opts);
    File.WriteAllText(p.CfgFile.FullName, cfgJson);

    p.Package!["scripts"]!["build"] = "tsc -b";
    var pkgJson = p.Package.ToJsonString(opts);
    File.WriteAllText(p.PkgFile.FullName, pkgJson);
}

static JsonObject? Read(FileInfo file)
{
    using var stream = file.OpenRead();
    return JsonNode.Parse(stream, documentOptions: new JsonDocumentOptions { AllowTrailingCommas = true })?.AsObject();
}