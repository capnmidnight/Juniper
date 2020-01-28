using System;

namespace Juniper.ConfigurationManagement
{
    [Flags]
    public enum PackageSources
    {
        None,

        Juniper = 0x01,
        UnityAssetStore = 0x02,
        UnityPackageManager = 0x04,

        CompressedPackage = Juniper | UnityAssetStore,
        Any = Juniper | UnityAssetStore | UnityPackageManager
    }
}
