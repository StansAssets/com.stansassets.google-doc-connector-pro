using System.Collections.Generic;
using StansAssets.GoogleDoc.Localization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StansAssets.GoogleDoc.Samples
{
    public class GoogleDocSampleLocalization : MonoBehaviour
    {
        public const string SpreadsheetId = "1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo";
        readonly List<string> m_Tokens = new List<string>() { "alreadyHaveAcc", "register", "letsGo" };
        LocalizationClient m_Client;
        public Dropdown Dropdown;
        public Button ButtonAdd;
        public InputField InputField;
        public Text TextTokens;
        [FormerlySerializedAs("PanelID")]
        public GameObject PanelId;

        // Start is called before the first frame update
        void Start()
        {
            try
            {
                GoogleDocConnector.GetSpreadsheet(SpreadsheetId);
                m_Client = LocalizationClient.Default;
            }
            catch
            {
                PanelId.SetActive(true);
            }

            Dropdown.ClearOptions();
            Dropdown.AddOptions(m_Client.Languages);
            Dropdown.onValueChanged.AddListener(DropdownChange);
            ButtonAdd.onClick.AddListener(AddToken);
            DropdownChange(0);
        }

        void AddToken()
        {
            var newToken = InputField.text;
            var newLoc = m_Client.GetLocalizedString(newToken);
            TextTokens.text += $"\n ● {newToken} : {newLoc}";
            m_Tokens.Add(newToken);
        }

        void DropdownChange(int position)
        {
            m_Client.SetLanguage(m_Client.Languages[position]);
            TextTokens.text = string.Empty;
            foreach (var token in m_Tokens)
            {
                TextTokens.text += $"\n ● {token} : {m_Client.GetLocalizedString(token)}";
            }
        }

    }
}
