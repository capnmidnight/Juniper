using Accord.Math;

using Juniper.IO;

using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Juniper
{
    public static class StringExt
    {
        public static string AddExtension(this string str, MediaType contentType)
        {
            return contentType.AddExtension(str);
        }
    }

    public partial class MediaType :
        IEquatable<MediaType>,
        IEquatable<string>
    {
        private static Dictionary<string, List<MediaType>> _byExtensions;
        private static Dictionary<string, List<MediaType>> ByExtensions => _byExtensions ??= new();

        private static Dictionary<string, MediaType> _byValue;
        private static Dictionary<string, MediaType> ByValue => _byValue ??= new();

        private static Regex _typePattern;
        private static Regex TypePattern => _typePattern ??= new("([^\\/]+)\\/(.+)", RegexOptions.Compiled);

        private static Regex _subTypePattern;
        private static Regex SubTypePattern => _subTypePattern ??= new("(?:([^\\.]+)\\.)?([^\\+;]+)(\\+[^;]*)?((?:; *([^=]+)=([^;]+))*)", RegexOptions.Compiled);

        public static MediaType Parse(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (TryParse(value, out var type))
            {
                return type;
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
                        yield return type;
                    }
                }
            }
        }

        public static bool TryParse(string value, out MediaType type)
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
            var weight = parsedType.Parameters.Get("q");
            var staticType = ByValue.Get(parsedType.Value);
            var basicType =  staticType ?? parsedType;

            type = weight is not null
                ? basicType.WithParameter("q", weight)
                : basicType;

            return type != null;
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

            if (ByExtensions.ContainsKey(ext))
            {
                return ByExtensions[ext];
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

        public static IReadOnlyList<MediaType> GuessByFile(FileInfo file)
        {
            if (file is null)
            {
                return Array.Empty<MediaType>();
            }

            return GuessByFileName(file.Name);
        }

        public static MediaType Lookup(string value)
        {
            // Trim off the weight, if one exists
            var parts = value.SplitX(';');
            if (parts.Length > 0)
            {
                value = parts[0];
            }

            if (ByValue.ContainsKey(value))
            {
                return ByValue[value];
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

        public static ContentReference operator +(string cacheID, MediaType contentType)
        {
            return new ContentReference(cacheID, contentType);
        }

        public static implicit operator string(MediaType mediaType)
        {
            return mediaType?.ToString();
        }

        public static implicit operator MediaTypeWithQualityHeaderValue(MediaType mediaType)
        {
            return new MediaTypeWithQualityHeaderValue(mediaType);
        }

        public string Type { get; }
        public string FullSubType { get; }
        public string Tree { get; }
        public string SubType { get; }
        public string Suffix { get; }

        private readonly Dictionary<string, string> _params = new();
        public IReadOnlyDictionary<string, string> Parameters => _params;

        public string Value { get; }
        public string FullValue { get; }
        public string Comment { get; set; }

        private readonly string[] _extensions;
        public IReadOnlyCollection<string> Extensions => _extensions;

        public string PrimaryExtension => _extensions.FirstOrDefault();

        protected internal MediaType(string type, string fullSubType, params string[] extensions)
        {
            Type = type;
            FullSubType = fullSubType;
            _extensions = extensions;

            var subTypeParts = SubTypePattern.Match(FullSubType);
            if (subTypeParts.Success)
            {
                Tree = subTypeParts.Groups[1].Value;
                SubType = subTypeParts.Groups[2].Value;
                Suffix = subTypeParts.Groups[3].Value;
                var paramStr = subTypeParts.Groups[4].Value;

                Value = FullValue = Type + "/";

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
                    var pairs = paramStr.SplitX(';')
                        .Select(p => p.Trim())
                        .Where(p => p.Length > 0)
                        .Select(p => p.SplitX('='));

                    foreach (var pair in pairs)
                    {
                        var key = pair.FirstOrDefault();
                        var value = pair.Skip(1).ToArray().Join("=");
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

            if (Type != "*" && SubType != "*"
                && Type != "unknown" && SubType != "unknown")
            {
                foreach (var ext in Extensions)
                {
                    if (!ByExtensions.ContainsKey(ext))
                    {
                        ByExtensions.Add(ext, new());
                    }

                    ByExtensions[ext].Add(this);
                }

                if (ByValue.ContainsKey(Value))
                {

                }
                else
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

        public bool Matches(string contentType)
        {
            return Matches(Parse(contentType));
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
                    && SubType == "*");
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

        public bool GuessMatches(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return GuessMatches(file.Name);
        }

        public string AddExtension(string fileName)
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

        public bool Equals(string other)
        {
            return TryParse(other, out var type)
                && Equals(type);
        }

        public override bool Equals(object obj)
        {
            return (obj is MediaType type && Equals(type))
                || (obj is string str && Equals(str));
        }

        public bool Equals(MediaType other)
        {
            return other is not null
                && (Tree == "*"
                    || other.Tree == "*"
                    || Tree == other.Tree)
                && (SubType == "*"
                    || other.SubType == "*"
                    || SubType == other.SubType);
        }

        public static bool operator ==(MediaType left, MediaType right)
        {
            return (left is null && right is null)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(MediaType left, MediaType right)
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
}
