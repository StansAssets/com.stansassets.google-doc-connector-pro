using System.IO;
using System.Linq;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEngine.UIElements;
using HelpBox = StansAssets.Foundation.UIElements.HelpBox;

namespace StansAssets.GoogleDoc
{
    class SpreadsheetsTab : BaseTab
    {
        const string k_SpreadsheetIdTextPlaceholder = "Paste Spreadsheet Id here...";

        readonly VisualElement m_SpreadsheetsContainer;
        VisualElement m_NoSpreadsheetsNote;

        VisualElement m_NoCredentials;
        Button m_UploadCredentials;
        HelpBox m_NoCredentialsHelpBox;

        TextField m_SpreadsheetIdField;
        Button m_AddSpreadsheetBtn;
        
        public SpreadsheetsTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/SpreadsheetsTab")
        {
            m_NoSpreadsheetsNote = this.Q("no-spreadsheets-note");
            m_NoCredentialsHelpBox = this.Q<HelpBox>("no-credentials-HelpBox");
            m_NoCredentials =  this.Q<VisualElement>("NoCredentials");
            m_SpreadsheetIdField = this.Q<TextField>("spreadsheetIdText");
            m_UploadCredentials = this.Q<Button>("upload-credentials");
            m_UploadCredentials.clicked += () =>
            {
                var path = EditorUtility.OpenFilePanel("Credentials", "", "json");
                if (File.Exists(path))  
                {
                    File.Copy(path, GoogleDocConnectorSettings.Instance.CredentialsPath, true);
                    NoCredentialsView("");
                    CheckCredentials();
                }
            };
            m_SpreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;
            m_SpreadsheetIdField.tooltip = k_SpreadsheetIdTextPlaceholder;

            m_AddSpreadsheetBtn = this.Q<Button>("addSpreadsheetBtn");
            m_AddSpreadsheetBtn.clicked += () =>
            {
                var spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(m_SpreadsheetIdField.text);

                spreadsheet.LoadAsync(true).ContinueWith(_ =>_);
                

                m_SpreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;

                RecreateSpreadsheetsView();
            };

            m_SpreadsheetsContainer = this.Q<VisualElement>("spreadsheets-container");
            RecreateSpreadsheetsView();

            CheckCredentials();
        }

        void CheckCredentials()
        {
            GoogleDocConnectorEditor.CheckCredentials().ContinueWith((b) =>
            {
                NoCredentialsView(b.Result);
            });
        }


        void RecreateSpreadsheetsView()
        {
            m_SpreadsheetsContainer.Clear();

            m_SpreadsheetIdField.SetEnabled(true);
            m_AddSpreadsheetBtn.SetEnabled(true);

            m_NoCredentials.style.display = DisplayStyle.None;
            m_NoSpreadsheetsNote.style.display = GoogleDocConnectorSettings.Instance.Spreadsheets.Any()
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            foreach (var item in GoogleDocConnectorSettings.Instance.Spreadsheets.Select(spreadsheet => new SpreadsheetView(GoogleDocConnector.GetSpreadsheet(spreadsheet.Id))))
            {
                item.OnRemoveClick += OnSpreadsheetRemoveClick;
                item.OnRefreshClick += OnSpreadsheetRefreshClick;
                item.SyncedWithErrorEvent += NoCredentialsView;
                m_SpreadsheetsContainer.Add(item);
            }
        }

        void OnSpreadsheetRemoveClick(SpreadsheetView sender, Spreadsheet spreadsheet)
        {
            var dialog = EditorUtility.DisplayDialog("Confirm",
                $"Are you sure want to remove '{spreadsheet.Name}' spreadsheet?",
                "Yes",
                "No");
            if (dialog)
            {
                m_SpreadsheetsContainer.Remove(sender);
                GoogleDocConnectorEditor.RemoveSpreadsheet(spreadsheet.Id);
                RecreateSpreadsheetsView();
            }
        }

        static void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.LoadAsync(true).ContinueWith(_ => _);
        }

        public void NoCredentialsView(string error)
        {
            if (error != string.Empty)
            {
                m_SpreadsheetIdField.SetEnabled(false);
                m_AddSpreadsheetBtn.SetEnabled(false);
                m_NoCredentials.style.display = DisplayStyle.Flex;
                m_NoSpreadsheetsNote.style.display = DisplayStyle.None;
                m_NoCredentialsHelpBox.Text = error;
            }
            else
            {
                m_SpreadsheetIdField.SetEnabled(true);
                m_AddSpreadsheetBtn.SetEnabled(true);
                m_NoSpreadsheetsNote.style.display = GoogleDocConnectorSettings.Instance.Spreadsheets.Any()
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
                m_NoCredentials.style.display = DisplayStyle.None;
            }
        }
    }
}
