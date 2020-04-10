namespace StansAssets.GoogleDoc
{
    public static class GoogleDocConnectorEditor
    {
        public static Spreadsheet CreateSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.CreateSpreadsheet(id);
        }

        public static void RemoveSpreadsheet(string id)
        {
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(id);
        }
    }
}
