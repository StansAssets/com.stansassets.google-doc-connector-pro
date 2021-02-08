using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace StansAssets.GoogleDoc.Editor
{
     public static class GoogleDocConnectorEditor
    {
        internal static Action SpreadsheetsChange = delegate { };

        internal static Spreadsheet CreateSpreadsheet(string id)
        {
            var spr = GoogleDocConnectorSettings.Instance.CreateSpreadsheet(id);
            return spr;
        }

        internal static void RemoveSpreadsheet(string id)
        {
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(id);
            SpreadsheetsChange();
        }
        
        /// <summary>
        /// Get all currently configured spreadsheets.
        /// </summary>
        public static IEnumerable<Spreadsheet> GetAllSpreadsheets() {
            return GoogleDocConnectorSettings.Instance.Spreadsheets;
        }
        
        /// <summary>
        /// Async update spreadsheet by spreadsheet id
        /// </summary>
        /// <param name="id">An id of the spreadsheet</param>
        public static void UpdateSpreadsheet(string id) {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
            spreadsheet.LoadAsync(true).ContinueWith(_ => _);
        }
        
        /// <summary>
        /// Async update all added spreadsheets 
        /// </summary>
        public static void UpdateAllSpreadsheets() {
            foreach (var spreadsheet in GoogleDocConnectorSettings.Instance.Spreadsheets) {
                spreadsheet.LoadAsync(true).ContinueWith(_ => _);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Message Error if CheckCredentials has errors, otherwise empty string. </returns>
        internal static async Task<string> CheckCredentials()
        {
            //errorMassage = "";
            try
            {
                using (var stream = new FileStream(GoogleDocConnectorSettings.Instance.CredentialsPath, FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.CredentialsFolderPath}/token.json";
                    await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { SheetsService.Scope.SpreadsheetsReadonly },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
