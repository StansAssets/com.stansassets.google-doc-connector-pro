using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
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

        static Spreadsheet s_Spreadsheet;
        static SheetsService s_SheetsService;
        static SheetsService Service => s_SheetsService ?? (s_SheetsService = Credential());

        public SpreadsheetSaverToGoogle(Spreadsheet spreadsheet)
        {
            s_Spreadsheet = spreadsheet;
        }

        public void Save()
        {
            try
            {
                var sheetChange = false;
                var batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest { Requests = new List<GoogleSheet.Request>() };
                foreach (var sheet in s_Spreadsheet.Sheets)
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
                    var request = Service.Spreadsheets.BatchUpdate(batchUpdate, s_Spreadsheet.Id);
                    request.Execute();
                }

                foreach (var sheet in s_Spreadsheet.Sheets)
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
                                var updateRequest = Service.Spreadsheets.Values.Update(valueRange, s_Spreadsheet.Id, range);
                                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                                updateRequest.Execute();
                            }
                        }
                    }
                }
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Message);
            }
        }

        public void CreateSheet(string name)
        {
            try
            {
                var batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest { Requests = new List<GoogleSheet.Request>() };

                var requestSheetCreate = new GoogleSheet.Request { AddSheet = new GoogleSheet.AddSheetRequest { Properties = new GoogleSheet.SheetProperties { Title = name } } };
                batchUpdate.Requests.Add(requestSheetCreate);

                var request = Service.Spreadsheets.BatchUpdate(batchUpdate, s_Spreadsheet.Id);
                request.Execute();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Message);
            }
        }

        public void UpdateCell(string range, string value)
        {
            try
            {
                var valueRange = new GoogleSheet.ValueRange { Values = new List<IList<object>> { new List<object>() { value } } };
                var updateRequest = Service.Spreadsheets.Values.Update(valueRange, s_Spreadsheet.Id, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                updateRequest.Execute();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Message);
            }
        }

        public void AppendCell(string range, List<object> value)
        {
            try
            {
                var valueRange = new GoogleSheet.ValueRange { Values = new List<IList<object>> { value } };
                var updateRequest = Service.Spreadsheets.Values.Append(valueRange, s_Spreadsheet.Id, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                updateRequest.Execute();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Message);
            }
        }

        public void DeleteCell(string range)
        {
            try
            {
                var requestBody = new GoogleSheet.ClearValuesRequest();

                var deleteRequest = Service.Spreadsheets.Values.Clear(requestBody, s_Spreadsheet.Id, range);
                deleteRequest.Execute();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Error.Message);
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, exception.Message);
            }
        }

         static SheetsService Credential()
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
                        HttpClientInitializer = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            s_Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)).Result,
                        ApplicationName = GoogleDocConnectorSettings.Instance.PackageName,
                    });
                }
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(s_Spreadsheet, ex.Message);
            }

            return new SheetsService(new BaseClientService.Initializer());
        }

        static void SetSpreadsheetSyncError(Spreadsheet spreadsheet, string exceptionMessage)
        {
            spreadsheet.SetError($"Error: {exceptionMessage}");
            spreadsheet.SetMachineName(SystemInfo.deviceName);
            spreadsheet.SyncDateTime = DateTime.Now;

            spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
        }
    }
}
