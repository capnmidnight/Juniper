using System;
using System.Collections.Generic;

namespace Juniper.ConfigurationManagement
{
    public class PackageReference :
        IEquatable<PackageReference>,
        IComparable<PackageReference>
    {
        public static string FormatUnityPackageManagerSpec(string packageID, string version)
        {
            if (string.IsNullOrEmpty(packageID))
            {
                throw new ArgumentException("Argument must have a value", nameof(packageID));
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException("Argument must have a value", nameof(version));
            }

            return packageID + '@' + version;
        }

        public PackageSources Source { get; }

        public string PackageID { get; }

        public virtual string Name => PackageID;

        public string VersionSpec { get; }

        public Version Version { get; }

        public bool ForRemoval => VersionSpec == "exclude";

        public string PackageSpec
        {
            get
            {
                if (Source == PackageSources.UnityPackageManager)
                {
                    return FormatUnityPackageManagerSpec(PackageID, VersionSpec);
                }
                else
                {
                    return PackageID;
                }
            }
        }

        private static PackageSources InferSource(string packageSpec)
        {
            if (packageSpec is null)
            {
                throw new ArgumentNullException(nameof(packageSpec));
            }

            return packageSpec.IndexOf('@') >= 0
                ? PackageSources.UnityPackageManager
                : PackageSources.CompressedPackage;
        }

        public PackageReference(string packageSpec)
            : this(InferSource(packageSpec), packageSpec)
        { }

        protected PackageReference(PackageSources source, string packageSpec)
        {
            if (packageSpec is null)
            {
                throw new ArgumentNullException(nameof(packageSpec));
            }

            var parts = packageSpec.SplitX('@');
            if (parts.Length == 0)
            {
                throw new ArgumentException("Value must not be empty string", nameof(packageSpec));
            }
            else if (parts.Length == 1)
            {
                Source = source;
                PackageID = parts[0];
                VersionSpec = null;
            }
            else if (parts.Length == 2)
            {
                Source = source;
                PackageID = parts[0];
                VersionSpec = parts[1];
                var versionStr = VersionSpec;
                Version v = null;
                while (versionStr.Length > 0
                    && !Version.TryParse(versionStr, out v))
                {
                    versionStr = versionStr.Substring(0, versionStr.Length - 1);
                }

                Version = v;
            }
            else
            {
                throw new ArgumentException("Value can only contain one @ symbol", nameof(packageSpec));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PackageReference pkg && Equals(pkg);
        }

        public bool Equals(PackageReference other)
        {
            return other is object
                && (Source & other.Source) != 0
                && PackageID == other.PackageID
                && (VersionSpec == other.VersionSpec
                    || ForRemoval
                    || other.ForRemoval);
        }

        public static bool operator ==(PackageReference left, PackageReference right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(PackageReference left, PackageReference right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            var hashCode = 1447557622;
            hashCode = (hashCode * -1521134295) + Source.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(PackageID);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(VersionSpec);
            return hashCode;
        }

        public override string ToString()
        {
            if (VersionSpec is null)
            {
                return $"[{Source} {Name}";
            }
            else
            {
                return $"[{Source}] {Name} @ {VersionSpec}";
            }
        }

        public int CompareTo(PackageReference other)
        {
            if (other is null)
            {
                return 1;
            }
            else
            {
                var sourceCompare = Source.CompareTo(other.Source);
                var idCompare = string.Compare(PackageID, other.PackageID, StringComparison.OrdinalIgnoreCase);
                var specCompare = string.Compare(VersionSpec, other.VersionSpec, StringComparison.OrdinalIgnoreCase);
                var versionCompare = Version is null || other.Version is null
                    ? specCompare
                    : Version.CompareTo(other.Version);
                if (sourceCompare == 0
                    && idCompare == 0)
                {
                    return versionCompare;
                }
                else if (sourceCompare == 0)
                {
                    return idCompare;
                }
                else
                {
                    return sourceCompare;
                }
            }
        }

        public static bool operator <(PackageReference left, PackageReference right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(PackageReference left, PackageReference right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(PackageReference left, PackageReference right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(PackageReference left, PackageReference right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
