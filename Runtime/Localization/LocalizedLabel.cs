#if TMP_AVAILABLE
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace StansAssets.GoogleDoc.Localization
{
    [ExecuteInEditMode]
    public class LocalizedLabel : MonoBehaviour
    {
        [SerializeField]
        LocalizationToken m_Token = default;

#if TMP_AVAILABLE
        TextMeshProUGUI m_TMPText;
#endif
        Text m_UGUIText;
        
        internal bool MoveUp = true; 

        void Awake()
        {
            m_UGUIText = GetComponent<Text>() ?? GetComponentInChildren<Text>();
#if TMP_AVAILABLE
            m_TMPText = GetComponent<TextMeshProUGUI>() ?? GetComponentInChildren<TextMeshProUGUI>();
#endif

            if (Application.isPlaying)
            {
                UpdateLocalization();
                enabled = false;
            }

            LocalizationClient.Default.OnLanguageChanged += UpdateLocalization;
        }

        void OnDestroy()
        {
            LocalizationClient.Default.OnLanguageChanged -= UpdateLocalization;
        }

        internal void UpdateLocalization()
        {
            var text = LocalizationClient.Default.GetLocalizedString(m_Token);
            UpdateText(text);
        }

        void UpdateText(string finalText)
        {
            if (!ReferenceEquals(m_UGUIText, null))
            {
                m_UGUIText.text = finalText;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_UGUIText);
#endif
            }

#if TMP_AVAILABLE
            if (!ReferenceEquals(m_TMPText, null))
            {
                m_TMPText.text = finalText;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_TMPText);
#endif
            }
#endif
        }
    }
}
