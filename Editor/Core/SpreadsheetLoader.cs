using System;
using System.IO;
using System.Threading;
using Google;
using Google.Apis.Auth.OAuth2;
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
    sealed class SpreadsheetLoader
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static readonly string[] s_Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

        readonly Spreadsheet m_Spreadsheet;

        public SpreadsheetLoader(Spreadsheet spreadsheet)
        {
            m_Spreadsheet = spreadsheet;
        }

        public void Load()
        {
            m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
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
                ApplicationName = GoogleDocConnectorSettings.Instance.PackageName,
            });

            // Define request parameters.
            var rangeRequest = service.Spreadsheets.Get(m_Spreadsheet.Id);
            rangeRequest.IncludeGridData = true;
            GoogleSheet.Spreadsheet spreadsheetData;
            try
            {
                spreadsheetData = rangeRequest.Execute();

                m_Spreadsheet.SyncDateTime = DateTime.Now;
                m_Spreadsheet.SetName(spreadsheetData.Properties.Title);

                var projectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                var spreadsheetPath = Path.Combine(projectRootPath, GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath, m_Spreadsheet.Name);
                m_Spreadsheet.SetPath(spreadsheetPath);
                m_Spreadsheet.SetMachineName(SystemInfo.deviceName);
                m_Spreadsheet.SetUrl(spreadsheetData.SpreadsheetUrl);
                m_Spreadsheet.CleanupSheets();

                //Set Sheets
                foreach (var sheetData in spreadsheetData.Sheets)
                {
                    var sheetId = sheetData.Properties.SheetId ?? 0;
                    var sheet = m_Spreadsheet.CreateSheet(sheetId, sheetData.Properties.Title);
                    sheet.CleanupRows();

                    // We always request the whole grid to get full data set.
                    // So there is only one item that represents the whole sheet.
                    var gridData = sheetData.Data[0];
                    if (gridData?.RowData == null)
                        continue;

                    var rowIndex = 0;
                    foreach (var rowData in gridData.RowData)
                    {
                        var row = new RowData();
                        if (rowData.Values != null)
                        {
                            var columnIndex = 0;
                            foreach (var cellData in rowData.Values)
                            {
                                if (cellData.FormattedValue == null)
                                {
                                    continue;
                                }

                                string stringValue;
                                if (cellData.EffectiveValue.BoolValue != null)
                                {
                                    stringValue = cellData.EffectiveValue.BoolValue.ToString();
                                }
                                else if (cellData.EffectiveValue.NumberValue != null)
                                {
                                    stringValue = cellData.EffectiveValue.NumberValue.ToString();
                                }
                                else
                                {
                                    stringValue = cellData.EffectiveValue.StringValue;
                                }

                                var cellValue = new CellValue(
                                    cellData.FormattedValue,
                                    cellData.EffectiveValue.FormulaValue,
                                    stringValue);

                                var cell = new Cell(rowIndex, columnIndex, cellValue);
                                row.AddCell(cell);

                                columnIndex++;
                            }
                        }

                        sheet.AddRow(row);

                        rowIndex++;
                    }
                }
                //Set NamedRanges
                if (spreadsheetData.NamedRanges != null)
                {
                    foreach (var namedRangeData in spreadsheetData.NamedRanges)
                    {
                        var sheetId = namedRangeData.Range.SheetId ?? 0;
                        var sheet = m_Spreadsheet.GetSheet(sheetId);
                        var namedRange = sheet.CreateNamedRange(namedRangeData.NamedRangeId, namedRangeData.Name);
                        var range = new GridRange(namedRangeData.Range.StartRowIndex, namedRangeData.Range.StartRowIndex, namedRangeData.Range.EndRowIndex, namedRangeData.Range.EndColumnIndex);

                        var cells = sheet.GetRange(range);
                        namedRange.SetCells(cells, range);
                    }
                }

                if (!Directory.Exists(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath))
                    Directory.CreateDirectory(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath);

                m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.Synced);
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

            public async Task LoadAsync()
            {
                m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
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
                    ApplicationName = GoogleDocConnectorSettings.Instance.PackageName,
                });

                // Define request parameters.
                var rangeRequest = service.Spreadsheets.Get(m_Spreadsheet.Id);
                rangeRequest.IncludeGridData = true;
                GoogleSheet.Spreadsheet spreadsheetData;
                try
                {
                    spreadsheetData = await rangeRequest.ExecuteAsync();

                    m_Spreadsheet.SyncDateTime = DateTime.Now;
                    m_Spreadsheet.SetName(spreadsheetData.Properties.Title);

                    var projectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                    var spreadsheetPath = Path.Combine(projectRootPath, GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath, m_Spreadsheet.Name);
                    m_Spreadsheet.SetPath(spreadsheetPath);
                    m_Spreadsheet.SetMachineName(SystemInfo.deviceName);
                    m_Spreadsheet.SetUrl(spreadsheetData.SpreadsheetUrl);
                    m_Spreadsheet.CleanupSheets();

                    //Set Sheets
                    foreach (var sheetData in spreadsheetData.Sheets)
                    {
                        var sheetId = sheetData.Properties.SheetId ?? 0;
                        var sheet = m_Spreadsheet.CreateSheet(sheetId, sheetData.Properties.Title);
                        sheet.CleanupRows();

                        // We always request the whole grid to get full data set.
                        // So there is only one item that represents the whole sheet.
                        var gridData = sheetData.Data[0];
                        if (gridData?.RowData == null)
                            continue;

                        var rowIndex = 0;
                        foreach (var rowData in gridData.RowData)
                        {
                            var row = new RowData();
                            if (rowData.Values != null)
                            {
                                var columnIndex = 0;
                                foreach (var cellData in rowData.Values)
                                {
                                    if (cellData.FormattedValue == null)
                                    {
                                        continue;
                                    }

                                    string stringValue;
                                    if (cellData.EffectiveValue.BoolValue != null)
                                    {
                                        stringValue = cellData.EffectiveValue.BoolValue.ToString();
                                    }
                                    else if (cellData.EffectiveValue.NumberValue != null)
                                    {
                                        stringValue = cellData.EffectiveValue.NumberValue.ToString();
                                    }
                                    else
                                    {
                                        stringValue = cellData.EffectiveValue.StringValue;
                                    }

                                    var cellValue = new CellValue(
                                        cellData.FormattedValue,
                                        cellData.EffectiveValue.FormulaValue,
                                        stringValue);

                                    var cell = new Cell(rowIndex, columnIndex, cellValue);
                                    row.AddCell(cell);

                                    columnIndex++;
                                }
                            }

                            sheet.AddRow(row);

                            rowIndex++;
                        }
                    }

                    //Set NamedRanges
                    if (spreadsheetData.NamedRanges != null)
                    {
                        foreach (var namedRangeData in spreadsheetData.NamedRanges)
                        {
                            var sheetId = namedRangeData.Range.SheetId ?? 0;
                            var sheet = m_Spreadsheet.GetSheet(sheetId);
                            var namedRange = sheet.CreateNamedRange(namedRangeData.NamedRangeId, namedRangeData.Name); 
                            var range = new GridRange(namedRangeData.Range.StartRowIndex, namedRangeData.Range.StartRowIndex, namedRangeData.Range.EndRowIndex, namedRangeData.Range.EndColumnIndex);

                            var cells = sheet.GetRange(range);
                            namedRange.SetCells(cells, range);
                        }
                    }

                    if (!Directory.Exists(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath))
                        Directory.CreateDirectory(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath);

                    m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.Synced);
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
            public Task CacheDocument()
            {
                return new Task(() => {try
                    {
                        m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
                        File.WriteAllText(m_Spreadsheet.Path, JsonConvert.SerializeObject(m_Spreadsheet));
                        m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.Synced);
                    }
                    catch (Exception e)
                    {
                        m_Spreadsheet.SetError($"Error: {e.Message}");
                        m_Spreadsheet.SyncDateTime = DateTime.Now;
                        m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
                    }});
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
