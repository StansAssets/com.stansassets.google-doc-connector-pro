using StansAssets.Plugins;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    class LocalizationSettings : PackageScriptableSettingsSingleton<LocalizationSettings>, ISerializationCallbackReceiver
    {
        
        public override string PackageName => "com.stansassets.google-doc-connector-pro";
        
        [SerializeField]
        string m_SpreadsheetId = "";
        public string SpreadsheetId => m_SpreadsheetId;

        internal void SpreadsheetIdSet(string newSpreadsheetId)
        {
            m_SpreadsheetId = newSpreadsheetId;
        }
        
        public void OnBeforeSerialize()
        {
            //Nothing to do here. We just need OnAfterDeserialize to repopulate m_SpreadsheetsMap
            //with serialized Spreadsheets data
        }

        public void OnAfterDeserialize()
        {
            //Nothing to do here. We just need OnAfterDeserialize to repopulate m_SpreadsheetsMap
            //with serialized Spreadsheets data
        }

    }
}
