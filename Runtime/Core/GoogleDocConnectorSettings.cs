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
        public string СredentialsFolderPath => $"{PackagesConfig.SettingsPath}/{PackageName}";
        public string SpreadsheetsFolderPath => $"{SettingsFolderPath}/{SpreadsheetsResourcesSubFolder}";

        [SerializeField]
        List<Spreadsheet> m_Spreadsheets = new List<Spreadsheet>();
        public List<Spreadsheet> Spreadsheets => m_Spreadsheets;

        readonly Dictionary<string, Spreadsheet> m_SpreadsheetsMap = new Dictionary<string, Spreadsheet>();

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
        }

        internal bool HasSpreadsheet(string id)
        {
            var spreadsheet = GetSpreadsheet(id);
            return spreadsheet != null;
        }

        internal Spreadsheet GetSpreadsheet(string id)
        {
            if (m_SpreadsheetsMap != null)
            {
                if (m_SpreadsheetsMap.TryGetValue(id, out var spreadsheet))
                {
                    if (spreadsheet.IsLoaded)
                    {
                        spreadsheet.InitFromCache();
                    }

                    return spreadsheet;
                }

                return null;
            }

            return null;
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
            }
        }
    }
}
