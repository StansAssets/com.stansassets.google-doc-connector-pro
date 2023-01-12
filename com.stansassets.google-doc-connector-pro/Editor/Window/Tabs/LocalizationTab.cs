using System;
using System.Globalization;
using System.Linq;
using StansAssets.Foundation.UIElements;
using StansAssets.GoogleDoc.Localization;
using StansAssets.Plugins.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using HelpBox = StansAssets.Foundation.UIElements.HelpBox;

namespace StansAssets.GoogleDoc.Editor
{
    class LocalizationTab : BaseTab
    {
        readonly VisualElement m_ListSpreadsheet;
        readonly VisualElement m_ListSheetIdVisualElement;

        readonly Label m_LabelLang;
        readonly VisualElement m_ListLang;
        readonly Label m_LabelSheet;
        readonly VisualElement m_ListSheet;
        readonly VisualElement m_DocumentInfo;

        readonly HelpBox m_LocalizationError;
        readonly HelpBox m_LocalizationWarning;

        readonly Button m_RefreshButton;
        readonly Button m_OpenBtn;
        readonly VisualElement m_Spinner;

        readonly Label m_LabelChooseSpreadsheet;
        PopupField<string> m_SpreadsheetField;
        PopupField<string> m_LocalizationSpreadsheetId;

        readonly HelpBox m_NoCredentialsHelpBox;

        readonly Label m_SpreadsheetDate;
        readonly Label m_SpreadsheetLastSyncMachineName;
        readonly Label m_SpreadsheetStatusIcon;
        readonly VisualElement m_SpreadsheetBottomPanel;
        readonly VisualElement m_SpreadsheetPanel;

        const string k_DefaultSpreadsheetField = "None";
        const int k_DefaultLocalizationSheetId = -1;

        public LocalizationTab()
            : base($"{GoogleDocConnectorPackage.LocalizationTabPath}")
        {
            m_ListSpreadsheet = this.Q<VisualElement>("list-spreadsheet");
            m_ListSheetIdVisualElement = this.Q<VisualElement>("list-sheet-ids");
            m_LabelLang = this.Q<Label>("labelLang");
            m_ListLang = this.Q<VisualElement>("listLang");
            m_LabelSheet = this.Q<Label>("labelSheet");
            m_ListSheet = this.Q<VisualElement>("listSheet");
            m_DocumentInfo = this.Q<VisualElement>("document-info");
            m_LocalizationError = this.Q<HelpBox>("localization-error");
            m_LocalizationWarning = this.Q<HelpBox>("localization-warning");
            m_RefreshButton = this.Q<Button>("refreshBtn");
            m_OpenBtn = this.Q<Button>("openBtn");
            m_LabelChooseSpreadsheet = this.Q<Label>("choose-spreadsheet");
            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_NoCredentialsHelpBox = this.Q<HelpBox>("no-spreadsheets-note");
            m_SpreadsheetPanel = this.Q<VisualElement>("spreadsheetPanel");
            m_SpreadsheetBottomPanel = this.Q<VisualElement>("spreadsheetBottomPanel");
            m_SpreadsheetDate = this.Q<Label>("spreadsheetDate");
            m_SpreadsheetLastSyncMachineName = this.Q<Label>("lastSyncMachineName");
            m_SpreadsheetStatusIcon = this.Q<Label>("statusIcon");
            GoogleDocConnectorEditor.SpreadsheetsChange += () =>
            {
                Spreadsheet spreadsheet = GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId) ?? new Spreadsheet();
                CreateListSpreadsheet(spreadsheet);
                CreateSheetList(spreadsheet);
            };

            Bind();

            // In case something is updated
            SpreadsheetLoader.OnSpreadsheetDataSavedOnDisk += spreadsheet =>
            {
                if (GoogleDocConnectorLocalization.SpreadsheetId.Equals(spreadsheet.Id))
                {
                    Bind();
                }
            };
            
            CheckLocalizationCacheFile();
        }

