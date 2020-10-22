using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Juniper.IO;

namespace Juniper
{
    public partial class MediaType :
        IEquatable<MediaType>,
        IEquatable<string>
    {
        private static Dictionary<string, List<MediaType>> byExtensions;

        private static Dictionary<string, MediaType> byValue;

        private static Regex typePattern;

        private static Regex TYPE_PATTERN
        {
            get
            {
                if (typePattern is null)
                {
                    typePattern = new Regex("(\\*|\\w+)/(\\*|(-|\\w)+)(?:;q=(1|0?\\.\\d+))?", RegexOptions.Compiled);
                }

                return typePattern;
            }
        }

        public static readonly MediaType Any = new MediaType("*/*");

        public static readonly IReadOnlyCollection<MediaType> All = new MediaType[] { Any };

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
                throw new ArgumentException($"Type value does not match expected pattern {TYPE_PATTERN.ToString()}", nameof(value));
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
            if (acceptTypes is object)
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

            var match = TYPE_PATTERN.Match(value);
            if (!match.Success)
            {
                return false;
            }

            var typeName = match.Groups[1].Value;
            var subTypeName = match.Groups[2].Value;
            var rawValue = typeName + "/" + subTypeName;

            if (match.Groups.Count == 4
                && float.TryParse(
                    match.Groups[3].Value,
                    NumberStyles.Integer | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out var weight))
            {
                type = new MediaType(typeName, subTypeName, weight);
            }
            else if (byValue.ContainsKey(rawValue))
            {
                type = byValue[rawValue];
            }
            else
            {
                type = new MediaType(typeName, subTypeName);
            }

            return type is object;
        }

        private static IReadOnlyList<MediaType> GuessByExtension(ref string ext)
        {
            if (string.IsNullOrEmpty(ext))
            {
                ext = "unknown";
            }
            else if (ext[0] == '.')
            {
                ext = ext.Substring(1);
            }

            if (byExtensions.ContainsKey(ext))
            {
                return byExtensions[ext];
            }
            else
            {
                return new MediaType[] { new Unknown("unknown/" + ext) };
            }
        }

        public static IReadOnlyList<MediaType> GuessByFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Must provide a valid fileName", nameof(fileName));
            }

            var ext = Path.GetExtension(fileName);
            return GuessByExtension(ref ext);
        }

        public static IReadOnlyList<MediaType> GuessByFile(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
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

            if (byValue.ContainsKey(value))
            {
                return byValue[value];
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

        private readonly string weightedValue;

        public string Value { get; }

        public string TypeName { get; }

        public string SubTypeName { get; }

        public float Weight { get; }

        public ReadOnlyCollection<string> Extensions { get; }

        public string PrimaryExtension { get; }

        private MediaType(string typeName, string subTypeName, float weight)
        {
            TypeName = typeName;
            SubTypeName = subTypeName;
            Weight = weight;
            Value = typeName + "/" + subTypeName;
            weightedValue = $"{Value};q={weight.ToString(CultureInfo.InvariantCulture)}";

            var extensions = Array.Empty<string>();
            Extensions = Array.AsReadOnly(extensions);
            PrimaryExtension = null;

            UpdateLookupTables();
        }

        private MediaType(string typeName, string subTypeName)
            : this(typeName, subTypeName, 1)
        { }

        protected internal MediaType(string value, string[] extensions = null)
        {
            Value = value;

            var match = TYPE_PATTERN.Match(value);
            if (!match.Success)
            {
                throw new ArgumentException($"Type value does not match expected pattern {TYPE_PATTERN.ToString()}", nameof(value));
            }

            TypeName = match.Groups[1].Value;
            SubTypeName = match.Groups[2].Value;
            Weight = 1;
            weightedValue = $"{Value};q={Weight.ToString(CultureInfo.InvariantCulture)}";

            if (extensions is null)
            {
                extensions = Array.Empty<string>();
            }

            Extensions = Array.AsReadOnly(extensions);
            if (Extensions.Count >= 1)
            {
                PrimaryExtension = extensions[0];
            }

            UpdateLookupTables();
        }

        private void UpdateLookupTables()
        {
            if (byValue is null)
            {
                byValue = new Dictionary<string, MediaType>(1000);
            }

            if (byExtensions is null)
            {
                byExtensions = new Dictionary<string, List<MediaType>>(1000);
            }

            _ = byValue.Default(Value, this);

            foreach (var ext in Extensions)
            {
                if (!byExtensions.ContainsKey(ext))
                {
                    byExtensions[ext] = new List<MediaType>();
                }

                var exts = byExtensions[ext];
                exts.Add(this);
            }
        }

        public virtual bool Matches(string mimeType)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                return false;
            }

            return ReferenceEquals(this, Any)
                || Value == mimeType;
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

            if (PrimaryExtension is object)
            {
                var currentExtension = PathExt.GetShortExtension(fileName);
                if (Extensions.IndexOf(currentExtension) == -1)
                {
                    fileName += "." + PrimaryExtension;
                }
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
            return other is object
                && (TypeName == "*"
                    || other.TypeName == "*"
                    || TypeName == other.TypeName)
                && (SubTypeName == "*"
                    || other.SubTypeName == "*"
                    || SubTypeName == other.SubTypeName);
        }

        public static bool operator ==(MediaType left, MediaType right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(MediaType left, MediaType right)
        {
            return !(left == right);
        }

        public static bool operator ==(MediaType left, string right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(MediaType left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(string left, MediaType right)
        {
            return (left is null && right is null)
                || (right is object && right.Equals(left));
        }

        public static bool operator !=(string left, MediaType right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            if (Weight == 1)
            {
                return Value;
            }
            else
            {
                return weightedValue;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -1006577490;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(TypeName);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(SubTypeName);
            hashCode = (hashCode * -1521134295) + Weight.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ReadOnlyCollection<string>>.Default.GetHashCode(Extensions);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(PrimaryExtension);
            return hashCode;
        }
    }
}
