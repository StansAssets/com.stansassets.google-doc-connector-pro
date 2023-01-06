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
        /// Sheet ID used for the Localization client
        /// </summary>
        internal static int LocalizationSheetId => GoogleDocConnectorSettings.Instance.LocalizationSheetId;

        /// <summary>
        ///  Set a new spreadsheet ID and sheet ID to be used for the Localization client
        /// </summary>
        /// <param name="newSpreadsheetId"></param>
        /// <param name="newSheetId"></param>
        internal static void SetSpreadsheet(string newSpreadsheetId, int newSheetId = 0)
        {
            GoogleDocConnectorSettings.Instance.LocalizationSpreadsheetIdSet(newSpreadsheetId, newSheetId);
            LocalizationClient.ClearDefault();
            SpreadsheetIdChanged.Invoke();
        }
    }
}
