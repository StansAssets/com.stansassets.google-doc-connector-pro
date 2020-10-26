namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Localization setting for Localization Client
    /// </summary>
    public static class GoogleDocConnectorLocalization
    {
        /// <summary>
        /// Spreadsheet ID used for the Localization client
        /// </summary>
        public static string SpreadsheetId => GoogleDocConnectorSettings.Instance.LocalizationSpreadsheetId; 
        
        /// <summary>
        /// Set a new spreadsheet ID to be used for the Localization client
        /// </summary>
        /// <param name="newSpreadsheetId">new spreadsheet ID</param>
        public static void SpreadsheetIdSet(string newSpreadsheetId)
        {
            GoogleDocConnectorSettings.Instance.LocalizationSpreadsheetIdSet(newSpreadsheetId);
        }
    }
}
