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
        readonly VisualElement m_SheetFoldoutLabelPanel;
        readonly ScrollView m_SheetFoldoutScrollView;

        readonly VisualElement m_SheetsContainer;

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
            m_SpreadsheetStatusIcon = this.Q<Label>("statusIcon");
            
            m_SheetFoldoutLabelPanel = this.Q<VisualElement>("sheetFoldoutLabelPanel");
            m_SheetFoldoutScrollView = this.Q<ScrollView>("sheetFoldoutScrollView");
            
            var spreadsheetExpandedPanel = this.Q<VisualElement>("spreadsheetExpandedPanel");

            m_SheetsContainer = this.Q<VisualElement>("sheetContainer");

            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            
            var arrowToggleFoldout = this.Q<Foldout>("arrowToggleFoldout");
            arrowToggleFoldout.value = spreadsheet.SpreadsheetFoldOutUIState;
            arrowToggleFoldout.RegisterValueChangedCallback( ev =>  ToggleElementDisplayState(ev.newValue, spreadsheetExpandedPanel, out spreadsheet.SpreadsheetFoldOutUIState));
            spreadsheetExpandedPanel.style.display = arrowToggleFoldout.value ? DisplayStyle.Flex : DisplayStyle.None;
            
            var sheetFoldout = this.Q<Foldout>("sheetFoldout");
            sheetFoldout.value = spreadsheet.SheetFoldOutUIState;
            sheetFoldout.RegisterValueChangedCallback(ev => ToggleElementDisplayState(ev.newValue, m_SheetsContainer, out spreadsheet.SheetFoldOutUIState));
            m_SheetsContainer.style.display = sheetFoldout.value ? DisplayStyle.Flex : DisplayStyle.None;

            var copyIdButton = this.Q<Button>("CopyIdBtn");
            copyIdButton.clicked += () => { OnCopyClick(spreadsheet.Id); };
            var copyUrlButton = this.Q<Button>("CopyURLBtn");
            copyUrlButton.clicked += () => { OnCopyClick(spreadsheet.Url); };

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
            
            var emptySheetList =  !spreadsheet.Sheets.Any();
            m_SheetFoldoutLabelPanel.style.display = emptySheetList ? DisplayStyle.Flex : DisplayStyle.None;
            m_SheetFoldoutScrollView.style.display = emptySheetList ? DisplayStyle.None : DisplayStyle.Flex;

            m_SheetsContainer.Clear();
            
            foreach (var sheet in spreadsheet.Sheets)
            {
                m_SheetsContainer.Add(new SheetView(sheet));
            }
        }

        void ToggleElementDisplayState(bool state, VisualElement element, out bool uiState)
        {
            element.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
            uiState = state;
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
        
        void OnCopyClick(string copyObject)
        {
            GUIUtility.systemCopyBuffer = copyObject;
        }
    }
}
