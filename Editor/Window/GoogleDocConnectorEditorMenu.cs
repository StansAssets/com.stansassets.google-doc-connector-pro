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
