using System.Collections.Generic;
using System.Text;
using StansAssets.Foundation.Editor.Plugins;
using UnityEngine;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class TestTab : BaseTab
    {
        const string k_SpreadsheetIdText = "Paste Spreadsheet Id here...";

        readonly ListView m_SpreadsheetsListView;

        const string k_SpreadsheetId = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_RangeName = "Bike3";

        public TestTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/TestTab.uxml")
        {
            var connectBtn = this.Q<Button>("loadExampleConfigBtn");
            connectBtn.clicked += () =>
            {
                var spreadsheet1 = GoogleDocConnector.GetSpreadsheet(k_SpreadsheetId);
                spreadsheet1 = spreadsheet1 ?? GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId);
                spreadsheet1.OnSyncStateChange += OnSheetStateChanged;
                spreadsheet1.Load();

                var spreadsheet = GoogleDocConnector.GetSpreadsheet(k_SpreadsheetId2);
                spreadsheet = spreadsheet ?? GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId2);
                spreadsheet.Load();

                PopulateListView();
            };

            var spreadsheetIdField = this.Q<TextField>("spreadsheetIdText");
            spreadsheetIdField.value = k_SpreadsheetIdText;

            var addSpreadsheetBtn = this.Q<Button>("addSpreadsheetBtn");
            addSpreadsheetBtn.clicked += () =>
            {
                var spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(spreadsheetIdField.text);
                spreadsheet.Load();
                spreadsheetIdField.value = k_SpreadsheetIdText;
                PopulateListView();
            };

            m_SpreadsheetsListView = this.Q<ListView>("spreadsheetsContainer");
            PopulateListView();
        }

        static void OnSheetStateChanged(Spreadsheet spreadsheet)
        {
            if (spreadsheet.State != Spreadsheet.SyncState.Synced)
            {
                return;
            }
            
            var sheet = spreadsheet.GetSheet(0);
            Debug.Log(sheet.GetCell(3, 0));
            List<object> range = sheet.GetRange(k_RangeName);
            var builder = new StringBuilder($"NamedRange Id:{k_RangeName} Data:");
            foreach (var obj in range)
            {
                builder.Append(obj);
                builder.Append(",");
            }
            Debug.Log(builder);
        } 

        void PopulateListView()
        {
            m_SpreadsheetsListView.Clear();
            foreach (var spreadsheet in GoogleDocConnectorSettings.Instance.Spreadsheets)
            {
                var item = new SpreadsheetItem(spreadsheet);
                item.OnRemoveClick += OnSpreadsheetRemoveClick;
                item.OnRefreshClick += OnSpreadsheetRefreshClick;
                m_SpreadsheetsListView.Add(item);
            }
        }

        void OnSpreadsheetRemoveClick(SpreadsheetItem sender, Spreadsheet spreadsheet)
        {
            GoogleDocConnectorEditor.RemoveSpreadsheet(spreadsheet.Id);
            m_SpreadsheetsListView.Remove(sender);
        }
        
        void OnSpreadsheetRefreshClick(SpreadsheetItem sender, Spreadsheet spreadsheet)
        {
            spreadsheet.Load();
        }
    }
}
