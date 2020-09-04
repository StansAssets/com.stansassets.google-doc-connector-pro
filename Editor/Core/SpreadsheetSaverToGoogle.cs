using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        /*public async Task Save()
        {
            UserCredential credential;

            try
            {
                using (var stream = new FileStream($"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        s_Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = k_ApplicationName,
            });

            try
            {
                var batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest { Requests = new List<GoogleSheet.Request>() };
                /*foreach (var sheet in m_Spreadsheet.Sheets)
                {
                    if (sheet.DataState == DataState.Created)
                    {
                        var requestSheetCreate = new GoogleSheet.Request { AddSheet = new GoogleSheet.AddSheetRequest { Properties = new GoogleSheet.SheetProperties { Title = sheet.Name, Index = sheet.Id } } };
                        batchUpdate.Requests.Add(requestSheetCreate);
                    }
                }

                var request = service.Spreadsheets.BatchUpdate(batchUpdate, m_Spreadsheet.Id);
                var response = await request.ExecuteAsync();
                batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest { Requests = new List<GoogleSheet.Request>() };*/
        /* foreach (var sheet in m_Spreadsheet.Sheets)
         {
             var requestCell = new GoogleSheet.Request
             {
                 UpdateCells = new GoogleSheet.UpdateCellsRequest()
                 {
                     Range = new GoogleSheet.GridRange
                     {
                         StartColumnIndex = 0,
                         StartRowIndex = 0,
                         SheetId = sheet.Id
                     }
                 }
             };
             requestCell.UpdateCells.Rows = new List<GoogleSheet.RowData>();
             foreach (var row in sheet.Rows)
             {
                 var googleRow = new GoogleSheet.RowData { Values = new List<GoogleSheet.CellData>() };
                 foreach (var cell in row.Cells)
                 {
                     var googleCell = new GoogleSheet.CellData();
                     if (cell.DataState == DataState.Updated)
                     {
                         googleCell.EffectiveValue = new GoogleSheet.ExtendedValue();
                         googleCell.FormattedValue = cell.Value.FormattedValue;
                         googleCell.EffectiveValue.FormulaValue = cell.Value.FormulaValue;

                         //googleCell.EffectiveValue.StringValue = cell.Value.StringValue;
                     }

                     googleRow.Values.Add(googleCell);
                 }

                 requestCell.UpdateCells.Rows.Add(googleRow);
             }

             requestCell.UpdateCells.Fields = '*';
             batchUpdate.Requests.Add(requestCell);
         }

         var request = service.Spreadsheets.BatchUpdate(batchUpdate, m_Spreadsheet.Id);
         var response = await request.ExecuteAsync();

         Console.WriteLine(JsonConvert.SerializeObject(response));
     }
     catch (GoogleApiException exception)
     {
         SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
     }
     catch (Exception exception)
     {
         SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
     }

     /*       try
      {
          foreach (var sheet in m_Spreadsheet.Sheets)
          {
              
          }
          SpreadsheetsResource.SheetsResource.
          var range = "Лист1!F63";
          var valueRange = new GoogleSheet.ValueRange();
          var objectList = new List<object>() { "Hello"};
          valueRange.Values = new List<IList<object>> { objectList };
          var UpdateRequest = service.Spreadsheets.Values.Update(valueRange, m_Spreadsheet.Id, range);
          service.Spreadsheets.Values.Update(valueRange, m_Spreadsheet.Id, "Лист1!H63");
          UpdateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
          var updateResponse = UpdateRequest.Execute();
      }
      catch (GoogleApiException exception)
      {
          SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
      }
      catch (Exception exception)
      {
          SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
      }*/

        /*// Define request parameters.
        var rangeRequest = service.Spreadsheets.Get(m_Spreadsheet.Id);

        rangeRequest.IncludeGridData = true;
        GoogleSheet.Spreadsheet spreadsheetData;
        
        var batchUpdate = new GoogleSheet.BatchUpdateSpreadsheetRequest();
        batchUpdate.Requests = new List<GoogleSheet.Request>();
        var request = new GoogleSheet.Request();
        try
        {
            spreadsheetData = await rangeRequest.ExecuteAsync();
            foreach (var localSheet in m_Spreadsheet.Sheets)
            {
                var sheetIndex = 0;
                var sheetIsAlive = false;
                for (; sheetIndex < spreadsheetData.Sheets.Count(); sheetIndex++)
                {
                    if (spreadsheetData.Sheets[sheetIndex].Properties?.SheetId == localSheet.Id)
                    {
                        sheetIsAlive = true;
                        break;
                    }
                }

                if (!sheetIsAlive)
                {
                    spreadsheetData.Sheets.Add(new GoogleSheet.Sheet());
                    sheetIndex = spreadsheetData.Sheets.Count() - 1;
                    request.AddSheet = new GoogleSheet.AddSheetRequest { Properties = new GoogleSheet.SheetProperties { Title = localSheet.Name } };
                }

                var googleSheet = spreadsheetData.Sheets[sheetIndex];
                if (googleSheet.Data == null)
                {
                    googleSheet.Data = new List<GoogleSheet.GridData>();
                    googleSheet.Data.Add(new GoogleSheet.GridData());
                }
                if (googleSheet.Data[0] == null)
                {
                    googleSheet.Data[0] = new GoogleSheet.GridData();
                }

                var googleGridData = googleSheet.Data[0];
                if (googleGridData.RowData == null)
                {
                    googleGridData.RowData = new List<GoogleSheet.RowData>();
                }

                var localRowList = localSheet.Rows.ToArray();
                for (var rowDataIndex = 0; rowDataIndex < localRowList.Length; rowDataIndex++)
                {
                    if (rowDataIndex + 1 > googleGridData.RowData.Count())
                    {
                        googleGridData.RowData.Add(new GoogleSheet.RowData());
                    }

                    var localRow = localRowList[rowDataIndex].Cells.ToArray();
                    if (googleGridData.RowData[rowDataIndex].Values == null)
                    {
                        googleGridData.RowData[rowDataIndex].Values = new List<GoogleSheet.CellData>();
                    }
                    var googleRow = googleGridData.RowData[rowDataIndex].Values;
                    for (var columnDataIndex = 0; columnDataIndex < localRow.Length; columnDataIndex++)
                    {
                        if (columnDataIndex + 1 > googleRow.Count())
                        {
                            googleRow.Add(new GoogleSheet.CellData());
                        }
                        
                        googleRow[columnDataIndex].FormattedValue = localRow[columnDataIndex].Value.FormattedValue;
                        if (googleRow[columnDataIndex].EffectiveValue == null)
                        {
                            googleRow[columnDataIndex].EffectiveValue = new GoogleSheet.ExtendedValue();
                        }
                        googleRow[columnDataIndex].EffectiveValue.FormulaValue = localRow[columnDataIndex].Value.FormulaValue;
                    }

                    googleGridData.RowData[rowDataIndex].Values = googleRow;
                }

                googleSheet.Data[0] = googleGridData;
                spreadsheetData.Sheets[sheetIndex] = googleSheet;
            }

            batchUpdate.Requests.Add(request);
            var req = service.Spreadsheets.BatchUpdate(batchUpdate, m_Spreadsheet.Id);
            var response = await req.ExecuteAsync();
            
            Console.WriteLine(JsonConvert.SerializeObject(response));
        }
        catch (GoogleApiException exception)
        {
            SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
        }
        catch (Exception exception)
        {
            SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
        }
    }*/
        public async Task CreateSheet(string name)
        {
            UserCredential credential;

            try
            {
                using (var stream = new FileStream($"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        s_Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = k_ApplicationName,
            });

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
            UserCredential credential;

            try
            {
                using (var stream = new FileStream($"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        s_Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = k_ApplicationName,
            });

            try
            {
                var valueRange = new GoogleSheet.ValueRange();

                var list = new List<object>() { value };
                valueRange.Values = new List<IList<object>> { list };

                var updateRequest = service.Spreadsheets.Values.Update(valueRange, m_Spreadsheet.Id, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
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
            UserCredential credential;

            try
            {
                using (var stream = new FileStream($"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.СredentialsFolderPath}/token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        s_Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = k_ApplicationName,
            });

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

        void SetSpreadsheetSyncError(Spreadsheet spreadsheet, string exceptionMessage)
        {
            spreadsheet.SetError($"Error: {exceptionMessage}");
            spreadsheet.SetMachineName(SystemInfo.deviceName);
            m_Spreadsheet.SyncDateTime = DateTime.Now;

            spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
        }
    }
}
