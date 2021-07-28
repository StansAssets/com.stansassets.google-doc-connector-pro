using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var sheetsJson = JsonConvert.DeserializeObject<IEnumerable<SheetJson>>(spreadsheetTextAsset.text);
                spreadsheet.SetSheets(sheetsJson.Select(s => s.ConvertToSheet()));
                foreach (var sheet in spreadsheet.Sheets)
                {
                    foreach (var namedRange in sheet.NamedRanges)
                    {
                        var cells = sheet.GetRange(namedRange.Range);
                        namedRange.SetCells(cells, namedRange.Range);
                    }
                }
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
