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
        public static string SpreadsheetId => LocalizationSettings.Instance.SpreadsheetId; 
        
        /// <summary>
        /// Set a new spreadsheet ID to be used for the Localization client
        /// </summary>
        /// <param name="newSpreadsheetId">new spreadsheet ID</param>
        public static void SpreadsheetIdSet(string newSpreadsheetId)
        {
            LocalizationSettings.Instance.SpreadsheetIdSet(newSpreadsheetId);
        }
    }
}
