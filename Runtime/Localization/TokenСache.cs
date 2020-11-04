using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc.Localization
{
    class TokenСache
    {
        Dictionary<string, TokenСacheSection> m_Sections = new Dictionary<string, TokenСacheSection>();
        
        /// <summary>
        /// Checks if the given token is in this section and get localized string
        /// </summary>
        /// <param name="localizedValue">localized string; otherwise empty string</param>
        /// <returns>true if contain; otherwise false</returns>
        internal bool TryGetLocalizedString(string token, string section, out string localizedValue)
        {
            localizedValue = string.Empty;
            return m_Sections.TryGetValue(section, out var tokens) && tokens.TryGetLocalizedString(token, out localizedValue);
        }

        /// <summary>
        /// Add new cached record
        /// </summary>
        internal void AddLocalizedString(string token, string section, string localizedValue)
        {
            if (m_Sections.TryGetValue(section, out var tokens))
            {
                tokens.AddLocalizedString(token, localizedValue);
            }
            else
            {
                m_Sections.Add(section, new TokenСacheSection().AddLocalizedString(token, localizedValue));
            }
        }
    }

    class TokenСacheSection
    {
        Dictionary<string, string> m_Tokens = new Dictionary<string, string>();
        
        internal bool TryGetLocalizedString(string token, out string localizedValue)
        {
            return m_Tokens.TryGetValue(token, out localizedValue);
        }
        
        internal TokenСacheSection AddLocalizedString(string token, string value)
        {
            m_Tokens.Add(token, value);
            return this;
        }
    }
}
