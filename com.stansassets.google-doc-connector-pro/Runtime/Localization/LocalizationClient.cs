using System;
using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc.Localization
{
    public class LocalizationClient
    {
        /// <summary>
        /// Action is fired, when localization language changed
        /// </summary>
        public event Action OnLanguageChanged = delegate { };

        /// <summary>
        /// Available Languages
        /// </summary>
        Dictionary<string, int> m_Languages;
        public List<string> Languages => m_Languages.Keys.ToList();

        /// <summary>
        /// Available Spreadsheet sheet names
        /// </summary>
        Dictionary<string, int> m_Sections;
        public List<string> Sections => m_Sections.Keys.ToList();

        /// <summary>
        /// Current chosen language
        /// </summary>
        public string CurrentLanguage { get; private set; }

        /// <summary>
        /// Current chosen language
        /// </summary>
        int m_CurrentLanguageCodeIndex;

        /// <summary>
        /// Current chosen sheet
        /// </summary>
        string m_CurrentSheetName;

        Spreadsheet m_LocalizationSpreadsheet;

        TokenСache m_TokenCache;
        ILocalizationToken m_LocalizationToken;

        static LocalizationClient s_DefaultLocalizationClient;

        public static LocalizationClient Default
        {
            get
            {
                if (s_DefaultLocalizationClient == null)
                {
                    var spr = GetSettingsLocalizationSpreadsheet();
                    s_DefaultLocalizationClient = new LocalizationClient(spr, GoogleDocConnectorLocalization.SheetId);
                }

                return s_DefaultLocalizationClient;
            }
        }

        internal static void ClearDefault()
        {
            s_DefaultLocalizationClient = null;
        }

        /// <summary>
        /// Create localization client with Spreadsheet
        /// </summary>
        /// <exception cref="Exception">Will return an error if the sheet hasn't been selected</exception>
        internal LocalizationClient(Spreadsheet spreadsheet, int sheetId)
        {
            spreadsheet.OnDataStoredOnDisk += NotifyLanguageChanged;
            Refresh(spreadsheet, sheetId);
        }

        /// <summary>
        /// Refresh all cached values.
        /// Method call will also trigger OnLanguageChanged callback
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet that will be used as the localization data source</param>
        /// <param name="localizationSheetId">Sheet id that will be used as the localization data source</param>
        /// <exception cref="InvalidOperationException">Will return an error if there is no first filled line in the first sheet of the spreadsheet</exception>
        public void Refresh(Spreadsheet spreadsheet, int localizationSheetId)
        {
            m_TokenCache = new TokenСache();
            m_LocalizationSpreadsheet = spreadsheet;
            if (!m_LocalizationSpreadsheet.Sheets.Any())
            {
                throw new InvalidOperationException("No sheets in the spreadsheet");
            }

            Sheet newSheet = m_LocalizationSpreadsheet.Sheets.FirstOrDefault(s => s.Id == localizationSheetId);
            if (newSheet == null)
            {
                throw new InvalidOperationException("There are no selected sheet");
            }

            m_CurrentSheetName = newSheet.Name.Trim();

            if (!newSheet.Rows.Any())
            {
                throw new InvalidOperationException("There are no filled lines on the first sheet of the table");
            }

            var row = newSheet.Rows.First();
            if (!row.Cells.Any())
            {
                throw new InvalidOperationException("The first row on the first sheet of the table has no filled cells");
            }

            var cellToken = row.Cells.FirstOrDefault();
            if (cellToken != null && (cellToken.Value.StringValue.ToLower() != "token" && cellToken.Value.StringValue.ToLower() != "tokens"))
            {
                throw new InvalidOperationException("Token column name not found");
            }

            var indexSheet = 0;
            m_Sections = new Dictionary<string, int>();
            foreach (var sheet in m_LocalizationSpreadsheet.Sheets)
            {
                m_Sections[sheet.Name.Trim()] = indexSheet;
                indexSheet++;
            }

            m_Languages = new Dictionary<string, int>();
            var indexRow = 0;
            foreach (var cell in row.Cells)
            {
                if (indexRow != 0 && !string.IsNullOrEmpty(cell.Value.StringValue))
                {
                    m_Languages[cell.Value.StringValue.ToUpper()] = indexRow;
                }

                indexRow++;
            }

            if (!m_Languages.Any())
            {
                throw new Exception("No headings found for available languages");
            }

            if (string.IsNullOrEmpty(CurrentLanguage))
            {
                CurrentLanguage = m_Languages.Keys.First();
                m_CurrentLanguageCodeIndex = 1;
            }
            else
            {
                m_CurrentLanguageCodeIndex = m_Languages[CurrentLanguage];
            }

            OnLanguageChanged.Invoke();
        }

        /// <summary>
        /// Set current chosen language
        /// </summary>
        /// <exception cref="Exception">Will return an error if it could not find langCode</exception>
        public void SetLanguage(string langCode)
        {
            SetLanguageWithoutNotify(langCode);
            OnLanguageChanged();
        }

        /// <summary>
        /// Set current chosen language without notification from Action OnLanguageChanged
        /// </summary>
        /// <exception cref="Exception">Will return an error if it could not find langCode</exception>
        public void SetLanguageWithoutNotify(string langCode)
        {
            langCode = langCode.Trim().ToUpper();
            if (!m_Languages.TryGetValue(langCode, out var indexLangCode))
            {
                throw new Exception($"Can't find {langCode} in the available languages");
            }

            CurrentLanguage = langCode;
            m_CurrentLanguageCodeIndex = indexLangCode;
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <exception cref="Exception">"Token <param name="token" /> not found in available tokens</exception>
        public string GetLocalizedString(string token)
        {
            m_LocalizationToken = new LocalizationToken()
            {
                Token = token,
                Section = m_CurrentSheetName
            };
            return GetLocalizedString(m_LocalizationToken, CurrentLanguage);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <param name="textType"> returns localized string in the text type</param>
        public string GetLocalizedString(string token, TextType textType)
        {
            m_LocalizationToken = new LocalizationToken()
            {
                Token = token,
                Section = m_CurrentSheetName,
                TextType = textType
            };
            return GetLocalizedString(m_LocalizationToken, CurrentLanguage);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token">Localization token</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <exception cref="Exception"> Can't find sheet with name <param name="section" />
        /// </exception>
        /// <exception cref="Exception">"Token <param name="token" /> not found in available tokens</exception>
        public string GetLocalizedString(string token, string section)
        {
            m_LocalizationToken = new LocalizationToken()
            {
                Token = token,
                Section = section
            };
            return GetLocalizedString(m_LocalizationToken, CurrentLanguage);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"><see cref="ILocalizationToken"/></param>
        public string GetLocalizedString(ILocalizationToken token)
        {
            return GetLocalizedString(token, CurrentLanguage);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token">Localization token</param>
        /// <param name="textType">returns localized string in the text type</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <exception cref="Exception"> Can't find sheet with name <param name="section" />
        /// </exception>
        public string GetLocalizedString(string token, string section, TextType textType)
        {
            m_LocalizationToken = new LocalizationToken()
            {
                Token = token,
                Section = section,
                TextType = textType
            };
            return GetLocalizedString(m_LocalizationToken, CurrentLanguage);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token">Localization token</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <param name="textType">returns localized string in the text type</param>
        /// <param name="langCode">Chosen language</param>
        /// <returns></returns>
        public string GetLocalizedString(string token, string section, TextType textType, string langCode)
        {
            m_LocalizationToken = new LocalizationToken()
            {
                Token = token,
                Section = section,
                TextType = textType
            };
            return GetLocalizedString(m_LocalizationToken, langCode);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <param name="args"> Insert the args in a string</param>
        public string GetLocalizedString(string token, string section, params object[] args)
        {
            m_LocalizationToken = new LocalizationToken()
            {
                Token = token,
                Section = section,
                Args = args
            };

            return GetLocalizedString(m_LocalizationToken, CurrentLanguage);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="localizationToken"><see cref="ILocalizationToken"/></param>
        /// <param name="langCode">Chosen language</param>
        /// <exception cref="Exception"> Can't find sheet with name <param name="localizationToken.Section" />
        /// </exception>
        /// <exception cref="Exception"> $"Token <param name="localizationToken.Token" /> not found in available tokens
        /// </exception>
        /// <exception cref="Exception"> Can't find <param name="langCode" /> in the available languages
        /// </exception>
        public string GetLocalizedString(ILocalizationToken localizationToken, string langCode)
        {
            var token = localizationToken.Token.Trim();
            var section = localizationToken.Section.Trim();

            Sheet localizationSheet = m_LocalizationSpreadsheet.Sheets.FirstOrDefault(s => s.Name == section);
            if (localizationSheet == null)
            {
                throw new InvalidOperationException("There are no selected sheet");
            }
  
            if (!localizationSheet.Rows.Any())
            {
                throw new InvalidOperationException("There are no filled lines on the first sheet of the table");
            }

            var row = localizationSheet.Rows.First();
            if (!row.Cells.Any())
            {
                throw new InvalidOperationException("The first row on the first sheet of the table has no filled cells");
            }

            var cellToken = row.Cells.FirstOrDefault();
            if (cellToken != null && cellToken.Value.StringValue.ToLower() != "token" && cellToken.Value.StringValue.ToLower() != "tokens")
            {
                throw new InvalidOperationException("Token column name not found");
            }

            
            if (!m_Sections.TryGetValue(section, out var sheetIndex))
            {
                throw new Exception($"Can't find sheet with name = {section}");
            }

            //TokenCache
            if (!m_TokenCache.TryGetLocalizedString(token, sheetIndex, out var tokenIndex))
            {
                tokenIndex = GetTokenIndex(m_LocalizationSpreadsheet.m_Sheets[sheetIndex], token);
                if (tokenIndex == 0)
                {
                    throw new Exception($"Token {token} not found in available tokens for {section}");
                }
            }

            //Language
            langCode = langCode.Trim().ToUpper();
            if (!m_Languages.TryGetValue(langCode, out var langCodeIndex))
            {
                throw new Exception($"Can't find {langCode} in the available languages");
            }

            //Get Localized string
            var cell = m_LocalizationSpreadsheet.m_Sheets[sheetIndex].GetCell(tokenIndex, langCodeIndex);
            var text = cell.Value.FormattedValue;

            //Insert the args in a string
            if (localizationToken.Args != null && localizationToken.Args.Length > 0)
            {
                text = string.Format(text, localizationToken.Args);
            }

            //Returns string converted by the textType
            text = ConvertLocalizedString(text, localizationToken.TextType);

            //Add Prefix and Suffix to Localized string
            var finalText = $"{localizationToken.Prefix}{text}{localizationToken.Suffix}";

            return finalText;
        }
        
        /// <summary>
        /// Notify that the language settings of the spreadsheet have been changed
        /// </summary>
        void NotifyLanguageChanged()
        {
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Returns string converted by the textType
        /// </summary>
        string ConvertLocalizedString(string str, TextType textType)
        {
            str = str.Trim();
            switch (textType)
            {
                case TextType.ToLower:
                    return str.ToLower();
                case TextType.ToUpper:
                    return str.ToUpper();
                case TextType.WithCapital:
                    return UppercaseFirst(str);
                case TextType.EachWithCapital:
                    return UppercaseWords(str);
                default:
                    return str;
            }
        }

        /// <summary>
        /// Returns a capitalized string
        /// </summary>
        static string UppercaseFirst(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var ch = value.ToCharArray();
            ch[0] = char.ToUpper(ch[0]);
            return new string(ch);
        }

        /// <summary>
        /// Returns a string with a capital letter each word
        /// </summary>
        static string UppercaseWords(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var array = value.ToCharArray();

            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }

            for (var i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }

            return new string(array);
        }

        /// <summary>
        /// Returns localization spreadsheet
        /// </summary>
        static Spreadsheet GetSettingsLocalizationSpreadsheet()
        {
            var id = GoogleDocConnectorLocalization.SpreadsheetId;
            return GoogleDocConnector.GetSpreadsheet(id);
        }

        /// <summary>
        /// Returns the position of the cell where the token is in the first column with add new tokens to TokenCache
        /// </summary>
        int GetTokenIndex(Sheet sheet, string token)
        {
            var index = 0;
            token = token.Trim();
            var sheetName = sheet.Name.Trim();
            foreach (var row in sheet.Rows)
            {
                if (row.Cells.Any())
                {
                    var cell = row.Cells.FirstOrDefault();
                    if (cell != null)
                    {
                        var currentToken = cell.Value.FormattedValue.Trim();

                        if (index > 0)
                            try
                            {
                                var localizedString = sheet.GetCellValue<string>(index, m_CurrentLanguageCodeIndex);
                                m_TokenCache.AddLocalizedString(currentToken, m_Sections[sheetName], index);
                            }
                            catch
                            {
                                // ignored
                            }

                        if (currentToken.Equals(token))
                        {
                            return index;
                        }
                    }
                }

                index++;
            }

            return 0;
        }
    }
}
