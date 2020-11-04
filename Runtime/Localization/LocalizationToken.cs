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

        public string Token
        {
            get => m_TokenId;
            set => m_TokenId = value;
        }

        public string Section
        {
            get => m_Section;
            set => m_Section = value;
        }

        public TextType TextType          
        {
            get => m_TextType;
            set => m_TextType = value;
        }
        
        public string Prefix
        {
            get => m_Prefix;
            set => m_Prefix = value;
        }
        
        public string Suffix
        {
            get => m_Suffix;
            set => m_Suffix = value;
        }
    }
}
