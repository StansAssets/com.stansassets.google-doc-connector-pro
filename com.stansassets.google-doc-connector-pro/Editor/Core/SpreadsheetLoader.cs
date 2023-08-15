using System;
using System.IO;
using System.Linq;
using System.Threading;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using StansAssets.GoogleDoc.Localization;
using UnityEditor;
using UnityEngine;
using FileMode = System.IO.FileMode;
using Task = System.Threading.Tasks.Task;
using GoogleSheet = Google.Apis.Sheets.v4.Data;

namespace StansAssets.GoogleDoc.Editor
{
    sealed class SpreadsheetLoader
    {
        public static event Action<Spreadsheet> OnSpreadsheetDataSavedOnDisk = delegate { };

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static readonly string[] s_Scopes = { SheetsService.Scope.Spreadsheets };

        readonly Spreadsheet m_Spreadsheet;

        const string k_DefaultSpreadsheetField = "None";

        public SpreadsheetLoader(Spreadsheet spreadsheet)
        {
            m_Spreadsheet = spreadsheet;
        }

        public void Load(bool saveSpreadsheet)
        {
            m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
            UserCredential credential = null;

            try
            {
                using (var stream = new FileStream(GoogleDocConnectorSettings.Instance.CredentialsPath, FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    var credPath = $"{GoogleDocConnectorSettings.Instance.CredentialsFolderPath}/token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        s_Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (TokenResponseException ex)
            {
                Debug.LogError(ex.Message);
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }

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

                UpdateSpreadsheetMeta(spreadsheetData);

                //Set Sheets
                SetSheets(spreadsheetData);

                //Set NamedRanges
                SetNamedRanges(spreadsheetData);

                if (!Directory.Exists(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath))
                    Directory.CreateDirectory(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath);

                if (saveSpreadsheet)
                {
                    var sheetJsons = m_Spreadsheet.m_Sheets.Select(s => new SheetJson(s));
                    File.WriteAllText(GoogleDocConnector.SpreadsheetPathInEditor(m_Spreadsheet), JsonConvert.SerializeObject(sheetJsons, Formatting.None,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore }));
                    var spreadSheetProjectPath = $"{GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath}/{m_Spreadsheet.Name}.json";
                    AssetDatabase.ImportAsset(spreadSheetProjectPath, ImportAssetOptions.ForceUpdate);
                    GoogleDocConnectorSettings.Instance.ForceUpdateSpreadsheet(m_Spreadsheet.Id);

                    if (!GoogleDocConnectorLocalization.SpreadsheetId.Equals(k_DefaultSpreadsheetField))
                    {
                        LocalizationClient.Default.Refresh(m_Spreadsheet, GoogleDocConnectorSettings.Instance.LocalizationSheetId);
                    }

                    OnSpreadsheetDataSavedOnDisk.Invoke(m_Spreadsheet);
                }

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

        public async Task LoadAsync(bool saveSpreadsheet)
        {
            m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
            UserCredential credential;
            try
            {
                using (var stream = new FileStream(GoogleDocConnectorSettings.Instance.CredentialsPath, FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.

                    var credPath = $"{GoogleDocConnectorSettings.Instance.CredentialsFolderPath}/token.json";
                    var task = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        s_Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));

                    credential = await task;

                    if (credential.Token.IsExpired(Google.Apis.Util.SystemClock.Default))
                    {
                        var t = credential.RefreshTokenAsync(CancellationToken.None).Result;
                    }

                    Debug.Log("Authorization request completed successfully.");
                }
            }
            catch (OperationCanceledException)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, "Authorization request cancelled.");
                return;
            } 
            catch (GoogleApiException ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                SetSpreadsheetSyncError(m_Spreadsheet, ex.Message);
                foreach (var innerException in ex.InnerExceptions)
                {
                    if (innerException is GoogleApiException apiException)
                    {
                        Debug.LogError("Google API error: " + apiException.Message);
                    }
                    else
                    {
                        Debug.LogError("Error: " + innerException.Message);
                    }
                }

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
            try
            {
                var spreadsheetData = await rangeRequest.ExecuteAsync();

                UpdateSpreadsheetMeta(spreadsheetData);

                //Set Sheets
                SetSheets(spreadsheetData);

                //Set NamedRanges
                SetNamedRanges(spreadsheetData);

                if (!Directory.Exists(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath))
                    Directory.CreateDirectory(GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath);

                if (saveSpreadsheet)
                {
                    var sheetJsons = m_Spreadsheet.m_Sheets.Select(s => new SheetJson(s));
                    File.WriteAllText(GoogleDocConnector.SpreadsheetPathInEditor(m_Spreadsheet), JsonConvert.SerializeObject(sheetJsons, Formatting.None,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore }));
                    var spreadSheetProjectPath = $"{GoogleDocConnectorSettings.Instance.SpreadsheetsFolderPath}/{m_Spreadsheet.Name}.json";
                    AssetDatabase.ImportAsset(spreadSheetProjectPath, ImportAssetOptions.ForceUpdate);
                    GoogleDocConnectorSettings.Instance.ForceUpdateSpreadsheet(m_Spreadsheet.Id);

                    if (GoogleDocConnectorLocalization.SpreadsheetId.Equals(m_Spreadsheet.Id))
                    {
                        LocalizationClient.Default.Refresh(m_Spreadsheet, GoogleDocConnectorSettings.Instance.LocalizationSheetId);
                    }

                    OnSpreadsheetDataSavedOnDisk.Invoke(m_Spreadsheet);
                }

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
            return new Task(() =>
            {
                try
                {
                    m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.InProgress);
                    File.WriteAllText(GoogleDocConnector.SpreadsheetPathInEditor(m_Spreadsheet), JsonConvert.SerializeObject(m_Spreadsheet));
                    m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.Synced);
                }
                catch (Exception e)
                {
                    m_Spreadsheet.SetError($"Error: {e.Message}");
                    m_Spreadsheet.SyncDateTime = DateTime.Now;
                    m_Spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
                }
            });
        }

