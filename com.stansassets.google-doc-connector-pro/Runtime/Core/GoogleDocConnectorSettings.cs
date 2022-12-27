using System;
using System.Collections.Generic;
using StansAssets.Plugins;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    class GoogleDocConnectorSettings : PackageScriptableSettingsSingleton<GoogleDocConnectorSettings>, ISerializationCallbackReceiver
    {
        public const string SpreadsheetsResourcesSubFolder = "Spreadsheets";
        public override string PackageName => "com.stansassets.google-doc-connector-pro";
        public string CredentialsFolderPath => $"{PackagesConfig.SettingsPath}/{PackageName}";
        public string SpreadsheetsFolderPath => $"{SettingsFolderPath}/{SpreadsheetsResourcesSubFolder}";
        public string CredentialsPath => $"{CredentialsFolderPath}/credentials.json";

        [SerializeField]
        List<Spreadsheet> m_Spreadsheets = new List<Spreadsheet>();
        public List<Spreadsheet> Spreadsheets => m_Spreadsheets;

        readonly Dictionary<string, Spreadsheet> m_SpreadsheetsMap = new Dictionary<string, Spreadsheet>();

        [SerializeField]
        string m_LocalizationSpreadsheetId = string.Empty;
        [SerializeField]
        int m_LocalizationSheetId = -1;
        
        internal string LocalizationSpreadsheetId => m_LocalizationSpreadsheetId;
        internal int LocalizationSheetId => m_LocalizationSheetId;

        internal Spreadsheet CreateSpreadsheet(string id)
        {
            if (m_SpreadsheetsMap.ContainsKey(id))
            {
                throw new ArgumentException($"Spreadsheet with Id:{id} already exists");
            }

            var spreadsheet = new Spreadsheet(id);
            m_Spreadsheets.Add(spreadsheet);
            m_SpreadsheetsMap.Add(id, spreadsheet);

            Save();
            return spreadsheet;
        }

        internal void LocalizationSpreadsheetIdSet(string newSpreadsheetId)
        {
            m_LocalizationSpreadsheetId = newSpreadsheetId;
            Save();
        }

        internal void LocalizationSpreadsheetIdSet(string newSpreadsheetId, int sheetId)
        {
            m_LocalizationSheetId = sheetId;
            m_LocalizationSpreadsheetId = newSpreadsheetId;
            Save();
        }

        internal void RemoveSpreadsheet(string id)
        {
            var spreadsheet = GetSpreadsheet(id);
            if (spreadsheet == null)
            {
                throw new KeyNotFoundException($"Spreadsheet with Id:{id} DOESN'T exist");
            }

            spreadsheet.CleanUpLocalCache();
            m_Spreadsheets.Remove(spreadsheet);
            m_SpreadsheetsMap.Remove(spreadsheet.Id);

            if (spreadsheet.Id == m_LocalizationSpreadsheetId)
            {
                m_LocalizationSpreadsheetId = string.Empty;
                m_LocalizationSheetId = -1;
            }
        }

        internal bool HasSpreadsheet(string id)
        {
            var spreadsheet = GetSpreadsheet(id);
            return spreadsheet != null;
        }

        internal Spreadsheet GetSpreadsheet(string id)
        {
            if (m_SpreadsheetsMap.TryGetValue(id, out var spreadsheet))
            {
                if (!spreadsheet.IsLoaded)
                {
                    spreadsheet.InitFromCache();
                }

                return spreadsheet;
            }

            return null;
        }

        internal void ForceUpdateSpreadsheet(string id)
        {
            if (m_SpreadsheetsMap.TryGetValue(id, out var spreadsheet))
            {
                spreadsheet.InitFromCache();
            }
        }

        public void OnBeforeSerialize()
        {
            //Nothing to do here. We just need OnAfterDeserialize to repopulate m_SpreadsheetsMap
            //with serialized Spreadsheets data
        }

        public void OnAfterDeserialize()
        {
            m_SpreadsheetsMap.Clear();
            foreach (var spreadsheet in m_Spreadsheets)
            {
                m_SpreadsheetsMap[spreadsheet.Id] = spreadsheet;
                spreadsheet.IsLoaded = false;
            }
        }
    }
}
