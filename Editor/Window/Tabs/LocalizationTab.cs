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

namespace StansAssets.GoogleDoc
{
    public class LocalizationTab : BaseTab
    {
        readonly VisualElement m_ListSpreadsheet;

        readonly Label m_LabelLang;
        readonly VisualElement m_ListLang;
        readonly Label m_LabelSheet;
        readonly VisualElement m_ListSheet;
        readonly VisualElement m_DocumentInfo;

        readonly HelpBox m_LocalizationError;

        readonly Button m_RefreshButton;
        readonly Button m_OpenBtn;
        readonly VisualElement m_Spinner;

        readonly Label m_LabelChooseSpreadsheet;
        PopupField<string> m_SpreadsheetField;

        readonly HelpBox m_NoCredentialsHelpBox;

        readonly Label m_SpreadsheetDate;
        readonly Label m_SpreadsheetLastSyncMachineName;
        readonly Label m_SpreadsheetStatusIcon;
        readonly VisualElement m_SpreadsheetBottomPane;

        const string k_DefaultSpreadsheetField = "None";

        public LocalizationTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/LocalizationTab")
        {
            m_ListSpreadsheet = this.Q<VisualElement>("list-spreadsheet");
            m_LabelLang = this.Q<Label>("labelLang");
            m_ListLang = this.Q<VisualElement>("listLang");
            m_LabelSheet = this.Q<Label>("labelSheet");
            m_ListSheet = this.Q<VisualElement>("listSheet");
            m_DocumentInfo = this.Q<VisualElement>("document-info");
            m_LocalizationError = this.Q<HelpBox>("localization-error");
            m_RefreshButton = this.Q<Button>("refreshBtn");
            m_OpenBtn = this.Q<Button>("openBtn");
            m_LabelChooseSpreadsheet = this.Q<Label>("choose-spreadsheet");
            m_Spinner = this.Q<LoadingSpinner>("loadingSpinner");
            m_NoCredentialsHelpBox = this.Q<HelpBox>("no-spreadsheets-note");
            m_SpreadsheetBottomPane = this.Q<VisualElement>("spreadsheetBottomPanel");
            m_SpreadsheetDate = this.Q<Label>("spreadsheetDate");
            m_SpreadsheetLastSyncMachineName = this.Q<Label>("lastSyncMachineName");
            m_SpreadsheetStatusIcon = this.Q<Label>("statusIcon");
            GoogleDocConnectorEditor.SpreadsheetsChange += () => { CreateListSpreadsheet(GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId) ?? new Spreadsheet()); };

            Bind();

            // In case something is updated 
            SpreadsheetLoader.OnSpreadsheetDataSavedOnDisk += spreadsheet =>
            {
                if (GoogleDocConnectorLocalization.SpreadsheetId.Equals(spreadsheet.Id))
                {
                    Bind();
                }
            };
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
            BindDocumentInfo(spreadsheet);
        }

        void LocalizationSpinner(Spreadsheet spr)
        {
            m_Spinner.style.display = spr.InProgress ? DisplayStyle.Flex : DisplayStyle.None;
            if (spr.Synced)
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
            ChosenDefault(m_SpreadsheetField.value);
            if (spreadsheet.Id == null)
            {
                return;
            }
            
            m_SpreadsheetDate.text = spreadsheet.SyncDateTime.HasValue ? spreadsheet.SyncDateTime.Value.ToString("dddd, MMMM d, yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) : $"[{Spreadsheet.NotSyncedStringStatus}]";
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
                if (loc.Sheets.Any())
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
                m_SpreadsheetBottomPane.style.display =
                    m_DocumentInfo.style.display = DisplayStyle.None;
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
                ChosenDefault(evt.newValue);

                spreadsheet.OnSyncStateChange -= LocalizationSpinner;
                m_Spinner.style.display = DisplayStyle.None;

                var newLocalization = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s.Name == evt.newValue);
                if (newLocalization != null)
                {
                    GoogleDocConnectorLocalization.SpreadsheetIdSet(newLocalization.Id);
                    Bind(newLocalization);
                }
            });
            m_ListSpreadsheet.Add(m_SpreadsheetField);
            ListSpreadsheetAvailability();
        }

        void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.LoadAsync(true).ContinueWith(_ => _);
        }

        void ChosenDefault(string newValue)
        {
            m_LocalizationError.style.display = DisplayStyle.None;
            if (newValue == k_DefaultSpreadsheetField)
            {
                m_LabelChooseSpreadsheet.style.display = DisplayStyle.Flex;
                GoogleDocConnectorLocalization.SpreadsheetIdSet("");
                m_DocumentInfo.style.display =
                    m_SpreadsheetBottomPane.style.display =
                        m_RefreshButton.style.display =
                            m_OpenBtn.style.display = DisplayStyle.None;
            }
            else
            {
                m_LabelChooseSpreadsheet.style.display = DisplayStyle.None;
                m_DocumentInfo.style.display =
                    m_SpreadsheetBottomPane.style.display =
                        m_RefreshButton.style.display =
                            m_OpenBtn.style.display = DisplayStyle.Flex;
            }

            ListSpreadsheetAvailability();
        }

        void ListSpreadsheetAvailability()
        {
            if (GoogleDocConnectorSettings.Instance.Spreadsheets.Any(v => v.Name != "<Spreadsheet>"))
            {
                m_NoCredentialsHelpBox.style.display = DisplayStyle.None;
                m_ListSpreadsheet.style.display =
                    m_LabelChooseSpreadsheet.style.display =
                        m_DocumentInfo.style.display =
                            m_SpreadsheetBottomPane.style.display =
                                m_RefreshButton.style.display =
                                    m_OpenBtn.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_NoCredentialsHelpBox.style.display = DisplayStyle.Flex;
                m_ListSpreadsheet.style.display =
                    m_LabelChooseSpreadsheet.style.display =
                        m_DocumentInfo.style.display =
                            m_SpreadsheetBottomPane.style.display =
                                m_RefreshButton.style.display =
                                    m_OpenBtn.style.display = DisplayStyle.None;
            }
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
    }
}
