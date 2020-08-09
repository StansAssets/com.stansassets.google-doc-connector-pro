using NUnit.Framework;
using UnityEngine;

namespace StansAssets.GoogleDoc.EditorTests
{
    public class SpreadsheetTest
    {
        const string k_SpreadsheetId1 = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_SpreadsheetId3 = "123456789";
        const string k_SpreadsheetId4 = "szsdgdgsfdsgsdgdsgsdg";

        [OneTimeSetUp]
        public void Setup()
        {
            AddSpreadsheet(k_SpreadsheetId1);
            AddSpreadsheet(k_SpreadsheetId2);
            AddSpreadsheet(k_SpreadsheetId3);
            AddSpreadsheet(k_SpreadsheetId4);
            System.Threading.Thread.Sleep(5000);
        }

        void AddSpreadsheet(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            spreadsheet = spreadsheet ?? GoogleDocConnectorEditor.CreateSpreadsheet(spreadsheetId);
            spreadsheet.Load();
        }
        
        [OneTimeTearDown]
        public void Teardown()
        {
            
            GoogleDocConnectorEditor.RemoveSpreadsheet(k_SpreadsheetId1);
            GoogleDocConnectorEditor.RemoveSpreadsheet(k_SpreadsheetId2);
            GoogleDocConnectorEditor.RemoveSpreadsheet(k_SpreadsheetId3);
            GoogleDocConnectorEditor.RemoveSpreadsheet(k_SpreadsheetId4);
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void CheckSyncedSpreadsheet(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            Debug.Log(spreadsheet.Id);
            Debug.Log("spreadsheet.Synced " + spreadsheet.Synced);
            Assert.True(spreadsheet.Synced, "Expected synced spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId3)]
        [TestCase(k_SpreadsheetId4)]
        public void CheckSyncedWithErrorSpreadsheet(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            Assert.True(spreadsheet.SyncedWithError, "Expected synced wit error spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void CheckFirstSheetSpreadsheet(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            Assert.True(spreadsheet.HasSheet(0), "Expected get first sheet from spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId3)]
        [TestCase(k_SpreadsheetId4)]
        public void CheckNoFirstSheetSpreadsheet(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            Assert.False(spreadsheet.HasSheet(0), "Unexpected get first sheet from spreadsheet but it was");
        }
    }
}