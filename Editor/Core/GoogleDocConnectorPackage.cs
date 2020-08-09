using StansAssets.Foundation.Editor;
using UnityEditor.PackageManager;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Google Doc Connector Package Static info.
    /// </summary>
    public static class GoogleDocConnectorPackage
    {
        /// <summary>
        /// The na
        /// </summary>
        public const string DisplayName = "Google Doc Connector";
        
        /// <summary>
        /// Foundation package root path.
        /// </summary>
        public static readonly string RootPath = PackageManagerUtility.GetPackageRootPath(GoogleDocConnectorSettings.Instance.PackageName);
            
        /// <summary>
        /// Google Doc Connector package info.
        /// </summary>
        public static readonly PackageInfo Info = PackageManagerUtility.GetPackageInfo(GoogleDocConnectorSettings.Instance.PackageName);

        
        internal static readonly string WindowTabsPath = $"{RootPath}/Editor/Window/Tabs";
        internal static readonly string UILayoutPath = $"{RootPath}/Editor/UI";
    }
}
