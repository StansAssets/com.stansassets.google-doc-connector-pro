using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Resource that represents a spreadsheet.
    /// </summary>
    [Serializable]
    public class Spreadsheet
    {
        /// <summary>
        /// The spreadsheet sync states.
        /// </summary>
        public enum SyncState
        {
            /// <summary>
            /// The spreadsheet is fully synced
            /// </summary>
            Synced,

            /// <summary>
            /// The spreadsheet is not yet synced
            /// </summary>
            NotSynced,

            /// <summary>
            /// Last spreadsheet sync attempt was finished with an error.
            /// </summary>
            SyncedWithError,

            /// <summary>
            /// The spreadsheet sync is currently in progress.
            /// </summary>
            InProgress
        }

        internal const string SyncedStringStatus = "Synced";
        internal const string NotSyncedStringStatus = "Not Synced";
        internal const string SyncedWithErrorStringStatus = "Synced With Error";

        const string k_DefaultName = "<Spreadsheet>";

        /// <summary>
        /// An event to track spreadsheet sync state.
        /// </summary>
        public event Action<Spreadsheet> OnSyncStateChange = delegate { };

        [SerializeField]
        SyncState m_State;

        /// <summary>
        /// The spreadsheet sync state. 
        /// </summary>
        public SyncState State => m_State;

        [SerializeField]
        string m_SyncErrorMassage;

        /// <summary>
        /// If last spreadsheet sync attempt was finished with an error,
        /// this property will contains a string with error details.
        /// The default value is `null`.
        /// </summary>
        public string SyncErrorMassage => m_SyncErrorMassage;

        [SerializeField]
        List<Sheet> m_Sheets = new List<Sheet>();

        /// <summary>
        /// The spreadsheet sheets list.
        /// </summary>
        public IEnumerable<Sheet> Sheets => m_Sheets;

        [SerializeField]
        string m_Id;

        /// <summary>
        /// The ID of the spreadsheet.
        /// </summary>
        public string Id => m_Id;

        [SerializeField]
        string m_Url;

        /// <summary>
        /// The spreadsheet absolute web URL.
        /// </summary>
        public string Url => m_Url;

        [SerializeField]
        string m_Name;

        /// <summary>
        /// The name of the spreadsheet.
        /// </summary>
        public string Name => m_Name;

        [SerializeField]
        string m_Path;

        internal string Path => m_Path;

        [SerializeField]
        string m_LastSyncMachineName;

        /// <summary>
        /// The name of the Machine where last sync was performed. 
        /// </summary>
        public string LastSyncMachineName => m_LastSyncMachineName;

        [SerializeField]
        string m_DateTimeStr;

        /// <summary>
        /// Property is `true` if <see cref="State"/> is equals to <see cref="SyncState.Synced"/>, otherwise 'false'
        /// </summary>
        public bool Synced => m_State == SyncState.Synced;

        /// <summary>
        /// Property is `true` if <see cref="State"/> is equals to <see cref="SyncState.InProgress"/>, otherwise 'false'
        /// </summary>
        public bool InProgress => m_State == SyncState.InProgress;

        /// <summary>
        /// Property is `true` if <see cref="State"/> is equals to <see cref="SyncState.NotSynced"/>, otherwise 'false'
        /// </summary>
        public bool NotSynced => m_State == SyncState.NotSynced;

        /// <summary>
        /// Property is `true` if <see cref="State"/> is equals to <see cref="SyncState.SyncedWithError"/>, otherwise 'false'
        /// </summary>
        public bool SyncedWithError => m_State == SyncState.SyncedWithError;

        /// <summary>
        /// The <see cref="DateTime"/> value when the last sync was performed.
        /// </summary>
        public DateTime? SyncDateTime
        {
            get
            {
                if (!string.IsNullOrEmpty(m_DateTimeStr))
                {
                    return DateTime.Parse(m_DateTimeStr);
                }

                return null;
            }
            set => m_DateTimeStr = value.ToString();
        }

        internal Spreadsheet() { }

        public Spreadsheet(string id)
        {
            m_Id = id;
            m_Name = k_DefaultName;
        }

        internal void ChangeStatus(SyncState state)
        {
            m_State = state;
            OnSyncStateChange(this);
        }

        internal void SetName(string name)
        {
            m_Name = name;
        }

        internal void SetUrl(string url)
        {
            m_Url = url;
        }

        internal void SetError(string error)
        {
            m_SyncErrorMassage = error;
        }

        internal void SetPath(string path)
        {
            m_Path = path;
        }

        internal void SetSheets(IEnumerable<Sheet> sheets)
        {
            m_Sheets = sheets.ToList();
        }

        internal void SetMachineName(string name)
        {
            m_LastSyncMachineName = name;
        }

        internal void CleanupSheets()
        {
            m_Sheets.Clear();
        }

        /// <summary>
        /// Returns `true` if spreadsheet contains a sheet with specified id, otherwise `false`.
        /// </summary>
        /// <param name="sheetId">A sheet id.</param>
        /// <returns>`true` if spreadsheet contains a sheet with specified id, otherwise `false`.</returns>
        public bool HasSheet(int sheetId)
        {
            foreach (var sheet in m_Sheets)
            {
                if (sheetId == sheet.Id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns `true` if spreadsheet contains a sheet with the name, otherwise `false`.
        /// </summary>
        /// <param name="sheetName">A sheet name.</param>
        /// <returns>`true` if spreadsheet contains a sheet with the name, otherwise `false`.</returns>
        public bool HasSheet(string sheetName)
        {
            foreach (var sheet in m_Sheets)
            {
                if (sheetName == sheet.Name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets spreadsheet sheet by it's id.
        /// </summary>
        /// <param name="sheetId">A sheet id.</param>
        /// <returns>
        /// Returns the <see cref="Sheets"/> object
        /// if spreadsheet contains a sheet with specified id, otherwise `null`
        /// </returns>
        public Sheet GetSheet(int sheetId)
        {
            return m_Sheets.FirstOrDefault(sheet => sheetId == sheet.Id);
        }

        /// <summary>
        /// Gets first spreadsheet sheet by it's name.
        /// </summary>
        /// <param name="sheetName">A sheet name.</param>
        /// <returns>
        /// Returns the <see cref="Sheets"/> object
        /// if spreadsheet contains a sheet with the name, otherwise `null`
        /// </returns>
        public Sheet GetSheet(string sheetName)
        {
            return m_Sheets.FirstOrDefault(sheet => sheetName == sheet.Name);
        }

        public Sheet GetSheetOrCreate(string sheetName)
        {
            if (HasSheet(sheetName))
                return GetSheet(sheetName);
            var sheet = CreateSheet(m_Sheets.Count, sheetName);
            sheet.SetDataState(DataState.Created);
            return sheet;
        }

        internal Sheet CreateSheet(int sheetId, string name)
        {
            //Create new Sheet if we don't have one
            var newSheet = new Sheet(sheetId, name);
            m_Sheets.Add(newSheet);
            return newSheet;
        }
    }
}
