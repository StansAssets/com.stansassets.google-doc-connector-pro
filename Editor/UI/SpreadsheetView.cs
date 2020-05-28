using System;
using System.Globalization;
using System.Linq;
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
        readonly Label m_SpreadsheetStatusIcon;

        readonly VisualElement m_Spinner;
        readonly VisualElement m_SpreadsheetExpandedPanel;
        readonly VisualElement m_SheetFoldoutLabelPanel;
        readonly ScrollView m_SheetFoldoutScrollView;

        readonly VisualElement m_SheetsContainer;

        bool m_ExpandedPanel = false;
        bool m_ExpandedSheets = true;

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
            m_SpreadsheetStatusIcon = this.Q<Label>("StatusIcon");
            
            m_SheetFoldoutLabelPanel = this.Q<VisualElement>("sheetFoldoutLabelPanel");
            m_SheetFoldoutScrollView = this.Q<ScrollView>("sheetFoldoutScrollView");
            
            m_SpreadsheetExpandedPanel = this.Q<VisualElement>("spreadsheetExpandedPanel");
            m_SpreadsheetExpandedPanel.style.display = m_ExpandedPanel ? DisplayStyle.Flex : DisplayStyle.None;

            m_SheetsContainer = this.Q<VisualElement>("sheetContainer");
            m_SheetsContainer.style.display = m_ExpandedSheets ? DisplayStyle.Flex : DisplayStyle.None;

            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            
            var arrowToggleButton = this.Q<Button>("ArrowToggleBtn");
            arrowToggleButton.clicked += () => { ExpandingPanelChange(arrowToggleButton, m_SpreadsheetExpandedPanel, ref m_ExpandedPanel); };
            
            var sheetArrowToggleButton = this.Q<Button>("SheetArrowToggleBtn");
            sheetArrowToggleButton.clicked += () => { ExpandingPanelChange(sheetArrowToggleButton, m_SheetsContainer, ref m_ExpandedSheets); };

            var copyIdButton = this.Q<Button>("CopyIdBtn");
            copyIdButton.clicked += () => { OnCopyIdClick(spreadsheet); };

            var removeButton = this.Q<Button>("removeBtn");
            removeButton.clicked += () => { OnRemoveClick(this, spreadsheet); };

            var refreshButton = this.Q<Button>("refreshBtn");
            refreshButton.clicked += () => { OnRefreshClick(spreadsheet); };

            spreadsheet.OnSyncStateChange += StateChange;

            InitWithData(spreadsheet);
        }
      
        void InitWithData(Spreadsheet spreadsheet)
        {
            m_SpreadsheetId.text = spreadsheet.Id;
            m_SpreadsheetName.text = spreadsheet.Name;
            m_SpreadsheetDate.text = spreadsheet.SyncDateTime.HasValue ? spreadsheet.SyncDateTime.Value.ToString("dddd, MMMM d, yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) : $"[{Spreadsheet.NotSyncedStringStatus}]";
            if (!string.IsNullOrEmpty(spreadsheet.LastSyncMachineName)) { m_SpreadsheetLastSyncMachineName.text = $"| {spreadsheet.LastSyncMachineName}"; }

            //Synced With Error
            m_SpreadsheetErrorMessage.text = spreadsheet.SyncErrorMassage;
            m_SpreadsheetErrorMessage.style.display = spreadsheet.SyncedWithError ? DisplayStyle.Flex : DisplayStyle.None;
            if (spreadsheet.SyncedWithError)
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-red");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.SyncedWithErrorStringStatus;
            } 
            else if (spreadsheet.Synced)
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-green");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.SyncedStringStatus;
            }
            
            m_SheetFoldoutLabelPanel.style.display = (spreadsheet.Sheets.Count < 1) ? DisplayStyle.Flex : DisplayStyle.None;
            m_SheetFoldoutScrollView.style.display = (spreadsheet.Sheets.Count < 1) ? DisplayStyle.None : DisplayStyle.Flex;

            m_SheetsContainer.Clear();
            
            spreadsheet.Sheets.ForEach(sheet => m_SheetsContainer.Add(new SheetView(sheet)));
        }
        
        void ExpandingPanelChange(Button btn, VisualElement element, ref bool state)
        {
            state = !state;
            if (state)
            {
                btn.text = "▼";
                element.style.display = DisplayStyle.Flex;
            }
            else
            {
                btn.text = "►";
                element.style.display = DisplayStyle.None;
            }
        }

        void StateChange(Spreadsheet spreadsheet)
        {
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            if (spreadsheet.Synced || spreadsheet.SyncedWithError)
            {
                InitWithData(spreadsheet);
            }
            else
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-yellow");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.NotSyncedStringStatus;
            }
        }
        
        void OnCopyIdClick(Spreadsheet spreadsheet)
        {
            var te = new TextEditor()
            {
                text = spreadsheet.Id
            };
                
            te.SelectAll();
            te.Copy();
        }
    }
}
