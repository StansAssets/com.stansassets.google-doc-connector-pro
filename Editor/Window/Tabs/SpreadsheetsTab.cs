using System;
using System.Linq;
using System.Text;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    class SpreadsheetsTab : BaseTab
    {
        const string k_SpreadsheetIdTextPlaceholder = "Paste Spreadsheet Id here...";

        readonly VisualElement m_SpreadsheetsContainer;
        VisualElement m_NoSpreadsheetsNote;

        const string k_SpreadsheetId = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_RangeName = "Bike3";

        public SpreadsheetsTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/SpreadsheetsTab")
        {
            var connectBtn = this.Q<Button>("loadExampleConfigBtn");
            connectBtn.clicked += () =>
            {
                var spreadsheet1 = GoogleDocConnector.GetSpreadsheet("1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo");
                spreadsheet1 ??= GoogleDocConnectorEditor.CreateSpreadsheet("1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo");
                //spreadsheet1.CreateGoogleSheet("Sheet4");
                spreadsheet1.UpdateGoogleCell("Sheet4!A1", "123");
                spreadsheet1.UpdateGoogleCell("Sheet4!A2", "456");
                spreadsheet1.UpdateGoogleCell("Sheet4!A3", "789");
                spreadsheet1.DeleteGoogleCell("Sheet4!A1");
                var sheet1 = spreadsheet1.GetSheetOrCreate("Лист1");
                sheet1.UpdateCell(0, 0, new CellValue("vasa", "", "vasa"));
                sheet1.UpdateCell(1, 0, new CellValue("vasa", "", "vasa"));
                sheet1.UpdateCell(0, 1, new CellValue("vasa", "", "vasa"));
                spreadsheet1.Save();
                /*spreadsheet1.OnSyncStateChange += OnSheetStateChanged;
               spreadsheet1.Load();

              var spreadsheet = GoogleDocConnector.GetSpreadsheet(k_SpreadsheetId2);
               spreadsheet ??= GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId2);
               spreadsheet.Load();*/

                RecreateSpreadsheetsView();
            };

            //  connectBtn.style.display = DisplayStyle.None;

            m_NoSpreadsheetsNote = this.Q("no-spreadsheets-note");
            var spreadsheetIdField = this.Q<TextField>("spreadsheetIdText");
            spreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;
            spreadsheetIdField.tooltip = k_SpreadsheetIdTextPlaceholder;

            var addSpreadsheetBtn = this.Q<Button>("addSpreadsheetBtn");
            addSpreadsheetBtn.clicked += () =>
            {
                var spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(spreadsheetIdField.text);
                spreadsheet.Load();
                spreadsheetIdField.value = k_SpreadsheetIdTextPlaceholder;
                RecreateSpreadsheetsView();
            };

            m_SpreadsheetsContainer = this.Q<VisualElement>("spreadsheets-container");
            RecreateSpreadsheetsView();
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
                builder.Append(obj.Value.FormattedValue ?? String.Empty);
                builder.Append(",");
            }

            Debug.Log(builder);
        }

        void RecreateSpreadsheetsView()
        {
            m_NoSpreadsheetsNote.style.display = GoogleDocConnectorSettings.Instance.Spreadsheets.Any()
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            m_SpreadsheetsContainer.Clear();
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
    }
}
