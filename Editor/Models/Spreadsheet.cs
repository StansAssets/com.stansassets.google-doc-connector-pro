using System;
using System.Collections.Generic;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    [Serializable]
    public class Spreadsheet
    {
        [SerializeField]
        List<Sheet> m_Sheets = new List<Sheet>();
        public IEnumerable<Sheet> Sheets => m_Sheets;

        [SerializeField]
        string m_Id;
        public string Id => m_Id;

        [SerializeField]
        string m_Name;
        public string Name => m_Name;

        const string k_DefaultName = "<Spreadsheet>";

        public Spreadsheet(string id)
        {
            m_Id = id;
            m_Name = k_DefaultName;
        }

        internal void SetName(string name)
        {
            m_Name = name;
        }

        internal Sheet GetOrCreateSheet(int sheetId)
        {
            foreach (var sheet in m_Sheets)
            {
                if (sheetId == sheet.Id)
                {
                    return sheet;
                }
            }

            //Create new Sheet if we don't have one
            var newSheet = new Sheet(sheetId);
            m_Sheets.Add(newSheet);
            return newSheet;
        }

        internal void Load()
        {
            var loader = new SpreadsheetLoader(this);
            loader.Load();
        }
    }
}
