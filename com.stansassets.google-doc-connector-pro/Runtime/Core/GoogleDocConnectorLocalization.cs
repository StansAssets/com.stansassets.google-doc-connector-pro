using System;
using StansAssets.GoogleDoc.Localization;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Localization setting for Localization Client
    /// </summary>
     static class GoogleDocConnectorLocalization
    {
        internal static Action SpreadsheetIdChanged = delegate { };
        /// <summary>
        /// Spreadsheet ID used for the Localization client
        /// </summary>
        internal static string SpreadsheetId => GoogleDocConnectorSettings.Instance.LocalizationSpreadsheetId; 
        
        /// <summary>
        /// Set a new spreadsheet ID to be used for the Localization client
        /// </summary>
        /// <param name="newSpreadsheetId">new spreadsheet ID</param>
        internal static void SpreadsheetIdSet(string newSpreadsheetId)
        {
            GoogleDocConnectorSettings.Instance.LocalizationSpreadsheetIdSet(newSpreadsheetId);
            LocalizationClient.ClearDefault();
            SpreadsheetIdChanged.Invoke();
        }
    }
}
