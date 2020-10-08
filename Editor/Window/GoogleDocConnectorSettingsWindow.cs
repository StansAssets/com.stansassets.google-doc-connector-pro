using StansAssets.Foundation.Editor;
using StansAssets.Foundation.UIElements;
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
            var documentationTab = new DocumentationTab();
            documentationTab = CreateDocumentationList(documentationTab);
            documentationTab = CreateSampleList(documentationTab);

            AddTab("Spreadsheets", new SpreadsheetsTab());
            AddTab("Localization", new LocalizationTab());
            AddTab("Documentation", documentationTab);
            AddTab("About", new AboutTab());
        }

        public static GUIContent WindowTitle => new GUIContent(GoogleDocConnectorPackage.DisplayName, GoogleDocConnectorPackage.Image);
        
        DocumentationTab CreateDocumentationList(DocumentationTab tab)
        {
            tab.AddDocumentationUrl("Credentials", "https://github.com/StansAssets/com.stansassets.google-doc-connector-pro/wiki/Setup");
            tab.AddDocumentationUrl("Wiki", "https://github.com/StansAssets/com.stansassets.google-doc-connector-pro/wiki");
            tab.AddDocumentationUrl("Documentation", "https://api.stansassets.com/google-doc/StansAssets.GoogleDoc.html");
            return tab;
        }
        
        DocumentationTab CreateSampleList(DocumentationTab tab)
        {
            var label = new Label() {text = "Please add this Id to Google Doc Connector for use Samples."};
            label.AddToClassList("id-sample-text");
            tab.AddToSampleTopPanel(label);
            var ve = new VisualElement();
            ve.AddToClassList("id-sample-panel");
            label = new Label() {text = "Id:"};
            label.AddToClassList("id-sample-text");
            var selectableLabel = new SelectableLabel() {text = "1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo"};
            label.AddToClassList("id-sample");
            ve.Add(label);
            ve.Add(selectableLabel);
            tab.AddToSampleTopPanel(ve);

            tab.AddSampleUrl("Receiving data Sample", GoogleDocConnectorPackage.SamplesPath + "/GoogleDocSampleScene.unity");
            tab.AddSampleUrl("Localization Sample", GoogleDocConnectorPackage.SamplesPath + "/GoogleDocSampleLocalization.unity");

            return tab;
        }
    }
}