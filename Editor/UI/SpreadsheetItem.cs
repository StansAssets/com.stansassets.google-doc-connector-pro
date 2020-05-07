using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SpreadsheetItem : VisualElement
    {
        public event Action<SpreadsheetItem, Spreadsheet> OnRemoveClick = delegate { };

        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly Label m_SpreadsheetDate;
        readonly Label m_SpreadsheetLastSyncMachineName;

        readonly Foldout m_SheetsContainer;
        

        public SpreadsheetItem(Spreadsheet spreadsheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SpreadsheetLayout.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("spreadsheetId");
            m_SpreadsheetName = this.Q<Label>("spreadsheetName");
            m_SpreadsheetDate = this.Q<Label>("spreadsheetDate");
            m_SpreadsheetLastSyncMachineName = this.Q<Label>("lastSyncMachineName");

            m_SheetsContainer = this.Q<Foldout>("sheetList");
            
            var removeButton = this.Q<Button>("removeBtn");
            removeButton.clicked += () => { OnRemoveClick(this, spreadsheet); };

            InitWithData(spreadsheet);
        }

        void InitWithData(Spreadsheet spreadsheet)
        {
            m_SpreadsheetId.text = spreadsheet.Id;
            m_SpreadsheetName.text = spreadsheet.Name;
            m_SpreadsheetDate.text = spreadsheet.SyncDateTime.HasValue ? spreadsheet.SyncDateTime.Value.ToString("U") : "[Not Synced]";
            m_SpreadsheetLastSyncMachineName.text = spreadsheet.LastSyncMachineName;
            if (!string.IsNullOrEmpty(spreadsheet.LastSyncMachineName)) { m_SpreadsheetLastSyncMachineName.text += " |"; }

            foreach (var sheet in spreadsheet.Sheets)
            {
                var item = new SheetItem(sheet);
                m_SheetsContainer.Add(item);
                foreach (var range in sheet.NamedRanges)
                {
                    item.AddNamedRange(new NamedRangeItem(range));
                }
            }
        }
    }
}
