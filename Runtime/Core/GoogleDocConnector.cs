namespace StansAssets.GoogleDoc
{
    public static class GoogleDocConnector
    {
        public static Spreadsheet GetSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
        }
    }
}
