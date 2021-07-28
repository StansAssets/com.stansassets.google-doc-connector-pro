using StansAssets.Plugins.Editor;
using UnityEditor;

namespace StansAssets.GoogleDoc.Editor
{
    static class GoogleDocConnectorEditorMenu
    {
        [MenuItem(PluginsDevKitPackage.RootMenu + "/" + GoogleDocConnectorPackage.DisplayName + "/Settings", false, 0)]
        public static void OpenSettingsTest()
        {
            var headerContent = GoogleDocConnectorSettingsWindow.WindowTitle;
            GoogleDocConnectorSettingsWindow.ShowTowardsInspector(headerContent.text, headerContent.image);
        }
    }
}
