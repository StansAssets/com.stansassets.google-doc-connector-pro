#if UNITY_2019_4_OR_NEWER || UNITY_2020_2_OR_NEWER
using System;
using System.Globalization;
using System.Linq;
using StansAssets.Foundation.UIElements;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using HelpBox = StansAssets.Foundation.UIElements.HelpBox;

namespace StansAssets.GoogleDoc
{
    class SpreadsheetView : BaseTab
    {
        public event Action<SpreadsheetView, Spreadsheet> OnRemoveClick = delegate { };
        public event Action<Spreadsheet> OnRefreshClick = delegate { };

        readonly Label m_SpreadsheetId;
        readonly HelpBox m_SpreadsheetErrorMessage;
        readonly Label m_SpreadsheetDate;
        readonly Label m_SpreadsheetLastSyncMachineName;
        readonly Label m_SpreadsheetStatusIcon;

        readonly Foldout m_SpreadsheetFoldout;

        readonly VisualElement m_Spinner;
        readonly VisualElement m_SheetFoldoutLabelPanel;
        readonly ScrollView m_SheetFoldoutScrollView;

        readonly VisualElement m_SheetsContainer;
        readonly VisualElement m_RangesContainer;

        public SpreadsheetView(Spreadsheet spreadsheet): 
            base($"{GoogleDocConnectorPackage.UILayoutPath}/SpreadsheetView")
        {
            /*var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SpreadsheetView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;*/

            m_SpreadsheetId = this.Q<SelectableLabel>("spreadsheet-id");
            m_SpreadsheetErrorMessage = this.Q<HelpBox>("spreadsheet-error");
            m_SpreadsheetErrorMessage.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Copy", (x) =>
                {
                    GUIUtility.systemCopyBuffer = m_SpreadsheetErrorMessage.Text;
                });
            }));
            m_SpreadsheetDate = this.Q<Label>("spreadsheetDate");
            m_SpreadsheetLastSyncMachineName = this.Q<Label>("lastSyncMachineName");
            m_SpreadsheetStatusIcon = this.Q<Label>("statusIcon");
            
            m_SheetFoldoutLabelPanel = this.Q<VisualElement>("sheetFoldoutLabelPanel");
            m_SheetFoldoutScrollView = this.Q<ScrollView>("sheetFoldoutScrollView");
            
            var spreadsheetExpandedPanel = this.Q<VisualElement>("spreadsheetExpandedPanel");

            m_SheetsContainer = this.Q<VisualElement>("sheets-container");
            m_RangesContainer = this.Q<VisualElement>("ranges-container");
           

            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            
            m_SpreadsheetFoldout = this.Q<Foldout>("arrowToggleFoldout");
            m_SpreadsheetFoldout.viewDataKey = $"spreadsheet_toggle_{spreadsheet.Id}";
            
            m_SpreadsheetFoldout.RegisterValueChangedCallback(e =>
            {
                spreadsheetExpandedPanel.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            spreadsheetExpandedPanel.style.display = m_SpreadsheetFoldout.value ? DisplayStyle.Flex : DisplayStyle.None;

            var removeButton = this.Q<Button>("removeBtn");
            removeButton.clicked += () => { OnRemoveClick(this, spreadsheet); };

            var refreshButton = this.Q<Button>("refreshBtn");
            refreshButton.clicked += () => { OnRefreshClick(spreadsheet); };
            
            var openBtn = this.Q<Button>("openBtn");
            openBtn.clicked += () => { Application.OpenURL(GoogleDocConnector.GetSpreadsheetWebUrl(spreadsheet.Id)); };
            
            spreadsheet.OnSyncStateChange += StateChange;

            Bind(spreadsheet);
        }
      
        void Bind(Spreadsheet spreadsheet)
        {
            m_SpreadsheetId.text = spreadsheet.Id;
            m_SpreadsheetFoldout.text = spreadsheet.Name;
            m_SpreadsheetDate.text = spreadsheet.SyncDateTime.HasValue ? spreadsheet.SyncDateTime.Value.ToString("dddd, MMMM d, yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) : $"[{Spreadsheet.NotSyncedStringStatus}]";
            if (!string.IsNullOrEmpty(spreadsheet.LastSyncMachineName)) { m_SpreadsheetLastSyncMachineName.text = $"| {spreadsheet.LastSyncMachineName}"; }

            //Synced With Error
            m_SpreadsheetErrorMessage.Text = spreadsheet.SyncErrorMassage;
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
            

            m_SheetsContainer.Clear();
            m_RangesContainer.Clear();
            foreach (var sheet in spreadsheet.Sheets)
            {
                var sheetLabel = new SelectableLabel();
                sheetLabel.text = $"~ {sheet.Name} ({sheet.Id})";
                m_SheetsContainer.Add(sheetLabel);

                if(sheet.NamedRanges == null)
                    continue;

                foreach (var namedRange in sheet.NamedRanges)
                {
                    var rangeLabel = new SelectableLabel();
                    rangeLabel.text = $"✔ {namedRange.Name} ({sheet.Name}!{namedRange.Range.Name}))";
                    m_RangesContainer.Add(rangeLabel);
                }
            }
        }

        void StateChange(Spreadsheet spreadsheet)
        {
            m_Spinner.style.display = spreadsheet.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            if (spreadsheet.Synced || spreadsheet.SyncedWithError)
            {
                Bind(spreadsheet);
            }
            else
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-yellow");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.NotSyncedStringStatus;
            }
        }
    }
}
#endif
