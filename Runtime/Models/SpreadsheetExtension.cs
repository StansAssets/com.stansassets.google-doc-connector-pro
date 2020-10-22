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
            if (spreadsheetTextAsset != null)
            {
                Debug.Log("DeserializeObject start");
                var spreadsheetJson = JsonConvert.DeserializeObject<Spreadsheet>(spreadsheetTextAsset.text);
                Debug.Log("DeserializeObject end");
                spreadsheet.SetSheets(spreadsheetJson.Sheets);
                Debug.Log("InitFromCache end");
            }
        }

        public static void CleanUpLocalCache(this Spreadsheet spreadsheet)
        {
            var path = GoogleDocConnector.SpreadsheetPathInEditor(spreadsheet);
            if (File.Exists(path))
            {
                File.Delete(path);
                File.Delete($"{path}.meta");
            }
        }
    }
}
