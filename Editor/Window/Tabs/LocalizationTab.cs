using System.Linq;
using StansAssets.Plugins.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class LocalizationTab : BaseTab
    {
        readonly VisualElement m_ListSpreadsheet;
        readonly Label m_LabelLang;
        readonly VisualElement m_ListLang;
        readonly Label m_LabelSheet;
        readonly VisualElement m_ListSheet;

        public LocalizationTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/LocalizationTab")
        {
            m_ListSpreadsheet = this.Q<VisualElement>("list-spreadsheet");
            m_LabelLang = this.Q<Label>("labelLang");
            m_ListLang = this.Q<VisualElement>("listLang");
            m_LabelSheet = this.Q<Label>("labelSheet");
            m_ListSheet = this.Q<VisualElement>("listSheet");

            Bind(GoogleDocConnector.GetSpreadsheet(LocalizationSettings.Instance.SpreadsheetId) ?? new Spreadsheet());
        }

        void Bind(Spreadsheet spreadsheet)
        {
            var refreshButton = this.Q<Button>("refreshBtn");
            refreshButton.clicked += () => { OnSpreadsheetRefreshClick(spreadsheet); };
            var openBtn = this.Q<Button>("openBtn");
            openBtn.clicked += () => { Application.OpenURL(GoogleDocConnector.GetSpreadsheetWebUrl(spreadsheet.Id)); };
            GoogleDocConnectorEditor.s_SpreadsheetsChange += () => { CreateListSpreadsheet(spreadsheet); };
            CreateListSpreadsheet(spreadsheet);
            BindDocumentInfo(spreadsheet);
        }

        void BindDocumentInfo(Spreadsheet spreadsheet)
        {
            if (spreadsheet.Id == null)
            {
                return;
            }

            if (spreadsheet.Sheets.Any())
            {
                m_ListSheet.Clear();
                m_LabelSheet.text = $"{spreadsheet.Sheets.Count()} Sheets";
                var indexSheet = 0;
                foreach (var sh in spreadsheet.Sheets)
                {
                    indexSheet++;
                    var el = new Label { text = $"{indexSheet}. {sh.Name}" };
                    el.AddToClassList("lang-element");
                    m_ListSheet.Add(el);
                }
            }

            var loc = new LocalizationClient();
            if (loc.Languages.Any())
            {
                m_ListLang.Clear();
                m_LabelLang.text = $"{loc.Languages.Count()} Languages";
                for (var i = 0; i < loc.Languages.Count(); i++)
                {
                    var el = new Label { text = $"{i + 1}. {loc.Languages[i]}" };
                    el.AddToClassList("lang-element");
                    m_ListLang.Add(el);
                }
            }
        }

        void CreateListSpreadsheet(Spreadsheet spreadsheet)
        {
            m_ListSpreadsheet.Clear();
            var listName = GoogleDocConnectorSettings.Instance.Spreadsheets.Select(v => v.Name).ToList();
            listName.Add("");
            var spreadsheetField = new PopupField<string>("", listName, 0) { value = spreadsheet.Name ?? "" };
            spreadsheetField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                spreadsheetField.value = evt.newValue;
                var newLocalization = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s.Name == evt.newValue);
                if (newLocalization != null) LocalizationSettings.Instance.SpreadsheetIdSet(newLocalization.Id);
                Bind(newLocalization);
            });
            m_ListSpreadsheet.Add(spreadsheetField);
        }

        void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.LoadAsync(true).ContinueWith(_ => _);
        }
    }
}
