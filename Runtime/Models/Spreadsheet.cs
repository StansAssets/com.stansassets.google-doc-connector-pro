using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    [Serializable]
    public class Spreadsheet
    {
        public enum SyncState
        {
            Synced,
            NotSynced,
            SyncedWithError,
            InProgress
        }
        
        public event Action<Spreadsheet> OnSyncStateChange = delegate { };
        
        public static readonly string SyncedWithErrorStringStatus ="[Synced With Error]";
        public static readonly string NotSyncedStringStatus ="[Not Synced]";

        [SerializeField]
        SyncState m_State;
        public SyncState State => m_State;
        
        [SerializeField]
        string m_SyncErrorMassage;
        public string SyncErrorMassage => m_SyncErrorMassage;
        
        [SerializeField]
        List<Sheet> m_Sheets = new List<Sheet>();
        public IEnumerable<Sheet> Sheets => m_Sheets;

        [SerializeField]
        string m_Id;
        public string Id => m_Id;

        [SerializeField]
        string m_Name;
        public string Name => m_Name;

        [SerializeField]
        string m_Path;
        public string Path => m_Path;
        
        [SerializeField]
        string m_LastSyncMachineName;
        public string LastSyncMachineName => m_LastSyncMachineName;

        [SerializeField]
        string m_DateTimeStr;
        
        public bool Synced => m_State == SyncState.Synced;
        public bool InProgress => m_State == SyncState.InProgress;
        public bool NotSynced => m_State == SyncState.NotSynced;
        public bool SyncedWithError => m_State == SyncState.SyncedWithError;

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

        const string k_DefaultName = "<Spreadsheet>";

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
        
        internal void SetError(string error)
        {
            m_SyncErrorMassage = error;
        }

        internal void SetPath(string path)
        {
            m_Path = path;
        }
        
        internal void SetMachineName(string name)
        {
            m_LastSyncMachineName = name;
        }
        
        public void CleanupSheets()
        {
            m_Sheets.Clear();
        }

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

        public Sheet GetSheet(int sheetId)
        {
            foreach (var sheet in m_Sheets)
            {
                if (sheetId == sheet.Id)
                {
                    return sheet;
                }
            }

            return null;
        }

        internal Sheet CreateSheet(int sheetId, string name)
        {
            //Create new Sheet if we don't have one
            var newSheet = new Sheet(sheetId, name);
            m_Sheets.Add(newSheet);
            return newSheet;
        }

        internal void InitFromCache()
        {
            if (File.Exists(m_Path))
            {
                string serializedData = File.ReadAllText(m_Path);
                var spreadsheet = JsonConvert.DeserializeObject<Spreadsheet>(serializedData);
                m_Sheets = spreadsheet.m_Sheets;
            }
        }

        internal void CleanUpLocalCache()
        {
            if (File.Exists(m_Path))
            {
                File.Delete(m_Path);
            }
        }

        public void Load()
        {
            var loader = new SpreadsheetLoader(this);
            _ = loader.Load();
        }
    }
}
