﻿using System;
using System.Collections.Generic;
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
        const string k_ApplicationName = "Google Sheets API .NET Quickstart";

        readonly Spreadsheet m_Spreadsheet;

        public SpreadsheetLoader(Spreadsheet spreadsheet)
        {
            m_Spreadsheet = spreadsheet;
        }

        public async Task Load()
        {
            m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
            UserCredential credential;

            //TODO: There is an option to NOT load Google client secrets every request. Gonna fix this soon
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

            // Define request parameters.
            SpreadsheetsResource.GetRequest rangeRequest = service.Spreadsheets.Get(m_Spreadsheet.Id);
            rangeRequest.IncludeGridData = true;
            GoogleSheet.Spreadsheet spreadsheetData;
            try
            {
                spreadsheetData = await rangeRequest.ExecuteAsync();
            }
            catch (GoogleApiException exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Error.Message);
                return;
            }
            catch (Exception exception)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, exception.Message);
                return;
            }

            m_Spreadsheet.SyncDateTime = DateTime.Now;
            m_Spreadsheet.SetName(spreadsheetData.Properties.Title);

            string projectRootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
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

                var gridData = sheetData.Data[0];
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
                            var cellValue = new CellValue(
                                cellData.FormattedValue, 
                                cellData.EffectiveValue.FormulaValue,
                                cellData.EffectiveValue.StringValue,
                                cellData.EffectiveValue.NumberValue,
                                cellData.EffectiveValue.BoolValue);

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
                    var range = namedRangeData.Range;

                    Debug.Assert(range.StartRowIndex != null, "range.StartRowIndex != null");
                    Debug.Assert(range.EndRowIndex != null, "range.EndRowIndex != null");
                    Debug.Assert(range.StartColumnIndex != null, "range.StartColumnIndex != null");
                    Debug.Assert(range.EndColumnIndex != null, "range.EndColumnIndex != null");

                    var cells = new List<Cell>();
                    for (var i = range.StartRowIndex.Value; i < range.EndRowIndex.Value; i++)
                    {
                        for (var j = range.StartColumnIndex.Value; j < range.EndColumnIndex.Value; j++)
                        {
                            var cell = new Cell(i, j);
                            cells.Add(cell);
                        }
                    }

                    namedRange.SetCells(cells);
                }
            }

            if (!Directory.Exists(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath))
                Directory.CreateDirectory(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath);

            File.WriteAllText(spreadsheetPath, JsonConvert.SerializeObject(m_Spreadsheet));
            m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.Synced);
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
