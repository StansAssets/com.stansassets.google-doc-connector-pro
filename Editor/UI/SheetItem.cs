using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class SheetItem : VisualElement
    {
        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;
        readonly Foldout m_NamedRangeFoldout;

        public SheetItem(Sheet sheet)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/SheetLayout.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("sheetId");
            m_SpreadsheetName = this.Q<Label>("sheetName");
            m_NamedRangeFoldout = this.Q<Foldout>("namedRangeFoldout");
            InitWithData(sheet);
        }

        void InitWithData(Sheet sheet)
        {
            m_SpreadsheetId.text = sheet.Id.ToString();
            m_SpreadsheetName.text = sheet.Name;
        }
        
        public void AddNamedRange(NamedRangeView range)
        {
            m_NamedRangeFoldout.Add(range);
        }
    }
}
