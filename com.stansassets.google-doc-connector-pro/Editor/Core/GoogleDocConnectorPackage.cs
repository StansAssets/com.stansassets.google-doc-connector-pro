using StansAssets.Foundation.Editor;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace StansAssets.GoogleDoc.Editor
{
    /// <summary>
    /// Google Doc Connector Package Static info.
    /// </summary>
    public static class GoogleDocConnectorPackage
    {
        /// <summary>
        /// The package name
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

        internal static readonly string UILayoutPath = $"{RootPath}/Editor/Window/UI";
        internal static readonly string WindowTabsPath = $"{RootPath}/Editor/Window/Tabs";
        internal static readonly string UILocalizationPath = $"{RootPath}/Editor/Localization/UIE";
        internal static readonly string SamplesPath = $"{RootPath}/Samples/Scene";
        internal static readonly string IconsPath = $"{RootPath}/Editor/Art/Icons";

        internal static readonly string LocalizationTabPath = $"{WindowTabsPath}/LocalizationTab";

        internal static Texture2D Image => EditorGUIUtility.isProSkin
            ? EditorAssets.GetImage($"{IconsPath}/logo-dark.png")
            :  EditorAssets.GetImage($"{IconsPath}/logo-light.png");
    }
}
