using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SheetView : VisualElement
    {
        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly VisualElement m_NamedRangeFoldoutLabelPanel;
        readonly VisualElement m_NamedRangeContainer;
        readonly Foldout m_NamedRangFoldout;
        

        public SheetView(Sheet sheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SheetView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("sheetId");
            m_SpreadsheetName = this.Q<Label>("sheetName");
            m_NamedRangeFoldoutLabelPanel = this.Q<VisualElement>("namedRangeFoldoutLabelPanel");
            m_NamedRangeContainer = this.Q<VisualElement>("namedRangeContainer");
            
            m_NamedRangFoldout = this.Q<Foldout>("namedRangFoldout");
            m_NamedRangFoldout.value = sheet.NamedRangeFoldOutUIState;
            m_NamedRangFoldout.RegisterValueChangedCallback(ev => ExpandingPanelChange(ev.newValue, m_NamedRangeContainer, out sheet.NamedRangeFoldOutUIState));
            m_NamedRangeContainer.style.display = m_NamedRangFoldout.value ? DisplayStyle.Flex : DisplayStyle.None;

            InitWithData(sheet);
        }

        void ExpandingPanelChange(bool state, VisualElement element, out bool uiState)
        {
            element.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
            uiState = state;
        }

        void InitWithData(Sheet sheet)
        {
            m_SpreadsheetId.text = $"Id: {sheet.Id.ToString()}";
            m_SpreadsheetName.text = $"Name: {sheet.Name}";
            var emptyNamedRangesList = !sheet.NamedRanges.Any();
            m_NamedRangeFoldoutLabelPanel.style.display = emptyNamedRangesList ? DisplayStyle.Flex : DisplayStyle.None;
            m_NamedRangFoldout.style.display = emptyNamedRangesList ? DisplayStyle.None : DisplayStyle.Flex;
            
            foreach (var namedRange in sheet.NamedRanges)
            {
                m_NamedRangeContainer.Add(new NamedRangeView(namedRange));
            }
        }
    }
}
