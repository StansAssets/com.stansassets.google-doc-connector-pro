using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SheetView : VisualElement
    {
        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly Foldout m_NamedRangeFoldout;
        readonly VisualElement m_NamedRangeLabelPanel;

        public SheetView(Sheet sheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SheetView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("sheetId");
            m_SpreadsheetName = this.Q<Label>("sheetName");
            m_NamedRangeLabelPanel = this.Q<VisualElement>("namedRangeLabelPanel");
            m_NamedRangeFoldout = this.Q<Foldout>("namedRangeFoldout");
            InitWithData(sheet);
        }

        void InitWithData(Sheet sheet)
        {
            m_SpreadsheetId.text = $"Id: {sheet.Id.ToString()}";
            m_SpreadsheetName.text = $"Name: {sheet.Name}";
            m_NamedRangeLabelPanel.style.display = (sheet.NamedRanges == null || sheet.NamedRanges.Count < 1) ? DisplayStyle.Flex : DisplayStyle.None;
            m_NamedRangeFoldout.style.display = (sheet.NamedRanges == null || sheet.NamedRanges.Count < 1) ? DisplayStyle.None : DisplayStyle.Flex;

            sheet.NamedRanges?.ForEach(range => m_NamedRangeFoldout.Add(new NamedRangeView(range)));
        }
    }
}
