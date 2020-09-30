using System;
using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc
{
    public class LocalizationClient
    {
        /// <summary>
        /// Action is fired, when localization language changed
        /// </summary>
        public event Action<string> OnLanguageChanged = delegate { };

        /// <summary>
        /// Available Languages
        /// </summary>
        public List<string> Languages { get; }
        
        /// <summary>
        /// Available Spreadsheet sheet names
        /// </summary>
        public List<string> Sheets { get; }

        /// <summary>
        /// Default language for localization language
        /// If the language is not defined in the constructor, it returns the first language in the list.
        /// </summary>
        public string DefaultLanguage => m_DefaultLanguage ?? "";
        readonly string m_DefaultLanguage;
        /// <summary>
        /// Current chosen language
        /// </summary>
        public string CurrentLanguage { get; private set; }

        /// <summary>
        /// Current chosen language
        /// </summary>
        int m_CurrentLanguageCodeIndex;

        public enum TextType
        {
            Default,
            ToUpper,
            ToLower,
            WithCapital,
            EachWithCapital
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception">Will return an error if there is no first filled line in the first sheet of the spreadsheet</exception>
        public LocalizationClient()
        {
            var spr = GetSpreadsheet();
            if (!spr.Sheets.Any())
            {
                throw new Exception("No sheets in the spreadsheet");
            }

            var sheet = spr.Sheets.First();
            if (!sheet.Rows.Any())
            {
                throw new Exception("There are no filled lines on the first sheet of the table");
            }

            var row = sheet.Rows.First();
            if (!row.Cells.Any())
            {
                throw new Exception("The first row on the first sheet of the table has no filled cells");
            }
            
            var cellToken = row.Cells.FirstOrDefault();
            if (cellToken != null && (cellToken.Value.StringValue.ToLower() != "token" &&  cellToken.Value.StringValue.ToLower() != "tokens"))
            {
                throw new Exception("Token column name not found");
            }

            Sheets =  spr.Sheets.Select(sh => sh.Name).ToList();
            
            Languages = new List<string>();
            var indexRow = 0;
            foreach (var cell in row.Cells)
            {
                if (indexRow != 0)
                {
                    Languages.Add(cell.Value.StringValue.ToUpper());
                }

                indexRow++;
            }
            if (!Languages.Any())
            {
                throw new Exception("No headings found for available languages");
            }

            m_DefaultLanguage = Languages[0];
            CurrentLanguage = DefaultLanguage;
            m_CurrentLanguageCodeIndex = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultLangCode">Set default language</param>
        /// <param name="currentLangCode">Set current language</param>
        /// <exception cref="Exception">Will return an error if it could not find langCode in the available languages and
        /// the first sheet of the spreadsheet does not have the first completed row</exception>
        public LocalizationClient(string defaultLangCode, string currentLangCode)
            : this()
        {
            defaultLangCode = defaultLangCode.ToUpper();
            if (!Languages.Contains(defaultLangCode))
            {
                throw new Exception("Can't find langCode in the available languages");
            }
            m_DefaultLanguage = defaultLangCode;
            
            currentLangCode = currentLangCode.ToUpper();
            if (!Languages.Contains(currentLangCode))
            {
                throw new Exception("Can't find langCode in the available languages");
            }
            CurrentLanguage = currentLangCode;
            
            m_CurrentLanguageCodeIndex = Languages.IndexOf(CurrentLanguage) + 1;
        }

        /// <summary>
        /// Set current chosen language
        /// </summary>
        /// <exception cref="Exception">Will return an error if it could not find langCode</exception>
        public void SetLanguage(string langCode)
        {
            langCode = langCode.ToUpper();
            if (!Languages.Contains(langCode))
            {
                throw new Exception("Can't find langCode in the available languages");
            }

            CurrentLanguage = langCode;
            m_CurrentLanguageCodeIndex = Languages.IndexOf(CurrentLanguage) + 1;
            OnLanguageChanged(langCode);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <exception cref="Exception">"Token <param name="token" /> not found in available tokens</exception>
        public string GetLocalizedString(string token)
        {
            var spr = GetSpreadsheet();
            var sheet = spr.Sheets.First();
            var tokenIndex = GetTokenIndex(sheet.Rows, token);
            if (tokenIndex == 0)
            {
                throw new Exception($"Token {token} not found in available tokens");
            }

            return sheet.GetCell(tokenIndex, m_CurrentLanguageCodeIndex).Value.FormattedValue;
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <param name="textType"> returns localized string in the text type</param>
        public string GetLocalizedString(string token, TextType textType)
        {
            var value = GetLocalizedString(token);
            return ConvertLocalizedString(value, textType);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <exception cref="Exception"> Can't find sheet with name <param name="section" />
        /// </exception>
        /// <exception cref="Exception">"Token <param name="token" /> not found in available tokens</exception>
        public string GetLocalizedString(string token, string section)
        {
            var spr = GetSpreadsheet();
            var sheet = spr.Sheets.FirstOrDefault(sh => sh.Name == section);
            if (sheet == null)
            {
                throw new Exception($"Can't find sheet with name = {section}");
            }

            var tokenIndex = GetTokenIndex(sheet.Rows, token);
            if (tokenIndex == 0)
            {
                throw new Exception($"Token {token} not found in available tokens");
            }

            return sheet.GetCell(tokenIndex, m_CurrentLanguageCodeIndex).Value.FormattedValue;
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <param name="textType"> returns localized string in the text type</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <exception cref="Exception"> Can't find sheet with name <param name="section" />
        /// </exception>
        public string GetLocalizedString(string token, string section, TextType textType)
        {
            var value = GetLocalizedString(token, section);
            return ConvertLocalizedString(value, textType);
        }

        /// <summary>
        /// Returns localized string by token
        /// </summary>
        /// <param name="token"> Localization token</param>
        /// <param name="section">Spreadsheet sheet name</param>
        /// <param name="args"> Insert the args in a string</param>
        public string GetLocalizedString(string token, string section, params object[] args)
        {
            var value = GetLocalizedString(token, section);
            if(args != null && args.Length > 0)
            {
                value = string.Format(value, args);
            }
            return value;
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
        Spreadsheet GetSpreadsheet()
        {
            var id = LocalizationSettings.Instance.SpreadsheetId;
            return GoogleDocConnector.GetSpreadsheet(id);
        }
        
        /// <summary>
        /// Returns the position of the cell where the token is in the first column
        /// </summary>
        int GetTokenIndex(IEnumerable<RowData> rowData, string token)
        {
            var index = 0;
            foreach (var row in rowData)
            {
                if (row.Cells.Any())
                {
                    var cell = row.Cells.FirstOrDefault();
                    if (cell != null && cell.Value.FormattedValue.Trim().Equals(token.Trim()))
                    {
                        return index;
                    }
                }

                index++;
            }

            return index;
        }
    }
}
