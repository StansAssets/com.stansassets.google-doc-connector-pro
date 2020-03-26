namespace StansAssets.GoogleDoc
{
    public static class GoogleDocConnector
    {
#if UNITY_EDITOR
        public static Spreadsheet CreateSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.CreateSpreadsheet(id);
        }

        public static void RemoveSpreadsheet(string id)
        {
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(id);
        }
#endif

        public static Spreadsheet GetSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
        }
    }
}
