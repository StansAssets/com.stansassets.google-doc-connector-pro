namespace StansAssets.GoogleDoc
{
    public static class SpreadsheetExtension
    {
        public static void Load(this Spreadsheet spreadsheet)
        {
            var loader = new SpreadsheetLoader(spreadsheet);
            _ = loader.Load();
        }
    }
}
