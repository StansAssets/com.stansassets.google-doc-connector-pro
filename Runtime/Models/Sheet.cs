﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    [Serializable]
    public class Sheet
    {
        [SerializeField]
        string m_Name;
        public string Name => m_Name;

        [SerializeField]
        int m_Id;
        public int Id => m_Id;

        List<NamedRange> m_NamedRanges = new List<NamedRange>();
        public IEnumerable<NamedRange> NamedRanges => m_NamedRanges;

        List<RowData> m_Rows = new List<RowData>();
        public IEnumerable<RowData> Rows => m_Rows;
        
        internal bool NamedRangeFoldOutUIState = false;

        public Sheet(int id, string name)
        {
            m_Id = id;
            m_Name = name;
        }

        internal void CleanupRows()
        {
            m_Rows.Clear();
        }

        internal void AddRow(RowData row)
        {
            m_Rows.Add(row);
        }

        internal void SetRows(List<RowData> rows)
        {
            m_Rows = rows;
        }
        
        /// <summary>
        /// Determines whether an element is in the sheet
        /// </summary>
        /// <param name="name">Name of Named Range to search</param>
        /// <returns>True if the element is in the sheet; otherwise, false</returns>
        internal bool HasNamedRange(string name)
        {
            return m_NamedRanges.Exists(n => name.Equals(n.Name));
        }
        
        /// <summary>
        /// Returns NamedRange with provided name
        /// </summary>
        /// <param name="name">Name of Named Range to search for</param>
        /// <returns>NamedRange if the element with provided name exists, otherwise null</returns>
        internal NamedRange GetNamedRange(string name)
        {
            return m_NamedRanges.FirstOrDefault(n => name.Equals(n.Name));
        }

        internal NamedRange CreateNamedRange(string id, string name)
        {
            var namedRange = new NamedRange(id, name);
            m_NamedRanges.Add(namedRange);
            return namedRange;
        }

        public object GetCell(int row, int column)
        {
            if (row < m_Rows.Count)
            {
                var r = m_Rows[row];
                if (column < r.Cells.Count())
                    return r.Cells.ElementAt(column).Data;
            }

            return null;
        }
      
        /// <summary>
        /// Returns a list of objects of the requested Name Range
        /// </summary>
        /// <param name="name">Name of the requested Named Range</param>
        public List<object> GetRange(string name)
        {
            var range = GetNamedRange(name);
            if (range is null)
                return new List<object>();

            return range.Cells.Select(cell => GetCell(cell.Row, cell.Column)).ToList();
        }

        public List<object> GetRow(int row)
        {
            List<object> rowData = new List<object>();
            if (row >= 0 && row < m_Rows.Count)
            {
                foreach (var cell in m_Rows[row].Cells)
                {
                    rowData.Add(cell.Data);
                }
            }
            return rowData;
        }

        public List<object> GetColumn(int column)
        {
            List<object> rowData = new List<object>();
            foreach (var row in m_Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (cell.Column == column)
                    {
                        rowData.Add(cell.Data);
                    }
                }
            }
            return rowData;
        }
    }
}
