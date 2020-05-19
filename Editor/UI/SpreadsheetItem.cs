using System;
using StansAssets.Foundation.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SpreadsheetItem : VisualElement
    {
        public event Action<SpreadsheetItem, Spreadsheet> OnRemoveClick = delegate { };
        public event Action<Spreadsheet> OnRefreshClick = delegate { };

        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly Label m_SpreadsheetDate;
        readonly Label m_SpreadsheetLastSyncMachineName;
        
        readonly VisualElement m_Spinner;

        readonly Foldout m_SheetsFoldout;
        

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

            m_SheetsFoldout = this.Q<Foldout>("sheetFoldout");
            
            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_Spinner.visible = (spreadsheet.State == Spreadsheet.SyncState.InProgress);
            
            var removeButton = this.Q<Button>("removeBtn");
            removeButton.clicked += () => { OnRemoveClick(this, spreadsheet); };
            
            var refreshButton = this.Q<Button>("refreshBtn");
            refreshButton.clicked += () => { OnRefreshClick(spreadsheet); };

            spreadsheet.OnSyncStateChange += StateChange;

            InitWithData(spreadsheet);
        }
        
        void StateChange(Spreadsheet spreadsheet)
        {
            m_Spinner.visible = (spreadsheet.State == Spreadsheet.SyncState.InProgress);
            if (spreadsheet.State == Spreadsheet.SyncState.Synced)
            {
                InitWithData(spreadsheet);
            }
        }

        void InitWithData(Spreadsheet spreadsheet)
        {
            m_SpreadsheetId.text = spreadsheet.Id;
            m_SpreadsheetName.text = spreadsheet.Name;
            m_SpreadsheetDate.text = spreadsheet.SyncDateTime.HasValue ? spreadsheet.SyncDateTime.Value.ToString("U") : "[Not Synced]";
            m_SpreadsheetLastSyncMachineName.text = spreadsheet.LastSyncMachineName;
            if (!string.IsNullOrEmpty(spreadsheet.LastSyncMachineName)) { m_SpreadsheetLastSyncMachineName.text += " |"; }
            
            m_SheetsFoldout.Clear();

            foreach (var sheet in spreadsheet.Sheets)
            {
                var item = new SheetItem(sheet);
                m_SheetsFoldout.Add(item);
                foreach (var range in sheet.NamedRanges)
                {
                    item.AddNamedRange(new NamedRangeView(range));
                }
            }
        }
    }
}
