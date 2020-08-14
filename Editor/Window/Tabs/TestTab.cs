﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    class TestTab : BaseTab
    {
        const string k_SpreadsheetIdText = "Paste Spreadsheet Id here...";

        readonly ListView m_SpreadsheetsListView;

        const string k_SpreadsheetId = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_RangeName = "Bike3";

        public TestTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/TestTab")
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
            /*var range = sheet.GetNamedRangeCells(k_RangeName);
            Debug.Log(sheet.GetCell(3, 0));
            var builder = new StringBuilder($"NamedRange Id:{k_RangeName} Data:");
            foreach (var obj in range)
            {
                builder.Append(obj.Value.FormattedValue);
                builder.Append(",");
            }

            Debug.Log(builder);

           var cell = sheet.GetCell("A1");
            var cell1 = sheet.GetCell("b1");
            Debug.Log(cell.Value.FormattedValue);
            Debug.Log(cell.Value.GetValue<int>());
            Debug.Log(cell.Value.GetValue<double>());
            Debug.Log(cell.Value.GetValue<string>());
            Debug.Log(cell1.Value.FormattedValue);
            //Debug.Log(cell1.Value.GetValue<int>());
            Debug.Log(cell1.Value.GetValue<double>());
            Debug.Log(cell1.Value.GetValue<string>());
            var column = sheet.GetColumn("A");
            var column1 = sheet.GetColumn("B");
            Debug.Log(column.Count);
            Debug.Log(column1.Count);
            var list = sheet.GetRange("A1:B2");
            Debug.Log(list.Count);
            var list1 = sheet.GetRange("A:B");
            Debug.Log(list1.Count);
            var list2 = sheet.GetRange("1:2");
            Debug.Log(list2.Count);
            var list3 = sheet.GetRange("0:B2");
            Debug.Log(list3.Count);*/
        }

        void PopulateListView()
        {
            m_SpreadsheetsListView.hierarchy.Clear();
            foreach (var spreadsheet in GoogleDocConnectorSettings.Instance.Spreadsheets)
            {
                var item = new SpreadsheetView(spreadsheet);
                item.OnRemoveClick += OnSpreadsheetRemoveClick;
                item.OnRefreshClick += OnSpreadsheetRefreshClick;
                m_SpreadsheetsListView.hierarchy.Add(item);
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
                GoogleDocConnectorEditor.RemoveSpreadsheet(spreadsheet.Id);
                m_SpreadsheetsListView.Remove(sender);
                PopulateListView();
            }
        }

        static void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.Load();
        }
    }
}
