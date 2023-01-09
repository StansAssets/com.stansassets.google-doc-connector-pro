using NUnit.Framework;
using StansAssets.GoogleDoc.Editor;
using StansAssets.GoogleDoc.Localization;

namespace StansAssets.GoogleDoc.Tests
{
    public class LocalizationClientTest
    {
        const string k_SpreadsheetId = "1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo"; 
        Spreadsheet m_Spreadsheet;
        LocalizationClient m_Client;
        string m_OldLocalizationClientId;

        [OneTimeSetUp]
        public void Setup()
        {
            m_Spreadsheet = GoogleDocConnectorEditor.CreateSpreadsheet(k_SpreadsheetId);
            m_Spreadsheet.Load();
            m_OldLocalizationClientId = GoogleDocConnectorLocalization.SpreadsheetId;
            GoogleDocConnectorLocalization.SetSpreadsheet(k_SpreadsheetId);
            m_Client = LocalizationClient.Default;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            GoogleDocConnectorLocalization.SetSpreadsheet(m_OldLocalizationClientId);
            GoogleDocConnectorEditor.RemoveSpreadsheet(k_SpreadsheetId);
        }

        [Test]
        [TestCase("signInUsing", "en", "Sign in using")]
        [TestCase("signInUsing", "ru", "Войдите используя")]
        [TestCase("signInUsing", "gr", "Melden Sie sich mit an")]
        [TestCase("alreadyHaveAcc", "en", "have an account")]
        [TestCase("skip", "en", "skip")]
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
        [TestCase("signInUsing", "en", "Sign in using", "Lobby")]
        [TestCase("signInUsing", "en", "Sign in using", "Auth")]
        [TestCase("sotringTableInfo", "ru", "Зарегистрируйтесь через электронную почту", "Room")]
        [TestCase("saved_to_camera_roll", "en", "Photo was saved to your device camera roll", "AR")]
        public void GetLocalizedString(string token, string langCode, string response, string section)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, section);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }

        [Test]
        [TestCase("room_arrangement", "en", "Room arrangement", TextType.Default, "Room")]
        [TestCase("room_arrangement", "en", "room arrangement", TextType.ToLower, "Room")]
        [TestCase("room_arrangement", "en", "ROOM ARRANGEMENT", TextType.ToUpper, "Room")]
        [TestCase("room_arrangement", "en", "Room arrangement", TextType.WithCapital, "Room")]
        [TestCase("room_arrangement", "en", "Room Arrangement", TextType.EachWithCapital, "Room")]
        public void GetLocalizedString(string token, string langCode, string response, TextType textType, string section)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, section, textType);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }

        [Test]
        [TestCase("welcomeMessage", "en", "Vasa joined in Ukraine. Welcome!", "Lobby", new[] { "Vasa", "in Ukraine" })]
        [TestCase("welcomeMessage", "en", "2 joined 4. Welcome!", "Lobby", new object[] { 2, "4" })]
        [TestCase("welcomeMessage", "en", "Family joined together. Welcome!", "Lobby", new[] { "Family", "together" })]
        public void GetLocalizedString(string token, string langCode, string response, string section, object[] args)
        {
            m_Client.SetLanguage(langCode);
            var result = m_Client.GetLocalizedString(token, section, args);
            Assert.True(response.Equals(result), "Token does not match expected response");
        }
    }
}
