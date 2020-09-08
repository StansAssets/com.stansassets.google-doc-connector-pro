using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StansAssets.GoogleDoc
{
    public static class SpreadsheetExtension
    {
        public static void Load(this Spreadsheet spreadsheet)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            loader.Load();
        }

        public static async Task LoadAsync(this Spreadsheet spreadsheet)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            await loader.LoadAsync();
        }

        /// <summary>
        /// Save local spreadsheet changes to docs.google.com
        /// </summary>
        public static void Save(this Spreadsheet spreadsheet)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.Save();
        }

        /// <summary>
        /// Update cell to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!F3</param>
        /// <param name="value"></param>
        public static void UpdateGoogleCell(this Spreadsheet spreadsheet, string range, string value)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.UpdateCell(range, value);
        }

        /// <summary>
        /// Append cell in end of sheet to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!F3:F6</param>
        /// <param name="value"></param>
        public static void AppendGoogleCell(this Spreadsheet spreadsheet, string range, List<object> value)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.AppendCell(range, value);
        }

        /// <summary>
        ///  Create sheet to docs.google.com
        /// </summary>
        /// <param name="name">sheet name. For example: Sheet3</param>
        public static void CreateGoogleSheet(this Spreadsheet spreadsheet, string name)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            saver.CreateSheet(name);
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
        public static void CacheDocument(this Spreadsheet spreadsheet)
        {
            try
            {
                spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
                File.WriteAllText(spreadsheet.Path, JsonConvert.SerializeObject(spreadsheet));
                spreadsheet.ChangeStatus(Spreadsheet.SyncState.Synced);
            }
            catch (Exception e)
            {
                spreadsheet.SetError($"Error: {e.Message}");
                spreadsheet.SyncDateTime = DateTime.Now;
                spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
            }

        }
    }
}
