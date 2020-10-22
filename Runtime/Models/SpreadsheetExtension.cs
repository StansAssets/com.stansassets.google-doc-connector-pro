using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    public static class SpreadsheetExtension
    {
        public static void InitFromCache(this Spreadsheet spreadsheet)
        {
            Debug.Log("InitFromCache");
            Debug.Log($"spreadsheet.Path: ");
            Debug.Log(spreadsheet.Name);
            
            

            var spreadsheetTextAsset = Resources.Load<TextAsset>($"{GoogleDocConnectorSettings.SpreadsheetsResourcesSubFolder}/{spreadsheet.Name}");
            Debug.Log(spreadsheetTextAsset.text);
            
            if (File.Exists(spreadsheet.Path))
            {
                var serializedData = File.ReadAllText(spreadsheet.Path);
                var spreadsheetJson = JsonConvert.DeserializeObject<Spreadsheet>(serializedData);
                spreadsheet.SetSheets(spreadsheetJson.Sheets);
            }
        }

        public static void CleanUpLocalCache(this Spreadsheet spreadsheet)
        {
            if (File.Exists(spreadsheet.Path))
            {
                File.Delete(spreadsheet.Path);
                File.Delete($"{spreadsheet.Path}.meta");
            }
        }
    }
}
