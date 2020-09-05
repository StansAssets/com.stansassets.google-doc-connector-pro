namespace StansAssets.GoogleDoc
{
    public static class SpreadsheetExtension
    {
        public static void Load(this Spreadsheet spreadsheet)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            _ = loader.Load();
        }
        /// <summary>
        /// Save local spreadsheet changes to docs.google.com
        /// </summary>
        public static void Save(this Spreadsheet spreadsheet)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            _ = saver.Save();
        }
        /// <summary>
        /// Update cell to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!F3</param>
        /// <param name="value"></param>
        public static void UpdateGoogleCell(this Spreadsheet spreadsheet, string range, string value)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            _ = saver.UpdateCell(range, value);
        }
        /// <summary>
        ///  Create sheet to docs.google.com
        /// </summary>
        /// <param name="name">sheet name. For example: Sheet3</param>
        public static void CreateGoogleSheet(this Spreadsheet spreadsheet, string name)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            _ = saver.CreateSheet(name);
        }
        /// <summary>
        /// Delete range of cells to docs.google.com
        /// </summary>
        /// <param name="range">Cell address. For example: Sheet1!A1:F1</param>
        public static void DeleteGoogleCell(this Spreadsheet spreadsheet, string range)
        {
            var saver = new SpreadsheetSaverToGoogle(spreadsheet);
            _ = saver.DeleteCell(range);
        }
    }
}
