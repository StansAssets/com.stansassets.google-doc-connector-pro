﻿using StansAssets.Foundation.Editor;
using UnityEditor.PackageManager;

namespace StansAssets.GoogleDoc
{
    public static class GoogleDocConnectorPackage
    {
        public const string DisplayName = "Google Doc Connector";
        public static readonly string RootPath = PackageManagerUtility.GetPackageRootPath(GoogleDocConnectorSettings.Instance.PackageName);
        public static readonly PackageInfo Info = PackageManagerUtility.GetPackageInfo(GoogleDocConnectorSettings.Instance.PackageName);

        public static readonly string WindowTabsPath = $"{RootPath}/Editor/Window/Tabs";
        public static readonly string UILayoutPath = $"{RootPath}/Editor/UI";
    }
}
