using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace StansAssets.GoogleDoc
{
    static class GoogleDocConnectorEditor
    {
        internal static Action s_SpreadsheetsChange = delegate {  };
        public static Spreadsheet CreateSpreadsheet(string id)
        {
            var spr = GoogleDocConnectorSettings.Instance.CreateSpreadsheet(id);
            s_SpreadsheetsChange();
            return spr;
        }

        public static void RemoveSpreadsheet(string id)
        {
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(id);
            s_SpreadsheetsChange();
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
                using var stream = new FileStream($"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/credentials.json", FileMode.Open, FileAccess.Read);

                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                var credPath = $"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/token.json";
                await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] {SheetsService.Scope.SpreadsheetsReadonly},
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
