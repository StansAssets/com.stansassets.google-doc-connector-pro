using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class NamedRangeItem : VisualElement
    {
        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;

        public NamedRangeItem(NamedRange range)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/NamedRangeLayout.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_SpreadsheetId = this.Q<Label>("namedRangeId");
            m_SpreadsheetName = this.Q<Label>("namedRangeName");
            InitWithData(range);
        }

        void InitWithData(NamedRange sheet)
        {
            m_SpreadsheetId.text = sheet.Id;
            m_SpreadsheetName.text = sheet.Name;
        }
    }
}
