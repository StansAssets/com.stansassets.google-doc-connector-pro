#if UNITY_2019_4_OR_NEWER || UNITY_2020_2_OR_NEWER
using StansAssets.Plugins.Editor;
using UnityEditor;

namespace StansAssets.GoogleDoc
{
    static class GoogleDocConnectorEditorMenu
    {
        [MenuItem(PackagesConfigEditor.RootMenu + "/" + GoogleDocConnectorPackage.DisplayName + "/Settings", false, 0)]
        public static void OpenSettingsTest()
        {
            var headerContent = GoogleDocConnectorSettingsWindow.WindowTitle;
            GoogleDocConnectorSettingsWindow.ShowTowardsInspector(headerContent.text, headerContent.image);
        }
    }
}
#endif
