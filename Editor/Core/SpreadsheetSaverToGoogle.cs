using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using UnityEngine;
using FileMode = System.IO.FileMode;
using Task = System.Threading.Tasks.Task;
using GoogleSheet = Google.Apis.Sheets.v4.Data;

namespace StansAssets.GoogleDoc
{
    sealed class SpreadsheetSaverToGoogle
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static readonly string[] s_Scopes = { SheetsService.Scope.Spreadsheets };
        const string k_ApplicationName = "Quickstart";

        readonly Spreadsheet m_Spreadsheet;

        public SpreadsheetSaverToGoogle(Spreadsheet spreadsheet)
        {
            m_Spreadsheet = spreadsheet;
        }

        public async Task Save()
        {
            var service = await Credential();

            try
            {
                var sheetChange = false;
                var batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest { Requests = new List<GoogleSheet.Request>() };
                foreach (var sheet in m_Spreadsheet.Sheets)
                {
                    if (sheet.DataState == DataState.Created)
                    {
                        var requestSheetCreate = new GoogleSheet.Request { AddSheet = new GoogleSheet.AddSheetRequest { Properties = new GoogleSheet.SheetProperties { Title = sheet.Name, Index = sheet.Id } } };
                        batchUpdate.Requests.Add(requestSheetCreate);
                        sheetChange = true;
                    }
                }
                if (sheetChange)
                {
                    var request = service.Spreadsheets.BatchUpdate(batchUpdate, m_Spreadsheet.Id);
                    await request.ExecuteAsync();
                }
                
                foreach (var sheet in m_Spreadsheet.Sheets)
                {
                    foreach (var row in sheet.Rows)
                    {
                        foreach (var cell in row.Cells)
                        {
                            if (cell.DataState == DataState.Updated)
                            {
                                var range = $"{sheet.Name}!{cell.Name}";
                                var valueRange = new GoogleSheet.ValueRange();
                                var list = new List<object>() { cell.Value.StringValue };
                                valueRange.Values = new List<IList<object>> { list };
                                var updateRequest = service.Spreadsheets.Values.Update(valueRange, m_Spreadsheet.Id, range);
                                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                                await updateRequest.ExecuteAsync();
                            }
                        }
                    }
                }
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
            }
        }

        public async Task CreateSheet(string name)
        {
            var service = await Credential();

            try
            {
                var batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest { Requests = new List<GoogleSheet.Request>() };

                var requestSheetCreate = new GoogleSheet.Request { AddSheet = new GoogleSheet.AddSheetRequest { Properties = new GoogleSheet.SheetProperties { Title = name } } };
                batchUpdate.Requests.Add(requestSheetCreate);

                var request = service.Spreadsheets.BatchUpdate(batchUpdate, m_Spreadsheet.Id);
                await request.ExecuteAsync();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
            }
        }

        public async Task UpdateCell(string range, string value)
        {
            var service = await Credential();

            try
            {
                var valueRange = new GoogleSheet.ValueRange();

                var list = new List<object>() { value };
                valueRange.Values = new List<IList<object>> { list };

                var updateRequest = service.Spreadsheets.Values.Update(valueRange, m_Spreadsheet.Id, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                await updateRequest.ExecuteAsync();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
            }
        }

        public async Task DeleteCell(string range)
        {
            var service = await Credential();

            try
            {
                var requestBody = new GoogleSheet.ClearValuesRequest();

                var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, m_Spreadsheet.Id, range);
                await deleteRequest.ExecuteAsync();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
            }
        }

        async Task<SheetsService> Credential()
        {
            try
            {
                using (var stream = new FileStream($"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/token.json";
                    // Create Google Sheets API service.
                    return new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            s_Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)),
                        ApplicationName = k_ApplicationName,
                    });
                }
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
            }

            return new SheetsService(new BaseClientService.Initializer());
        }

        void SetSpreadsheetSyncError(Spreadsheet spreadsheet, string exceptionMessage)
        {
            spreadsheet.SetError($"Error: {exceptionMessage}");
            spreadsheet.SetMachineName(SystemInfo.deviceName);
            m_Spreadsheet.SyncDateTime = DateTime.Now;

            spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
        }
    }
}
