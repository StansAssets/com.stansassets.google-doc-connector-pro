using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class NamedRangeView : VisualElement
    {
        readonly Label m_SpreadsheetId;
        readonly Label m_SpreadsheetName;

        public NamedRangeView(NamedRange range)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/NamedRangeView.uxml";
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
