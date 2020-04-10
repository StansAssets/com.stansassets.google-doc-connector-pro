using StansAssets.Foundation.Editor;
using StansAssets.Foundation.Editor.Plugins;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace StansAssets.GoogleDoc
{
    public class GoogleDocConnectorSettingsWindow : PackageSettingsWindow<GoogleDocConnectorSettingsWindow>
    {
        protected override PackageInfo GetPackageInfo()
            => PackageManagerUtility.GetPackageInfo(GoogleDocConnectorSettings.Instance.PackageName);

        protected override void OnWindowEnable(VisualElement root)
        {
            AddTab("TEST", new TestTab());
        }

        [MenuItem("Stan's Assets/Google Doc Connector/Settings", false, 0)]
        public static void OpenSettingsTest() {
            var window = ShowTowardsInspector(WindowTitle.text, WindowTitle.image);
        }

        static GUIContent WindowTitle => new GUIContent("Google Doc Connector Pro");
    }
}
