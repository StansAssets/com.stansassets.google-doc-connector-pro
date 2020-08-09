using StansAssets.Foundation.Editor;
using StansAssets.Plugins.Editor;
using UnityEditor;
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
            AddTab("TEST", new TestTab());
            AddTab("About", new AboutTab());
        }

        [MenuItem(PackagesConfigEditor.RootMenu + "/" + GoogleDocConnectorPackage.DisplayName + "/Settings", false, 0)]
        public static void OpenSettingsTest()
        {
            ShowTowardsInspector(WindowTitle.text, WindowTitle.image);
        }

        static GUIContent WindowTitle => new GUIContent(GoogleDocConnectorPackage.DisplayName);
    }
}
