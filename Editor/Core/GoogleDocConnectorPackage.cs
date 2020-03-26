using StansAssets.Foundation.Editor;
using UnityEditor.PackageManager;

namespace StansAssets.GoogleDoc
{
    public static class GoogleDocConnectorPackage
    {
        public static readonly string RootPath = PackageManagerUtility.GetPackageRootPath(GoogleDocConnectorSettings.Instance.PackageName);
        public static readonly PackageInfo Info = PackageManagerUtility.GetPackageInfo(GoogleDocConnectorSettings.Instance.PackageName);

        public static readonly string WindowTabsPath = $"{RootPath}/Editor/Window/Tabs";
    }
}
