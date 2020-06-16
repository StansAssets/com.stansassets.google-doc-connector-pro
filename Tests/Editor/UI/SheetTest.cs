using NUnit.Framework;
using StansAssets.GoogleDoc;

namespace StansAssets.GoogleDoc.EditorTests
{
    public class SheetTest
    {
        const string k_SpreadsheetId1 = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_SpreadsheetId3 = "123456789";
        const string k_SpreadsheetId4 = "szsdgdgsfdsgsdgdsgsdg";
        const string k_RangeName = "Bike3";
        
        [OneTimeSetUp]
        public void Setup()
        {
            var connector = GoogleDocConnectorSettings.Instance;
            var spreadsheet = connector.GetSpreadsheet(k_SpreadsheetId1);
            spreadsheet = spreadsheet ?? connector.CreateSpreadsheet(k_SpreadsheetId1);
            spreadsheet.Load();
            spreadsheet = connector.GetSpreadsheet(k_SpreadsheetId2);
            spreadsheet = spreadsheet ?? connector.CreateSpreadsheet(k_SpreadsheetId2);
            spreadsheet.Load();
            spreadsheet = connector.GetSpreadsheet(k_SpreadsheetId3);
            spreadsheet = spreadsheet ?? connector.CreateSpreadsheet(k_SpreadsheetId3);
            spreadsheet.Load();
            spreadsheet = connector.GetSpreadsheet(k_SpreadsheetId4);
            spreadsheet = spreadsheet ?? connector.CreateSpreadsheet(k_SpreadsheetId4);
            spreadsheet.Load();
            System.Threading.Thread.Sleep(5000);
        }
        
        [OneTimeTearDown]
        public void Teardown()
        {
            
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(k_SpreadsheetId1);
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(k_SpreadsheetId2);
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(k_SpreadsheetId3);
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(k_SpreadsheetId4);
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        public void GetRangeName(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var rangeName = sheet.GetRange(k_RangeName);
            Assert.True(rangeName.Count > 0, "Expected rangeName spreadsheet Count > 0 but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId2)]
        public void GetRangeNameEmpty(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var rangeName = sheet.GetRange(k_RangeName);
            Assert.False(rangeName.Count > 0, "Expected rangeName spreadsheet Count > 0 but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void GetCell(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetCell(0, 0);
            Assert.False(string.IsNullOrEmpty(cell.ToString()), "Expected get first cell from spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void GetCellEmpty(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetCell(1000, 1000);
            Assert.True(cell == null, "Unexpected get first cell from spreadsheet but it was");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void GetColumn(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var column = sheet.GetColumn(0);
            Assert.True(column.Count > 0, "Expected get first column from sheet but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void GetColumnEmpty(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var column = sheet.GetColumn(-1);
            Assert.False(column.Count > 0, "Unexpected get first column from sheet but it was");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void GetRow(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnectorSettings.Instance.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var row = sheet.GetRow(0);
            Assert.True(row.Count > 0, "Expected get first row from sheet but it was not");
        }
        
        [Test]
        [TestCase(k_SpreadsheetId1)]
        [TestCase(k_SpreadsheetId2)]
        public void GetRowEmpty(string spreadsheetId)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(spreadsheetId);
            var sheet = spreadsheet.GetSheet(0);
            var row = sheet.GetRow(-1);
            Assert.False(row.Count > 0, "Unexpected get first row from sheet but it was");
        }
    }
}
