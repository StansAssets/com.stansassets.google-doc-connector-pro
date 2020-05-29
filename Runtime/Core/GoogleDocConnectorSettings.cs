using System;
using System.Collections.Generic;
using StansAssets.Plugins;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StansAssets.GoogleDoc
{
    class GoogleDocConnectorSettings : PackageScriptableSettingsSingleton<GoogleDocConnectorSettings>, ISerializationCallbackReceiver
    {
        public override string PackageName => "com.stansassets.google-doc-connector-pro";

        [SerializeField]
        List<Spreadsheet> m_Spreadsheets = new List<Spreadsheet>();
        public IEnumerable<Spreadsheet> Spreadsheets => m_Spreadsheets;

        readonly Dictionary<string, Spreadsheet> m_SpreadsheetsMap = new Dictionary<string, Spreadsheet>();


        public Spreadsheet CreateSpreadsheet(string id)
        {
            if (m_SpreadsheetsMap.ContainsKey(id))
            {
                throw new ArgumentException($"Spreadsheet with Id:{id} already exists");
            }

            var spreadsheet = new Spreadsheet(id);
            m_Spreadsheets.Add(spreadsheet);
            m_SpreadsheetsMap.Add(id, spreadsheet);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            return spreadsheet;
        }

        public void RemoveSpreadsheet(string id)
        {
            var spreadsheet = GetSpreadsheet(id);
            if (spreadsheet == null)
            {
                throw new KeyNotFoundException ($"Spreadsheet with Id:{id} DOESN'T exist");
            }

            spreadsheet.CleanUpLocalCache();
            m_Spreadsheets.Remove(spreadsheet);
            m_SpreadsheetsMap.Remove(spreadsheet.Id);
        }

        internal Spreadsheet GetSpreadsheet(string id)
        {
            if (m_SpreadsheetsMap.TryGetValue(id, out var spreadsheet))
            {
                return spreadsheet;
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
                spreadsheet.InitFromCache();
            }
        }
    }
}
