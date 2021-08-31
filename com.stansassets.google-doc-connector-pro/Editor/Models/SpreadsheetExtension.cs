using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;

namespace StansAssets.GoogleDoc.Editor
{
    static class SpreadsheetExtension
    {
        /// <summary>
        /// Load spreadsheet from google api
        /// </summary>
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = false</param>
        public static void Load(this Spreadsheet spreadsheet, bool saveSpreadsheet = false)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            loader.Load(saveSpreadsheet);
        }

        /// <summary>
        /// Load asynchronously spreadsheet from google API
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = false</param>
        /// <returns></returns>
        internal static async Task LoadAsync(this Spreadsheet spreadsheet, bool saveSpreadsheet = false)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            await loader.LoadAsync(saveSpreadsheet);
            if(saveSpreadsheet)
                GoogleDocConnectorSettings.Save();
        }

       /*/// <summary>
        /// Save local spreadsheet changes to docs.google.com
        /// </summary>
        public static void Save(this Spreadsheet spreadsheet)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.Save();
        }*/

        /// <summary>
        /// Update cell to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!F3</param>
        /// <param name="value">New value to update</param>
        public static void UpdateGoogleCell(this Spreadsheet spreadsheet, string range, string value)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.UpdateCell(range, value);
        }

        /// <summary>
        /// Append cell in end of sheet to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!F3:F6</param>
        /// <param name="value">new list of values to append</param>
        public static void AppendGoogleCell(this Spreadsheet spreadsheet, string range, List<object> value)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.AppendCell(range, value);
        }


        /// <summary>
        ///  Create sheet to docs.google.com
        /// </summary>
        public static int CreateGoogleSheet(this Spreadsheet spreadsheet, SheetProperties sheetProperties)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            return saver.CreateSheet(sheetProperties);
        }

        /// <summary>
        /// Delete range of cells to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!A1:F1</param>
        public static void DeleteGoogleCell(this Spreadsheet spreadsheet, string range)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.DeleteCell(range);
        }

        /// <summary>
        /// Save spreadsheet to local json file. File will be saving in spreadsheet.Path
        /// </summary>
        internal static void CacheDocument(this Spreadsheet spreadsheet)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            _ = loader.CacheDocument();
        }
    }
}
