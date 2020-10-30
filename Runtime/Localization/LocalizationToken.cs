using System;
using UnityEngine;

namespace StansAssets.GoogleDoc.Localization
{
    [Serializable]
    public class LocalizationToken : ILocalizationToken
    {
        [SerializeField]
        string m_TokenId = "token";

        [SerializeField]
        string m_Section = default;

        [SerializeField]
        TextType m_TextType = TextType.Default;

        [SerializeField]
        string m_Prefix = default;

        [SerializeField]
        string m_Suffix = default;

        public string Token => m_TokenId;
        public string Section => m_Section;
        public TextType TextType => m_TextType;
        public string Prefix => m_Prefix;
        public string Suffix => m_Suffix;

        internal void TokenSet(string newValue)
        {
            m_TokenId = newValue;
        }

        internal void SectionSet(string newValue)
        {
            m_Section = newValue;
        }

        internal void TextTypeSet(TextType newValue)
        {
            m_TextType = newValue;
        }

        internal void PrefixSet(string newValue)
        {
            m_Prefix = newValue;
        }

        internal void SuffixSet(string newValue)
        {
            m_Suffix = newValue;
        }
    }
}
