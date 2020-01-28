namespace Juniper.ConfigurationManagement
{
    public class PackageRequirement :
        PackageReference
    {
        public PackageRequirement(string packageSpec)
            : base(packageSpec)
        { }

        public PackageRequirement(string packageID, string version)
            : this(FormatUnityPackageManagerSpec(packageID, version))
        { }
    }
}
