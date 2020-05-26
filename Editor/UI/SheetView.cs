using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SheetView : VisualElement
    {
        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly Label m_NamedRangeFoldoutLabel;
        readonly Foldout m_NamedRangeFoldout;

        public SheetView(Sheet sheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SheetView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("sheetId");
            m_SpreadsheetName = this.Q<Label>("sheetName");
            m_NamedRangeFoldoutLabel = this.Q<Label>("namedRangeFoldoutLabel");
            m_NamedRangeFoldout = this.Q<Foldout>("namedRangeFoldout");
            InitWithData(sheet);
        }

        void InitWithData(Sheet sheet)
        {
            m_SpreadsheetId.text = $"Id: {sheet.Id.ToString()}";
            m_SpreadsheetName.text = $"Name: {sheet.Name}";
            m_NamedRangeFoldoutLabel.visible = (sheet.NamedRanges == null || sheet.NamedRanges.Count < 1);

            sheet.NamedRanges?.ForEach(range => m_NamedRangeFoldout.Add(new NamedRangeView(range)));
        }
        
        public void AddNamedRange(NamedRangeView range)
        {
            m_NamedRangeFoldout.Add(range);
            m_NamedRangeFoldoutLabel.visible = false;
        }
    }
}
