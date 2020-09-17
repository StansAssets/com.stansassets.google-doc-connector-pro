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
        public LocalizationTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/LocalizationTab")
        {
            m_ListSpreadsheet = this.Q<VisualElement>("list-spreadsheet");

            Bind(GoogleDocConnector.GetSpreadsheet(LocalizationSettings.Instance.SpreadsheetId));
        }

        void Bind(Spreadsheet localization)
        {
            var refreshButton = this.Q<Button>("refreshBtn");
            refreshButton.clicked += () => { OnSpreadsheetRefreshClick(localization); };
            var openBtn = this.Q<Button>("openBtn");
            openBtn.clicked += () => { Application.OpenURL(GoogleDocConnector.GetSpreadsheetWebUrl(localization.Id)); };
            
           m_ListSpreadsheet.Clear();
           var listName = GoogleDocConnectorSettings.Instance.Spreadsheets.Select(v => v.Name).ToList();
           listName.Add("");
           var spreadsheetField = new PopupField<string>("", listName, 0) { value = localization.Name };
           spreadsheetField.RegisterCallback<ChangeEvent<string>>((evt) =>
           {
               spreadsheetField.value = evt.newValue;
               var newLocalization = GoogleDocConnectorSettings.Instance.Spreadsheets.FirstOrDefault(s => s.Name == evt.newValue);
               if (newLocalization != null) LocalizationSettings.Instance.SpreadsheetIdSet(newLocalization.Id);
           });
           m_ListSpreadsheet.Add(spreadsheetField);
        }
        void OnSpreadsheetRefreshClick(Spreadsheet spreadsheet)
        {
            spreadsheet.LoadAsync().ContinueWith(_ => {spreadsheet.CacheDocument();});
        }
    }
}