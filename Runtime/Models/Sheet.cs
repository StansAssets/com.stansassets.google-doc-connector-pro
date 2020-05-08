using System;
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
        /// <param name="name">name of Named range to search</param>
        /// <returns>boolean</returns>
        internal bool HasNamedRange(string name)
        {
            return m_NamedRanges.Exists(n => name.Equals(n.Name));
        }
        
        /// <summary>
        /// Get Named range by name
        /// </summary>
        /// <param name="name">name of Named range to search</param>
        /// <returns>NamedRange if ok and null if not find range</returns>
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
        /// Get range by name of Named range
        /// </summary>
        /// <param name="name">name of Named range to search</param>
        /// <returns>List of object if ok and empty list if not find range </returns>
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
