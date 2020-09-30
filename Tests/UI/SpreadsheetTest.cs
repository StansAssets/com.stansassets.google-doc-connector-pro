using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace StansAssets.GoogleDoc.Tests
{
    public class SpreadsheetTest
    {
        const string k_SpreadsheetId1 = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_SpreadsheetId2 = "1QuJ0M7s25KxX_E0mRtmJiZQciKjvVt77yKMlUkvOWrc";
        const string k_SpreadsheetId3 = "123456789";
        const string k_SpreadsheetId4 = "szsdgdgsfdsgsdgdsgsdg";
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
        [TestCase(1)]
        public void CheckSyncedSpreadsheet(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            Debug.Log(spreadsheet.Id);
            Debug.Log("spreadsheet.Synced " + spreadsheet.Synced);
            Assert.True(spreadsheet.Synced, "Expected synced spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void CheckSyncedWithErrorSpreadsheet(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            Assert.True(spreadsheet.SyncedWithError, "Expected synced wit error spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void CheckFirstSheetSpreadsheet(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            Assert.True(spreadsheet.HasSheet(0), "Expected get first sheet from spreadsheet but it was not");
        }
        
        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public void CheckNoFirstSheetSpreadsheet(int index)
        {
            var spreadsheet = m_Spreadsheets[index];
            Assert.False(spreadsheet.HasSheet(0), "Unexpected get first sheet from spreadsheet but it was");
        }
    }
}