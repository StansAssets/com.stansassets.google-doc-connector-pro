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
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = true</param>
        public static IEnumerable<Spreadsheet> GetAllSpreadsheets() {
            return GoogleDocConnectorSettings.Instance.Spreadsheets;
        }
        
        /// <summary>
        /// Async update spreadsheet by spreadsheet id
        /// </summary>
        /// <param name="id">An id of the spreadsheet</param>
        /// <param name="callback">return updated spreadsheet</param>
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = true</param>
        public static void UpdateSpreadsheetAsync(string id, Action<Spreadsheet> callback, bool saveSpreadsheet = true) {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
            spreadsheet.LoadAsync(saveSpreadsheet).ContinueWith(_ => callback?.Invoke(spreadsheet));
        }
        
        /// <summary>
        /// Async update all added spreadsheets 
        /// </summary>
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = true</param>
        public static async Task<List<Spreadsheet>> UpdateAllSpreadsheetsAsync(bool saveSpreadsheet = true) {
            var list = new List<Spreadsheet>();
            foreach (var spreadsheet in GoogleDocConnectorSettings.Instance.Spreadsheets) {
                await spreadsheet.LoadAsync(saveSpreadsheet);
                list.Add(spreadsheet);
            }
            return list;
        }
        
        /// <summary>
        /// Sync update spreadsheet by spreadsheet id
        /// </summary>
        /// <param name="id">An id of the spreadsheet</param>
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = true</param>
        public static Spreadsheet UpdateSpreadsheet(string id, bool saveSpreadsheet = true) {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
            spreadsheet.Load(saveSpreadsheet);
            return spreadsheet;
        }
        
        /// <summary>
        /// Sync update all added spreadsheets 
        /// </summary>
        /// <param name="saveSpreadsheet">Save spreadsheet in local cache, default = true</param>
        public static IEnumerable<Spreadsheet> UpdateAllSpreadsheets(bool saveSpreadsheet = true) {
            var list = new List<Spreadsheet>();
            foreach (var spreadsheet in GoogleDocConnectorSettings.Instance.Spreadsheets) {
                spreadsheet.Load(saveSpreadsheet);
                list.Add(spreadsheet);
            }
            return list;
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
