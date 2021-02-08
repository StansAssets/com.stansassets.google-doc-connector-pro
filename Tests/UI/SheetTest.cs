using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StansAssets.GoogleDoc.Editor;

namespace StansAssets.GoogleDoc.Tests
{
    public class SheetTest
    {
        const string k_SpreadsheetId1 = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_SpreadsheetId3 = "123456789";
        const string k_SpreadsheetId4 = "szsdgdgsfdsgsdgdsgsdg";
        const string k_RangeName = "Bike3";
        readonly List<Spreadsheet> m_Spreadsheets = new List<Spreadsheet>();

        [OneTimeSetUp]
        public void Setup()
        {
            AddSpreadsheet(k_SpreadsheetId1);
            AddSpreadsheet(k_SpreadsheetId2);
            AddSpreadsheet(k_SpreadsheetId3);
            AddSpreadsheet(k_SpreadsheetId4);
        }

        void AddSpreadsheet(string spreadsheetId)
        {
            var spreadsheet = new Spreadsheet(spreadsheetId);
            spreadsheet.Load();
            m_Spreadsheets.Add(spreadsheet);
        }

        [Test]
        [TestCase(0)]
        public void GetRangeName(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var rangeName = sheet.GetNamedRange(k_RangeName);
            Assert.True(rangeName.Cells.Count() > 1, "Expected rangeName spreadsheet Count > 1 but it was not");
        }

        [Test]
        [TestCase(1)]
        public void GetRangeNameEmpty(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var rangeName = sheet.GetNamedRange(k_RangeName);
            Assert.False(rangeName != null, "Expected rangeName spreadsheet Count < 1 but it was not");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void GetCell(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetCell(0, 0);
            Assert.False(string.IsNullOrEmpty(cell.GetValue<string>()), "Expected get first cell from spreadsheet but it was not");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void GetCellEmpty(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetCell(1000, 1000);
            Assert.True(string.IsNullOrEmpty(cell.GetValue<string>()), "Unexpected get thousandth cell from spreadsheet but it was");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void GetColumn(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var column = sheet.GetColumn(0);
            Assert.True(column.Count > 0, "Expected get first column from sheet but it was not");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void GetColumnEmpty(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var column = sheet.GetColumn(-1);
            Assert.False(column.Count > 0, "Unexpected get first column from sheet but it was");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void GetRow(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var row = sheet.GetRow(0);
            Assert.True(row.Count > 0, "Expected get first row from sheet but it was not");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void GetRowEmpty(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            var sheet = spreadsheet.GetSheet(0);
            var row = sheet.GetRow(-1);
            Assert.False(row.Count > 0, "Unexpected get first row from sheet but it was");
        }

        [Test]
        [TestCase("A1")]
        [TestCase("b1")]
        public void GetValueT(string cellName)
        {
            var spreadsheet = m_Spreadsheets[0];
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetCell(cellName);
            var res = cell.Value.StringValue;
            var res2 = cell.Value.GetValue<double>();
            var res3 = cell.Value.GetValue<string>();
            var cell1 = sheet.GetCell("c1");
            var res11 = cell1.Value.GetValue<bool>();
            var cell2 = sheet.GetCell("a2");
            var res21 = cell2.Value.GetValue<DateTime>();
            Assert.True(!string.IsNullOrEmpty(res) && !res11  && res2 > 0 && !string.IsNullOrEmpty(res3) && res21 == new DateTime(2020, 08, 09), "Expected get first cell from spreadsheet but it was not");
        }

        [Test]
        [TestCase("A")]
        [TestCase("b")]
        public void GetColumnByName(string columnName)
        {
            var spreadsheet = m_Spreadsheets[0];
            var sheet = spreadsheet.GetSheet(0);
            var column = sheet.GetColumn(columnName);
            Assert.True(column.Count > 0, "Expected get first cell from spreadsheet but it was not");
        }

        [Test]
        [TestCase("A1:B2")]
        [TestCase("A:B")]
        [TestCase("1:2")]
        public void GetRangeByName(string nameRange)
        {
            var spreadsheet = m_Spreadsheets[0];
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetRange(nameRange);
            Assert.True(cell.Count > 0, "Expected get first cell from spreadsheet but it was not");
        }

        [Test]
        [TestCase("A1")]
        [TestCase("b1")]
        public void GetCellByName(string cellName)
        {
            var spreadsheet = m_Spreadsheets[0];
            var sheet = spreadsheet.GetSheet(0);
            var cell = sheet.GetCell(cellName);
            var res = cell.Value.FormattedValue;
            Assert.True(!string.IsNullOrEmpty(res), "Expected get first cell from spreadsheet but it was not");
        }
    }
}
