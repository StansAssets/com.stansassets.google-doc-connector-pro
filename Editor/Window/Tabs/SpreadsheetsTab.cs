#if UNITY_2019_4_OR_NEWER || UNITY_2020_2_OR_NEWER
using System;
using System.Linq;
using System.Text;
using StansAssets.Foundation.UIElements;
using StansAssets.Plugins.Editor;
using UnityEditor;
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
        HelpBox m_NoCredentialsHelpBox;
        
        TextField m_SpreadsheetIdField;
        Button m_AddSpreadsheetBtn;

        const string k_SpreadsheetId = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_RangeName = "Bike3";

        public SpreadsheetsTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/SpreadsheetsTab")
        {
            var connectBtn = this.Q<Button>("loadExampleConfigBtn");
            connectBtn.clicked += () =>
            {
                /*
                var spreadsheet1 = GoogleDocConnector.GetSpreadsheet(k_SpreadsheetId);
                spreadsheet1 ??= GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId);
                spreadsheet1.OnSyncStateChange += OnSheetStateChanged;
                spreadsheet1.Load();

                var spreadsheet = GoogleDocConnector.GetSpreadsheet(k_SpreadsheetId2);
                spreadsheet ??= GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId2);
                spreadsheet.Load();*/

                RecreateSpreadsheetsView();
            };

            //  connectBtn.style.display = DisplayStyle.None;

            m_NoSpreadsheetsNote = this.Q("no-spreadsheets-note");
            m_NoCredentialsHelpBox = this.Q<HelpBox>("no-credentials");
            m_NoCredentials = this.Q("NoCredentials");
            m_SpreadsheetIdField = this.Q<TextField>("spreadsheetIdText");
            m_SpreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;
            m_SpreadsheetIdField.tooltip = k_SpreadsheetIdTextPlaceholder;

            m_AddSpreadsheetBtn = this.Q<Button>("addSpreadsheetBtn");
            m_AddSpreadsheetBtn.clicked += () =>
            {
                var spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(m_SpreadsheetIdField.text);
                spreadsheet.Load();
                m_SpreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;
                RecreateSpreadsheetsView();
            };

            m_SpreadsheetsContainer = this.Q<VisualElement>("spreadsheets-container");
            RecreateSpreadsheetsView();

            //Bind Documentation Panel
            var docExpandedPanel = this.Q<VisualElement>("DocExpandedPanel");
            CreateDocumentationList();
            var docFoldout = this.Q<Foldout>("DocArrowToggleFoldout");

            docFoldout.RegisterValueChangedCallback(e =>
            {
                docExpandedPanel.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            docExpandedPanel.style.display = docFoldout.value ? DisplayStyle.Flex : DisplayStyle.None;

            //Bind Samples Panel
            var sampleExpandedPanel = this.Q<VisualElement>("SampleExpandedPanel");
            var sampleFoldout = this.Q<Foldout>("SampleArrowToggleFoldout");

            sampleFoldout.RegisterValueChangedCallback(e =>
            {
                sampleExpandedPanel.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            sampleExpandedPanel.style.display = docFoldout.value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        static void OnSheetStateChanged(Spreadsheet spreadsheet)
        {
            if (spreadsheet.State != Spreadsheet.SyncState.Synced)
            {
                return;
            }

            var sheet = spreadsheet.GetSheet(0);
            var range = sheet.GetNamedRangeCells(k_RangeName);
            Debug.Log(sheet.GetCell(3, 0).Value.FormattedValue);
            var builder = new StringBuilder($"NamedRange Id:{k_RangeName} Data:");
            foreach (var obj in range)
            {
                builder.Append(obj.Value.FormattedValue ?? string.Empty);
                builder.Append(",");
            }

            Debug.Log(builder);
        }

        void RecreateSpreadsheetsView()
        {
            m_SpreadsheetsContainer.Clear();
            
            if (!GoogleDocConnectorSettings.Instance.CheckCredentials(out var errorMassage))
            {
                m_NoSpreadsheetsNote.style.display = DisplayStyle.None;
                m_NoCredentials.style.display = DisplayStyle.Flex;
                m_NoCredentialsHelpBox.Text = errorMassage;
                
                m_SpreadsheetIdField.SetEnabled(false);
                m_AddSpreadsheetBtn.SetEnabled(false);
                return;
            }
            
            m_SpreadsheetIdField.SetEnabled(true);
            m_AddSpreadsheetBtn.SetEnabled(true);
            
            m_NoCredentials.style.display = DisplayStyle.None;
            m_NoSpreadsheetsNote.style.display = GoogleDocConnectorSettings.Instance.Spreadsheets.Any()
                ? DisplayStyle.None
                : DisplayStyle.Flex;
            
            foreach (var spreadsheet in GoogleDocConnectorSettings.Instance.Spreadsheets)
            {
                var item = new SpreadsheetView(spreadsheet);
                item.OnRemoveClick += OnSpreadsheetRemoveClick;
                item.OnRefreshClick += OnSpreadsheetRefreshClick;
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
            spreadsheet.Load();
        }

        void CreateDocumentationList()
        {
            var docExpandedPanel = this.Q<VisualElement>("DocItemsPanel");
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
            docExpandedPanel.Add(DocumentationItem("Credentials", "https://developers.google.com/sheets/api/quickstart/dotnet"));
        }

        VisualElement DocumentationItem(string nameItem, string link)
        {
            var label = new Label { text = $"➠ {nameItem}" };
            var hyperlink = new Hyperlink { Link = link };
            hyperlink.Add(label);
            var item = new VisualElement();
            item.AddToClassList("doc-item");
            item.Add(hyperlink);
            return item;
        }
    }
}
#endif
