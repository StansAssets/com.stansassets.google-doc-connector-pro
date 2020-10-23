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
        string m_Token = "token";

        [SerializeField]
        string m_Section = default;
        
        [SerializeField]
        TextType m_TextType = TextType.Default;

        [SerializeField]
        string m_Prefix = default;

        [SerializeField]
        string m_Suffix = default;

#if TMP_AVAILABLE
        TextMeshProUGUI m_TMPText;
#endif
        Text m_UGUIText;

        string m_LastToken;
        
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
            m_LastToken = m_Token;
            LocalizationClient.Default.OnLanguageChanged += UpdateLocalization;
        }

        void Update()
        {
            if (m_LastToken.Equals(m_Token))
            {
                return;
            }

            m_LastToken = m_Token;
            UpdateLocalization();
        }

        [ContextMenu("Test")]
        void TestNextLand()
        {
            Debug.Log(LocalizationClient.Default.CurrentLanguage);
        }

        void OnDestroy()
        {
            LocalizationClient.Default.OnLanguageChanged -= UpdateLocalization;
        }

        void UpdateLocalization()
        {
            var text = LocalizationClient.Default.GetLocalizedString(m_Token, m_Section, m_TextType);
            var finalText = $"{m_Prefix}{text}{m_Suffix}";
            UpdateText(finalText);
        }

        void UpdateText(string finalText)
        {
            if (m_UGUIText != null)
            {
                m_UGUIText.text = finalText;
            }

#if TMP_AVAILABLE
            if (m_TMPText != null)
            {
                m_TMPText.text = finalText;
            }
#endif
        }
    }
}
