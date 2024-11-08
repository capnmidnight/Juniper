using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Juniper;

public static class MediaTypeExtensions
{
    public static IReadOnlyList<MediaType> GuessMediaType(this FileInfo file)
    {
        if (file is null)
        {
            return Array.Empty<MediaType>();
        }

        return MediaType.GuessByFileName(file.Name);
    }

    public static bool Matches(this FileInfo file, MediaType type)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        return type.GuessMatches(file.Name);
    }

    public static FileInfo AddExtension(this FileInfo file, MediaType type)
    {
        return new FileInfo(type.AddExtension(file.FullName));
    }
}

public partial class MediaType :
    IEquatable<MediaType>,
    IEquatable<string?>
{
    
    private static Dictionary<string, List<MediaType>>? _byExtensions;
    private static Dictionary<string, List<MediaType>> ByExtensions => _byExtensions ??= new();

    private static Dictionary<string, MediaType>? _byValue;
    private static Dictionary<string, MediaType> ByValue => _byValue ??= new();

    private static Regex? _typePattern;
    private static Regex TypePattern => _typePattern ??= new("([^\\/]+)\\/(.+)", RegexOptions.Compiled);

    private static Regex? _subTypePattern;
    private static Regex SubTypePattern => _subTypePattern ??= new("(?:([^\\.]+)\\.)?([^\\+;]+)(\\+[^;]*)?((?:; *([^=]+)=([^;]+))*)", RegexOptions.Compiled);

    public static MediaType Parse(string value)
    {
        if (TryParse(value, out var type))
        {
            return type!;
        }
        else
        {
            throw new ArgumentException($"Type value does not match expected pattern {TypePattern}", nameof(value));
        }
    }

    public static IReadOnlyCollection<MediaType> ParseAll(string[] acceptTypes)
    {
        if (acceptTypes is null || acceptTypes.Length == 0)
        {
            return All;
        }
        else
        {
            return InternalParseAll(acceptTypes).ToArray();
        }
    }

    private static IEnumerable<MediaType> InternalParseAll(string[] acceptTypes)
    {
        if (acceptTypes is not null)
        {
            foreach (var typeStr in acceptTypes)
            {
                if (TryParse(typeStr, out var type))
                {
                    yield return type!;
                }
            }
        }
    }

    public static bool TryParse(string? value, out MediaType? type)
    {
        type = null;

        if (value is null)
        {
            return false;
        }

        var match = TypePattern.Match(value);
        if (!match.Success)
        {
            return false;
        }


        var typeName = match.Groups[1].Value;
        var subTypeName = match.Groups[2].Value;
        var parsedType = new MediaType(typeName, subTypeName);
        var gotWeight = parsedType.Parameters.TryGetValue("q", out var weight);
        ByValue.TryGetValue(parsedType.Value, out var staticType);
        var basicType = staticType ?? parsedType;

        type = gotWeight && weight is not null
            ? basicType.WithParameter("q", weight!)
            : basicType;

        return type is not null;
    }

    public static IReadOnlyList<MediaType> GuessByExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext))
        {
            ext = "unknown";
        }
        else if (ext[0] == '.')
        {
            ext = ext[1..];
        }

        if (ByExtensions.TryGetValue(ext, out var value))
        {
            return value;
        }
        else
        {
            return new[] { new Unknown(ext) };
        }
    }

    public static IReadOnlyList<MediaType> GuessByFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return Array.Empty<MediaType>();
        }

        var ext = Path.GetExtension(fileName);
        return GuessByExtension(ext);
    }

    public static MediaType Lookup(string str)
    {
        // Trim off the weight, if one exists
        var parts = str.Split(';');
        if (parts.Length > 0)
        {
            str = parts[0];
        }

        if (ByValue.TryGetValue(str, out var value))
        {
            return value;
        }

        var name = Array.Find(parts, p => p.Length > 0);

        if (string.IsNullOrEmpty(name))
        {
            return Lookup("unknown/unknown");
        }
        else
        {
            return new Unknown(name);
        }
    }

    public static implicit operator string(MediaType mediaType) =>
        mediaType.ToString();

    public static implicit operator MediaTypeWithQualityHeaderValue(MediaType mediaType) =>
        new(mediaType.ToString());

    public string Type { get; }
    public string FullSubType { get; }

    public string? Tree { get; }
    public string SubType { get; }
    public string? Suffix { get; }
    public string Value { get; }
    public string FullValue { get; }
    public string? Comment { get; set; }

    private readonly Dictionary<string, string> _params = new();
    public IReadOnlyDictionary<string, string> Parameters => _params;

    private readonly string[] _extensions;
    public IReadOnlyCollection<string> Extensions => _extensions;

    public string? PrimaryExtension => _extensions.FirstOrDefault();

    protected internal MediaType(string type, string fullSubType, params string[] extensions)
    {
        Type = type;
        Value = FullValue = Type + "/";

        FullSubType = fullSubType;
        _extensions = extensions;

        string? paramStr = null;

        var subTypeParts = SubTypePattern.Match(FullSubType);
        if (subTypeParts.Success)
        {
            Tree = subTypeParts.Groups[1].Value;
            SubType = subTypeParts.Groups[2].Value;
            Suffix = subTypeParts.Groups[3].Value;
            paramStr = subTypeParts.Groups[4].Value;
        }
        else
        {
            SubType = FullSubType;
        }

        if (!string.IsNullOrEmpty(Tree))
        {
            Value = FullValue += Tree + ".";
        }

        Value = FullValue += SubType;

        if (!string.IsNullOrEmpty(Suffix))
        {
            Value = FullValue += Suffix;
            Suffix = Suffix[1..];
        }

        if (!string.IsNullOrEmpty(paramStr))
        {
            var pairs = paramStr.Split(';')
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .Select(p => p.Split('='));

            foreach (var pair in pairs)
            {
                var key = pair.FirstOrDefault();
                if (key is not null)
                {
                    var value = string.Join("=", pair.Skip(1));
                    _params[key] = value;
                    var slug = $"; {key}={value}";
                    FullValue += slug;
                    if (key != "q")
                    {
                        Value += slug;
                    }
                }
            }
        }

        if (Type != "*"
            && SubType != "*"
            && Type != "unknown"
            && SubType != "unknown")
        {
            foreach (var ext in Extensions)
            {
                if (!ByExtensions.TryGetValue(ext, out var value))
                {
                    ByExtensions.Add(ext, value = new());
                }

                value.Add(this);
            }

            if (!ByValue.ContainsKey(Value))
            {
                ByValue.Add(Value, this);
            }
        }
    }

    public MediaType WithParameter(string key, string value)
    {
        var newSubType = $"{FullSubType}; {key}={value}";
        return new MediaType(Type, newSubType, _extensions);
    }

    public bool Matches(string? contentType)
    {
        return contentType is not null && Matches(Parse(contentType));
    }

    public virtual bool Matches(MediaType value)
    {
        if (value is null)
        {
            return false;
        }

        return ReferenceEquals(this, Any)
            || Value == value.Value
            || (Type == value.Type
                && (SubType == "*"
                || value.SubType == "*"));
    }

    public virtual bool GuessMatches(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return false;
        }

        var types = GuessByFileName(fileName);
        return types.Contains(this);
    }

    public string AddExtension(string? fileName)
    {
        if (fileName is null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (PrimaryExtension is not null)
        {
            var idx = fileName.LastIndexOf('.');
            if (idx > -1)
            {
                var currentExtension = fileName[(idx + 1)..];
                if (Extensions.Contains(currentExtension))
                {
                    fileName = fileName[0..idx];
                }
            }

            fileName = $"{fileName}.{PrimaryExtension}";
        }

        return fileName;
    }

    public bool Equals(string? other)
    {
        return TryParse(other, out var type)
            && Equals(type);
    }

    public override bool Equals(object? obj)
    {
        return (obj is MediaType type && Equals(type))
            || (obj is string str && Equals(str));
    }

    public bool Equals(MediaType? other)
    {
        return other is not null
            && (Tree == "*"
                || other.Tree == "*"
                || Tree == other.Tree)
            && (SubType == "*"
                || other.SubType == "*"
                || SubType == other.SubType);
    }

    public static bool operator ==(MediaType? left, MediaType? right)
    {
        return (left is null && right is null)
            || (left is not null && left.Equals(right));
    }

    public static bool operator !=(MediaType? left, MediaType? right)
    {
        return !(left == right);
    }

    public static bool operator ==(MediaType left, string right)
    {
        return (left is null && right is null)
            || (left is not null && left.Equals(right));
    }

    public static bool operator !=(MediaType left, string right)
    {
        return !(left == right);
    }

    public static bool operator ==(string left, MediaType right)
    {
        return (left is null && right is null)
            || (right is not null && right.Equals(left));
    }

    public static bool operator !=(string left, MediaType right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        if (!Parameters.ContainsKey("q")
            || Parameters["q"] == "1")
        {
            return Value;
        }
        else
        {
            return FullValue;
        }
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Tree);
        hash.Add(SubType);
        foreach (var (key, value) in Parameters)
        {
            hash.Add(key);
            hash.Add(value);
        }
        foreach (var value in Extensions)
        {
            hash.Add(value);
        }
        return hash.ToHashCode();
    }
}
