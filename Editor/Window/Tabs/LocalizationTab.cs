using System;
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

        HelpBox m_NoCredentialsHelpBox;

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
            GoogleDocConnectorEditor.SpreadsheetsChange += () => { CreateListSpreadsheet(GoogleDocConnector.GetSpreadsheet(LocalizationSettings.Instance.SpreadsheetId) ?? new Spreadsheet()); };

            Bind(GoogleDocConnector.GetSpreadsheet(LocalizationSettings.Instance.SpreadsheetId) ?? new Spreadsheet());
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
        }

        void BindDocumentInfo(Spreadsheet spreadsheet)
        {
            ChosenDefault(m_SpreadsheetField.value);
            if (spreadsheet.Id == null)
            {
                return;
            }

            try
            {
                var loc = LocalizationClient.Default;
                if (loc.Sheets.Any())
                {
                    m_ListSheet.Clear();
                    m_LabelSheet.text = $"{spreadsheet.Sheets.Count()} Sheets";
                    foreach (var nameSheet in loc.Sheets)
                    {
                        var el = new SelectableLabel() { text = $"{nameSheet}" };
                        el.AddManipulator(new ContextualMenuManipulator(evt =>
                        {
                            evt.menu.AppendAction("Copy", (x) =>
                            {
                                GUIUtility.systemCopyBuffer = nameSheet;
                            });
                        }));
                        el.AddToClassList("lang-element");
                        m_ListSheet.Add(el);
                    }
                }

                if (loc.Languages.Any())
                {
                    m_ListLang.Clear();
                    m_LabelLang.text = $"{loc.Languages.Count()} Languages";
                    foreach (var lang in loc.Languages)
                    {
                        var el = new SelectableLabel { text = $"{lang}" };
                        el.AddManipulator(new ContextualMenuManipulator(evt =>
                        {
                            evt.menu.AppendAction("Copy", (x) =>
                            {
                                GUIUtility.systemCopyBuffer = lang;
                            });
                        }));
                        el.AddToClassList("lang-element");
                        m_ListLang.Add(el);
                    }
                }
            }
            catch (Exception ex)
            {
                m_LocalizationError.Text = ex.Message;
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

            m_SpreadsheetField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                ChosenDefault(evt.newValue);

                spreadsheet.OnSyncStateChange -= LocalizationSpinner;
                m_Spinner.style.display = DisplayStyle.None;

                var newLocalization = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s.Name == evt.newValue);
                if (newLocalization != null)
                {
                    LocalizationSettings.Instance.SpreadsheetIdSet(newLocalization.Id);
                    Bind(newLocalization);
                }
            });
            m_ListSpreadsheet.Add(m_SpreadsheetField);
            ListSpreadsheetAvailability();
        }

        void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.LoadAsync(true).ContinueWith(_ =>
            {
                GoogleDocConnectorSettings.Save();
            });
        }

        void ChosenDefault(string newValue)
        {
            m_LocalizationError.style.display = DisplayStyle.None;
            if (newValue == k_DefaultSpreadsheetField)
            {
                m_LabelChooseSpreadsheet.style.display = DisplayStyle.Flex;
                LocalizationSettings.Instance.SpreadsheetIdSet("");
                m_DocumentInfo.style.display =
                    m_RefreshButton.style.display =
                        m_OpenBtn.style.display = DisplayStyle.None;
            }
            else
            {
                m_LabelChooseSpreadsheet.style.display = DisplayStyle.None;
                m_DocumentInfo.style.display =
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
                            m_RefreshButton.style.display =
                                m_OpenBtn.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_NoCredentialsHelpBox.style.display = DisplayStyle.Flex;
                m_ListSpreadsheet.style.display =
                    m_LabelChooseSpreadsheet.style.display =
                        m_DocumentInfo.style.display =
                            m_RefreshButton.style.display =
                                m_OpenBtn.style.display = DisplayStyle.None;
            }
        }
    }
}
