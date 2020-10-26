using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    public static class SpreadsheetExtension
    {
        public static void InitFromCache(this Spreadsheet spreadsheet)
        {
            var spreadsheetTextAsset = Resources.Load<TextAsset>($"{GoogleDocConnectorSettings.SpreadsheetsResourcesSubFolder}/{spreadsheet.Name}");
            if (!ReferenceEquals(spreadsheetTextAsset, null))
            {
                var spreadsheetJson = JsonConvert.DeserializeObject<Spreadsheet>(spreadsheetTextAsset.text);
                spreadsheet.SetSheets(spreadsheetJson.Sheets);
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
