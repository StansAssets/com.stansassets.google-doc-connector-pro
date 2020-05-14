using UnityEditor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class NamedRangeView : VisualElement
    {
        readonly Label m_NamedRangeId;
        readonly Label m_NamedRangeName;

        public NamedRangeView(NamedRange range)
        {
            var uxmlPath = $"{GoogleDocConnectorPackage.UILayoutPath}/NamedRangeView.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
            style.flexGrow = 1.0f;

            m_NamedRangeId = this.Q<Label>("namedRangeId");
            m_NamedRangeName = this.Q<Label>("namedRangeName");
            InitWithData(range);
        }

        void InitWithData(NamedRange sheet)
        {
            m_NamedRangeId.text = sheet.Id;
            m_NamedRangeName.text = sheet.Name;
        }
    }
}
