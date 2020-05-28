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
        readonly VisualElement m_NamedRangeLabelPanel;

        bool m_ExpandedNamedRanges = false;

        public SheetView(Sheet sheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SheetView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("sheetId");
            m_SpreadsheetName = this.Q<Label>("sheetName");
            m_NamedRangeFoldoutLabelPanel = this.Q<VisualElement>("namedRangeFoldoutLabelPanel");
            m_NamedRangeLabelPanel = this.Q<VisualElement>("namedRangeLabelPanel");
            m_NamedRangeContainer = this.Q<VisualElement>("namedRangeContainer");
            m_NamedRangeContainer.style.display = m_ExpandedNamedRanges ? DisplayStyle.Flex : DisplayStyle.None;
            var arrowToggleButton = this.Q<Button>("namedRangeArrowToggleBtn");
            arrowToggleButton.clicked += () => { ExpandingPanelChange(arrowToggleButton, m_NamedRangeContainer, ref m_ExpandedNamedRanges); };

            InitWithData(sheet);
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

        void InitWithData(Sheet sheet)
        {
            m_SpreadsheetId.text = $"Id: {sheet.Id.ToString()}";
            m_SpreadsheetName.text = $"Name: {sheet.Name}";
            m_NamedRangeFoldoutLabelPanel.style.display = (sheet.NamedRanges == null || sheet.NamedRanges.Count < 1) ? DisplayStyle.Flex : DisplayStyle.None;
            m_NamedRangeLabelPanel.style.display = (sheet.NamedRanges == null || sheet.NamedRanges.Count < 1) ? DisplayStyle.None : DisplayStyle.Flex;

            sheet.NamedRanges?.ForEach(range => m_NamedRangeContainer.Add(new NamedRangeView(range)));
        }
    }
}
