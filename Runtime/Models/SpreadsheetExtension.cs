using System.IO;
using Newtonsoft.Json;

namespace StansAssets.GoogleDoc
{
    public static class SpreadsheetExtension
    {
        public static void InitFromCache(this Spreadsheet spreadsheet)
        {
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
