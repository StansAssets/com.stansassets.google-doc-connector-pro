using NUnit.Framework;
using StansAssets.GoogleDoc.Localization;

namespace StansAssets.GoogleDoc.Tests
{
    public class LocalizationClientTest
    {
        const string k_SpreadsheetId = "1eAqrhsWRP5hw9T_AuCMpJRSslh2_9S2dFaUkw_Vml4c";
        Spreadsheet m_Spreadsheet;
        LocalizationClient m_Client;
        string m_OldLocalizationClientId;

        [OneTimeSetUp]
        public void Setup()
        {
            m_Spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId);
            m_Spreadsheet.Load();
            m_OldLocalizationClientId = LocalizationSettings.Instance.SpreadsheetId;
            LocalizationSettings.Instance.SpreadsheetIdSet(k_SpreadsheetId);
            m_Client = LocalizationClient.Default;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            LocalizationSettings.Instance.SpreadsheetIdSet(m_OldLocalizationClientId);
            GoogleDocConnectorEditor.RemoveSpreadsheet(k_SpreadsheetId);
        }

        [Test]
        [TestCase("singin", "en", "Sign in")]
        [TestCase("singin", "ru", "Войти")]
        [TestCase("singin", "hz", "לבה")]
        [TestCase("add_content", "en", "Add your own content")]
        [TestCase("successfullyLabel", "en", "Successfully!")]
        public void GetLocalizedString(string token, string langCode, string response)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }

        [Test]
        [TestCase("add_content", "en", "Add your own content", TextType.Default)]
        [TestCase("add_content", "en", "add your own content", TextType.ToLower)]
        [TestCase("add_content", "en", "ADD YOUR OWN CONTENT", TextType.ToUpper)]
        [TestCase("add_content", "en", "Add your own content", TextType.WithCapital)]
        [TestCase("add_content", "en", "Add Your Own Content", TextType.EachWithCapital)]
        public void GetLocalizedString(string token, string langCode, string response, TextType textType)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, textType);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }

        [Test]
        [TestCase("singin", "en", "Sign in", "Lobby")]
        [TestCase("singin", "en", "Sign in", "General")]
        [TestCase("sotringTableInfo", "ru", "Это ваш контейнер ресурсов. Вы можете загружать контент нажав на ⊕ и перемещать его в комнату. Чтобы удалить содержимое из вашей комнаты сначала выберите это, а затем нажмите на урну. Чтобы удалить содержимое из таблицы содержимого -  просто выберите и перетащите в корзину.", "Room")]
        [TestCase("saved_to_camera_roll", "en", "Photo was saved to your device camera roll", "AR")]
        public void GetLocalizedString(string token, string langCode, string response, string section)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, section);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }

        [Test]
        [TestCase("room_arrangement", "en", "Room arrangement", TextType.Default, "Room Creator")]
        [TestCase("room_arrangement", "en", "room arrangement", TextType.ToLower, "Room Creator")]
        [TestCase("room_arrangement", "en", "ROOM ARRANGEMENT", TextType.ToUpper, "Room Creator")]
        [TestCase("room_arrangement", "en", "Room arrangement", TextType.WithCapital, "Room Creator")]
        [TestCase("room_arrangement", "en", "Room Arrangement", TextType.EachWithCapital, "Room Creator")]
        public void GetLocalizedString(string token, string langCode, string response, TextType textType, string section)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, section, textType);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }

        [Test]
        [TestCase("welcomeMessage", "en", "Vasa joined in Ukraine. Welcome!", "MatchMaking", new[] { "Vasa", "in Ukraine" })]
        [TestCase("welcomeMessage", "en", "2 joined 4. Welcome!", "MatchMaking", new object[] { 2, "4" })]
        [TestCase("welcomeMessage", "en", "Family joined together. Welcome!", "MatchMaking", new[] { "Family", "together" })]
        public void GetLocalizedString(string token, string langCode, string response, string section, object[] args)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, section, args);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }
    }
}