        void SetSpreadsheetSyncError(Spreadsheet spreadsheet, string exceptionMessage)
        {
            spreadsheet.SetError($"Error: {exceptionMessage}");
            spreadsheet.SetMachineName(SystemInfo.deviceName);
            m_Spreadsheet.SyncDateTime = DateTime.Now;

            spreadsheet.ChangeStatus(Spreadsheet.SyncState.SyncedWithError);
        }

        void UpdateSpreadsheetMeta(GoogleSheet.Spreadsheet spreadsheetData)
        {
            m_Spreadsheet.SyncDateTime = DateTime.Now;
            m_Spreadsheet.SetName(spreadsheetData.Properties.Title);

            m_Spreadsheet.SetMachineName(SystemInfo.deviceName);
            m_Spreadsheet.SetUrl(spreadsheetData.SpreadsheetUrl);
            m_Spreadsheet.CleanupSheets();
        }

        void SetSheets(GoogleSheet.Spreadsheet spreadsheetData)
        {
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
                            var cellValue = new CellValue();
                            if (cellData.FormattedValue != null)
                            {
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

                                cellValue = new CellValue(
                                    cellData.FormattedValue?.Trim(),
                                    cellData.EffectiveValue.FormulaValue?.Trim(),
                                    stringValue?.Trim());
                            }

                            var cell = new Cell(rowIndex, columnIndex, cellValue);
                            row.AddCell(cell);

                            columnIndex++;
                        }
                    }

                    sheet.AddRow(row);

                    rowIndex++;
                }
            }
        }

        void SetNamedRanges(GoogleSheet.Spreadsheet spreadsheetData)
        {
            if (spreadsheetData.NamedRanges != null)
            {
                foreach (var namedRangeData in spreadsheetData.NamedRanges)
                {
                    var sheetId = namedRangeData.Range.SheetId ?? 0;
                    var sheet = m_Spreadsheet.GetSheet(sheetId);
                    var namedRange = sheet.CreateNamedRange(namedRangeData.NamedRangeId, namedRangeData.Name);
                    var range = new GridRange(namedRangeData.Range.StartRowIndex, namedRangeData.Range.StartColumnIndex, namedRangeData.Range.EndRowIndex - 1, namedRangeData.Range.EndColumnIndex - 1);

                    var cells = sheet.GetRange(range);
                    namedRange.SetCells(cells, range);
                }
            }
        }
    }
}
