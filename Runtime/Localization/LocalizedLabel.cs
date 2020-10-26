#if TMP_AVAILABLE
using TMPro;
#endif
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StansAssets.GoogleDoc.Localization
{
    [ExecuteInEditMode]
    public class LocalizedLabel : ILocalizationLable
    {
        internal string Lang = default;
#if TMP_AVAILABLE
        TextMeshProUGUI m_TMPText;
#endif
        Text m_UGUIText;

        public LocalizedLabel(ILocalizationLable label)
        {
            Token = label.Token;
            Section = label.Section;
            TextType = label.TextType;
            Prefix = label.Prefix;
            Suffix = label.Suffix;
        }

        void Awake()
        {
            Section = LocalizationClient.Default.Sheets.FirstOrDefault() ?? default;
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

        [ContextMenu("Test")]
        void TestNextLand()
        {
            Debug.Log(LocalizationClient.Default.CurrentLanguage);
        }

        void OnDestroy()
        {
            LocalizationClient.Default.OnLanguageChanged -= UpdateLocalization;
        }

        internal void UpdateLocalization()
        {
            var text = LocalizationClient.Default.GetLocalizedString(Token, Section, TextType);
            var finalText = $"{Prefix}{text}{Suffix}";
            UpdateText(finalText);
        }
        
        internal void UpdateLocalizationWithLang()
        {
            var text = LocalizationClient.Default.GetLocalizedString(Token, Section, TextType, Lang);
            var finalText = $"{Prefix}{text}{Suffix}";
            UpdateText(finalText);
        }

        void UpdateText(string finalText)
        {
            if (!ReferenceEquals(m_UGUIText, null))
            {
                m_UGUIText.text = finalText;
            }

#if TMP_AVAILABLE
            if (!ReferenceEquals(m_UGUIText, null))
            {
                m_TMPText.text = finalText;
            }
#endif
        }
    }
}
