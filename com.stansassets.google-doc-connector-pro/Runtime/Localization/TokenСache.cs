using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc.Localization
{
    class TokenСache
    {
        Dictionary<int, TokenСacheSection> m_Sections = new Dictionary<int, TokenСacheSection>();
        
        /// <summary>
        /// Checks if the given token is in this section and get localized string index
        /// </summary>
        /// <param name="localizedValue">localized string index; otherwise -1</param>
        /// <returns>true if contain; otherwise false</returns>
        internal bool TryGetLocalizedString(string token, int section, out int localizedValue)
        {
            localizedValue = -1;
            return m_Sections.TryGetValue(section, out var tokens) && tokens.TryGetLocalizedString(token, out localizedValue);
        }

        /// <summary>
        /// Add new cached record
        /// </summary>
        internal void AddLocalizedString(string token, int section, int localizedValue)
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
        Dictionary<string, int> m_Tokens = new Dictionary<string, int>();
        
        internal bool TryGetLocalizedString(string token, out int localizedValue)
        {
            return m_Tokens.TryGetValue(token, out localizedValue);
        }
        
        internal TokenСacheSection AddLocalizedString(string token, int value)
        {
            m_Tokens.Add(token, value);
            return this;
        }
    }
}
