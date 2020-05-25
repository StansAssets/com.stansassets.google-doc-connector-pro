using System;
using StansAssets.Foundation.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SpreadsheetView : VisualElement
    {
        public event Action<SpreadsheetView, Spreadsheet> OnRemoveClick = delegate { };
        public event Action<Spreadsheet> OnRefreshClick = delegate { };

        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly Label m_SpreadsheetErrorMessage;
        readonly Label m_SpreadsheetDate;
        readonly Label m_SpreadsheetLastSyncMachineName;

        readonly VisualElement m_Spinner;
        readonly VisualElement m_SpreadsheetExpandedPanel;

        readonly Foldout m_SheetsFoldout;

        bool expandedPanel = false;

        public SpreadsheetView(Spreadsheet spreadsheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SpreadsheetView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("spreadsheetId");
            m_SpreadsheetName = this.Q<Label>("spreadsheetName");
            m_SpreadsheetErrorMessage = this.Q<Label>("spreadsheetError");
            m_SpreadsheetDate = this.Q<Label>("spreadsheetDate");
            m_SpreadsheetLastSyncMachineName = this.Q<Label>("lastSyncMachineName");
            
            m_SpreadsheetExpandedPanel = this.Q<VisualElement>("spreadsheetExpandedPanel");
            m_SpreadsheetExpandedPanel.style.display = expandedPanel ? DisplayStyle.Flex : DisplayStyle.None;

            m_SheetsFoldout = this.Q<Foldout>("sheetFoldout");

            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            
            var arrowToggleButton = this.Q<Button>("ArrowToggleBtn");
            arrowToggleButton.clicked += () => { ExpandingPanel(arrowToggleButton); };
            
            var copyIdButton = this.Q<Button>("CopyIdBtn");
            copyIdButton.clicked += () =>
            {
                TextEditor te = new TextEditor();
                te.text = spreadsheet.Id;
                te.SelectAll();
                te.Copy();
            };

            var removeButton = this.Q<Button>("removeBtn");
            removeButton.clicked += () => { OnRemoveClick(this, spreadsheet); };

            var refreshButton = this.Q<Button>("refreshBtn");
            refreshButton.clicked += () => { OnRefreshClick(spreadsheet); };

            spreadsheet.OnSyncStateChange += StateChange;

            InitWithData(spreadsheet);
        }
        
        void ExpandingPanel(Button btn)
        {
            expandedPanel = !expandedPanel;
            if (expandedPanel)
            {
                btn.AddToClassList("spreadsheet-arrowToggle-down");
                m_SpreadsheetExpandedPanel.style.display = DisplayStyle.Flex;
            }
            else
            {
                btn.RemoveFromClassList("spreadsheet-arrowToggle-down"); 
                m_SpreadsheetExpandedPanel.style.display = DisplayStyle.None;
            }
        }

        void StateChange(Spreadsheet spreadsheet)
        {
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            if (spreadsheet.Synced || spreadsheet.SyncedWithError)
            {
                InitWithData(spreadsheet);
            }
        }

        void InitWithData(Spreadsheet spreadsheet)
        {
            m_SpreadsheetId.text = spreadsheet.Id;
            m_SpreadsheetName.text = spreadsheet.Name;
            m_SpreadsheetDate.text = spreadsheet.SyncDateTime.HasValue ? spreadsheet.SyncDateTime.Value.ToString("U") : Spreadsheet.NotSyncedStringStatus;
            m_SpreadsheetLastSyncMachineName.text = spreadsheet.LastSyncMachineName;
            if (!string.IsNullOrEmpty(spreadsheet.LastSyncMachineName)) { m_SpreadsheetLastSyncMachineName.text += " |"; }

            //Synced With Error
            m_SpreadsheetErrorMessage.text = spreadsheet.SyncErrorMassage;
            m_SpreadsheetErrorMessage.style.display = spreadsheet.SyncedWithError ? DisplayStyle.Flex : DisplayStyle.None;
            if (spreadsheet.SyncedWithError) { m_SpreadsheetDate.text += $" | {Spreadsheet.SyncedWithErrorStringStatus}"; }

            m_SheetsFoldout.Clear();

            foreach (var sheet in spreadsheet.Sheets)
            {
                var item = new SheetView(sheet);
                m_SheetsFoldout.Add(item);
                if (sheet.NamedRanges != null)
                {
                    foreach (var range in sheet.NamedRanges)
                    {
                        item.AddNamedRange(new NamedRangeView(range));
                    }
                }
            }
        }
    }
}
