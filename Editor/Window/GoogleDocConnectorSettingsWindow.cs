using StansAssets.Foundation.Editor;
using StansAssets.Plugins.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace StansAssets.GoogleDoc
{
    class GoogleDocConnectorSettingsWindow : PackageSettingsWindow<GoogleDocConnectorSettingsWindow>
    {
        protected override PackageInfo GetPackageInfo()
            => PackageManagerUtility.GetPackageInfo(GoogleDocConnectorSettings.Instance.PackageName);

        protected override void OnWindowEnable(VisualElement root)
        {
            AddTab("Spreadsheets", new SpreadsheetsTab());
            AddTab("Localization", new LocalizationTab());
            AddTab("About", new AboutTab());
        }

        public static GUIContent WindowTitle => new GUIContent(GoogleDocConnectorPackage.DisplayName);
    }
}
