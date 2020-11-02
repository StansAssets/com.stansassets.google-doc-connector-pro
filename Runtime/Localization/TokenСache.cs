using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc.Localization
{
    class TokenСache
    {
        Dictionary<string, TokenСacheSection> m_Sections = new Dictionary<string, TokenСacheSection>();
        /// <summary>
        /// List of stored tokens for this section
        /// </summary>
        /// <returns>List of tokens; otherwise empty string List</returns>
        internal List<string> Tokens(string section) => m_Sections.ContainsKey(section) ? m_Sections[section].Tokens : new List<string>();
        /// <summary>
        /// List of stored Localized strings for this section
        /// </summary>
        /// <returns>List of tokens; otherwise empty string List</returns>
        internal List<string> LocalizedValues(string section) => m_Sections.ContainsKey(section) ? m_Sections[section].LocalizedValues : new List<string>();

        /// <summary>
        /// Checks if the given token is in this section
        /// </summary>
        /// <returns>true if contain; otherwise false</returns>
        internal bool ContainToken(string token, string section)
        {
            token = token.Trim();
            section = section.Trim();
            return m_Sections.TryGetValue(section, out var tokens) && tokens.ContainToken(token);
        }
        
        /// <summary>
        /// Checks if the given token is in this section and get localized string
        /// </summary>
        /// <param name="localizedValue">localized string; otherwise empty string</param>
        /// <returns>true if contain; otherwise false</returns>
        internal bool TryGetLocalizedString(string token, string section, out string localizedValue)
        {
            token = token.Trim();
            section = section.Trim();
            localizedValue = string.Empty;
            return m_Sections.TryGetValue(section, out var tokens) && tokens.TryGetLocalizedString(token, out localizedValue);
        }

        /// <summary>
        /// Get localized string
        /// </summary>
        /// <returns>localized string; otherwise null</returns>
        internal string GetLocalizedString(string token, string section)
        {
            token = token.Trim();
            section = section.Trim();
            return m_Sections.TryGetValue(section, out var tokens) ? tokens.GetLocalizedString(token) : null;
        }

        /// <summary>
        /// Add new cached record
        /// </summary>
        internal void AddLocalizedString(string token, string section, string localizedValue)
        {
            token = token.Trim();
            section = section.Trim();
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
        internal List<string> Tokens => m_Tokens.Keys.ToList();
        internal List<string> LocalizedValues => m_Tokens.Values.ToList();

        internal bool ContainToken(string token)
        {
            return m_Tokens.ContainsKey(token);
        }
        
        internal bool TryGetLocalizedString(string token, out string localizedValue)
        {
            return m_Tokens.TryGetValue(token, out localizedValue);
        }

        internal string GetLocalizedString(string token)
        {
            return m_Tokens.TryGetValue(token, out var localizedValue) ? localizedValue : null;
        }

        internal TokenСacheSection AddLocalizedString(string token, string value)
        {
            m_Tokens.Add(token, value);
            return this;
        }
    }
}
