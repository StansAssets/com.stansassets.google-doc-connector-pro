using System.IO;
using System.Linq;
using System.Text;
using StansAssets.Foundation.UIElements;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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

        const string k_SampleSpreadsheetId = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SampleSpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_SampleRangeName = "Bike3";

        public SpreadsheetsTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/SpreadsheetsTab")
        {
            m_NoSpreadsheetsNote = this.Q("no-spreadsheets-note");
            m_NoCredentialsHelpBox = this.Q<HelpBox>("no-credentials");
            m_NoCredentials =  this.Q<VisualElement>("NoCredentials");
            m_SpreadsheetIdField = this.Q<TextField>("spreadsheetIdText");
            m_UploadCredentials = this.Q<Button>("upload-credentials");
            m_UploadCredentials.clicked += () =>
            {
                var path = EditorUtility.OpenFilePanel("Credentials", "", "json");
                if (File.Exists(path))  
                {
                    File.Copy(path, GoogleDocConnectorSettings.Instance.CredentialsPath, true);
                    CheckCredentials();
                }
            };
            m_SpreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;
            m_SpreadsheetIdField.tooltip = k_SpreadsheetIdTextPlaceholder;

            m_AddSpreadsheetBtn = this.Q<Button>("addSpreadsheetBtn");
            m_AddSpreadsheetBtn.clicked += () =>
            {
                var spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(m_SpreadsheetIdField.text);

                spreadsheet.LoadAsync(true).ContinueWith(_ => _);

                m_SpreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;

                RecreateSpreadsheetsView();
            };

            m_SpreadsheetsContainer = this.Q<VisualElement>("spreadsheets-container");
            RecreateSpreadsheetsView();

            CheckCredentials();
            //schedule.Execute(CheckCredentials).Every(100000);
        }

        void CheckCredentials()
        {
            GoogleDocConnectorEditor.CheckCredentials().ContinueWith((b) =>
            {
                NoCredentialsView(b.Result);
            });
        }

        /*void BindFoldoutPanel(string visualElementName, string foldoutName)
        {
            var visualElement = this.Q<VisualElement>(visualElementName);
            var foldout = this.Q<Foldout>(foldoutName);
            foldout.value = false;
            visualElement.style.display = DisplayStyle.None;

            foldout.RegisterValueChangedCallback(e =>
            {
                visualElement.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            foldout.schedule.Execute(() =>
            {
                visualElement.style.display = foldout.value ? DisplayStyle.Flex : DisplayStyle.None;
            }).StartingIn(1000);
        }*/
        

        void RecreateSpreadsheetsView()
        {
            m_SpreadsheetsContainer.Clear();

            m_SpreadsheetIdField.SetEnabled(true);
            m_AddSpreadsheetBtn.SetEnabled(true);

            m_NoCredentials.style.display = DisplayStyle.None;
            m_NoSpreadsheetsNote.style.display = GoogleDocConnectorSettings.Instance.Spreadsheets.Any()
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            foreach (var item in GoogleDocConnectorSettings.Instance.Spreadsheets.Select(spreadsheet => new SpreadsheetView(spreadsheet)))
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
                m_NoCredentials.style.display = DisplayStyle.Flex;
                m_NoSpreadsheetsNote.style.display = DisplayStyle.None;
                m_NoCredentialsHelpBox.Text = error;

                m_SpreadsheetIdField.SetEnabled(false);
                m_AddSpreadsheetBtn.SetEnabled(false);
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
