using System;

namespace Juniper.ConfigurationManagement
{
    [Flags]
    public enum PackageSources
    {
        None,

        JuniperZip = 0x01,
        UnityAssetStore = 0x02,
        UnityPackageManager = 0x04,

        CompressedPackage = JuniperZip | UnityAssetStore,
        Any = JuniperZip | UnityAssetStore | UnityPackageManager
    }
}