        void Bind()
        {
            Bind(GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId) ?? new Spreadsheet());
        }

        void Bind(Spreadsheet spreadsheet)
        {
            m_Spinner.style.display = DisplayStyle.None;
            m_RefreshButton.clicked += () => { OnSpreadsheetRefreshClick(spreadsheet); };
            m_OpenBtn.clicked += () => Application.OpenURL(spreadsheet.Url);
            spreadsheet.OnSyncStateChange += LocalizationSpinner;
            CreateListSpreadsheet(spreadsheet);
            CreateSheetList(spreadsheet);
            BindDocumentInfo(spreadsheet);
        }

        void LocalizationSpinner(Spreadsheet spr)
        {
            m_Spinner.style.display = spr.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            if (spr.Synced || spr.SyncedWithError)
            {
                BindDocumentInfo(spr);
            }
            else
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-yellow");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.NotSyncedStringStatus;
            }
        }

        void BindDocumentInfo(Spreadsheet spreadsheet)
        {
            SelectingComponentsRender(m_SpreadsheetField.value);
            if (spreadsheet.Id == null)
            {
                return;
            }

            if (spreadsheet.SyncDateTime == DateTime.MinValue)
            {
                m_SpreadsheetDate.text = Spreadsheet.NotSyncedStringStatus;
            }
            else
            {
                m_SpreadsheetDate.text = spreadsheet.SyncDateTime.ToString("dddd, MMMM d, yyyy HH:mm:ss",
                    CultureInfo.CreateSpecificCulture("en-US"));
            }

            if (!string.IsNullOrEmpty(spreadsheet.LastSyncMachineName)) { m_SpreadsheetLastSyncMachineName.text = $"| {spreadsheet.LastSyncMachineName}"; }

            if (spreadsheet.SyncedWithError)
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-red");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.SyncedWithErrorStringStatus;
            }
            else if (spreadsheet.Synced)
            {
                m_SpreadsheetStatusIcon.ClearClassList();
                m_SpreadsheetStatusIcon.AddToClassList("status-icon-green");
                m_SpreadsheetStatusIcon.tooltip = Spreadsheet.SyncedStringStatus;
            }

            try
            {
                var loc = LocalizationClient.Default;
                if (loc.Sections.Any())
                {
                    m_ListSheet.Clear();
                    m_LabelSheet.text = $"{spreadsheet.Sheets.Count()} Sheets";
                    foreach (var sheet in spreadsheet.Sheets)
                    {
                        var countTokens = sheet.Rows.Count(r => r.Cells.Any());
                        var el = new SelectableLabel { text = $"{sheet.Name} - {countTokens}", tooltip = $"This sheet has {countTokens} tokens" };
                        el.AddManipulator(new ContextualMenuManipulator(evt =>
                        {
                            evt.menu.AppendAction("Copy", (x) =>
                            {
                                GUIUtility.systemCopyBuffer = sheet.Name;
                            });
                        }));
                        el.AddToClassList("sheet-element");
                        m_ListSheet.Add(el);
                    }
                }

                SelectedLang(loc.CurrentLanguage);
            }
            catch (Exception ex)
            {
                m_LocalizationError.Text = ex.Message;
                m_SpreadsheetBottomPanel.style.display =
                    m_DocumentInfo.style.display =
                        m_LocalizationWarning.style.display = DisplayStyle.None;
                m_LocalizationError.style.display = DisplayStyle.Flex;
            }
        }

        void CreateListSpreadsheet(Spreadsheet spreadsheet)
        {
            m_LabelChooseSpreadsheet.style.display = DisplayStyle.None;
            m_ListSpreadsheet.Clear();
            var listName = GoogleDocConnectorSettings.Instance.Spreadsheets.Where(v => v.Name != "<Spreadsheet>").Select(v => v.Name).ToList();
            listName.Add(k_DefaultSpreadsheetField);
            m_SpreadsheetField = new PopupField<string>("", listName, 0) { value = spreadsheet.Name ?? k_DefaultSpreadsheetField };

            m_SpreadsheetField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                SelectingComponentsRender(evt.newValue);

                spreadsheet.OnSyncStateChange -= LocalizationSpinner;
                m_Spinner.style.display = DisplayStyle.None;

                Spreadsheet localizationSpreadsheet = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s.Name == evt.newValue);
                if (localizationSpreadsheet != null)
                {
                    GoogleDocConnectorLocalization.SetSpreadsheet(localizationSpreadsheet.Id, k_DefaultLocalizationSheetId);
                    Bind(localizationSpreadsheet);
                }
            });
            m_ListSpreadsheet.Add(m_SpreadsheetField);
            ListSpreadsheetAvailability();
        }

        void CreateSheetList(Spreadsheet spreadsheet)
        {
            m_ListSheetIdVisualElement.Clear();
            if (string.IsNullOrEmpty(spreadsheet.Name) || spreadsheet.m_Sheets.Count == 0)
            {
                m_ListSheetIdVisualElement.style.display = DisplayStyle.None;
                return;
            }

            var sheetNames = spreadsheet.m_Sheets.Select(v => v.Name).ToList();
            Spreadsheet localization = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s == spreadsheet);
            string selectedSheetName = GoogleDocConnectorLocalization.SheetId == k_DefaultLocalizationSheetId ? 
                sheetNames[0] : spreadsheet.m_Sheets.FirstOrDefault(s => s.Id == GoogleDocConnectorLocalization.SheetId)?.Name;

            
            if (localization != null)
            {
                int selectedSheetId = localization.m_Sheets.First(s => s.Name == selectedSheetName).Id;
                GoogleDocConnectorLocalization.SetSpreadsheet(localization.Id, selectedSheetId);
            }

            m_LocalizationSpreadsheetId = new PopupField<string>("", sheetNames, 0) { value = selectedSheetName };

            m_LocalizationSpreadsheetId.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                Spreadsheet localizationSpreadsheet = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s == spreadsheet);
                if (localizationSpreadsheet != null)
                {
                    int selectedSheetId = evt.newValue == k_DefaultSpreadsheetField ? k_DefaultLocalizationSheetId : localizationSpreadsheet.m_Sheets.First(s => s.Name == evt.newValue).Id;
                    GoogleDocConnectorLocalization.SetSpreadsheet(localizationSpreadsheet.Id, selectedSheetId);
                }
            });

            m_ListSheetIdVisualElement.Add(m_LocalizationSpreadsheetId);
        }

        void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.LoadAsync(true).ContinueWith(_ => _);
        }

        void ChosenDefault(string newValue)
        {
            m_LocalizationError.style.display = DisplayStyle.None;
            m_SpreadsheetPanel.ClearClassList();
            if (newValue == k_DefaultSpreadsheetField)
            {
                m_LabelChooseSpreadsheet.style.display = DisplayStyle.Flex;
                m_DocumentInfo.style.display =
                    m_SpreadsheetBottomPanel.style.display =
                        m_RefreshButton.style.display =
                            m_OpenBtn.style.display =
                                m_ListSheetIdVisualElement.style.display =
                                    m_LocalizationWarning.style.display = DisplayStyle.None;
                GoogleDocConnectorLocalization.SetSpreadsheet("", k_DefaultLocalizationSheetId);
            }
            else
            {
                m_LabelChooseSpreadsheet.style.display =
                    m_LocalizationWarning.style.display = DisplayStyle.None;
                m_DocumentInfo.style.display =
                    m_SpreadsheetBottomPanel.style.display =
                        m_RefreshButton.style.display =
                            m_OpenBtn.style.display =
                                m_ListSheetIdVisualElement.style.display = DisplayStyle.Flex;
                m_SpreadsheetPanel.AddToClassList("spreadsheet-panel");
            }
        }

        void ListSpreadsheetAvailability()
        {
            if (GoogleDocConnectorSettings.Instance.Spreadsheets.Any(v => v.Name != "<Spreadsheet>"))
            {
                m_NoCredentialsHelpBox.style.display =
                    m_LocalizationWarning.style.display = DisplayStyle.None;
                m_ListSpreadsheet.style.display =
                    m_DocumentInfo.style.display =
                        m_SpreadsheetBottomPanel.style.display =
                            m_RefreshButton.style.display =
                                m_OpenBtn.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_NoCredentialsHelpBox.style.display = DisplayStyle.Flex;
                m_ListSpreadsheet.style.display =
                    m_DocumentInfo.style.display =
                        m_SpreadsheetBottomPanel.style.display =
                            m_RefreshButton.style.display =
                                m_OpenBtn.style.display =
                                    m_LocalizationWarning.style.display = DisplayStyle.None;
            }
        }

        void SelectingComponentsRender(string newValue)
        {
            ListSpreadsheetAvailability();
            ChosenDefault(newValue);
        }

        void SelectedLang(string langNew)
        {
            var loc = LocalizationClient.Default;
            loc.SetLanguage(langNew);
            if (loc.Languages.Any())
            {
                m_ListLang.Clear();
                m_LabelLang.text = $"{loc.Languages.Count()} Languages";
                foreach (var lang in loc.Languages)
                {
                    var but = new Button { text = $"{lang}" };

                    but.AddManipulator(new ContextualMenuManipulator(evt =>
                    {
                        evt.menu.AppendAction("Copy", (x) =>
                        {
                            GUIUtility.systemCopyBuffer = lang;
                        });
                    }));

                    but.AddToClassList(lang == langNew ? "lang-element-selected" : "lang-element");

                    but.clicked += () => { SelectedLang(lang); };
                    m_ListLang.Add(but);
                }
            }
        }
        
        void CheckLocalizationCacheFile()
        {
            if (string.IsNullOrEmpty(GoogleDocConnectorLocalization.SpreadsheetId) || GoogleDocConnectorLocalization.SheetId == k_DefaultLocalizationSheetId) return;

            var localizationSpreadsheet = GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId);
            if (!localizationSpreadsheet.IsSpreadsheetFileExist())
            {
                DisplayWarning("No cached localization file");
            }
        }

        void DisplayWarning(string warningMessage)
        {
            m_LocalizationWarning.Text = warningMessage;

            m_DocumentInfo.style.display =
                m_SpreadsheetBottomPanel.style.display =
                    m_LocalizationError.style.display = DisplayStyle.None;

            m_RefreshButton.style.display =
                m_OpenBtn.style.display =
                    m_ListSheetIdVisualElement.style.display =
                        m_LocalizationWarning.style.display = DisplayStyle.Flex;
        }
    }
}
